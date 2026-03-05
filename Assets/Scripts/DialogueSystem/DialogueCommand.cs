using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Dialogue
{
    public class DialogueCommand
    {
        public string Text;
        public DialogueActor Actor;

        public DialogueBehavior Behavior; 

        public bool IsValidated;
        public bool IsRoot;

        public DialogueCommand Next;

        public static DialogueCommand Empty
        {
            get
            {
                return new DialogueCommand(true);
            }
        }

        public DialogueCommand(bool isRoot)
        {
            Text = string.Empty;
            Behavior = DialogueBehavior.None;
            IsValidated = false;
            IsRoot = isRoot;

            Actor = null;
            Next = null;
        }

        public void Add(string text, DialogueActor actor, DialogueBehavior behavior)
        {
            Next = new DialogueCommand(false);
            Next.Text = text;
            Next.Actor = actor;
            Next.Behavior = behavior;
        }

        public void Clear()
        {
            Next.Clear();
            Next = null;
        }
    }
}