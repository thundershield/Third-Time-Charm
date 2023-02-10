using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GridWorldGenerator : IWorldGenerator
{
    public void Generate(Random random, Map map, Vector2Int size)
    {
        var seed = random.Next();
        _lastSeed = seed;
        ChooseRooms(seed);
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

    public delegate void OnRegenerate();

    private static readonly Dictionary<RoomType, int> RoomTypePriority = new()
    {
        { RoomType.Optional, 0 },
        { RoomType.LeftRightOpen, 1 },
        { RoomType.AllOpen, 2 },
        { RoomType.AllOpenSpawn, 3 },
        { RoomType.AllOpenExit, 3 }
    };

    private readonly TileType[] _wallTiles =
    {
        TileType.Wall,
        TileType.Tree,
    };

    private const TileType EmptyTile = TileType.Grass;

    private const int MapSize = 10;
    private const int MaxHorizontalMoves = MapSize;
    private readonly RoomType[] _rooms = new RoomType[MapSize * MapSize];
    private Random _random;
    private int _lastSeed;

    private void SetRoom(int x, int y, RoomType roomType)
    {
        if (x is < 0 or >= MapSize || y is < 0 or >= MapSize) return;
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
        const int mapWidth = MapSize * Rooms.RoomWidth;
        const int mapHeight = MapSize * Rooms.RoomHeight;

        var borderTile = Choose(_wallTiles);
        map.SetRect(borderTile, -1, -1, mapWidth + 2, 1);
        map.SetRect(borderTile, -1, -1, 1, mapHeight + 2);
        map.SetRect(borderTile, -1, mapHeight, mapWidth + 2, 1);
        map.SetRect(borderTile, mapWidth, -1, 1, mapHeight + 2);

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
                var tileTemplate = roomTemplate[i];
                var tilePosition = new Vector3Int(roomX + tileX, roomY + tileY, 0);
                var centeredTilePosition = tilePosition + new Vector3(0.5f, 0.5f);

                switch (tileTemplate)
                {
                    case Rooms.Spawn:
                        SpawnPosition = centeredTilePosition;
                        break;
                    case Rooms.Exit:
                        ExitPosition = centeredTilePosition;
                        break;
                }

                var tile = Rooms.CharTiles[tileTemplate];

                map.SetTile(tile, tilePosition.x, tilePosition.y);
            }
        }
    }

    private void ChooseRooms(int seed)
    {
        _random = new Random(seed);
        Array.Fill(_rooms, RoomType.Optional);
        var x = _random.Next(MapSize);
        var horizontalMoves = 0;

        SetRoom(x, MapSize - 1, RoomType.AllOpenSpawn);

        for (var y = MapSize - 1; y >= 0;)
        {
            SetRoom(x, y, RoomType.LeftRightOpen);

            var nextMove = _random.Next(11);

            if (horizontalMoves > MaxHorizontalMoves || nextMove == 10 || x < 0 || x >= MapSize)
            {
                x = Math.Clamp(x, 0, MapSize - 1);
                SetRoom(x, y, RoomType.AllOpen);
                y--;
                SetRoom(x, y, RoomType.AllOpen);
                horizontalMoves = 0;
                continue;
            }

            horizontalMoves++;

            switch (nextMove)
            {
                case < 5:
                    x--;
                    continue;
                default:
                    x++;
                    break;
            }
        }

        SetRoom(x, 0, RoomType.AllOpenExit);
    }
}