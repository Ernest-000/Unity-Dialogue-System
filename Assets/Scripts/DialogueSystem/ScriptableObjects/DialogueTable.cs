using System;
using UnityEngine;

namespace Dialogue
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue Table", menuName = "Dialogue/Dialogue Table")]
    public class DialogueTable : ScriptableObject
    {
        public DialogueTableEntry[] Entries;

        [Serializable]
        public class DialogueTableEntry
        {
            public DialogueActor Actor;

            [TextArea]
            public string Text;
            public DialogueBehavior Behavior;
        }

        public DialogueCommand ToCommands()
        {
            if(Entries.Length == 0)
            {
                return null;
            }

            DialogueCommand root = new DialogueCommand(true);
            root.Text = Entries[0].Text;
            root.Actor = Entries[0].Actor;
            root.Behavior = Entries[0].Behavior;

            if(Entries.Length == 1)
            {
                return root;
            }

            DialogueCommand current = root;
            for (int i = 0; i < Entries.Length - 1; i++)
            {
                current.Add(Entries[i + 1].Text, Entries[i + 1].Actor, Entries[i + 1].Behavior);
                current = current.Next;
            }

            return root;
        }
    }
}