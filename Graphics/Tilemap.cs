using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Monolith.Util;
using Monolith;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
            _tiles = new int[Count];

            Engine.DrawManager.Tilemaps.Add(this);
        }

        
        public void SetTile(int index, int tilesetID)
        {
            _tiles[index] = tilesetID;
        }

        public void SetTile(int column, int row, int tilesetID)
        {
            int index = row * Columns + column;
            SetTile(index, tilesetID);
        }

        public TextureRegion GetTile(int index)
        {
            return _tileset.GetTile(_tiles[index]);
        }

        public TextureRegion GetTile(int column, int row)
        {
            int index = row * Columns + column;
            return GetTile(index);
        }
    
        public void Draw(SpriteBatch spriteBatch, float layerDepth)
        {
            for (int i = 0; i < Count; i++)
            {
                int tileSetIndex = _tiles[i];
                TextureRegion tile = _tileset.GetTile(tileSetIndex);

                int x = i % Columns;
                int y = i / Columns;

                Vector2 position = new Vector2(x * TileWidth, y * TileHeight);

                Engine.DrawManager.Draw(
                    tile,
                    position,
                    Color.White,
                    layerDepth: layerDepth
                );
            }
        }
        
    }
}