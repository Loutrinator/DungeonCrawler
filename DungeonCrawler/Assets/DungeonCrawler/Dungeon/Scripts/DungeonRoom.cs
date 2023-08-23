using System;
using UnityEngine;

namespace DungeonCrawler
{
    [Serializable]
    public class DungeonRoom
    {
        [SerializeField]
        private Vector2Int _size = Vector2Int.zero;
        [SerializeField]
        private Vector2Int _position = Vector2Int.zero;

        private Vector2Int _min = Vector2Int.zero;
        private Vector2Int _max = Vector2Int.zero;
        
        public Vector2Int Min => _min;
        public Vector2Int Max => _max;
        public Vector2Int TL => new Vector2Int(_min.x,_max.y);
        public Vector2Int TR => _max;
        public Vector2Int BL => _min;
        public Vector2Int BR => new Vector2Int(_max.x,_min.y);
        public Vector2Int Position => _position;

        public DungeonRoom(Vector2Int position, Vector2Int size)
        {
            _position = position;
            _size = size;
            _min = position - size / 2;
            _max = position + size / 2;
        }
    }
}