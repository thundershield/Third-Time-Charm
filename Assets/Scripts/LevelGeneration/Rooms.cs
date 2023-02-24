using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace LevelGeneration
{
    public static class Rooms
    {
        public const int RoomWidth = 10;
        public const int RoomHeight = 8;
        public const int VerticalOpeningStart = 4;
        public const int VerticalOpeningSize = 2;
        public const int HorizontalOpeningStart = 6;
        public const int HorizontalOpeningSize = 1;

        public static readonly Dictionary<char, TileCategory> CharCategories = new()
        {
            { '.', TileCategory.Ground },
            { '[', TileCategory.Start },
            { ']', TileCategory.End },
            { '^', TileCategory.Plant },
            { '#', TileCategory.Wall },
            { '_', TileCategory.Floor },
            { '~', TileCategory.IndoorDecor }
        };

        public static readonly string[] AllOpenSpawn =
        {
            ".........." +
            ".........." +
            "....[....." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........."
        };

        public static readonly string[] AllOpenExit =
        {
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            "......]..." +
            ".........." +
            ".........."
        };

        public static readonly string[] LeftRightOpen =
        {
            "##########" +
            "#____~~__#" +
            "#________#" +
            "#__~~____#" +
            "#________#" +
            "#________#" +
            "__________" +
            "##########",
            "##########" +
            "#________#" +
            "#________#" +
            "#__~_____#" +
            "#______~~#" +
            "#________#" +
            "__________" +
            "##########",
            "^^^^^^^^^^" +
            "^....^^..^" +
            "^........^" +
            "^..^^....^" +
            "^........^" +
            "^........^" +
            ".........." +
            "^^^^^^^^^^",
            "^^^^^^^^^^" +
            "^....^^..^" +
            "^........^" +
            "^..^^....^" +
            "^........^" +
            "^........^" +
            ".........." +
            "^^^^^^^^^^"
        };

        public static readonly string[] AllOpen =
        {
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            "..........",
            ".........." +
            "...^^....." +
            "...^^....." +
            ".........." +
            "......^^.." +
            "......^..." +
            ".........." +
            "..........",
            "####__####" +
            "#____~~__#" +
            "#________#" +
            "#__~~____#" +
            "#________#" +
            "#________#" +
            "__________" +
            "####__####",
            "####__####" +
            "#________#" +
            "#~_______#" +
            "#~_______#" +
            "#_______~#" +
            "#________#" +
            "__________" +
            "####__####"
        };

        public static readonly string[] Optional =
        {
            "####__####" +
            "#________#" +
            "#________#" +
            "#____~~__#" +
            "#____~~__#" +
            "#________#" +
            "#________#" +
            "##########",
            "##########" +
            "#________#" +
            "#__~_~~__#" +
            "#________#" +
            "#________#" +
            "#________#" +
            "#________#" +
            "####__####",
            "##########" +
            "#________#" +
            "#__~_____#" +
            "#________#" +
            "#________#" +
            "#____~___#" +
            "#_________" +
            "##########",
            "##########" +
            "#________#" +
            "#________#" +
            "#_______~#" +
            "#_______~#" +
            "#__~_____#" +
            "_________#" +
            "##########",
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            "..........",
            "....^....." +
            ".........." +
            ".........." +
            "....^^^..." +
            ".........." +
            ".........." +
            ".^^......." +
            "..........",
            ".........." +
            "...^^^^..." +
            "....^^^^.." +
            ".........." +
            ".........." +
            "..^^^....." +
            "...^^^...." +
            ".........."
        };

        private static void ValidateRoom(string name, string room, int roomI, bool horizontalMustBeOpen, bool verticalMustBeOpen)
        {
            const int verticalOpeningEnd = VerticalOpeningStart + VerticalOpeningSize;
            const int horizontalOpeningEnd = HorizontalOpeningStart + HorizontalOpeningSize;
            
            for (var y = 0; y < RoomHeight; y++)
            {
                if (verticalMustBeOpen && y is 0 or RoomHeight - 1)
                {
                    for (var x = VerticalOpeningStart; x < verticalOpeningEnd; x++)
                    {
                        var xChar = room[x + y * RoomWidth];
                        var charCategory = CharCategories[xChar];
                        if (charCategory is TileCategory.Floor or TileCategory.Ground) continue;

                        throw new ArgumentException($"Room set '{name}' is missing vertical opening in room {roomI}");
                    }
                }

                if (horizontalMustBeOpen && y is >= HorizontalOpeningStart and < horizontalOpeningEnd)
                {
                    var leftChar = room[y * RoomWidth];
                    var leftCharCategory = CharCategories[leftChar];
                    var rightChar = room[RoomWidth - 1 + y * RoomWidth];
                    var rightCharCategory = CharCategories[rightChar];

                    if (leftCharCategory is not TileCategory.Floor and not TileCategory.Ground ||
                        rightCharCategory is not TileCategory.Floor and not TileCategory.Ground)
                    {
                        throw new ArgumentException($"Room set '{name}' is missing horizontal opening in room {roomI}");
                    }
                }
            }
        }
        
        private static void ValidateRoomSet(string name, string[] rooms, bool horizontalMustBeOpen,
            bool verticalMustBeOpen)
        {
            for (var i = 0; i < rooms.Length; i++)
            {
                var room = rooms[i];
                
                if (room.Length != RoomWidth * RoomHeight)
                    throw new ArgumentException($"Room set '{name}' has an invalid size for room {i}");

                ValidateRoom(name, room, i, horizontalMustBeOpen, verticalMustBeOpen);
            }
        }

        public static void ValidateAllRooms()
        {
            ValidateRoomSet(nameof(AllOpenSpawn), AllOpenSpawn, true, true);
            ValidateRoomSet(nameof(AllOpenExit), AllOpenExit, true, true);
            ValidateRoomSet(nameof(LeftRightOpen), LeftRightOpen, true, false);
            ValidateRoomSet(nameof(AllOpen), AllOpen, true, true);
            ValidateRoomSet(nameof(Optional), Optional, false, false);
        }
    }
}