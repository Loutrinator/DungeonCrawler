
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap _tilemap = null;

    public void ClearTiles()
    {
        _tilemap.ClearAllTiles();
    }

    public void BoxFill(Vector2Int min, Vector2Int max, TileBase tile)
    {
        _tilemap.BoxFill((Vector3Int)min,tile,min.x,max.x,min.y,max.y);
    }
    
    protected void DrawTiles(IEnumerable<Vector2Int> positions, TileBase tileBase)
    {
        foreach (var position in positions)
        {
            DrawSingleTile(position, tileBase);
        }
    }

    private void DrawSingleTile(Vector2Int position, TileBase tile)
    {
        var tilePosition = _tilemap.WorldToCell((Vector3Int)position);
        _tilemap.SetTile(tilePosition,tile);
    }
}
