using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.IO;
using Monolith.Managers;
using Monolith.Attributes;

namespace Monolith.Nodes
{
    public record class TilemapConfig : SpatialNodeConfig
    {
        public Tileset Tileset { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
    }
    
    public class Tilemap : Node2D
    {
        private readonly int[] _tiles;

        public readonly Tileset Tileset;
        public int Rows { get; }
        public int Columns { get; }
        public int Count { get; }

        public float TileWidth => Tileset.TileWidth;
        public float TileHeight => Tileset.TileHeight;

        public Tilemap(TilemapConfig cfg) : base(cfg)
        {
            Tileset = cfg.Tileset;
            Rows = cfg.Rows;
            Columns = cfg.Columns;
            Count = Columns * Rows;

            _tiles = new int[Count];
            for (int i = 0; i < Count; i++)
                _tiles[i] = -1;
        }

        private void SetTile(int index, int tilesetID)
        {
            _tiles[index] = tilesetID;
        }

        private void SetTile(int column, int row, int tilesetID)
        {
            SetTile(row * Columns + column, tilesetID);
        }

        public void SetData(Rectangle size, List<int> tileData)
        {   
            for (int row = 0; row < size.Y; row++)
            {
                for (int col = 0; col < size.X; col++)
                {
                    int index = row * size.X + col;
                    int tile = (index >= 0 && index < tileData.Count) ? tileData[index] : 0;
                    SetTile(col, row, tile);
                }
            }
        }

        public MTexture GetTile(int index)
        {
            return Tileset.GetTile(_tiles[index]);
        }

        public MTexture GetTile(int column, int row)
        {
            return GetTile(row * Columns + column);
        }

        public override void SubmitCall()
        {
            base.SubmitCall();

            for (int i = 0; i < Count; i++)
            {
                int tileSetIndex = _tiles[i];

                if (tileSetIndex < 0)
                    continue;

                MTexture tile = Tileset.GetTile(tileSetIndex);

                int x = i % Columns;
                int y = i / Columns;

               Vector2 localTilePos = new Vector2(
                    x * TileWidth,
                    y * TileHeight
                );

                Vector2 worldTilePos = localTilePos + GlobalTransform.Position;

                tile.Draw(
                    worldTilePos,
                    GlobalVisibility.Modulate,
                    GlobalTransform.Rotation,
                    Vector2.Zero,
                    GlobalTransform.Scale,
                    GlobalMaterial.SpriteEffects,
                    GlobalMaterial.Shader,
                    GlobalOrdering.Depth
                );
            }
        }
    }
}
