using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class Map : MonoBehaviour
{
    public Texture seedTexture;
    public AutoTile testTile;

    private Tilemap tilemap;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (x == 1 && y == 1) continue;

                tilemap.SetTile(new Vector3Int(x, y, 0), testTile);
            }
        }
    }

    private void Update()
    {
        tilemap.SetTile(new Vector3Int(1, 1, 0), testTile);
    }

    private void OnGUI()
    {
        // Graphics.DrawTexture(new Rect(-4000, 0, testTile.texture.Texture.width, testTile.texture.Texture.height), testTile.texture.Texture);
    }
}
