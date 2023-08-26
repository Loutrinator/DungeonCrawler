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

        public void OnDungeonGenerated()
        {
            _myDungeon = _dungeonGenerator.GetGeneratedDungeon();
            _tilemapVisualizer.PaintTiles(_myDungeon.Levels[0].GetRoomPositions);
        }
    }
}