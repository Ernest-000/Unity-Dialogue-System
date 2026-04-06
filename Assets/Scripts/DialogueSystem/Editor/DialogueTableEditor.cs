using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DialogueTable))] 
    public class DialogueTableEditor : Editor
    {
        private DialogueTable m_table;

        private bool m_nodeFolder;

        void OnEnable()
        {
            m_table = serializedObject.targetObject as DialogueTable;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            // DrawInspectorNodes();

            if(GUILayout.Button("Add Node"))
            {
                m_table.AddNode("Node");
            }

            if(GUILayout.Button("Relink"))
            {
                m_table.TryRelink();
            }

            if(GUILayout.Button("Print Dialogue"))
            {
                foreach (DialogueCommand command in m_table)
                {
                    Debug.Log(command.Text);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    
        void DrawInspectorNodes()
        {
            m_nodeFolder = EditorGUILayout.Foldout(m_nodeFolder, "Table");
            
            if (m_nodeFolder)
            {
                DialogueCommand command = m_table.Root;
                
                while (command != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    command.Text = EditorGUILayout.TextField(command.Text);
                    EditorGUILayout.TextField(command.GUID);
                    EditorGUILayout.EndHorizontal();
                    command = command.Next;
                }
            } 
        }
    }
}