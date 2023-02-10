using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace LevelGeneration
{
    public class GridWorldGenerator : IWorldGenerator
    {
        public void Generate(Random random, Map map, Vector2Int size)
        {
            var seed = random.Next();
            ChooseRooms(seed);
            GenerateBorder(map);
            GenerateRooms(map);
        }

        private enum RoomType
        {
            AllOpenSpawn,
            AllOpenExit,
            LeftRightOpen,
            AllOpen,
            Optional
        }

        public Vector2 SpawnPosition { get; private set; }
        public Vector2 ExitPosition { get; private set; }
        
        private static readonly Dictionary<RoomType, int> RoomTypePriority = new()
        {
            { RoomType.Optional, 0 },
            { RoomType.LeftRightOpen, 1 },
            { RoomType.AllOpen, 2 },
            { RoomType.AllOpenSpawn, 3 },
            { RoomType.AllOpenExit, 3 }
        };

        private readonly TileType[] _borderTiles =
            Map.TileCategories[TileCategory.Wall].Concat(Map.TileCategories[TileCategory.Plant]).ToArray();

        private const int MapSize = 10;

        private const int MaxHorizontalMoves = MapSize;
        private const int PossibleMoves = 10;
        private const int MoveDown = PossibleMoves;
        private const int MoveLeft = PossibleMoves / 2;

        private readonly RoomType[] _rooms = new RoomType[MapSize * MapSize];
        private Random _random;

        private void SetRoom(int x, int y, RoomType roomType)
        {
            if (x is < 0 or >= MapSize || y is < 0 or >= MapSize) return;
            
            // Replacing the current room with a room type that has fewer openings could
            // make the level impossible to complete.
            var oldRoomType = GetRoom(x, y);
            if (RoomTypePriority[roomType] < RoomTypePriority[oldRoomType]) return;

            _rooms[x + y * MapSize] = roomType;
        }

        private RoomType GetRoom(int x, int y)
        {
            if (x is < 0 or >= MapSize || y is < 0 or >= MapSize) return RoomType.Optional;
            return _rooms[x + y * MapSize];
        }

        private T Choose<T>(T[] array)
        {
            return array[_random.Next(array.Length)];
        }

        private void GenerateRooms(Map map)
        {
            for (var x = 0; x < MapSize; x++)
            for (var y = 0; y < MapSize; y++)
            {
                var roomTemplate = Choose(GetRoom(x, y) switch
                {
                    RoomType.LeftRightOpen => Rooms.LeftRightOpen,
                    RoomType.AllOpen => Rooms.AllOpen,
                    RoomType.AllOpenSpawn => Rooms.AllOpenSpawn,
                    RoomType.AllOpenExit => Rooms.AllOpenExit,
                    _ => Rooms.Optional
                });

                var roomX = x * Rooms.RoomWidth;
                var roomY = y * Rooms.RoomHeight;

                for (var i = 0; i < roomTemplate.Length; i++)
                {
                    var tileX = i % Rooms.RoomWidth;
                    var tileY = Rooms.RoomHeight - 1 - i / Rooms.RoomWidth;
                    var tile = Choose(Map.TileCategories[Rooms.CharCategories[roomTemplate[i]]]);
                    var tilePosition = new Vector3Int(roomX + tileX, roomY + tileY, 0);
                    var centeredTilePosition = tilePosition + new Vector3(0.5f, 0.5f);

                    switch (tile)
                    {
                        case TileType.Start:
                            SpawnPosition = centeredTilePosition;
                            break;
                        case TileType.End:
                            ExitPosition = centeredTilePosition;
                            break;
                    }

                    map.SetTile(tile, tilePosition.x, tilePosition.y);
                }
            }
        }

        private void GenerateBorder(Map map)
        {
            const int mapWidth = MapSize * Rooms.RoomWidth;
            const int mapHeight = MapSize * Rooms.RoomHeight;

            var borderTile = Choose(_borderTiles);
            map.SetRect(borderTile, -1, -1, mapWidth + 2, 1);
            map.SetRect(borderTile, -1, -1, 1, mapHeight + 2);
            map.SetRect(borderTile, -1, mapHeight, mapWidth + 2, 1);
            map.SetRect(borderTile, mapWidth, -1, 1, mapHeight + 2);
        }

        private void ChooseRooms(int seed)
        {
            _random = new Random(seed);
            Array.Fill(_rooms, RoomType.Optional);
            var x = _random.Next(MapSize);
            var horizontalMoves = 0;

            SetRoom(x, MapSize - 1, RoomType.AllOpenSpawn);

            // Fill every row of the map from top to bottom.
            for (var y = MapSize - 1; y >= 0;)
            {
                SetRoom(x, y, RoomType.LeftRightOpen);

                var nextMove = _random.Next(PossibleMoves + 1);

                if (horizontalMoves > MaxHorizontalMoves || nextMove == MoveDown || x is < 0 or >= MapSize)
                {
                    x = Math.Clamp(x, 0, MapSize - 1);
                    
                    // The current room needs to be changed to have an opening on the bottom.
                    SetRoom(x, y, RoomType.AllOpen);
                    
                    // The new room below it will have an opening on the top to connect to it.
                    y--;
                    SetRoom(x, y, RoomType.AllOpen);
                    
                    horizontalMoves = 0;
                    continue;
                }

                horizontalMoves++;

                x = nextMove switch
                {
                    < MoveLeft => x - 1,
                    _ => x + 1
                };
            }

            SetRoom(x, 0, RoomType.AllOpenExit);
        }
    }
}