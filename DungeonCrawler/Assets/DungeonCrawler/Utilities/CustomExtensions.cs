using UnityEngine;

namespace DungeonCrawler.Utilities
{
    public static class CustomExtensions
    {
        public static Vector3 ToVector3(this Vector2Int vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }
    }
}