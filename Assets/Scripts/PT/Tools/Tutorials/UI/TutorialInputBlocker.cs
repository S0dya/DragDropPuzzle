using UnityEngine;
using UnityEngine.UI;

namespace PT.Tools.Tutorials.UI
{
    public class TutorialInputBlocker : MonoBehaviour
    {
        [SerializeField] private CanvasGroup cg;

        void Awake()
        {
            Unblock();
        }

        public void Block(float dimAlpha = 0.25f)
        {
            cg.blocksRaycasts = true;
            cg.alpha = dimAlpha;
        }
        public void Unblock()
        {
            cg.blocksRaycasts = false;
            cg.alpha = 0f;
        }
    }
}