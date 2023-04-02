using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace LevelGeneration
{
    public class GridWorldGenerator : IWorldGenerator
    {
        private const int RoomOverlappingWidth = LevelGeneration.Rooms.RoomWidth - 1;
        private const int RoomOverlappingHeight = LevelGeneration.Rooms.RoomHeight - 1;
        
        public LevelLoadData Generate(Random random, Map map, Vector2Int size)
        {
            var seed = random.Next();
            ClearRoomTypes();
            ChooseRooms(seed);
            GenerateBorder(map);
            var enemyPositions = GenerateRooms(map);

            return new LevelLoadData
            {
                StartPosition = StartPosition,
                EndPosition = EndPosition,
                EnemyPositions = enemyPositions,
                Size = size
            };
        }

        public enum RoomType
        {
            AllOpenSpawn,
            AllOpenExit,
            LeftRightOpen,
            AllOpen,
            Optional
        }

        public Vector2 StartPosition { get; private set; }
        public Vector2 EndPosition { get; private set; }

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
        private const int MaxEnemiesPerRoom = 3;
        private const int RoomsPerEnemySpawn = 4;

        private const int MaxHorizontalMoves = MapSize;
        private const int PossibleMoves = 10;
        private const int MoveDown = PossibleMoves;
        private const int MoveLeft = PossibleMoves / 2;

        public readonly RoomType[] Rooms = new RoomType[MapSize * MapSize];
        private Random _random;

        public GridWorldGenerator()
        {
            ClearRoomTypes();
        }

        public void ClearRoomTypes()
        {
            Array.Fill(Rooms, RoomType.Optional);
        }
        
        public void SetRoom(int x, int y, RoomType roomType)
        {
            if (x is < 0 or >= MapSize || y is < 0 or >= MapSize) return;

            // Replacing the current room with a room type that has fewer openings could
            // make the level impossible to complete.
            var oldRoomType = GetRoom(x, y);
            if (RoomTypePriority[roomType] <= RoomTypePriority[oldRoomType]) return;

            Rooms[x + y * MapSize] = roomType;
        }

        public RoomType GetRoom(int x, int y)
        {
            if (x is < 0 or >= MapSize || y is < 0 or >= MapSize) return RoomType.Optional;
            return Rooms[x + y * MapSize];
        }
        
        private void AddRoomEnemyPosition(Map map, int roomX, int roomY, List<Vector2> enemyPositions)
        {
            if (_random.Next(RoomsPerEnemySpawn) != 0) return;

            var enemyCount = _random.Next(MaxEnemiesPerRoom + 1);

            for (var i = 0; i < enemyCount; i++)
            {
                var enemyX = roomX * LevelGeneration.Rooms.RoomWidth + _random.Next(LevelGeneration.Rooms.RoomWidth);
                var enemyY = roomY * LevelGeneration.Rooms.RoomHeight + _random.Next(LevelGeneration.Rooms.RoomHeight);

                if (map.IsTileOccupied(enemyX, enemyY)) continue;

                enemyPositions.Add(new Vector2(enemyX, enemyY));
            }
        }

        private Dictionary<RoomCategory, List<RoomTemplate>> GenerateRoomTemplates(Map map)
        {
            var roomTemplates = new Dictionary<RoomCategory, List<RoomTemplate>>();
            
            for (var x = 0; x < MapSize; x++)
            for (var y = 0; y < MapSize; y++)
            {
                var roomType = GetRoom(x, y);
                
                var room = (roomType switch
                {
                    RoomType.LeftRightOpen => LevelGeneration.Rooms.LeftRightOpen,
                    RoomType.AllOpen => LevelGeneration.Rooms.AllOpen,
                    RoomType.AllOpenSpawn => LevelGeneration.Rooms.AllOpenSpawn,
                    RoomType.AllOpenExit => LevelGeneration.Rooms.AllOpenExit,
                    _ => LevelGeneration.Rooms.Optional
                }).Choose(_random);

                if (!roomTemplates.ContainsKey(room.Category))
                {
                    roomTemplates.Add(room.Category, new List<RoomTemplate>());
                }
                
                roomTemplates[room.Category].Add(new RoomTemplate(room, roomType, new Vector2Int(x, y)));
            }

            return roomTemplates;
        }

        private void GenerateRoomFromTemplate(Map map, RoomTemplate roomTemplate, List<Vector2> enemyPositions)
        {
            var roomX = roomTemplate.Position.x * RoomOverlappingWidth;
            var roomY = roomTemplate.Position.y * RoomOverlappingHeight;

            for (var i = 0; i < roomTemplate.Room.Chars.Length; i++)
            {
                var tileX = i % LevelGeneration.Rooms.RoomWidth;
                var tileY = LevelGeneration.Rooms.RoomHeight - 1 - i / LevelGeneration.Rooms.RoomWidth;
                var tile = Map.TileCategories[LevelGeneration.Rooms.CharCategories[roomTemplate.Room.Chars[i]]].Choose(_random);
                var tilePosition = new Vector3Int(roomX + tileX, roomY + tileY, 0);
                var centeredTilePosition = tilePosition + new Vector3(0.5f, 0.5f);

                switch (tile)
                {
                    case TileType.Start:
                        StartPosition = centeredTilePosition;
                        break;
                    case TileType.End:
                        EndPosition = centeredTilePosition;
                        break;
                }

                map.SetTile(tile, tilePosition.x, tilePosition.y);
            }

            // Don't spawn enemies near the player's spawn or exit.
            if (roomTemplate.RoomType is RoomType.AllOpenSpawn or RoomType.AllOpenExit) return;

            AddRoomEnemyPosition(map, roomTemplate.Position.x, roomTemplate.Position.y, enemyPositions);
        }

        private List<Vector2> GenerateRooms(Map map)
        {
            var enemyPositions = new List<Vector2>();
            var roomTemplates = GenerateRoomTemplates(map);

            foreach (var roomTemplate in roomTemplates[RoomCategory.Outdoor])
            {
                GenerateRoomFromTemplate(map, roomTemplate, enemyPositions);
            }
            
            foreach (var roomTemplate in roomTemplates[RoomCategory.Indoor])
            {
                GenerateRoomFromTemplate(map, roomTemplate, enemyPositions);
            }

            return enemyPositions;
        }

        public void GenerateBorder(Map map)
        {
            const int mapWidth = MapSize * RoomOverlappingWidth + 1;
            const int mapHeight = MapSize * RoomOverlappingHeight + 1;

            var borderTile = _borderTiles.Choose(_random);
            map.SetRect(borderTile, -1, -1, mapWidth + 2, 1);
            map.SetRect(borderTile, -1, -1, 1, mapHeight + 2);
            map.SetRect(borderTile, -1, mapHeight, mapWidth + 2, 1);
            map.SetRect(borderTile, mapWidth, -1, 1, mapHeight + 2);
        }

        private void ChooseRooms(int seed)
        {
            _random = new Random(seed);
            Array.Fill(Rooms, RoomType.Optional);
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