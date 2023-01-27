using UnityEngine;
using Random = System.Random;

public interface IWorldGenerator
{
    public void Generate(Random random, Map map, Vector2Int size);
}