using TMPro;
using UnityEngine;

namespace Dialogue
{
    public class DialogueSystemInstance : MonoBehaviour
    {
        public CanvasGroup PanelGroup;
        public TMP_Text DialogueTextComponent; 
        public TMP_Text CharacterTextComponent; 

        private DialogueSystem m_system;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            m_system = new DialogueSystem(DoDialogue);
            Hide();
        }

        // Update is called once per frame
        void Update()
        {
            m_system.Poll();

            if (Input.GetKeyUp(KeyCode.Space))
            {
                m_system.Next();
            }
        }

        void Show()
        {
            PanelGroup.alpha = 1.0f;
        }

        void Hide()
        {
            PanelGroup.alpha = 0.0f;
        
            CharacterTextComponent.text = string.Empty;  
            DialogueTextComponent.text = string.Empty;  
        }

        private void DoDialogue(DialogueCommand cmd)
        {
            if(cmd.Text == string.Empty)
            {
                Hide();
                return;
            }

            DialogueTextComponent.text = cmd.Text;
            CharacterTextComponent.text = cmd.Actor.Name;

            Show();
        }
    }

}
