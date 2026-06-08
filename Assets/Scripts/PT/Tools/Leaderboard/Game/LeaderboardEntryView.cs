using System;
using PT.Backend.Types;
using PT.Tools.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PT.Tools.Leaderboard.Game
{
    public enum EntryHighlightEnum
    {
        None,
        Player,
        Top1,
        Top2,
        Top3,
    }
    
    [Serializable]
    internal class EntryHighlightObjects
    {
        [SerializeField] private GameObject[] objects;
        
        public GameObject[] Objects => objects;
    } 
    
    public class LeaderboardEntryView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image iconImage;
        [Space] 
        [SerializeField] private SerializableKeyValue<EntryHighlightEnum, EntryHighlightObjects> entryHighlights;

        public void Set(LeaderboardEntry entry, Sprite icon = null, EntryHighlightEnum highlight = EntryHighlightEnum.None)
        {
            rankText.text = $"#{entry.Rank}";
            nameText.text = entry.DisplayName;
            scoreText.text = entry.Score.ToString();
            
            if (icon != null) iconImage.sprite = icon;
            
            if (highlight != EntryHighlightEnum.None)
            {
                 entryHighlights.Dictionary[highlight].Objects.SetActive(true);
            }
        }
    }
}