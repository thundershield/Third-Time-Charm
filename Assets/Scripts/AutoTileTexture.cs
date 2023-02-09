using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoTileTexture
{
    // A hash-able way to store the layout of a tile's texture, used to check if a texture is unique.
    private struct Layout : IEquatable<Layout>
    {
        private int _x0;
        private int _y0;
        
        private int _x1;
        private int _y1;
        
        private int _x2;
        private int _y2;
        
        private int _x3;
        private int _y3;

        public bool Equals(Layout other)
        {
            return _x0 == other._x0 && _y0 == other._y0 && _x1 == other._x1 && _y1 == other._y1 && _x2 == other._x2 &&
                   _y2 == other._y2 && _x3 == other._x3 && _y3 == other._y3;
        }

        public override bool Equals(object obj)
        {
            return obj is Layout other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_x0, _y0, _x1, _y1, _x2, _y2, _x3, _y3);
        }

        public void SetTexPos(int i, int x, int y)
        {
            switch (i)
            {
                case 0:
                    _x0 = x;
                    _y0 = y;
                    break;
                case 1:
                    _x1 = x;
                    _y1 = y;
                    break;
                case 2:
                    _x2 = x;
                    _y2 = y;
                    break;
                case 3:
                    _x3 = x;
                    _y3 = y;
                    break;
            }
        }
    }
    
    private readonly Dictionary<int, Sprite> _maskSprites = new();
    private readonly Dictionary<Layout, Sprite> _layoutSprites = new();
    private int _tileTexCount;

    private readonly bool[] _nearbyTiles = new bool[8];

    public AutoTileTexture(Texture seedTexture, int tileSize)
    {
        GenerateTexture(seedTexture, tileSize);
    }

    private Texture2D _texture;

    // Get offset of the tile's texture based on which neighbor tiles are not the same as this one.
    public Sprite GetSprite(bool[] neighbors)
    {
        var mask = 0;
        for (var i = 0; i < neighbors.Length; i++)
        {
            if (!neighbors[i]) continue;
            mask |= 1 << i;
        }

        return _maskSprites[mask];
    }

    private void GenerateTexture(Texture seedTexture, int tileSize)
    {
        // 256 combinations of neighbors are possible, but only 47 are unique.
        const int tileCount = 47;
        
        var halfTileSize = tileSize / 2;
        var minWidth = tileCount * tileSize;
        var texWidth = 1;

        while (texWidth < minWidth) texWidth *= 2;

        _texture = new Texture2D(texWidth, tileSize, TextureFormat.ARGB32, false)
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
        for (var i = 0; i < 8; i++) _nearbyTiles[i] = (mask & (1 << i)) != 0;
    }
    
    private void CreateSubTileTextureLayout(int mask, int tileSize, int halfTileSize, Texture seedTexture)
    {
        var layout = new Layout();
        var tileTexX = _tileTexCount * tileSize;

        for (var i = 0; i < 4; i++)
        {
            var texX = 1;
            var texY = 1;

            var subTileX = i % 2;
            var subTileY = i / 2;

            var offsetX = subTileX * 2 - 1;
            var offsetY = subTileY * 2 - 1;

            if (GetNearby(offsetX, 0)) texX += offsetX;
            if (GetNearby(0, offsetY)) texY += offsetY;
            if (texX == 1 && texY == 1 && GetNearby(offsetX, offsetY))
            {
                texX = 4 - subTileX;
                texY = 1 - subTileY;
            }

            var subTileTexX = texX * halfTileSize;
            var subTileTexY = seedTexture.height - texY * halfTileSize - halfTileSize;

            Graphics.CopyTexture(seedTexture, 0, 0, subTileTexX, subTileTexY, halfTileSize, halfTileSize, _texture,
                0, 0, tileTexX + subTileX * halfTileSize, _texture.height - subTileY * halfTileSize - halfTileSize);
            
            layout.SetTexPos(i, texX, texY);
        }

        // Reuse existing sprites if this new one is not unique.
        if (_layoutSprites.TryGetValue(layout, out Sprite existingSprite))
        {
            _maskSprites.Add(mask, existingSprite);
        }
        else
        {
            var sprite = Sprite.Create(_texture, new Rect(tileTexX, 0, tileSize, tileSize), new Vector2(0.5f, 0.5f),
                tileSize, 0);
            _layoutSprites.Add(layout, sprite);
            _maskSprites.Add(mask, sprite);
            _tileTexCount++;
        }
    }

    // Get the index of a neighbor tile, skip the center tile which isn't a neighbor.
    private bool GetNearby(int x, int y)
    {
        x += 1;
        y += 1;

        var index = x + y * 3;

        if (y > 1 || (y == 1 && x > 1)) index--;

        return _nearbyTiles[index];
    }
}