using System.Collections.Generic;
using Gameplay.BallDrop.Balls;
using Gameplay.Session;
using TMPro;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.UI
{
    public class BallsNamesController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private Transform textsParent;
        [SerializeField] private float yOffset = 100f;

        [Inject] private GameSessionData _gameSessionData;

        private readonly Dictionary<Transform, TextMeshProUGUI> _ballNameTexts = new();

        public void Init(Ball[] balls)
        {
            ClearTexts();

            foreach (var ball in balls)
            {
                var nameText = Instantiate(characterNameText, textsParent);
                nameText.text = ball.Info.CharacterNameKey; 
                nameText.gameObject.SetActive(true);

                _ballNameTexts.Add(ball.transform, nameText);
            }
        }

        private void Update()
        {
            if (_ballNameTexts.Count == 0) return;

            var toRemove = new List<Transform>();

            foreach (var pair in _ballNameTexts)
            {
                var ballTransform = pair.Key;
                var nameText = pair.Value;

                if (ballTransform == null)
                {
                    if (nameText != null) Destroy(nameText.gameObject);
                    toRemove.Add(pair.Key);
                    continue;
                }

                nameText.transform.position = ballTransform.position + Vector3.up * yOffset;
            }

            foreach (var key in toRemove)
            {
                _ballNameTexts.Remove(key);
            }
        }

        private void ClearTexts()
        {
            foreach (var pair in _ballNameTexts)
            {
                if (pair.Value != null)
                {
                    Destroy(pair.Value.gameObject);
                }
            }

            _ballNameTexts.Clear();
        }
    }
}