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
        private int[,] _tiles;

        public Tileset Tileset { get; set; }

        private int rows;
        private int columns;

        public int Rows
        {
            get => rows;
            set
            {
                rows = value;
                Rebuild();
            }
        }

        public int Columns
        {
            get => columns;
            set
            {
                columns = value;
                Rebuild();
            }
        }

        public int IndexOffset { get; set; } = 0;

        public Tilemap() {}

        private void Rebuild()
        {
            if (columns <= 0 || rows <= 0)
                return;

            _tiles = new int[columns, rows];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    _tiles[x, y] = -1;
                }
            }
        }

        public MTexture GetTile(int column, int row)
        {
            if (_tiles == null ||
                column < 0 || column >= columns ||
                row < 0 || row >= rows)
                return null;

            int tileIndex = _tiles[column, row] + IndexOffset;
            if (tileIndex < 0)
                return null;

            return Tileset.GetTile(tileIndex);
        }

        public void SetTile(int column, int row, int tileIndex)
        {
            if (_tiles == null ||
                column < 0 || column >= columns ||
                row < 0 || row >= rows)
                return;

            _tiles[column, row] = tileIndex - IndexOffset;
        }

        public void SetData(int[,] data)
        {
            if (data == null)
                return;

            int w = data.GetLength(0);
            int h = data.GetLength(1);

            Columns = w;
            Rows = h;

            _tiles = new int[Columns, Rows];

            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                    _tiles[x, y] = data[x, y] - IndexOffset;
        }

        public override void SubmitCall()
        {
            base.SubmitCall();

            if (_tiles == null || Tileset == null)
                return;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int tileSetIndex = _tiles[x, y];
                    if (tileSetIndex < 0)
                        continue;

                    MTexture tile = Tileset.GetTile(tileSetIndex);


                    Vector2 localTilePos = new Vector2(
                        x * Tileset.TileWidth,
                        y * Tileset.TileHeight
                    );


                    Vector2 worldTilePos = localTilePos + GlobalPosition;

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
}
