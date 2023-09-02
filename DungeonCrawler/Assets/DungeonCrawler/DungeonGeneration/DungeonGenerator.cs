using System;
using System.Collections;
using System.Collections.Generic;
using DungeonCrawler.Utilities.Math;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using Edge = DungeonCrawler.Utilities.Math.Edge;
using Graph = DungeonCrawler.Utilities.Math.Graph;
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
        private DungeonLevel _currentDungeonlevel;
        private Action _onGenerationFinished = null; 
        
        private Graph _mst = null;
        
        public void StartGeneratingDungeon(Action onGenerationFinished)
        {
            _dungeon = new Dungeon();
            _onGenerationFinished = onGenerationFinished;
            for (int i = 0; i < _levelAmount; i++)
            {
                _currentDungeonlevel = GenerateDungeonLevel(_roomsPerLevel);
                _dungeon.AddLevel(_currentDungeonlevel);
                SeparateRooms();
                SortRoomsByArea();
                RemoveRemainingOverlaps();
                SelectGameplayRooms();
                DelaunayOnGameplayRooms();
            }
        }

        private void OnDelaunayFinished(Graph graph)
        {
            Graph mst = graph.GetMinnimumSpanningTree();
            _mst = mst;
            AddOptionalPathways(mst, graph);
            Graph corridors = TraceCorridors(mst);
            IncludeOverlapRoomsWithCorridors();
            ComputeFloorPositions();
            ComputeCorridorCells(corridors);
            ComputeWallPositions();
            AddContentInRooms();
            
            foreach (var dungeonlevel in _dungeon.Levels)
            {
                dungeonlevel.ComputeFloorPositions();
                dungeonlevel.SetWallPositions(WallGenerator.GetWalls(dungeonlevel));
            }
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

        private void AddOptionalPathways(Graph mst, Graph graph)
        {
            
        }

        private Graph TraceCorridors(Graph mst)
        {
            return new Graph();
        }

        private void IncludeOverlapRoomsWithCorridors()
        {
            
        }

        private void ComputeFloorPositions()
        {
        }

        private void ComputeCorridorCells(Graph corridors)
        {
        }

        private void ComputeWallPositions()
        {
            
        }

        private void AddContentInRooms()
        {
            
        }

        private void DelaunayOnGameplayRooms()
        {
            var points = _currentDungeonlevel.GetGameplayRoomPoints();
            Graph graph = new Graph();
            graph.points = points;
            DelaunayManager.Instance.ResetManager();
            DelaunayManager.Instance.IncrementalTriangulation(graph);
            DelaunayManager.Instance.FlipStart(graph, OnDelaunayFinished);
            
        }
        
        private void RemoveRemainingOverlaps()
        {
            //TODO
        }

        private void SortRoomsByArea()
        {
            _currentDungeonlevel.Rooms.Sort((r1,r2) => r2.Area.CompareTo(r1.Area));
        }

        private void SelectGameplayRooms()
        {
            int amountOfRoomsToKeep = Mathf.RoundToInt(_gameplayRoomsPercentage * _currentDungeonlevel.Rooms.Count);

            for (int i = 0; i < amountOfRoomsToKeep; i++)
            {
                _currentDungeonlevel.Rooms[i].SetGameplay(true);
            }
        }

        public Dungeon GetGeneratedDungeon()
        {
            return _dungeon;
        }

        private void SeparateRooms()
        {
            int tryCount = 0;
            int roomsWithoutOverlaps;
            Vector2[] roomOffsetDirections = new Vector2[_currentDungeonlevel.Rooms.Count];
            do
            {
                tryCount++;
                roomsWithoutOverlaps = 0;
                
                for (int i = 0; i < _currentDungeonlevel.Rooms.Count; i++)
                {
                    DungeonRoom room = _currentDungeonlevel.Rooms[i];
                    Vector2 separationDirection = new Vector2();
                    int overlapCount = 0;
                    
                    foreach (var otherRoom in _currentDungeonlevel.Rooms)
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

                for (int i = 0; i < _currentDungeonlevel.Rooms.Count; i++)
                {
                    DungeonRoom room = _currentDungeonlevel.Rooms[i];
                    Vector2 offset = (roomOffsetDirections[i] * _separationIntensity);
                    room.Position -= new Vector2Int(Mathf.RoundToInt(offset.x),Mathf.RoundToInt(offset.y));
                }
            } while (roomsWithoutOverlaps != _currentDungeonlevel.Rooms.Count && tryCount < 1000);
            
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