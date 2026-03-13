using System;
using UnityEngine;

namespace Dialogue
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue Table", menuName = "Dialogue/Dialogue Table")]
    public class DialogueTable : ScriptableObject
    {
        public string Name;
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
            root.RootName = Name;
            root.Text = Entries[0].Text;
            root.Actor = Entries[0].Actor;
            root.Behavior = Entries[0].Behavior;

            if(Entries.Length == 1)
            {
                return root;
            }

            DialogueTableEntry entry;
            DialogueCommand current = root;
            for (int i = 0; i < Entries.Length - 1; i++)
            {
                entry = Entries[i + 1];

                current.Add(entry.Text);
                current.RootName = Name;
                current.Behavior = entry.Behavior;
                current.Actor = entry.Actor;
                
                current = current.Next;
            }

            return root;
        }

        public static implicit operator DialogueCommand(DialogueTable table) => table.ToCommands();
    }
}