using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoTileTexture
{
    public Texture2D Texture { get; private set; }

    private bool[] nearbyTiles = new bool[8];
    private Dictionary<int, Sprite> maskOffset = new();

    public AutoTileTexture(Texture seedTexture, int tileSize)
    {
        GenerateTexture(seedTexture, tileSize);
    }

    // Get offset of the tile's texture based on which neighbor tiles are not the same as this one.
    public Sprite GetSprite(bool tl, bool tc, bool tr, bool cl, bool cr, bool bl, bool bc, bool br)
    {
        int mask = 0;
        // TODO: Maybe don't do a bitshift every time, these could be constants.
        if (tl) mask |= 1 << 0;
        if (tc) mask |= 1 << 1;
        if (tr) mask |= 1 << 2;
        if (cl) mask |= 1 << 3;
        if (cr) mask |= 1 << 4;
        if (bl) mask |= 1 << 5;
        if (bc) mask |= 1 << 6;
        if (br) mask |= 1 << 7;

        return maskOffset[mask];
    }

    private void GenerateTexture(Texture seedTexture, int tileSize)
    {
        int halfTileSize = tileSize / 2;
        const int tileCount = 257; // TODO: Determine the actual size, probably around 40.
        int minWidth = tileCount * tileSize;
        int texWidth = 1;

        while (texWidth < minWidth)
        {
            texWidth *= 2;
        }

        Texture = new Texture2D(texWidth, tileSize, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };
        int tileIndex = 0;

        // Loop over a bitmask representing all possible combinations of neighboring tiles.
        for (int mask = 0; mask <= 255; mask++)
        {
            int tileTexX = tileIndex * tileSize;

            // Update neighbors:
            for (int i = 0; i < 8; i++)
            {
                nearbyTiles[i] = (mask & (1 << i)) != 0;
            }

            // Loop over subtiles:
            for (int i = 0; i < 4; i++)
            {
                int texX = 1;
                int texY = 1;

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

                Graphics.CopyTexture(seedTexture, 0, 0, subTileTexX, subTileTexY, halfTileSize, halfTileSize, Texture, 0, 0, tileTexX + subTileX * halfTileSize, Texture.height - subTileY * halfTileSize - halfTileSize);
            }

            maskOffset.Add(mask, Sprite.Create(Texture, new Rect(tileTexX, 0, tileSize, tileSize), new Vector2(0.5f, 0.5f), tileSize, 0));

            tileIndex++;
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
