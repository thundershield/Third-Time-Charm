﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace LevelGeneration
{
    public class GridWorldGenerator : IWorldGenerator
    {
        private const int RoomOverlappingWidth = LevelGeneration.Rooms.RoomWidth - 1;
        private const int RoomOverlappingHeight = LevelGeneration.Rooms.RoomHeight - 1;
        
        public LevelLoadData Generate(Random random, Map map, Vector2Int size)
        {
            var tileCategories = map.PickTileTheme(random).GetTileCategories();
            ClearRoomTypes();
            ChooseRooms(random);
            GenerateBorder(random, map, tileCategories);
            var loadData = GenerateRooms(random, map, tileCategories);
            loadData.Size = size;
            loadData.StartPosition = StartPosition;
            loadData.EndPosition = EndPosition;
            LastLoadData = loadData;

            return LastLoadData;
        }

        public LevelLoadData GetLastLoadData()
        {
            return LastLoadData;
        }

        public enum RoomType
        {
            AllOpenSpawn,
            AllOpenExit,
            LeftRightOpen,
            AllOpen,
            Optional,
            // The boss room takes up four spaces,
            // one is the room itself, and the other
            // three are reserved and will be generated
            // over by the main boss room.
            Boss,
            BossReserved,
        }

        public Vector2 StartPosition { get; private set; }
        public Vector2 EndPosition { get; private set; }

        private static readonly Dictionary<RoomType, int> RoomTypePriority = new()
        {
            { RoomType.Optional, 0 },
            { RoomType.LeftRightOpen, 1 },
            { RoomType.AllOpen, 2 },
            { RoomType.AllOpenSpawn, 3 },
            { RoomType.AllOpenExit, 3 },
            { RoomType.Boss, 3 },
            { RoomType.BossReserved, 3 }
        };
        
        private const int MapSize = 10;
        private const int MaxEnemiesPerRoom = 3;
        private const int RoomsPerEnemySpawn = 4;

        private const int MaxHorizontalMoves = MapSize;
        private const int PossibleMoves = 10;
        private const int MoveDown = PossibleMoves;
        private const int MoveLeft = PossibleMoves / 2;

        public readonly RoomType[] Rooms = new RoomType[MapSize * MapSize];
        
        public LevelLoadData LastLoadData { get; private set; }

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
        
        private void AddRoomEnemyPosition(Random random, Map map, int roomX, int roomY, LevelLoadData loadData)
        {
            if (random.Next(RoomsPerEnemySpawn) != 0) return;

            var enemyCount = random.Next(MaxEnemiesPerRoom + 1);

            for (var i = 0; i < enemyCount; i++)
            {
                // Place the enemy anywhere on the interior of the room, don't put on enemy on the edges.
                // Edges could later be replace by walls when neighboring rooms are generated.
                var enemyX = roomX * RoomOverlappingWidth + 1 + random.Next(RoomOverlappingWidth - 2);
                var enemyY = roomY * RoomOverlappingHeight + 1 + random.Next(RoomOverlappingHeight - 2);

                if (map.IsTileOccupied(enemyX, enemyY)) continue;

                loadData.EnemyPositions.Add(new SpawnPosition
                {
                    Position = new Vector2(enemyX + 0.5f, enemyY + 0.5f),
                    SpawnData = random.Next()
                });
            }
        }

        private Dictionary<RoomCategory, List<RoomTemplate>> GenerateRoomTemplates(Random random, Map map)
        {
            var roomTemplates = new Dictionary<RoomCategory, List<RoomTemplate>>();
            
            for (var x = 0; x < MapSize; x++)
            for (var y = 0; y < MapSize; y++)
            {
                var roomType = GetRoom(x, y);

                if (roomType == RoomType.BossReserved)
                {
                    continue;
                }
                
                var room = (roomType switch
                {
                    RoomType.LeftRightOpen => LevelGeneration.Rooms.LeftRightOpen,
                    RoomType.AllOpen => LevelGeneration.Rooms.AllOpen,
                    RoomType.AllOpenSpawn => LevelGeneration.Rooms.AllOpenSpawn,
                    RoomType.AllOpenExit => LevelGeneration.Rooms.AllOpenExit,
                    RoomType.Boss => LevelGeneration.Rooms.BossArena,
                    _ => LevelGeneration.Rooms.Optional
                }).Choose(random);

                if (!roomTemplates.ContainsKey(room.Category))
                {
                    roomTemplates.Add(room.Category, new List<RoomTemplate>());
                }
                
                roomTemplates[room.Category].Add(new RoomTemplate(room, roomType, new Vector2Int(x, y)));
            }
            
            return roomTemplates;
        }

        private void GenerateRoomFromTemplate(Random random, Map map, Dictionary<TileCategory, Tile[]> tileCategories, RoomTemplate roomTemplate, LevelLoadData loadData)
        {
            var roomX = roomTemplate.Position.x * RoomOverlappingWidth;
            var roomY = roomTemplate.Position.y * RoomOverlappingHeight;

            var roomWidth = LevelGeneration.Rooms.RoomWidth;
            var roomHeight = LevelGeneration.Rooms.RoomHeight;

            if (roomTemplate.RoomType == RoomType.Boss)
            {
                roomWidth = roomWidth * 2 - 1;
                roomHeight = roomHeight * 2 - 1;
            }

            for (var i = 0; i < roomTemplate.Room.Chars.Length; i++)
            {
                var tileX = i % roomWidth;
                var tileY = roomHeight - 1 - i / roomWidth;
                var tileCategory = LevelGeneration.Rooms.CharCategories[roomTemplate.Room.Chars[i]];
                var tile = tileCategories[tileCategory].Choose(random);
                var tilePosition = new Vector3Int(roomX + tileX, roomY + tileY, 0);
                var centeredTilePosition = tilePosition + new Vector3(0.5f, 0.5f);

                switch (tileCategory)
                {
                    case TileCategory.Start:
                        StartPosition = centeredTilePosition;
                        break;
                    case TileCategory.End:
                        EndPosition = centeredTilePosition;
                        break;
                }

                map.SetTile(tile, tilePosition.x, tilePosition.y);
            }

            // Don't spawn enemies near the player's spawn or exit.
            if (roomTemplate.RoomType is RoomType.AllOpenSpawn or RoomType.AllOpenExit) return;

            if (roomTemplate.RoomType is RoomType.Boss)
            {
                loadData.BossPosition = new Vector2(roomX + LevelGeneration.Rooms.RoomWidth,
                    roomY + LevelGeneration.Rooms.RoomHeight);
                return;
            }
            
            AddRoomEnemyPosition(random, map, roomTemplate.Position.x, roomTemplate.Position.y, loadData);
        }

        private LevelLoadData GenerateRooms(Random random, Map map, Dictionary<TileCategory, Tile[]> tileCategories)
        {
            var loadData = new LevelLoadData()
            {
                EnemyPositions = new List<SpawnPosition>(),
            };
            var roomTemplates = GenerateRoomTemplates(random, map);

            foreach (var roomTemplate in roomTemplates[RoomCategory.Outdoor])
            {
                GenerateRoomFromTemplate(random, map, tileCategories, roomTemplate, loadData);
            }
            
            foreach (var roomTemplate in roomTemplates[RoomCategory.Indoor])
            {
                GenerateRoomFromTemplate(random, map, tileCategories, roomTemplate, loadData);
            }

            return loadData;
        }

        public void GenerateBorder(Random random, Map map, Dictionary<TileCategory, Tile[]> tileCategories)
        {
            const int mapWidth = MapSize * RoomOverlappingWidth + 1;
            const int mapHeight = MapSize * RoomOverlappingHeight + 1;
        
            var borderTiles = tileCategories[TileCategory.Wall].Concat(tileCategories[TileCategory.Plant]).ToArray();

            var borderTile = borderTiles.Choose(random);
            map.SetRect(borderTile, -1, -1, mapWidth + 2, 1);
            map.SetRect(borderTile, -1, -1, 1, mapHeight + 2);
            map.SetRect(borderTile, -1, mapHeight, mapWidth + 2, 1);
            map.SetRect(borderTile, mapWidth, -1, 1, mapHeight + 2);
        }

        private void ChooseRooms(Random random)
        {
            Array.Fill(Rooms, RoomType.Optional);
            var x = random.Next(MapSize);
            var horizontalMoves = 0;

            SetRoom(x, MapSize - 1, RoomType.AllOpenSpawn);

            // Fill every row of the map from top to bottom.
            for (var y = MapSize - 1; y >= 0;)
            {
                SetRoom(x, y, RoomType.LeftRightOpen);

                var nextMove = random.Next(PossibleMoves + 1);

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
            
            // Place the boss room, make sure it isn't in the first or last row, which have the start and end rooms.
            var bossY = random.Next(1, MapSize - 1);
            var bossX = random.Next(0, MapSize - 1);

            for (var i = 0; i < MapSize; i++)
            {
                bossX = (bossX + 1) % (MapSize - 1);
                
                // We don't want the boss room to spawn outside of the main path.
                if (GetRoom(bossX, bossY) == RoomType.Optional) continue;
                
                SetRoom(bossX, bossY, RoomType.Boss);
                SetRoom(bossX + 1, bossY, RoomType.BossReserved);
                SetRoom(bossX, bossY + 1, RoomType.BossReserved);
                SetRoom(bossX + 1, bossY + 1, RoomType.BossReserved);

                break;
            }
        }
    }
}