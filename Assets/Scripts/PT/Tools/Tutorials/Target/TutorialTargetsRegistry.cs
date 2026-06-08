using PT.Tools.Helper;
using UnityEngine;
using Zenject;

namespace PT.Tools.Tutorials.Target
{
    public class TutorialTargetsRegistry : MonoBehaviour
    {
        [SerializeField] private SerializableKeyValue<TutorialTargetEnum, TutorialTargetBase> map = new();
        
        [Inject] private TutorialManager _tutorialManager;
        
        private void Awake() 
        {
            RegisterAll();
        }

        private void OnEnable()
        {
            _tutorialManager.AddTargetsRegistry(this);
        }
        private void OnDisable()
        {
            _tutorialManager.RemoveTargetsRegistry(this);
        }
        
        private void RegisterAll()
        {
            if (map.Dictionary.Count > 0) return;
            
            foreach (var tutorialTarget in FindObjectsOfType<TutorialTargetBase>()) 
            {
                map.Dictionary[tutorialTarget.Id] = tutorialTarget;
            }
        }

        public bool Resolve(TutorialTargetEnum id, out TutorialTargetBase target)
        {
            target = map.Dictionary.TryGetValue(id, out var t) ? t : null;

            return target;
        }
    }
}