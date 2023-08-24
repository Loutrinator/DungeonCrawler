using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace DungeonCrawler.DungeonGeneration
{
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] 
        private Vector2 roomPositionRangeX = new Vector2();
        [SerializeField] 
        private Vector2 roomPositionRangeY = new Vector2();
        [SerializeField]
        private Vector2 roomSizeRangeX = new Vector2();
        [SerializeField]
        private Vector2 roomSizeRangeY = new Vector2();
        [SerializeField]
        private float _separationIntensity = 1f;
        
        private Dungeon _dungeon;
        public IEnumerator StartGeneratingDungeon(int levelAmount, int roomAmountPerLevel)
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

        public Dungeon GetGeneratedDungeon()
        {
            return _dungeon;
        }

        private IEnumerator SeparateRooms(int dungeonLevelNumber)
        {
            DungeonLevel dungeonLevel = _dungeon.Levels[dungeonLevelNumber];
            int tryCount = 0;
            int roomsWithoutOverlaps;
            Vector2[] roomOffsetDirections = new Vector2[dungeonLevel.Rooms.Count];
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
                        roomOffsetDirections[i] = separationDirection;
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
                    room.Position -= roomOffsetDirections[i] * _separationIntensity;
                }

                yield return null;
            } while (roomsWithoutOverlaps != dungeonLevel.Rooms.Count && tryCount < 1000);
            
        }

        private DungeonLevel GenerateDungeonLevel(int roomAmountPerLevel)
        {
            DungeonLevel dungeonLevel = new DungeonLevel();
            for (int i = 0; i < roomAmountPerLevel; i++)
            {
                Vector2 pos = new Vector2(Random.Range(roomPositionRangeX.x, roomPositionRangeX.y),Random.Range(roomPositionRangeY.x, roomPositionRangeY.y));
                Vector2 size = new Vector2(Random.Range(roomSizeRangeX.x, roomSizeRangeX.y),Random.Range(roomSizeRangeY.x, roomSizeRangeY.y));
                DungeonRoom dungeonRoom = new DungeonRoom(pos,size);
                dungeonLevel.AddRoom(dungeonRoom);
            }

            return dungeonLevel;
        }
    }
}