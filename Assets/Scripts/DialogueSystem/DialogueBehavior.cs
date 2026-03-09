using System;

namespace Dialogue
{
    [Flags]
    public enum DialogueBehavior
    {
        WaitForInput,
        AutoSkip,
        EndOfDialogue,
        
        // default is wait for input
        None = WaitForInput,
    }
}