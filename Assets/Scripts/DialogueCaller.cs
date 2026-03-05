using System;
using Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueCaller : MonoBehaviour 
{
    public DialogueTable Table;

    private DialoguePtr m_diag;

    public void Start()
    {
        m_diag = DialogueSystem.RegisterDialogue(Table.ToCommands());
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            DialogueSystem.PlayDialogue(m_diag);
        }
    }
}