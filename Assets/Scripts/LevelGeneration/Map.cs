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

        public delegate void LoadHandler(LevelLoadData levelLoadData);

        public event LoadHandler OnLoad;
        
        private Tilemap _tilemap;
        private IWorldGenerator _worldGenerator;

        private void Start()
        {
            Rooms.ValidateAllRooms();

            Generate(new GridWorldGenerator());
        }

        public void Generate(IWorldGenerator worldGenerator)
        {
            _tilemap = GetComponent<Tilemap>();
            
            var levelLoadData = worldGenerator.Generate(new Random(), this, new Vector2Int(64, 64));
            OnLoad?.Invoke(levelLoadData);
        }

        public void SetTile(TileType tile, int x, int y)
        {
            var tileIndex = (int)tile;
            _tilemap.SetTile(new Vector3Int(x, y, 0), tiles[tileIndex]);
        }

        public bool IsTileOccupied(int x, int y)
        {
            var position = new Vector3Int(x, y, 0);
            var tile = _tilemap.GetTile<Tile>(position);
            return tile is not null && tile.colliderType != Tile.ColliderType.None;
        }

        public void SetRect(TileType tile, int x, int y, int width, int height)
        {
            for (var xi = 0; xi < width; xi++)
            for (var yi = 0; yi < height; yi++)
                SetTile(tile, x + xi, y + yi);
        }
    }
}