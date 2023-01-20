using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoTile : Tile
{
    [SerializeField] private Texture seedTexture;
    [SerializeField] private int tileSize;
    public AutoTileTexture texture;

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
        bool tl = !HasSelf(tilemap, position + new Vector3Int(-1, 1, 0));
        bool tc = !HasSelf(tilemap, position + new Vector3Int(0, 1, 0));
        bool tr = !HasSelf(tilemap, position + new Vector3Int(1, 1, 0));
        bool cl = !HasSelf(tilemap, position + new Vector3Int(-1, 0, 0));
        bool cr = !HasSelf(tilemap, position + new Vector3Int(1, 0, 0));
        bool bl = !HasSelf(tilemap, position + new Vector3Int(-1, -1, 0));
        bool bc = !HasSelf(tilemap, position + new Vector3Int(0, -1, 0));
        bool br = !HasSelf(tilemap, position + new Vector3Int(1, -1, 0));

        tileData.sprite = texture.GetSprite(tl, tc, tr, cl, cr, bl, bc, br);
        tileData.color = Color.white;
        tileData.colliderType = ColliderType.None;
    }

    private bool HasAutoTile(ITilemap tilemap, Vector3Int position)
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
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AutoTile>(), path);
    }
#endif
}
