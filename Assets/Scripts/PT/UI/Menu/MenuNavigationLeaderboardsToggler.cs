using PT.Backend.Interfaces;
using PT.Tools.Helper;
using UnityEngine;
using Zenject;

namespace PT.UI.Menu
{
    public class MenuNavigationLeaderboardsToggler : MonoBehaviour
    {
        [SerializeField] private Transform leaderboardButtonContainer;
        
        [Inject (Optional = true)] private IAuthentificationService _authentificationService;

        private void Start()
        {
            if (_authentificationService == null)
            {
                leaderboardButtonContainer.SetActive(false);
            }
        }
    }
}