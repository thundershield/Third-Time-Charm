using UnityEngine;
using Random = System.Random;

namespace LevelGeneration
{
    public interface IWorldGenerator
    {
        public void Generate(Random random, Map map, Vector2Int size);

        // Map is an IWorldGenerator so even though it's public you can't accsess it -Kevin
        public Vector2 getSpawnPosition();
    }
}