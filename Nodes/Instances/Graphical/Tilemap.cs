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
    public class Tilemap : Node2D
    {
        private int[] _tiles;

        public Tileset Tileset { get; set; }

        private int rows;
        public int Rows
        {
            get => rows;
            set
            {
                rows = value;
                Rebuild();
            }
        }

        private int columns;
        public int Columns
        {
            get => columns;
            set
            {
                columns = value;
                Rebuild();
            }
        }

        private int count;
        public int Count { get => count; }

        public List<int> Data { get; set; }

        public Tilemap() {}

        private void Rebuild()
        {
            if (Columns <= 0 || Rows <= 0)
                return;

            count = Columns * Rows;

            _tiles = new int[Count];

            for (int i = 0; i < Count; i++)
                _tiles[i] = -1;

            ApplyData();
        }

        private void ApplyData()
        {
            if (_tiles == null || Data == null)
                return;

            int max = Math.Min(Data.Count, Count);

            for (int i = 0; i < max; i++)
            {
                int tile = Data[i];

                _tiles[i] = tile == 0 ? -1 : tile;
            }
        }

        public MTexture GetTile(int index)
        {
            if (_tiles == null || index < 0 || index >= Count)
                return null;

            int tileIndex = _tiles[index];

            if (tileIndex < 0)
                return null;

            return Tileset.GetTile(tileIndex);
        }

        public MTexture GetTile(int column, int row)
        {
            return GetTile(row * Columns + column);
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

        public override void SubmitCall()
        {
            base.SubmitCall();

            if (_tiles == null || Tileset == null)
                return;

            for (int i = 0; i < Count; i++)
            {
                int tileSetIndex = _tiles[i];

                if (tileSetIndex < 0)
                    continue;

                MTexture tile = Tileset.GetTile(tileSetIndex);

                int x = i % Columns;
                int y = i / Columns;

               Vector2 localTilePos = new Vector2(
                    x * Tileset.TileWidth,
                    y * Tileset.TileHeight
                );

                Vector2 worldTilePos = localTilePos + GlobalTransform.Position;

                Engine.Canvas.Call(new TextureDrawCall
                {
                    Texture = tile,
                    Position = worldTilePos,
                    Color = GlobalVisibility.Modulate,
                    Rotation = GlobalTransform.Rotation,
                    Origin = Vector2.Zero,
                    Scale = GlobalTransform.Scale,
                    Effects = GlobalMaterial.SpriteEffects,
                    Depth = GlobalOrdering.Depth,
                    SpriteBatchConfig = SpriteBatchConfig.Default with
                    {
                        Effect = GlobalShader
                    }
                });
            }
        }
    }
}
