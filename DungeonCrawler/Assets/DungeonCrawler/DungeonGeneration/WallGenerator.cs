using System.Collections.Generic;
using UnityEngine;


namespace DungeonCrawler.DungeonGeneration
{
    public static class WallGenerator
    {

        public static HashSet<Vector2Int> GetWalls(DungeonLevel dungeonLevel)
        {
            return FindWallPositions(dungeonLevel.RoomPositions);
        }

        private static HashSet<Vector2Int> FindWallPositions(HashSet<Vector2Int> floorPositions)
        {
            HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
            

            foreach (var floorPosition in floorPositions)
            {
                CheckForDirection(floorPosition, Vector2Int.left);
                CheckForDirection(floorPosition, Vector2Int.right);
                CheckForDirection(floorPosition, Vector2Int.up);
                CheckForDirection(floorPosition, Vector2Int.down);
            }
            
            void CheckForDirection(Vector2Int floorPosition, Vector2Int direction)
            {
                var wallPos = floorPosition + direction;
                if (!floorPositions.Contains(wallPos))
                {
                    wallPositions.Add(wallPos);
                }
            }

            return wallPositions;
        }

    }

}