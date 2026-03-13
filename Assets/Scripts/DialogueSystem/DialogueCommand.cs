using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Dialogue
{
    public class DialogueCommand
    {
        public string RootName;

        public string Text;
        public DialogueActor Actor;

        public DialogueBehavior Behavior; 

        public bool IsValidated;
        public bool IsRoot;

        public DialogueCommand Next;

        public static DialogueCommand Empty => new DialogueCommand(true);
        public static DialogueCommand EndOfDialogue => new DialogueCommand(DialogueBehavior.EndOfDialogue);

        public DialogueCommand(bool isRoot=false)
        {
            RootName = string.Empty;
            Text = string.Empty;
            Behavior = DialogueBehavior.None;
            IsValidated = false;
            IsRoot = isRoot;

            Actor = null;
            Next = null;
        }

        public DialogueCommand(string text, bool isRoot=false)
        {
            Text = text;
            Behavior = DialogueBehavior.None;
            IsValidated = false;
            IsRoot = isRoot;

            Actor = null;
            Next = null;
        }

        public DialogueCommand(DialogueBehavior behavior, bool isRoot=false)
        {
            Text = string.Empty;
            Behavior = behavior;
            IsValidated = false;
            IsRoot = isRoot;

            Actor = null;
            Next = null;
        }

        public DialogueCommand(string text, DialogueBehavior behavior, bool isRoot=false)
        {
            Text = text;
            Behavior = behavior;
            IsValidated = false;
            IsRoot = isRoot;

            Actor = null;
            Next = null;
        }

        public void Add(string text)
        {
            Next = new DialogueCommand(false);
            Next.Text = text;
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