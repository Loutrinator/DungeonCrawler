using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.Utilities.Math;
using UnityEngine;

namespace DungeonCrawler
{
    [Serializable]
    public class DungeonLevel
    {
        #region Serialized Fields

        private List<DungeonRoom> _rooms = null;

        #endregion Serialized Fields
        
        public List<DungeonRoom> Rooms => _rooms;
        private HashSet<Vector2Int> _roomPositions = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> _wallPositions = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> RoomPositions => _roomPositions;
        public HashSet<Vector2Int> WallPositions => _wallPositions;

        public void ComputeFloorPositions()
        {
            _roomPositions.Clear();
                foreach (var room in _rooms)
                {
                    if (room.IsGameplay)
                    {
                        _roomPositions.UnionWith(room.GetPositions());
                    }
                }
        }

        public void SetWallPositions(HashSet<Vector2Int> wallPositions)
        {
            _wallPositions = wallPositions;
        }

        public DungeonLevel()
        {
            _rooms = new List<DungeonRoom>();
        }
        public void AddRoom(DungeonRoom room)
        {
            if (room != null)
            {
                _rooms.Add(room);
            }
        }

        public List<Point> GetGameplayRoomPoints()
        {
            return _rooms.FindAll(r => r.IsGameplay).Select(r => r.Point).ToList();
        }

    }
}