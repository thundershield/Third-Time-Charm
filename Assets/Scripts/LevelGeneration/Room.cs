namespace LevelGeneration
{
    public class Room
    {
        public readonly string Chars;
        public readonly RoomCategory Category;

        public Room(RoomCategory category, string chars)
        {
            Chars = chars;
            Category = category;
        }
    }
}