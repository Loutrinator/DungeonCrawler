﻿using DungeonCrawler.Utilities;
using UnityEngine;

namespace DungeonCrawler.Render
{
    public class DungeonRenderer : MonoBehaviour
    {
        [SerializeField] private DungeonManager _dungeonManager = null;
        public void OnDrawGizmos()
        {
            Dungeon dungeon = _dungeonManager.CurrentDungeon;
            if (dungeon == null)
            {
                return;
            }
            foreach (var dungeonLevel in dungeon.Levels)
            {
                foreach (var dungeonRoom in dungeonLevel.Rooms)
                {
                    Gizmos.color = dungeonRoom.IsGameplay ? Color.green : Color.cyan;
                    Vector3 BL = dungeonRoom.BL.ToVector3();
                    Vector3 BR = dungeonRoom.BR.ToVector3();
                    Vector3 TL = dungeonRoom.TL.ToVector3();
                    Vector3 TR = dungeonRoom.TR.ToVector3();
                    Gizmos.DrawLine(BL,TL);
                    Gizmos.DrawLine(TL,TR);
                    Gizmos.DrawLine(TR,BR);
                    Gizmos.DrawLine(BR,BL);
                }
            }
        }
    }
}