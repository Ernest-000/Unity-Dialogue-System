namespace Dialogue
{
    public struct DialoguePtr
    {
        public int Handle;

        public DialoguePtr(int value)
        {
            Handle = value;
        }

        public static implicit operator int(DialoguePtr ptr) => ptr.Handle; 
    }
}