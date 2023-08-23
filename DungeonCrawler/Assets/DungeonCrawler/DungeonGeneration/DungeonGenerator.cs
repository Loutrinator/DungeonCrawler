using UnityEngine;

namespace DungeonCrawler.DungeonGeneration
{
    public static class DungeonGenerator
    {
        public static Dungeon GenerateDungeon(int levelAmount, int roomAmountPerLevel)
        {
            Dungeon dungeon = new Dungeon();
            for (int i = 0; i < levelAmount; i++)
            {
                DungeonLevel dungeonlevel = GenerateDungeonLevel(roomAmountPerLevel);
                dungeon.AddLevel(dungeonlevel);
            }
            return dungeon;
        }

        private static DungeonLevel GenerateDungeonLevel(int roomAmountPerLevel)
        {
            DungeonLevel dungeonLevel = new DungeonLevel();
            for (int i = 0; i < roomAmountPerLevel; i++)
            {
                Vector2Int pos = new Vector2Int(Random.Range(-10, 10),Random.Range(-10, 10));
                Vector2Int size = new Vector2Int(Random.Range(5, 10),Random.Range(5, 10));
                DungeonRoom dungeonRoom = new DungeonRoom(pos,size);
                dungeonLevel.AddRoom(dungeonRoom);
            }

            return dungeonLevel;
        }
    }
}