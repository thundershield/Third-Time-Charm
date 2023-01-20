using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class Map : MonoBehaviour
{
    [SerializeField] private AutoTile testTile;
    private Tilemap tilemap;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        for (var x = 0; x < 3; x++)
        {
            for (var y = 0; y < 3; y++)
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
}
