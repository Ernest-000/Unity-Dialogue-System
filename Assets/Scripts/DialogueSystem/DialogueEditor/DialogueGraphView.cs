using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;

namespace DialogueSystem
{
    [UxmlElement]
    public partial class DialogueGraphView : GraphView
    {
        //public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> {}

        private DialogueTable m_table;

        public DialogueGraphView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/DialogueSystem/DialogueEditor/DialogueGraphEditor.uss");
            styleSheets.Add(styleSheet);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction("Create Node", (a) => { m_table.AddNode(""); BuildNodeTree(m_table); });
            
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        internal void BuildNodeTree(DialogueTable table)
        {
            m_table = table;

            graphViewChanged -= OnGraphViewChange;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChange;

            // create nodes
            foreach (DialogueCommand cmd in m_table)
            {
                DialogueNodeView node = new DialogueNodeView(cmd);
                AddElement(node);
            }

            // create edges
            foreach (DialogueCommand cmd in m_table)
            {
                if (cmd.Next != null)
                {
                    DialogueNodeView parent = GetDialogueNodeView(cmd);
                    DialogueNodeView child = GetDialogueNodeView(cmd.Next);
                    
                    var e = parent.NextOutput.ConnectTo(child.ParentInput);
                    AddElement(e);
                }
            }
        }

        private GraphViewChange OnGraphViewChange(GraphViewChange graphViewChange)
        {
            if(graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    DialogueNodeView view = element as DialogueNodeView;
                    if(view != null)
                    {
                        m_table.RemoveNode(view.Command);
                    }
                }
            }

            if(graphViewChange.edgesToCreate != null)
            {
                foreach(var e in graphViewChange.edgesToCreate)
                {
                    DialogueNodeView parent = e.output.node as DialogueNodeView;
                    DialogueNodeView child = e.input.node as DialogueNodeView;
                    
                    if (!m_table.Contains(child.Command))
                    {
                        m_table.AddNode(parent.Command, "New");                    
                    }

                    parent.Command.Next = child.Command;
                }
            }

            return graphViewChange;
        } 
        
        private DialogueNodeView GetDialogueNodeView(DialogueCommand cmd)
        {
            return GetNodeByGuid(cmd.GUID) as DialogueNodeView;
        }
    }
}
