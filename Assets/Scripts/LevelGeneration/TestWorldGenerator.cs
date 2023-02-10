using System;
using UnityEngine;
using Random = System.Random;

namespace LevelGeneration
{
    public class TestWorldGenerator : IWorldGenerator
    {
        private const int ChunkSize = 16;

        private enum HalfChunkType
        {
            Road,
            Building
        }

        private const int HalfChunkSize = ChunkSize / 2;
        private HalfChunkType[] _halfChunks;
        private int _yChunks;
        private int _xChunks;

        public void Generate(Random random, Map map, Vector2Int size)
        {
            _yChunks = size.y / ChunkSize;
            _xChunks = size.x / ChunkSize;
            _halfChunks = new HalfChunkType[_yChunks * 2 * _xChunks * 2];
            Ground(random, map, size);
        }

        private void Ground(Random random, Map map, Vector2Int size)
        {
            var yHalfChunks = _yChunks * 2;
            var xHalfChunks = _xChunks * 2;

            Array.Fill(_halfChunks, HalfChunkType.Building);

            const int axisRoadCount = 2;

            for (var i = 0; i < axisRoadCount; i++)
            {
                var y = random.Next(yHalfChunks);

                for (var x = 0; x < xHalfChunks; x++) _halfChunks[x + y * xHalfChunks] = HalfChunkType.Road;
            }

            for (var i = 0; i < axisRoadCount; i++)
            {
                var x = random.Next(xHalfChunks);

                for (var y = 0; y < xHalfChunks; y++) _halfChunks[x + y * xHalfChunks] = HalfChunkType.Road;
            }

            for (var y = 0; y < yHalfChunks; y++)
            for (var x = 0; x < xHalfChunks; x++)
                switch (_halfChunks[x + y * xHalfChunks])
                {
                    case HalfChunkType.Road:
                        RoadChunk(map, x, y, size);
                        break;
                    case HalfChunkType.Building:
                        BuildingChunk(random, map, x, y, size);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        private static void RoadChunk(Map map, int halfChunkX, int halfChunkY, Vector2Int size)
        {
            var startY = halfChunkY * HalfChunkSize;
            var startX = halfChunkX * HalfChunkSize;

            map.SetRect(TileType.Sand, startX, startY,
                HalfChunkSize, HalfChunkSize);
        }

        private static void BuildingChunk(Random random, Map map, int halfChunkX, int halfChunkY, Vector2Int size)
        {
            var startY = halfChunkY * HalfChunkSize;
            var startX = halfChunkX * HalfChunkSize;

            const int floorHeight = 6;
            var maxFloors = size.y / floorHeight;
            var floors = random.Next(1, maxFloors + 1);

            map.SetRect(TileType.Grass, startX, startY, HalfChunkSize, HalfChunkSize);
        }
    }
}