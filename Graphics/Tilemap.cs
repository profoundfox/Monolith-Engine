using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Managers;

namespace Monolith.Graphics
{
    public class Tilemap
    {
        private readonly Tileset _tileset;
        private readonly int[] _tiles;

        public int Rows { get; }
        public int Columns { get; }
        public int Count { get; }

        public float LayerDepth { get; set; }
        public float TileWidth => _tileset.TileWidth;
        public float TileHeight => _tileset.TileHeight;

        public Tilemap(Tileset tileset, int columns, int rows, float layerDepth)
        {
            LayerDepth = layerDepth;
            _tileset = tileset;
            Rows = rows;
            Columns = columns;
            Count = Columns * Rows;

            // Initialize all tiles as empty (-1)
            _tiles = new int[Count];
            for (int i = 0; i < Count; i++)
                _tiles[i] = -1;

            Engine.DrawManager.Tilemaps.Add(this);
        }

        public void SetTile(int index, int tilesetID)
        {
            _tiles[index] = tilesetID;
        }

        public void SetTile(int column, int row, int tilesetID)
        {
            SetTile(row * Columns + column, tilesetID);
        }

        public MTexture GetTile(int index)
        {
            return _tileset.GetTile(_tiles[index]);
        }

        public MTexture GetTile(int column, int row)
        {
            return GetTile(row * Columns + column);
        }

        public void Draw()
        {
            for (int i = 0; i < Count; i++)
            {
                int tileSetIndex = _tiles[i];

                if (tileSetIndex < 0)
                    continue;

                MTexture tile = _tileset.GetTile(tileSetIndex);

                int x = i % Columns;
                int y = i / Columns;

                Vector2 position = new Vector2(
                    x * TileWidth,
                    y * TileHeight
                );

                tile.Draw(position, Color.White, 0f, Vector2.Zero, null, SpriteEffects.None, null, LayerDepth);
                
            }
        }
    }
}
