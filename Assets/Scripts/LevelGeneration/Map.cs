using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace LevelGeneration
{
    [RequireComponent(typeof(Tilemap))]
    public class Map : MonoBehaviour
    {
        [SerializeField] private TileTheme[] tileThemes;
        
        public delegate void LoadHandler(LevelLoadData levelLoadData);

        public event LoadHandler OnLoad;
        
        private Tilemap _tilemap;
        private IWorldGenerator _worldGenerator;
        private Random _random;

        private void Start()
        {
            _random = new Random();
            
            Rooms.ValidateAllRooms();

            _worldGenerator = new GridWorldGenerator();

            Generate();
        }

        public void Generate()
        {
            _tilemap = GetComponent<Tilemap>();
            
            var levelLoadData = _worldGenerator.Generate(_random, this, new Vector2Int(64, 64));
            OnLoad?.Invoke(levelLoadData);
        }

        public LevelLoadData GetLastLoadData()
        {
            return _worldGenerator.GetLastLoadData();
        }

        public void SetTile(Tile tile, int x, int y)
        {
            var position = new Vector3Int(x, y, 0);
            _tilemap.SetTile(position, tile);
        }
        
        public bool IsTileOccupied(int x, int y)
        {
            var position = new Vector3Int(x, y, 0);
            var tile = _tilemap.GetTile<Tile>(position);
            return tile is not null && tile.colliderType != Tile.ColliderType.None;
        }
        
        public void SetRect(Tile tile, int x, int y, int width, int height)
        {
            for (var xi = 0; xi < width; xi++)
            for (var yi = 0; yi < height; yi++)
                SetTile(tile, x + xi, y + yi);
        }

        public TileTheme PickTileTheme(Random random)
        {
            return tileThemes.Choose(random);
        }
    }
}