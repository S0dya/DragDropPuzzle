using System.Collections.Generic;
using PT.Tools.Debugging;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PT.Tools.Sequences
{
    public class SequenceSkipper : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sequencer[] sequencers;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private readonly List<Sequencer> _activeSequencers = new();
        
        private void Awake()
        {
            canvasGroup.alpha = 0f;
        }

        private void OnEnable()
        {
            ToggleCG(false);
            
            foreach (var sequencer in sequencers) 
            {
                if (_activeSequencers.Contains(sequencer))
                {
                    DebugManager.Log(DebugCategory.Sequence, $"Sequence duplicate detected on");
                    continue;
                }
                
                sequencer.OnSequenceStarted += HandleStart;
                sequencer.OnSequenceFinished += HandleFinish;
            }
        }
        private void OnDisable()
        {
            foreach (var sequencer in sequencers)
            {
                if (_activeSequencers.Contains(sequencer))
                {
                    // DebugManager.Log(DebugCategory.Sequence, $"Sequence duplicate detected on");
                    continue;
                }
                
                sequencer.OnSequenceStarted -= HandleStart;
                sequencer.OnSequenceFinished -= HandleFinish;
            }
        }
        
        private void HandleStart(Sequencer sequencer)
        {
            if (!sequencer) return;
            
            _activeSequencers.Add(sequencer);
            
            ToggleCG(true);
        }

        private void HandleFinish(Sequencer sequencer)
        {
            if (!sequencer) return;
            
            _activeSequencers.Remove(sequencer);

            if (_activeSequencers.Count == 0) ToggleCG(false);
            
        }

        private void ToggleCG(bool toggle)
        {
            canvasGroup.blocksRaycasts = toggle;
            canvasGroup.interactable = toggle;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_activeSequencers.Count == 0) return;
            
            DebugManager.Log(DebugCategory.Sequence, $"Trying Skip Sequence");

            var sequencers = _activeSequencers.ToArray();
            for (int i = 0; i < sequencers.Length; i++) sequencers[i]?.Skip();
        }
    }
}