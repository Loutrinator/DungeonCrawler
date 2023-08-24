using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace DungeonCrawler.DungeonGeneration
{
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField]
        private int _levelAmount = 1;
        [SerializeField]
        private int _roomsPerLevel = 1;
        [SerializeField] 
        private Vector2Int roomPositionRangeX = new Vector2Int();
        [SerializeField] 
        private Vector2Int roomPositionRangeY = new Vector2Int();
        [SerializeField]
        private Vector2Int roomSizeRangeX = new Vector2Int();
        [SerializeField]
        private Vector2Int roomSizeRangeY = new Vector2Int();
        [SerializeField]
        private float _separationIntensity = 1f;
        [SerializeField]
        private float _gameplayRoomsPercentage = 0.3f;
        
        private Dungeon _dungeon;
        public IEnumerator StartGeneratingDungeon()
        {
            _dungeon = new Dungeon();
            for (int i = 0; i < _levelAmount; i++)
            {
                DungeonLevel dungeonlevel = GenerateDungeonLevel(_roomsPerLevel);
                _dungeon.AddLevel(dungeonlevel);
                yield return new WaitForSeconds(1f);
                yield return SeparateRooms(i);
                SortRoomsByArea(i);
                RemoveRemainingOverlaps(i);
                SelectGameplayRooms(i);
            }
        }

        private void RemoveRemainingOverlaps(int dungeonLevelNumber)
        {
            DungeonLevel dungeonLevel = _dungeon.Levels[dungeonLevelNumber];

            //TODO
        }

        private void SortRoomsByArea(int dungeonLevelNumber)
        {
            DungeonLevel dungeonLevel = _dungeon.Levels[dungeonLevelNumber];
            dungeonLevel.Rooms.Sort((r1,r2) => r2.Area.CompareTo(r1.Area));
        }

        private void SelectGameplayRooms(int dungeonLevelNumber)
        {
            DungeonLevel dungeonLevel = _dungeon.Levels[dungeonLevelNumber];
            int amountOfRoomsToKeep = Mathf.RoundToInt(_gameplayRoomsPercentage * dungeonLevel.Rooms.Count);

            for (int i = 0; i < amountOfRoomsToKeep; i++)
            {
                dungeonLevel.Rooms[i].SetGameplay(true);
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
                    Vector2 offset = (roomOffsetDirections[i] * _separationIntensity);
                    room.Position -= new Vector2Int(Mathf.RoundToInt(offset.x),Mathf.RoundToInt(offset.y));
                }

                yield return null;
            } while (roomsWithoutOverlaps != dungeonLevel.Rooms.Count && tryCount < 1000);
            
        }

        private DungeonLevel GenerateDungeonLevel(int roomAmountPerLevel)
        {
            DungeonLevel dungeonLevel = new DungeonLevel();
            for (int i = 0; i < roomAmountPerLevel; i++)
            {
                Vector2Int pos = new Vector2Int(Random.Range(roomPositionRangeX.x, roomPositionRangeX.y),Random.Range(roomPositionRangeY.x, roomPositionRangeY.y));
                Vector2Int size = new Vector2Int(Random.Range(roomSizeRangeX.x, roomSizeRangeX.y),Random.Range(roomSizeRangeY.x, roomSizeRangeY.y));
                DungeonRoom dungeonRoom = new DungeonRoom(pos,size);
                dungeonLevel.AddRoom(dungeonRoom);
            }

            return dungeonLevel;
        }
    }
}