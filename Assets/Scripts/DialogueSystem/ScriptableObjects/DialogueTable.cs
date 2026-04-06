using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;

using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue Table", menuName = "Dialogue/Dialogue Table")]
    public class DialogueTable : ScriptableObject, IEnumerable<DialogueCommand>
    {
        public DialogueCommand Root => m_root;

        public DialogueCommand Last
        {
            get
            {
                DialogueCommand cmd = m_root;
                while(cmd.Next != null)
                {
                    cmd = cmd.Next;
                }

                return cmd;
            }
        }
        
        public string Name;

        [SerializeField]
        private List<DialogueCommand> m_nodes = new();
        private DialogueCommand m_root;

        public DialogueCommand AddNode(string name)
        {
            if(m_nodes.Count == 0)
            {
                m_root = new DialogueCommand(name);
                m_nodes.Add(m_root);
                return m_root;
            }

            DialogueCommand cmd = Last.AddChild(name);
            m_nodes.Add(cmd);
            return cmd;
        }

        public DialogueCommand AddNode(DialogueCommand parent, string name)
        {
            DialogueCommand cmd = parent.AddChild(name);
            if (!m_nodes.Contains(cmd))
            {
                m_nodes.Add(cmd);            
            }
            
            return cmd;
        }

        public void RemoveNode(DialogueCommand command)
        {
            m_nodes.Remove(command);
            command.Remove();
        }

        /// <summary>
        /// PLEASE DO NOT USE IT WITHOUT KNOWING WHAT THIS SHIT DOES
        /// Try to relink lonely nodes.
        /// </summary>
        public void TryRelink()
        {
            foreach(DialogueCommand cmd in m_nodes)
            {
                int index = m_nodes.IndexOf(cmd);
                if(index == 0 && !cmd.IsRoot)
                {
                    m_root = cmd;
                    cmd.Parent = cmd;    
                }

                if(cmd.Parent == null && index != 0)
                {
                    cmd.Parent = m_nodes[index - 1];
                }
            }
        }

        [OnOpenAsset]
        public static bool OnOpenEditor(int instance, int line)
        {
            DialogueTable table = EditorUtility.EntityIdToObject(instance) as DialogueTable;
            
            if(table != null)
            {
                DialogueGraphEditor editor = DialogueGraphEditor.CreateWindow();
                return true;
            } 

            return false;
        }

        public IEnumerator<DialogueCommand> GetEnumerator()
        {
            List<DialogueCommand> nodes = new();

            DialogueCommand cmd = m_root;
            while(cmd != null)
            {
                nodes.Add(cmd);
                cmd = cmd.Next;
            }

            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}