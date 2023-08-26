
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap _tilemap = null;

    [SerializeField]
    private TileBase _tileBase = null;

    public void PaintTiles(IEnumerable<Vector2Int> positions)
    {
        PaintTiles(positions, _tilemap, _tileBase);
    }

    public void DebugPaintTiles()
    {
        PaintSingleTile(new Vector2Int(0,0), _tilemap, _tileBase);
        PaintSingleTile(new Vector2Int(0,1), _tilemap, _tileBase);
        PaintSingleTile(new Vector2Int(1,0), _tilemap, _tileBase);
        PaintSingleTile(new Vector2Int(1,1), _tilemap, _tileBase);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tileBase)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(position, tilemap, tileBase);
        }
    }

    private void PaintSingleTile(Vector2Int position, Tilemap tilemap, TileBase tile)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile((Vector3Int)position,tile);
    }
}
