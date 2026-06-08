using System;
using PT.Logic.Configs;
using PT.Logic.ProjectContext;
using PT.Tools.Tutorials.Target;
using UnityEngine;

namespace PT.Tools.Tutorials
{
    [Serializable]
    public class TutorialStepData
    {
        [SerializeField] private TutorialStepType stepType;
        [SerializeField] private LocalizationKeyEnum title;
        [SerializeField] private LocalizationKeyEnum description;
        [Space]
        [SerializeField] private TutorialTargetEnum tutorialTargetEnum;
        [Space]
        [SerializeField][Min(0.5f)] private float duration = 1.0f;
        [Space]
        [SerializeField] private bool lockInput = true;
        [Space]
        [SerializeField] private bool showPointer = true;
        [SerializeField] private Vector2 pointerOffset = Vector2.zero;

        public TutorialStepType StepType => stepType;
        public LocalizationKeyEnum Title => title;
        public LocalizationKeyEnum Description => description;
        public TutorialTargetEnum TutorialTargetEnum => tutorialTargetEnum;
        public float Duration => duration;
        public bool LockInput => lockInput;
        public bool ShowPointer => showPointer;
        public Vector2 PointerOffset => pointerOffset;
    }
}