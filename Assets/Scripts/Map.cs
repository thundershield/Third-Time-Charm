using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

[RequireComponent(typeof(Tilemap))]
public class Map : MonoBehaviour
{
    [SerializeField] private Tile[] tiles;
    private Tilemap _tilemap;
    private IWorldGenerator _worldGenerator;

    private void Start()
    {
        _worldGenerator = new GridWorldGenerator();
        _tilemap = GetComponent<Tilemap>();
        
        _worldGenerator.Generate(new Random(), this, new Vector2Int(64, 64));
    }

    public void SetTile(TileType tile, int x, int y)
    {
        var tileIndex = (int)tile;
        _tilemap.SetTile(new Vector3Int(x, y, 0), tiles[tileIndex]);
    }

    public void SetRect(TileType tile, int x, int y, int width, int height)
    {
        for (var xi = 0; xi < width; xi++)
        {
            for (var yi = 0; yi < height; yi++)
            {
                SetTile(tile, x + xi, y + yi);
            }
        }
    }
}
