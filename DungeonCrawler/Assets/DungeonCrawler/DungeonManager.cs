using DungeonCrawler.DungeonGeneration;
using UnityEngine;
using UnityExtendedEditor.Attributes;

namespace DungeonCrawler
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] private DungeonGenerator _dungeonGenerator = null;
        [SerializeField] private Dungeon _myDungeon = null;
        public Dungeon CurrentDungeon => _myDungeon;
        
        [Button]    
        private void StartGeneration()
        {
            
            StartCoroutine(_dungeonGenerator.StartGeneratingDungeon());
            _myDungeon = _dungeonGenerator.GetGeneratedDungeon();
        }
    }
}