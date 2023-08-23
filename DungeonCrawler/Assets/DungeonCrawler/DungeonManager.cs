using DungeonCrawler.DungeonGeneration;
using UnityEngine;

namespace DungeonCrawler
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] private Dungeon _myDungeon = null;
        public Dungeon CurrentDungeon => _myDungeon;
        private void Start()
        {
            _myDungeon = DungeonGenerator.GenerateDungeon(1,20);
        }
    }
}