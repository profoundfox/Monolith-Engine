using System;
using Microsoft.Xna.Framework;
using Monolith.Graphics;

namespace Monolith.Graphics
{
    public class Tileset
    {
        private readonly MTexture[] _tiles;

        /// <summary>
        /// Gets the width, in pixels, of each tile in this tileset.
        /// </summary>
        public int TileWidth { get; }

        /// <summary>
        /// Gets the height, in pixels, of each tile in this tileset.
        /// </summary>
        public int TileHeight { get; }

        /// <summary>
        /// Gets the total number of columns in this tileset.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Gets the total number of rows in this tileset.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// Gets the total number of tiles in this tileset.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Creates a new tileset based on the given texture with the specified
        /// tile width and height.
        /// </summary>
        /// <param name="texture">The texture that contains the tiles for the tileset.</param>
        /// <param name="tileWidth">The width of each tile in the tileset.</param>
        /// <param name="tileHeight">The height of each tile in the tileset.</param>
        public Tileset(MTexture texture, int tileWidth, int tileHeight)
        {
            if (texture == null) throw new ArgumentNullException(nameof(texture));

            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Columns = texture.Width / tileWidth;
            Rows = texture.Height / tileHeight;
            Count = Columns * Rows;

            _tiles = new MTexture[Count];

            Rectangle baseRect = texture.SourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);

            for (int i = 0; i < Count; i++)
            {
                int x = i % Columns * tileWidth;
                int y = i / Columns * tileHeight;

                _tiles[i] = texture.CreateSubTexture(
                    new Rectangle
                    (
                        baseRect.X + x,
                        baseRect.Y + y,
                        tileWidth,
                        tileHeight
                    )
                );
            }
        }

        /// <summary>
        /// Gets the tile at the specified index.
        /// </summary>
        public MTexture GetTile(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException($"Tile index {index} is out of bounds.");

            return _tiles[index];
        }

        /// <summary>
        /// Gets the tile at the specified column and row.
        /// </summary>
        public MTexture GetTile(int column, int row)
        {
            if (column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException(nameof(column), "Column is out of bounds.");

            if (row < 0 || row >= Rows)
                throw new ArgumentOutOfRangeException(nameof(row), "Row is out of bounds.");

            int index = row * Columns + column;
            return GetTile(index);
        }
    }
}
