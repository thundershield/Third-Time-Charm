using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelGeneration
{
    public class AutoTile : Tile
    {
        [SerializeField] private Texture seedTexture;
        [SerializeField] private int tileSize;
        private AutoTileTexture _texture;
        private readonly bool[] _neighborBuffer = new bool[8];

        public void OnEnable()
        {
            _texture = new AutoTileTexture(seedTexture, tileSize);
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            for (var y = -1; y <= 1; y++)
            for (var x = -1; x <= 1; x++)
            {
                var neighborPos = position + new Vector3Int(x, y, 0);
                if (HasAutoTile(tilemap, neighborPos)) tilemap.RefreshTile(neighborPos);
            }
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            var i = 0;
            for (var y = 1; y >= -1; y--)
            for (var x = -1; x <= 1; x++)
            {
                if (y == 0 && x == 0) continue;

                _neighborBuffer[i] = !HasSelf(tilemap, position + new Vector3Int(x, y, 0));
                i++;
            }

            tileData.sprite = _texture.GetSprite(_neighborBuffer);
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
            var path = EditorUtility.SaveFilePanelInProject("Save AutoTile", "New AutoTile", "Asset", "Save AutoTile",
                "Assets");
            if (path == "") return;
            AssetDatabase.CreateAsset(CreateInstance<AutoTile>(), path);
        }
#endif
    }
}