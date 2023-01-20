using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoTileTexture
{
    // A hash-able way to store the layout of a tile's texture, used to check if a texture is unique.
    private struct Layout : IEquatable<Layout>
    {
        private int x0;
        private int y0;
        
        private int x1;
        private int y1;
        
        private int x2;
        private int y2;
        
        private int x3;
        private int y3;

        public bool Equals(Layout other)
        {
            return x0 == other.x0 && y0 == other.y0 && x1 == other.x1 && y1 == other.y1 && x2 == other.x2 &&
                   y2 == other.y2 && x3 == other.x3 && y3 == other.y3;
        }

        public override bool Equals(object obj)
        {
            return obj is Layout other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x0, y0, x1, y1, x2, y2, x3, y3);
        }

        public void SetTexPos(int i, int x, int y)
        {
            switch (i)
            {
                case 0:
                    x0 = x;
                    y0 = y;
                    break;
                case 1:
                    x1 = x;
                    y1 = y;
                    break;
                case 2:
                    x2 = x;
                    y2 = y;
                    break;
                case 3:
                    x3 = x;
                    y3 = y;
                    break;
            }
        }
    }
    
    private readonly Dictionary<int, Sprite> maskSprites = new();
    private readonly Dictionary<Layout, Sprite> layoutSprites = new();
    private int tileTexCount;

    private readonly bool[] nearbyTiles = new bool[8];

    public AutoTileTexture(Texture seedTexture, int tileSize)
    {
        GenerateTexture(seedTexture, tileSize);
    }

    private Texture2D texture;

    // Get offset of the tile's texture based on which neighbor tiles are not the same as this one.
    public Sprite GetSprite(bool[] neighbors)
    {
        var mask = 0;
        for (var i = 0; i < neighbors.Length; i++)
        {
            if (!neighbors[i]) continue;
            mask |= 1 << i;
        }

        return maskSprites[mask];
    }

    private void GenerateTexture(Texture seedTexture, int tileSize)
    {
        // 256 combinations of neighbors are possible, but only 47 are unique.
        const int tileCount = 47;
        
        int halfTileSize = tileSize / 2;
        int minWidth = tileCount * tileSize;
        var texWidth = 1;

        while (texWidth < minWidth) texWidth *= 2;

        texture = new Texture2D(texWidth, tileSize, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };

        // Loop over a bitmask representing all possible combinations of neighboring tiles.
        for (var mask = 0; mask <= 255; mask++)
        {
            UpdateNeighbors(mask);
            CreateSubTileTextureLayout(mask, tileSize, halfTileSize, seedTexture);
        }
    }

    private void UpdateNeighbors(int mask)
    {
        for (var i = 0; i < 8; i++) nearbyTiles[i] = (mask & (1 << i)) != 0;
    }
    
    private void CreateSubTileTextureLayout(int mask, int tileSize, int halfTileSize, Texture seedTexture)
    {
        var layout = new Layout();
        int tileTexX = tileTexCount * tileSize;

        for (var i = 0; i < 4; i++)
        {
            var texX = 1;
            var texY = 1;

            int subTileX = i % 2;
            int subTileY = i / 2;

            int offsetX = subTileX * 2 - 1;
            int offsetY = subTileY * 2 - 1;

            if (GetNearby(offsetX, 0)) texX += offsetX;
            if (GetNearby(0, offsetY)) texY += offsetY;
            if (texX == 1 && texY == 1 && GetNearby(offsetX, offsetY))
            {
                texX = 4 - subTileX;
                texY = 1 - subTileY;
            }

            int subTileTexX = texX * halfTileSize;
            int subTileTexY = seedTexture.height - texY * halfTileSize - halfTileSize;

            Graphics.CopyTexture(seedTexture, 0, 0, subTileTexX, subTileTexY, halfTileSize, halfTileSize, texture,
                0, 0, tileTexX + subTileX * halfTileSize, texture.height - subTileY * halfTileSize - halfTileSize);
            
            layout.SetTexPos(i, texX, texY);
        }

        // Reuse existing sprites if this new one is not unique.
        if (layoutSprites.TryGetValue(layout, out Sprite existingSprite))
        {
            maskSprites.Add(mask, existingSprite);
        }
        else
        {
            var sprite = Sprite.Create(texture, new Rect(tileTexX, 0, tileSize, tileSize), new Vector2(0.5f, 0.5f),
                tileSize, 0);
            layoutSprites.Add(layout, sprite);
            maskSprites.Add(mask, sprite);
            tileTexCount++;
        }
    }

    // Get the index of a neighbor tile, skip the center tile which isn't a neighbor.
    private bool GetNearby(int x, int y)
    {
        x += 1;
        y += 1;

        int index = x + y * 3;

        if (y > 1 || (y == 1 && x > 1)) index--;

        return nearbyTiles[index];
    }
}