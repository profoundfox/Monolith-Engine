using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith.Geometry;

namespace Monolith.Tools
{
    public static class ShapeTools
    {
        public static RectangleShape2D ToShape(this Rectangle normRect)
        {
            return new RectangleShape2D(normRect.Width, normRect.Height);
        }

        public static Rectangle ToRectangle(this IShape2D shape, Point location)
        {
            return new Rectangle(location.X, location.Y, shape.Size.Width, shape.Size.Height);
        }

        public static Rectangle Snap(this Rectangle rect, int tileWidth, int tileHeight)
        {
            if (tileWidth <= 0 || tileHeight <= 0)
                throw new ArgumentException("Tile size must be greater than 0.");

            int newX = (rect.X / tileWidth) * tileWidth;
            int newY = (rect.Y / tileHeight) * tileHeight;

            int right = rect.X + rect.Width;
            int bottom = rect.Y + rect.Height;

            int newRight = (right / tileWidth) * tileWidth;
            int newBottom = (bottom / tileHeight) * tileHeight;

            int newWidth = newRight - newX;
            int newHeight = newBottom - newY;

            return new Rectangle(newX, newY, newWidth, newHeight);
        }

        public static List<Rectangle> Split(this Rectangle rect, int splitWidth, int splitHeight)
        {
            var result = new List<Rectangle>();

            for (int y = rect.Top; y < rect.Bottom; y += splitHeight)
            {
                for (int x = rect.Left; x < rect.Right; x += splitWidth)
                {
                    int width = Math.Min(splitWidth, rect.Right - x);
                    int height = Math.Min(splitHeight, rect.Bottom - y);

                    result.Add(new Rectangle(x, y, width, height));
                }
            }

            return result;
        }
    }
}