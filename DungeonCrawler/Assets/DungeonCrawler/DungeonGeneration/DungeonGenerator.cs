using System;
using System.Collections;
using System.Collections.Generic;
using DungeonCrawler.Utilities.Math;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
        private Action _onGenerationFinished = null; 
        
        public IEnumerator StartGeneratingDungeon(Action onGenerationFinished)
        {
            _dungeon = new Dungeon();
            _onGenerationFinished = onGenerationFinished;
            for (int i = 0; i < _levelAmount; i++)
            {
                DungeonLevel dungeonlevel = GenerateDungeonLevel(_roomsPerLevel);
                _dungeon.AddLevel(dungeonlevel);
                yield return new WaitForSeconds(1f);
                yield return SeparateRooms(i);
                SortRoomsByArea(i);
                RemoveRemainingOverlaps(i);
                SelectGameplayRooms(i);
                DelaunayOnGameplayRooms(i);
            }
        }

        private void DelaunayOnGameplayRooms(int dungeonLevelNumber)
        {
            DungeonLevel dungeonLevel = _dungeon.Levels[dungeonLevelNumber];
            var points = dungeonLevel.GetGameplayRoomPoints();
            Graph graph = new Graph();
            graph.points = points;
            DelaunayManager.Instance.ResetManager();
            DelaunayManager.Instance.IncrementalTriangulation(graph);
            DelaunayManager.Instance.FlipStart(graph, MinimumSpanningTree);
            
        }

        private void MinimumSpanningTree(Graph graph)
        {
            _onGenerationFinished?.Invoke();
            /*
             Define an empty List A = [ ]
            For each vertex V
            Make-Set(V)
            Sort edges of graph order by weight
            For each edge E (u, v)
            If Find-Set(u) != Find-Set(v)
            Append E (u, v) in A
            Union (u, v)
            Return A
            */
        }
        
        /*
        public static void Kruskal(Graph graph)
        {
            int verticesCount = graph.points.Count;
            Edge[] result = new Edge[verticesCount];
            int i = 0;
            int e = 0;

            graph.edges.Sort((e1, e2) => e1.Length.CompareTo(e2.Length));  //, delegate (Edge a, Edge b)
            //{
            //    return a.Weight.CompareTo(b.Weight);
            //});

            Subset[] subsets = new Subset[verticesCount];

            for (int v = 0; v < verticesCount; ++v)
            {
                subsets[v].Parent = v;
                subsets[v].Rank = 0;
            }

            while (e < verticesCount - 1)
            {
                Edge nextEdge = graph.edge[i++];
                int x = Find(subsets, nextEdge.Source);
                int y = Find(subsets, nextEdge.Destination);

                if (x != y)
                {
                    result[e++] = nextEdge;
                    Union(subsets, x, y);
                }
            }

            Print(result, e);
        }
        */
        
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