using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Hierarchy;

namespace Monolith.Tools
{
    public static class CordTools
    {
        public static readonly Point[] Directions =
        {
            new Point(-1, 0),
            new Point(1, 0),
            new Point(0, 1),
            new Point(0, -1),

            new Point(-1, -1),
            new Point(-1, 1),
            new Point(1, -1),
            new Point(1, 1)
        };

        public static Point ToWorldCords(this Point baseData, int cellWidth, int cellHeight)
        {
            return new Point(baseData.X * cellWidth, baseData.Y * cellHeight);
        }

        public static Point ToPlaneCords(this Point baseData, int cellWidth, int cellHeight)
        {
            return new Point(baseData.X / cellWidth, baseData.Y / cellHeight);
        }


        public static List<Point> ToWorldCords(this List<Point> baseData, int cellWidth, int cellHeight)
        {
            var data = new List<Point>();

            foreach (var point in baseData)
            {
                data.Add(ToWorldCords(point, cellWidth, cellHeight));
            }

            return data;
        }

        public static List<Point> ToPlaneCords(this List<Point> baseData, int cellWidth, int cellHeight)
        {
            var data = new List<Point>();

            foreach (var point in baseData)
            {
                data.Add(ToPlaneCords(point, cellWidth, cellHeight));
            }

            return data;
        }

        public static Point FindNearestSafeTile(this Point unsafeTile, int[,] grid)
        {
            if (unsafeTile.IsValidLocation(grid))
                return unsafeTile;

            var safeCord = new Point();
            foreach (var dir in Directions)
            {
                var newLocation = unsafeTile + dir;

                if (newLocation.IsValidLocation(grid))
                {
                    safeCord = newLocation;
                    break;
                }
            }

            return safeCord;
        }

        public static void Write(this int[,] data, string filePath)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                for (int i = 0; i < rows; i++)
                {
                    string[] row = new string[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        row[j] = data[i, j].ToString();
                    }
                    sw.WriteLine(string.Join(",", row));
                }
            }
        }

        public static int[,] CreateNavGrid(Rectangle area, int tileWidth, int tileHeight)
        {
            var usableArea = area.Snap(tileWidth, tileHeight);

            int gridWidth = usableArea.Width / tileWidth;
            int gridHeight = usableArea.Height / tileHeight;

            int[,] grid = new int[gridHeight, gridWidth];

            var nearby = Engine.Physics.Query([usableArea]);

            foreach (var body in nearby)
            {
                var shapes = body.CollisionShapes;

                foreach (var shape in shapes)
                {
                    if (shape == null)
                        continue;

                    var snapShape = shape.Shape.ToRectangle(shape.Transform.Global.Position.ToPoint()).Snap(tileWidth, tileHeight);

                    int startX = snapShape.X / tileWidth;
                    int endX = (snapShape.X + snapShape.Width) / tileWidth;

                    int startY = snapShape.Y / tileHeight;
                    int endY = (snapShape.Y + snapShape.Height) / tileHeight;

                    for (int y = startY; y < endY; y++)
                    {
                        for (int x = startX; x < endX; x++)
                        {
                            var p = new Point(x, y);
                            if (p.IsInBounds(grid))
                            {
                                grid[y, x] = 1;
                            }
                        }
                    }
                }
            }

            return grid;
        }

        public static bool IsValidLocation(this Point location, int[,] grid)
        {
            return location.IsInBounds(grid) && grid[location.Y, location.X] == 0;
        }

        public static bool IsInBounds(this Point location, int[,] grid)
        {
            return location.X >= 0 &&
                location.Y >= 0 &&
                location.X < grid.GetLength(1) &&
                location.Y < grid.GetLength(0);
        }
    }
}