using System.Collections;
using UnityEngine;

namespace DungeonCrawler.DungeonGeneration
{
    public static class DungeonGenerator
    {
        private static Dungeon _dungeon;
        public static IEnumerator StartGeneratingDungeon(int levelAmount, int roomAmountPerLevel)
        {
            _dungeon = new Dungeon();
            for (int i = 0; i < levelAmount; i++)
            {
                DungeonLevel dungeonlevel = GenerateDungeonLevel(roomAmountPerLevel);
                _dungeon.AddLevel(dungeonlevel);
                yield return new WaitForSeconds(1f);
                yield return SeparateRooms(i);
            }
        }

        public static Dungeon GetGeneratedDungeon()
        {
            return _dungeon;
        }

        private static IEnumerator SeparateRooms(int dungeonLevelNumber)
        {
            DungeonLevel dungeonLevel = _dungeon.Levels[dungeonLevelNumber];
            int tryCount = 0;
            int roomsWithoutOverlaps;
            Vector2Int[] roomOffsetDirections = new Vector2Int[dungeonLevel.Rooms.Count];
            do
            {
                tryCount++;
                roomsWithoutOverlaps = 0;
                
                for (int i = 0; i < dungeonLevel.Rooms.Count; i++)
                {
                    DungeonRoom room = dungeonLevel.Rooms[i];
                    Vector2 separationDirection = new Vector2();
                    int overlapCount = 0;
                    
                    foreach (var otherRoom in dungeonLevel.Rooms)
                    {
                        if (otherRoom.IsOverlappingWithRoom(room))
                        {
                            overlapCount++;
                            separationDirection += otherRoom.Position - room.Position;
                        }
                    }

                    if (overlapCount > 0)
                    {
                        separationDirection /= (float)overlapCount;
                        roomOffsetDirections[i] = new Vector2Int(Mathf.CeilToInt(separationDirection.x),Mathf.CeilToInt(separationDirection.y));
                    }
                    else
                    {
                        roomOffsetDirections[i] = Vector2Int.zero;
                        roomsWithoutOverlaps++;
                    }
                }

                for (int i = 0; i < dungeonLevel.Rooms.Count; i++)
                {
                    DungeonRoom room = dungeonLevel.Rooms[i];
                    room.Position -= roomOffsetDirections[i];
                }

                yield return new WaitForSeconds(0.1f);
            } while (roomsWithoutOverlaps != dungeonLevel.Rooms.Count && tryCount < 1000);
            
        }

        private static DungeonLevel GenerateDungeonLevel(int roomAmountPerLevel)
        {
            DungeonLevel dungeonLevel = new DungeonLevel();
            for (int i = 0; i < roomAmountPerLevel; i++)
            {
                Vector2Int pos = new Vector2Int(Random.Range(-15, 15),Random.Range(-15, 15));
                Vector2Int size = new Vector2Int(Random.Range(3, 15),Random.Range(3, 15));
                DungeonRoom dungeonRoom = new DungeonRoom(pos,size);
                dungeonLevel.AddRoom(dungeonRoom);
            }

            return dungeonLevel;
        }
    }
}