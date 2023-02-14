using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace LevelGeneration
{
    [RequireComponent(typeof(Tilemap))]
    public class Map : MonoBehaviour
    {
        [SerializeField] private Tile[] tiles;
        
        public static readonly Dictionary<TileCategory, TileType[]> TileCategories = new()
        {
            { TileCategory.Ground, new[] { TileType.Grass, TileType.Path } },
            { TileCategory.Start, new[] { TileType.Start } },
            { TileCategory.End, new[] { TileType.End } },
            { TileCategory.Plant, new[] { TileType.Bush, TileType.Tree } },
            { TileCategory.Wall, new[] { TileType.Wall } },
            { TileCategory.Floor, new[] { TileType.Floor } },
            { TileCategory.IndoorDecor, new[]
            {
                TileType.Bin,
                TileType.Bucket,
                TileType.Cabinet,
                TileType.ClosedBook,
                TileType.DownChair,
                TileType.Dresser,
                TileType.Inkwell,
                TileType.OpenBook,
                TileType.RightChair,
                TileType.Table
            } }
        };

        public bool isDone;

        private Tilemap _tilemap;
        private IWorldGenerator _worldGenerator;

        private void Start()
        {
            _worldGenerator = new GridWorldGenerator();
            _tilemap = GetComponent<Tilemap>();

            _worldGenerator.Generate(new Random(), this, new Vector2Int(64, 64));
            isDone = true;
        }

        public void SetTile(TileType tile, int x, int y)
        {
            var tileIndex = (int)tile;
            _tilemap.SetTile(new Vector3Int(x, y, 0), tiles[tileIndex]);
        }

        public void SetRect(TileType tile, int x, int y, int width, int height)
        {
            for (var xi = 0; xi < width; xi++)
            for (var yi = 0; yi < height; yi++)
                SetTile(tile, x + xi, y + yi);
        }
    }
}