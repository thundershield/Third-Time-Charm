using UnityEngine;

namespace LevelGeneration
{
    public struct RoomTemplate
    {
        public readonly Room Room;
        public readonly GridWorldGenerator.RoomType RoomType;
        public readonly Vector2Int Position;

        public RoomTemplate(Room room, GridWorldGenerator.RoomType roomType, Vector2Int position)
        {
            Room = room;
            RoomType = roomType;
            Position = position;
        }
    }
}