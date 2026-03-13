using UnityEngine;

namespace Dialogue
{
    public class DialogueSource : MonoBehaviour
    {
        public DialogueTable Dialogue;

        private DialoguePtr m_ptr;

        void Start()
        {
            m_ptr = DialogueSystem.RegisterDialogue(Dialogue);
        }
    }    
}