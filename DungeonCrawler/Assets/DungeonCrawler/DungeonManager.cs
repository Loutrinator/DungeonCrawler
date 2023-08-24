using DungeonCrawler.DungeonGeneration;
using UnityEngine;
using UnityExtendedEditor.Attributes;

namespace DungeonCrawler
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] private Dungeon _myDungeon = null;
        public Dungeon CurrentDungeon => _myDungeon;
        
        [Button]    
        private void StartGeneration()
        {
            
            StartCoroutine(DungeonGenerator.StartGeneratingDungeon(1, 20));
            _myDungeon = DungeonGenerator.GetGeneratedDungeon();
        }
    }
}