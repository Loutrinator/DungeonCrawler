using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonCrawler.Render
{
    public class DungeonVisualizer : TilemapVisualizer
    {
        [SerializeField] private TileBase _floorTile = null;

        [SerializeField] private TileBase _wallTile = null;

        
        public void DrawRooms(HashSet<Vector2Int> roomPositions)
        {
            DrawTiles(roomPositions, _floorTile);
        }
        public void DrawWalls(HashSet<Vector2Int> wallPositions)
        {
            DrawTiles(wallPositions, _wallTile);
        }
    }
}