using System.Collections.Generic;
using DungeonCrawler.DungeonGeneration;
using DungeonCrawler.Render;
using UnityEngine;
using UnityEngine.Serialization;
using UnityExtendedEditor.Attributes;

namespace DungeonCrawler
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] private DungeonGenerator _dungeonGenerator = null;
        [SerializeField] private Dungeon _myDungeon = null;
        [SerializeField] private DungeonVisualizer _dungeonVisualizer = null;
        public Dungeon CurrentDungeon => _myDungeon;
        
        [Button]    
        private void StartGeneration()
        {
            
            StartCoroutine(_dungeonGenerator.StartGeneratingDungeon(OnDungeonGenerated));
        }

        public void OnDungeonGenerated()
        {
            _myDungeon = _dungeonGenerator.GetGeneratedDungeon();
            _dungeonVisualizer.ClearTiles();
            _dungeonVisualizer.DrawRooms(_myDungeon.Levels[0].RoomPositions);
            _dungeonVisualizer.DrawWalls(_myDungeon.Levels[0].WallPositions);
        }
    }
}