using UnityEngine;
using UnityEngine.UI;

namespace PT.UI.Buttons
{
    public class ExitButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            if (!button) button = GetComponent<Button>();
            
            button.onClick.AddListener(Application.Quit);
        }
    }
}