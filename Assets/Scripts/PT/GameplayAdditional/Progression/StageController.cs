using UnityEngine;
using Zenject;

namespace PT.GameplayAdditional.Progression
{
    public class StageController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer backgroundRenderer;
        
        [Inject] private StageProvider _stageProvider;

        private void Start()
        {
            UpdateBackground();
        }

        private void UpdateBackground()
        {
            var stage = _stageProvider.GetCurrentStageInfo();
            
            backgroundRenderer.sprite = stage.Pattern;
        }
    }
}