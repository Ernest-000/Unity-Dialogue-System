using DialogueSystem;

using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    [UxmlElement]
    public partial class DialogueNodeView : Node
    {
        public DialogueCommand Command => m_command;
        
        public Port ParentInput;
        public Port NextOutput;
        

        private DialogueCommand m_command;

        public DialogueNodeView(){}
        public DialogueNodeView(DialogueCommand command) : this()
        {
            m_command = command;
            
            title = command.Name;
            viewDataKey = command.GUID;

            style.left = m_command.p_nodePosition.x;
            style.top = m_command.p_nodePosition.y;

            CreateInputPorts();
            CreateOutputPorts();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            m_command.p_nodePosition.x = newPos.xMin;
            m_command.p_nodePosition.y = newPos.yMin;
        }

        void CreateInputPorts()
        {
            if (m_command.IsRoot)
            {
                // root does not have parent
                return;
            }

            ParentInput = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(DialogueCommand));
            if (ParentInput != null)
            {
                ParentInput.portName = "Parent";
                ParentInput.portColor = Color.darkRed;

                inputContainer.Add(ParentInput);
            }
        }

        void CreateOutputPorts()
        {
            NextOutput = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(DialogueCommand));
            if(NextOutput != null)
            {
                NextOutput.portName = "Next";
                NextOutput.portColor = Color.darkRed;


                outputContainer.Add(NextOutput);
            }
        }
    }
}
