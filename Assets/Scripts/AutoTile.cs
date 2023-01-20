using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoTile : Tile
{
    [SerializeField] private Texture seedTexture;
    [SerializeField] private int tileSize;
    private AutoTileTexture texture;
    private readonly bool[] neighborBuffer = new bool[8];

    public void OnEnable()
    {
        texture = new AutoTileTexture(seedTexture, tileSize);
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector3Int neighborPos = position + new Vector3Int(x, y, 0);
                if (HasAutoTile(tilemap, neighborPos)) tilemap.RefreshTile(neighborPos);
            }
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var i = 0;
        for (var y = 1; y >= -1; y--)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (y == 0 && x == 0) continue;
                
                neighborBuffer[i] = !HasSelf(tilemap, position + new Vector3Int(x, y, 0));
                i++;
            }
        }

        tileData.sprite = texture.GetSprite(neighborBuffer);
        tileData.color = Color.white;
        tileData.colliderType = ColliderType.None;
    }

    private static bool HasAutoTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) is AutoTile;
    }

    private bool HasSelf(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/AutoTile")]
    public static void CreateAutoTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save AutoTile", "New AutoTile", "Asset", "Save AutoTile", "Assets");
        if (path == "") return;
        AssetDatabase.CreateAsset(CreateInstance<AutoTile>(), path);
    }
#endif
}
