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

        private Rect _rect;
        public Vector2Int Min => _min;
        public Vector2Int Max => _max;
        public Vector2Int TL => new Vector2Int(_min.x,_max.y);
        public Vector2Int TR => _max;
        public Vector2Int BL => _min;
        public Vector2Int BR => new Vector2Int(_max.x,_min.y);

        public Vector2Int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                RecalculateBounds();
            }
        }

        private void RecalculateBounds()
        {
            _min = new Vector2Int(_position.x - Mathf.FloorToInt(_size.x / 2f),
                _position.y - Mathf.FloorToInt(_size.y / 2f));
            _max = new Vector2Int(_position.x + Mathf.CeilToInt(_size.x / 2f),
                _position.y + Mathf.CeilToInt(_size.y / 2f));
        }

        public DungeonRoom(Vector2Int position, Vector2Int size)
        {
            _position = position;
            _size = size;
            RecalculateBounds();
        }

        public bool IsOverlappingWithRoom(DungeonRoom otherRoom)
        {
            return
                _min.x < otherRoom.Max.x &&
                _max.x > otherRoom.Min.x &&
                _min.y < otherRoom.Max.y &&
                _max.y > otherRoom.Min.y;
        }
    }
}