using System;
using Dialogue;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueCaller : MonoBehaviour 
{
    public DialogueTable Table;

    public InputActionReference InputAction;

    private DialoguePtr m_diag;
    private InputAction m_input;

    public void Start()
    {
        m_diag = DialogueSystem.RegisterDialogue(Table.ToCommands());
        
    }

    void OnEnable()
    {
        m_input = InputActionReference.Create(InputAction);
        m_input.Enable();
    }

    void OnDisable()
    {
        m_input.Disable();
    }

    public void Update()
    {
        if (m_input.ReadValue<float>() > float.Epsilon){
            DialogueSystem.PlayDialogue(m_diag);
        }
    }
}