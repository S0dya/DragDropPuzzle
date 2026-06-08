using System;
using TMPro;
using UnityEngine;

namespace PT.Tools.RewWheel
{
    [Serializable]
    public class RewardWheelView 
    {
        [SerializeField] private Transform arrow;
        [SerializeField] private TextMeshProUGUI arrowText;
        [Space]
        [SerializeField] private bool arrowIsPositiveDirection;

        public void SetAngle(float angle)
        {
            arrow.localEulerAngles = new Vector3(0, 0, angle * (arrowIsPositiveDirection ? 1 : -1));
        }

        public void SetText(string text)
        {
            arrowText.text = text;
        }
    }
}