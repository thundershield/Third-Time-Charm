using System;
using System.Collections.Generic;

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
            { '~', TileCategory.IndoorDecor },
            { 'L', TileCategory.LockedDoor },
            { 'O', TileCategory.OpenDoor }
        };

        public static readonly Room[] AllOpenSpawn =
        {
            new (RoomCategory.Outdoor, 
            ".........." +
            ".........." +
            "....[....." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            "..........")
        };

        public static readonly Room[] AllOpenExit =
        {
            new (RoomCategory.Outdoor,
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            "......]..." +
            ".........." +
            "..........")
        };

        public static readonly Room[] LeftRightOpen =
        {
            new (RoomCategory.Indoor,
            "####LL####" +
            "#____~~__#" +
            "#________#" +
            "#__~~____#" +
            "#________#" +
            "#________#" +
            "O________O" +
            "####LL####"),
            new (RoomCategory.Indoor,
            "####LL####" +
            "#________#" +
            "#________#" +
            "#__~_____#" +
            "#______~~#" +
            "#________#" +
            "O________O" +
            "####LL####"),
            new (RoomCategory.Outdoor,
            "^^^^^^^^^^" +
            "^....^^..^" +
            "^........^" +
            "^..^^....^" +
            "^........^" +
            "^........^" +
            ".........." +
            "^^^^^^^^^^"),
            new (RoomCategory.Outdoor,
            "^^^^^^^^^^" +
            "^....^^..^" +
            "^........^" +
            "^..^^....^" +
            "^........^" +
            "^........^" +
            ".........." +
            "^^^^^^^^^^")
        };

        public static readonly Room[] AllOpen =
        {
            new (RoomCategory.Outdoor,
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........."),
            new (RoomCategory.Outdoor,
            ".........." +
            "...^^....." +
            "...^^....." +
            ".........." +
            "......^^.." +
            "......^..." +
            ".........." +
            ".........."),
            new (RoomCategory.Indoor,
            "####OO####" +
            "#____~~__#" +
            "#________#" +
            "#__~~____#" +
            "#________#" +
            "#________#" +
            "O________O" +
            "####OO####"),
            new (RoomCategory.Indoor,
            "####OO####" +
            "#________#" +
            "#~_______#" +
            "#~_______#" +
            "#_______~#" +
            "#________#" +
            "O________O" +
            "####OO####")
        };

        public static readonly Room[] Optional =
        {
            new (RoomCategory.Indoor,
            "####OO####" +
            "#________#" +
            "#________#" +
            "#____~~__#" +
            "#____~~__#" +
            "#________#" +
            "L________L" +
            "##########"),
            new (RoomCategory.Indoor,
            "####LL####" +
            "#________#" +
            "#__~_~~__#" +
            "#________#" +
            "#________#" +
            "#________#" +
            "L________L" +
            "####OO####"),
            new (RoomCategory.Indoor,
            "####LL####" +
            "#________#" +
            "#__~_____#" +
            "#________#" +
            "#________#" +
            "#____~___#" +
            "L________O" +
            "####LL####"),
            new (RoomCategory.Indoor,
            "####LL####" +
            "#________#" +
            "#________#" +
            "#_______~#" +
            "#_______~#" +
            "#__~_____#" +
            "O________L" +
            "####LL####"),
            new (RoomCategory.Outdoor,
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........." +
            ".........."),
            new (RoomCategory.Outdoor,
            "....^....." +
            ".........." +
            ".........." +
            "....^^^..." +
            ".........." +
            ".........." +
            ".^^......." +
            ".........."),
            new (RoomCategory.Outdoor,
            ".........." +
            "...^^^^..." +
            "....^^^^.." +
            ".........." +
            ".........." +
            "..^^^....." +
            "...^^^...." +
            "..........")
        };

        public static bool IsTileCategoryValidDoorway(TileCategory category)
        {
            return category is TileCategory.Floor or TileCategory.Ground or TileCategory.OpenDoor;
        }

        public static void ValidateRoom(string name, Room room, int roomI, bool horizontalMustBeOpen, bool verticalMustBeOpen)
        {
            const int verticalOpeningEnd = VerticalOpeningStart + VerticalOpeningSize;
            const int horizontalOpeningEnd = HorizontalOpeningStart + HorizontalOpeningSize;
            
            for (var y = 0; y < RoomHeight; y++)
            {
                if (verticalMustBeOpen && y is 0 or RoomHeight - 1)
                {
                    for (var x = VerticalOpeningStart; x < verticalOpeningEnd; x++)
                    {
                        var xChar = room.Chars[x + y * RoomWidth];
                        var charCategory = CharCategories[xChar];
                        if (IsTileCategoryValidDoorway(charCategory)) continue;

                        throw new ArgumentException($"Room set '{name}' is missing vertical opening in room {roomI}");
                    }
                }

                if (horizontalMustBeOpen && y is >= HorizontalOpeningStart and < horizontalOpeningEnd)
                {
                    var leftChar = room.Chars[y * RoomWidth];
                    var leftCharCategory = CharCategories[leftChar];
                    var rightChar = room.Chars[RoomWidth - 1 + y * RoomWidth];
                    var rightCharCategory = CharCategories[rightChar];

                    if (!IsTileCategoryValidDoorway(leftCharCategory) ||
                        !IsTileCategoryValidDoorway(rightCharCategory) )
                    {
                        throw new ArgumentException($"Room set '{name}' is missing horizontal opening in room {roomI}");
                    }
                }
            }
        }
        
        public static void ValidateRoomSet(string name, Room[] rooms, bool horizontalMustBeOpen,
            bool verticalMustBeOpen)
        {
            for (var i = 0; i < rooms.Length; i++)
            {
                var room = rooms[i];
                
                if (room.Chars.Length != RoomWidth * RoomHeight)
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