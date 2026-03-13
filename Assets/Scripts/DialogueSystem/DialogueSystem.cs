using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;

namespace Dialogue
{
    public delegate void OnDialogueCallback(DialogueCommand cmd);
    public delegate void OnDialogueStartCallback();
    public delegate void OnDialogueCloseCallback();

    /// <summary>
    /// This is the main dialogue class.
    /// It class handles the dialogue queuing and event handlers. 
    /// 
    /// This class is a singleton which means that you can access to a static instance.
    /// </summary>
    public class DialogueSystem : ICollection
    {
        public static DialogueSystem Instance => s_instance;

        public event OnDialogueCallback OnDialogueEvent;
        public event OnDialogueStartCallback OnDialogueStartEvent;
        public event OnDialogueCloseCallback OnDialogueCloseEvent;

        public int Count => m_commands.Count;

        public bool IsSynchronized => false;
        public object SyncRoot => false;

        public DialogueSettings Settings
        {
            get; private set;
        }

        public DialogueCommand Current => m_currentCmd;

        private List<DialogueCommand> m_commands;
        private Queue<DialoguePtr> m_dialogQueue;
        
        private DialogueCommand m_currentCmd; 

        private static DialogueSystem s_instance = null;

        public DialogueSystem(DialogueSettings settings)
        {
            if (s_instance != null)
            {
                Debug.LogError(new Exception("a Dialogue System instance already exists!"));
            }

            Settings = settings;

            m_commands = new List<DialogueCommand>();
            m_dialogQueue = new Queue<DialoguePtr>();
            m_currentCmd = null;

            s_instance = this;
        }

        /// <summary>
        /// Dialogue polling
        /// </summary>
        /// <returns>Returns true if it can pool to the next one</returns>
        public bool Poll()
        {
            // when the current dialogue is not finished
            if (!CanPoll())
            {
                return false;
            }

            // iterate through each command in the linked command list
            if (DoCommand(m_currentCmd))
            {
                m_currentCmd.IsValidated = false;
                m_currentCmd = m_currentCmd.Next;
            }

            return true;
        }
 
        /// <summary>
        /// Validate the dialogue and go to the next one.
        /// </summary>
        public void Next()
        {
            if(m_currentCmd == null)
            {
                // when there is no dialogue after
                // we can dequeue the next one
                if(m_dialogQueue.TryDequeue(out DialoguePtr dialoguePtr))
                {
                    if(dialoguePtr == -1)
                    {
                        Debug.Log("failed to call dialogue");
                        return;
                    }

                    m_currentCmd = m_commands[dialoguePtr];
                }
                else
                {
                    DoCommand(DialogueCommand.EndOfDialogue);
                    OnDialogueCloseEvent?.Invoke();
                }

                return;
            }

            m_currentCmd.IsValidated = true;
        }

        public DialoguePtr Find(string name)
        {
            int index = m_commands.FindIndex(cmd => cmd.RootName == name);
            // check for invalid index?
            return new DialoguePtr(index);
        }

        private bool DoCommand(DialogueCommand cmd)
        { 
            if(!(cmd.IsRoot || cmd.IsValidated))
            {
                return false;
            }

            // handle start event
            if (cmd.IsRoot)
            {
                OnDialogueStartEvent?.Invoke();
            }

            // generic event
            OnDialogueEvent?.Invoke(cmd);

            return true;
        }

        private bool CanPoll()
        {
            if(m_currentCmd == null)
            {
                return false;
            }

            return true;
        }

        public void CopyTo(Array array, int index)
        {
            Array.Copy(m_commands.ToArray(), index, array, index, Count - index);
        }

        public IEnumerator GetEnumerator()
        {
            return m_commands.GetEnumerator();
        }

        /// <summary>
        /// Register a new command linked array.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Returns a pointer to the dialogue</returns>
        public static DialoguePtr RegisterDialogue(DialogueCommand command)
        {
            if(s_instance == null)
            {
                Debug.LogError(new Exception("no instance exists!"));
                return DialoguePtr.k_INVALID;
            }

            if(command == null)
            {
                return DialoguePtr.k_INVALID;
            }

            // if is already registered
            if (s_instance.m_commands.Contains(command))
            {
                return new DialoguePtr(s_instance.m_commands.IndexOf(command));
            }

            s_instance.m_commands.Add(command);
            return new DialoguePtr(s_instance.m_commands.Count - 1);
        }

        /// <summary>
        /// Start a dialogue.
        /// </summary>
        /// <param name="ptr">A pointer to an existing registered dialogue</param>
        /// <returns>Returns if the dialogue has been played.</returns>
        public static bool PlayDialogue(DialoguePtr ptr)
        {
            if(s_instance == null)
            {
                Debug.LogError(new Exception("no instance exists!"));
                return false;
            }

            if (!DialoguePtr.IsValid(ptr))
            {
                Debug.Log(new ArgumentException("invalid dialogue pointer", nameof(ptr)));
            }

            s_instance.m_dialogQueue.Enqueue(ptr);

            // start dialogue
            if (s_instance.m_currentCmd == null)
            {
                s_instance.Next();
            }

            return true;
        }
    }   
}
