using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dialogue
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue Settings", menuName = "Dialogue/Dialogue Settings")]
    public class DialogueSettings : ScriptableObject
    {
        [Header("Typewritter")]
        [Range(0.0f, 1.0f)]
        public float CharacterDelay;

        [Header("Behaviour")]
        public DialogueBehavior EndDialogueBehaviour;

        [Header("Inputs")]
        public InputActionReference InputActionNext;
        public InputActionReference InputActionSkip;
    }
}