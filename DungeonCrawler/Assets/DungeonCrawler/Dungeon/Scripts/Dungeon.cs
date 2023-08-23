using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler
{
    [Serializable]
    public class Dungeon
    {
        [SerializeField]
        private List<DungeonLevel> _levels = null;
        
        [SerializeField]
        public List<DungeonLevel> Levels => _levels;

        public Dungeon()
        {
            _levels = new List<DungeonLevel>();
        }
        public void AddLevel(DungeonLevel dungeonlevel)
        {
            if (dungeonlevel != null)
            {
                _levels.Add(dungeonlevel);
            }
            else
            {
                Debug.Log("Dungeon Level null when adding it to the dungeon.");
            }
        }
    }
}