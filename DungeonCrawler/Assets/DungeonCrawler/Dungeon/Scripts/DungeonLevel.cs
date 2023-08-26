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

        public HashSet<Vector2Int> GetRoomPositions
        {
            get
            {
                HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
                foreach (var room in _rooms)
                {
                    roomPositions.UnionWith(room.GetPositions());
                }

                return roomPositions;
            }
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