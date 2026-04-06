using DialogueSystem;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class DialogueGraphEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private DialogueGraphView m_graph;

        [MenuItem("Window/Dialogue/Dialogue Graph")]
        public static DialogueGraphEditor CreateWindow()
        {
            DialogueGraphEditor win = GetWindow<DialogueGraphEditor>();
            win.titleContent = new GUIContent("Dialogue Graph Editor");
            return win;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            m_VisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/DialogueSystem/DialogueEditor/DialogueGraphEditor.uxml");
            m_VisualTreeAsset.CloneTree(root);

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/DialogueSystem/DialogueEditor/DialogueGraphEditor.uss");
            root.styleSheets.Add(styleSheet);

            m_graph = root.Q<DialogueGraphView>();
        }

        private void OnEnable()
        {
            //Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            //Selection.selectionChanged -= OnSelectionChanged;
        }

        void OnFocus() => UpdateGraph();
        void OnLostFocus() => UpdateGraph(); 

        void UpdateGraph()
        {
            DialogueTable table = Selection.activeObject as DialogueTable;
            
            if (table != null && m_graph != null)
            {
                this.titleContent = new GUIContent(table.Name);
                m_graph.BuildNodeTree(table);
            }
        }
    }

}

