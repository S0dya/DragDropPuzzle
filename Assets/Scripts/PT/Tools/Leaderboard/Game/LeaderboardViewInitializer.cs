using System;
using System.Collections.Generic;
using PT.Backend.Types;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace PT.Tools.Leaderboard.Game
{
    [Serializable]
    public class LeaderboardViewInitializer 
    {
        [SerializeField] private Transform mainContainer;
        [SerializeField] private Transform outsideContainer;
        [Space]
        [SerializeField] private LeaderboardEntryView leaderboardEntryViewPrefab;

        private ObjectPool<LeaderboardEntryView> _entriesViewsPool;
        
        public List<LeaderboardEntryView> SpawnEntries(IEnumerable<LeaderboardEntry> entries)
        {
            CreatePool();
            
            var list = new List<LeaderboardEntryView>();

            foreach (var entry in entries)
            {
                var view = _entriesViewsPool.Get();
                view.transform.SetParent(mainContainer);
                view.Set(entry, highlight: GetHighlightTypeForEntry(entry));
                list.Add(view);
            }

            return list;
        }

        public LeaderboardEntryView SpawnPlayer(LeaderboardEntry entry, bool inside)
        {
            var view = _entriesViewsPool.Get();
            view.Set(entry, highlight: EntryHighlightEnum.Player);
            view.transform.SetParent(inside ? mainContainer : outsideContainer);
            
            if (!inside) view.transform.localPosition = Vector3.zero;
            
            return view;
        }

        private void CreatePool()
        {
            if (_entriesViewsPool != null) return;
            
            _entriesViewsPool = new ObjectPool<LeaderboardEntryView>(
                () => GameObject.Instantiate(leaderboardEntryViewPrefab, mainContainer), 
                entry => entry.gameObject.SetActive(true),
                entry => entry.gameObject.SetActive(false),
                defaultCapacity: 30);
        }
        
        private EntryHighlightEnum GetHighlightTypeForEntry(LeaderboardEntry entry)
        {
            return entry.Rank switch
            {
                1 => EntryHighlightEnum.Top1,
                2 => EntryHighlightEnum.Top2,
                3 => EntryHighlightEnum.Top3,
                _ => EntryHighlightEnum.None
            };
        }
    }
}