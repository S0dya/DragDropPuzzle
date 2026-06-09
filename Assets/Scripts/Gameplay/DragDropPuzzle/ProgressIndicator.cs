using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.DragDropPuzzle
{
    public class ProgressIndicator : MonoBehaviour
    {
        [SerializeField] private Image progressFill;
        [SerializeField] private TextMeshProUGUI progressText;

        private int _totalPieces;
        private int _placedPieces;

        public void Initialize(int totalPieces)
        {
            _totalPieces = totalPieces;
            _placedPieces = 0;
            UpdateUI();
        }

        public void OnPiecePlaced()
        {
            _placedPieces++;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_totalPieces > 0)
            {
                progressFill.fillAmount = (float)_placedPieces / _totalPieces;
            }
            else
            {
                progressFill.fillAmount = 0f;
            }

            progressText.text = $"{_placedPieces}/{_totalPieces}";
        }
    }
}
