using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dialogue
{
    public class DialogueSystemBehaviour : MonoBehaviour
    {
        [Header("Behaviour")]
        public DialogueSettings Settings;

        [Header("Components")]
        public DialogueTypewritter Typewritter;

        [Header("Tables")]
        public DialogueTable[] RegisteredTables;

        private DialogueSystem m_system;

        private InputAction m_inputNext;
        private InputAction m_inputSkip;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if(Settings == null) Debug.LogError(new ArgumentNullException(nameof(Settings)));
            if(Typewritter == null) Debug.LogError(new ArgumentNullException(nameof(Typewritter)));

            m_system = new DialogueSystem(Settings);

            // register event
            m_system.OnDialogueEvent += DoDialogue;
            m_system.OnDialogueStartEvent += Typewritter.Show;
            m_system.OnDialogueCloseEvent += Typewritter.Hide;

            // register tables
            foreach (DialogueTable table in RegisteredTables)
            {
                DialogueSystem.RegisterDialogue(table);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Typewritter.IsFinish)
            {
                bool canPoll = m_system.Poll();

                UpdateHandleSkipping(m_system.Current, canPoll);
            }
        }

        public void PlayDialogue(DialoguePtr ptr) => DialogueSystem.PlayDialogue(ptr);
        public void PlayDialogue(int index) => DialogueSystem.PlayDialogue(new DialoguePtr(index));
        public void PlayDialogue(string str) => DialogueSystem.PlayDialogue(PlayDialogueString(str));

        public DialoguePtr PlayDialogueCommand(DialogueCommand cmd)
        {
            DialoguePtr ptr = DialogueSystem.RegisterDialogue(cmd);
            DialogueSystem.PlayDialogue(ptr);
            return ptr;
        }

        public DialoguePtr PlayDialogueString(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return DialoguePtr.k_INVALID;
            }

            DialoguePtr ptr = m_system.Find(name);
            DialogueSystem.PlayDialogue(ptr);
            return ptr;    
        }

        void OnEnable()
        {
            // create inputs
            m_inputNext = InputActionReference.Create(Settings.InputActionNext);
            m_inputSkip = InputActionReference.Create(Settings.InputActionSkip);

            m_inputNext.Enable();
            m_inputSkip.Enable();
        }

        void OnDisable()
        {
            m_inputNext.Disable();
            m_inputSkip.Disable();
        }

        private void DoDialogue(DialogueCommand cmd)
        {
            if(cmd.Behavior.HasFlag(DialogueBehavior.EndOfDialogue))
            {
                Typewritter.Hide();
 
                return;
            }

            Typewritter.Show();

            // write
            Typewritter.TryEnqueueCommand(cmd);
        }

        private void UpdateHandleSkipping(DialogueCommand cmd, bool canPoll)
        {
            DialogueBehavior behaviour = Settings.EndDialogueBehaviour;

            if(cmd != null)
            {
                behaviour = cmd.Behavior;
            }

            if (behaviour.HasFlag(DialogueBehavior.AutoSkip))
            {
                m_system.Next();
            }

            if (behaviour.HasFlag(DialogueBehavior.WaitForInput))
            {
                if (m_inputNext.ReadValue<float>() > float.Epsilon)
                {
                    m_system.Next();
                } 
            }
        }
    }

}
