using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    public struct LevelLoadData
    {
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public List<Vector2> EnemyPositions;
    }
}