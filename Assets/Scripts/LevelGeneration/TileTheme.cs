using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelGeneration
{
    [CreateAssetMenu(fileName = "New Tile Theme", menuName = "LevelGeneration/TileTheme")]
    public class TileTheme : ScriptableObject
    {
        public Tile[] groundTiles;
        public Tile[] startTiles;
        public Tile[] endTiles;
        public Tile[] plantTiles;
        public Tile[] wallTiles;
        public Tile[] floorTiles;
        public Tile[] indoorDecorTiles;
        public Tile[] lockedDoorTiles;
        public Tile[] openDoorTiles;

        // Unity can't directly serialize dictionaries for the inspector.
        public Dictionary<TileCategory, Tile[]> GetTileCategories()
        {
            var categories = new Dictionary<TileCategory, Tile[]>
            {
                { TileCategory.Ground, groundTiles },
                { TileCategory.Start, startTiles },
                { TileCategory.End, endTiles },
                { TileCategory.Plant, plantTiles },
                { TileCategory.Wall, wallTiles },
                { TileCategory.Floor, floorTiles },
                { TileCategory.IndoorDecor, indoorDecorTiles },
                { TileCategory.LockedDoor, lockedDoorTiles },
                { TileCategory.OpenDoor, openDoorTiles }
            };

            return categories;
        }
    }
}