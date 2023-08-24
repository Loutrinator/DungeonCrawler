using System;
using UnityEngine;

namespace DungeonCrawler
{
    [Serializable]
    public class DungeonRoom
    {
        [SerializeField]
        private Vector2 _size = Vector2.zero;
        [SerializeField]
        private Vector2 _position = Vector2.zero;

        private Vector2 _min = Vector2.zero;
        private Vector2 _max = Vector2.zero;

        private Rect _rect;
        public Vector2 Min => _min;
        public Vector2 Max => _max;
        public Vector2 TL => new Vector2(_min.x,_max.y);
        public Vector2 TR => _max;
        public Vector2 BL => _min;
        public Vector2 BR => new Vector2(_max.x,_min.y);

        public Vector2 Position
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
            _min = new Vector2(_position.x - _size.x / 2f,
                _position.y - _size.y / 2f);
            _max = new Vector2(_position.x + _size.x / 2f,
                _position.y + _size.y / 2f);
        }

        public DungeonRoom(Vector2 position, Vector2 size)
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