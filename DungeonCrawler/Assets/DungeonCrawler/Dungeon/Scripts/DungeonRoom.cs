using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DungeonCrawler.Utilities.Math;
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
        private Point _point = null;
        
        private bool _isGameplay;
        public Vector2Int Min => _min;
        public Vector2Int Max => _max;
        public Vector2Int TL => new Vector2Int(_min.x,_max.y);
        public Vector2Int TR => _max;
        public Vector2Int BL => _min;
        public Vector2Int BR => new Vector2Int(_max.x,_min.y);

        public Point Point => _point;

        public Vector2Int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                _point = new Point(new Vector3(_position.x, _position.y, 0));
                RecalculateBounds();
            }
        }

        public int Area => _size.x * _size.y;
        public bool IsGameplay => _isGameplay;

        private void RecalculateBounds()
        {
            _min = new Vector2Int(Mathf.FloorToInt(_position.x - _size.x / 2f),
                Mathf.CeilToInt(_position.y - _size.y / 2f));
            _max = new Vector2Int(Mathf.FloorToInt(_position.x + _size.x / 2f),
                Mathf.CeilToInt(_position.y + _size.y / 2f));
        }

        public DungeonRoom(Vector2Int position, Vector2Int size)
        {
            _position = position;
            _point = new Point(new Vector3(_position.x, _position.y, 0));
            _size = size;
            _isGameplay = false;
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

        public void SetGameplay(bool isGameplay)
        {
            _isGameplay = isGameplay;
        }

        public HashSet<Vector2Int> GetPositions()
        {
            HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
            for (int x = _min.x; x < _max.x; x++)
            {
                for (int y = _min.y; y < _max.y; y++)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }

            return positions;
        }
    }
}