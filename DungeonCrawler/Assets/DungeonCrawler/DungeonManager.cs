using System.Collections.Generic;
using DungeonCrawler.DungeonGeneration;
using UnityEngine;
using UnityExtendedEditor.Attributes;

namespace DungeonCrawler
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] private DungeonGenerator _dungeonGenerator = null;
        [SerializeField] private Dungeon _myDungeon = null;
        [SerializeField] private TilemapVisualizer _tilemapVisualizer = null;
        public Dungeon CurrentDungeon => _myDungeon;
        
        [Button]    
        private void StartGeneration()
        {
            
            StartCoroutine(_dungeonGenerator.StartGeneratingDungeon(OnDungeonGenerated));
        }
        [Button]    
        private void TestTilemap()
        {
            _tilemapVisualizer.PaintSingleTile(new Vector2Int(5, 2));
            _tilemapVisualizer.PaintSingleTile(new Vector2Int(8, 1));
            _tilemapVisualizer.PaintSingleTile(new Vector2Int(4, 3));
            _tilemapVisualizer.PaintSingleTile(new Vector2Int(8, 4));
        }

        public void OnDungeonGenerated()
        {
            _myDungeon = _dungeonGenerator.GetGeneratedDungeon();
            _tilemapVisualizer.PaintTiles(_myDungeon.Levels[0].GetRoomPositions);
        }
    }
}