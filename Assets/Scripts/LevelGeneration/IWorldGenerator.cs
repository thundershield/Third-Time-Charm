using UnityEngine;
using Random = System.Random;

namespace LevelGeneration
{
    public interface IWorldGenerator
    {
        public LevelLoadData Generate(Random random, Map map, Vector2Int size);
        public LevelLoadData GetLastLoadData();
    }
}