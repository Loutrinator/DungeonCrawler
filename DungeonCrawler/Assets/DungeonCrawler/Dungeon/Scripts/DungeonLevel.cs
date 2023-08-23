using System;
using System.Collections.Generic;

namespace DungeonCrawler
{
    [Serializable]
    public class DungeonLevel
    {
        #region Serialized Fields

        private List<DungeonRoom> _rooms = null;

        #endregion Serialized Fields
        
        public List<DungeonRoom> Rooms => _rooms;

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
    }
}