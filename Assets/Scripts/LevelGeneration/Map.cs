using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;
using Pathfinding;

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
        private List<GameObject> _spawnedObjects;

        private void Start()
        {
            _random = new Random();
            
            Rooms.ValidateAllRooms();

            _spawnedObjects = new List<GameObject>();
            _worldGenerator = new GridWorldGenerator();

            Generate();
            //This terrible bit of code is to get around a weird bug where the collider doesn't immediately load, causing the scan to fail
            //deactivating and reactivating it fixes the issue, but this should be properly fixed at some point
            TilemapCollider2D collider = GetComponent<TilemapCollider2D>();
            collider.enabled = false;
            collider.enabled = true;
            AstarPath.active.Scan();
        }

        public void Generate()
        {
            _tilemap = GetComponent<Tilemap>();

            foreach (var spawnedObject in _spawnedObjects)
            {
                if (!spawnedObject) continue;
                
                Destroy(spawnedObject);
            }
            
            _spawnedObjects.Clear();
            
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

        public void SpawnObject(GameObject gameObject, Vector2 position)
        {
            var newObject = Instantiate(gameObject, position, Quaternion.identity);
            _spawnedObjects.Add(newObject);
        }
    }
}