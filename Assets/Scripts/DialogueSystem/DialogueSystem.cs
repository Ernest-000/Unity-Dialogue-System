using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public delegate void DoDialogueCallback(DialogueCommand cmd);

    public class DialogueSystem : ICollection
    {
        private List<DialogueCommand> m_commands;
        private Queue<DialoguePtr> m_dialogQueue;
        
        private DialogueCommand m_currentCmd; 

        private static DialogueSystem s_instance = null;

        public int Count => m_commands.Count;

        public bool IsSynchronized => false;
        public object SyncRoot => false;

        private DoDialogueCallback m_dialogueCallback;

        public DialogueSystem(DoDialogueCallback callback)
        {
            if (s_instance != null)
            {
                Debug.LogError(new Exception("a Dialogue System instance already exists!"));
            }

            m_commands = new List<DialogueCommand>();
            m_dialogQueue = new Queue<DialoguePtr>();
            m_currentCmd = null;
            m_dialogueCallback = callback;

            s_instance = this;
        }

        /// <summary>
        /// Poll dialogue commands
        /// </summary>
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
                    DoCommand(DialogueCommand.Empty);
                }

                return;
            }

            m_currentCmd.IsValidated = true;
        }

        private bool DoCommand(DialogueCommand cmd)
        { 
            if(!(cmd.IsRoot || cmd.IsValidated))
            {
                return false;
            }

            m_dialogueCallback(cmd);

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
                return new DialoguePtr(-1);
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

            s_instance.m_dialogQueue.Enqueue(ptr);
            if (s_instance.m_currentCmd == null)
            {
                s_instance.Next();
            }

            return true;
        }
    }   
}
