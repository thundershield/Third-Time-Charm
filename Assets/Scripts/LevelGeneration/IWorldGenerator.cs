using UnityEngine;
using Random = System.Random;

namespace LevelGeneration
{
    public interface IWorldGenerator
    {
        public void Generate(Random random, Map map, Vector2Int size);
    }
}