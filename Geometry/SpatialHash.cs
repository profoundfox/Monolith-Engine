

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public class SpatialHash<T> where T : IHashAble
    {
        private readonly Dictionary<Point, List<T>> _cells = new();
        private readonly HashSet<T> _queryCache = new();
        private float _cellSize;

        public SpatialHash(float cellSize = 64f)
        {
            _cellSize = cellSize;
        }

        /// <summary>
        /// Gets the cell a rectangle intersects.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private IEnumerable<Point> GetCellsForBounds(Rectangle bounds)
        {
            int minX = (int)Math.Floor(bounds.Left / _cellSize);
            int maxX = (int)Math.Floor(bounds.Right / _cellSize);
            int minY = (int)Math.Floor(bounds.Top / _cellSize);
            int maxY = (int)Math.Floor(bounds.Bottom / _cellSize);

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    yield return new Point(x, y);
        }

        private IEnumerable<Point> GetCellsForBounds(IEnumerable<Rectangle> boundsList)
        {
            var seen = new HashSet<Point>();

            foreach (var bounds in boundsList)
            {
                int minX = (int)Math.Floor(bounds.Left / _cellSize);
                int maxX = (int)Math.Floor((bounds.Right - 1) / _cellSize);
                int minY = (int)Math.Floor(bounds.Top / _cellSize);
                int maxY = (int)Math.Floor((bounds.Bottom - 1) / _cellSize);

                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        var p = new Point(x, y);
                        if (seen.Add(p))
                            yield return p;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts an object.
        /// </summary>
        /// <param name="obj"></param>
        public void Insert(T obj)
        {
            foreach(var cell in GetCellsForBounds(obj.Bounds))
            {
                if (!_cells.TryGetValue(cell, out var list))
                {
                    list = new List<T>();
                    _cells[cell] = list;
                }
                list.Add(obj);
            }
        }

        /// <summary>
        /// Removes an object.
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(T obj)
        {
            foreach(var cell in GetCellsForBounds(obj.Bounds))
            {
                if (_cells.TryGetValue(cell, out var list))
                {
                    list.Remove(obj);

                    if (list.Count == 0)
                        _cells.Remove(cell);
                }
            }
        }

        /// <summary>
        /// Removes an object from old cells.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="oldBounds"></param>
        private void RemoveFromOldCells(T obj, List<Rectangle> oldBounds)
        {
            foreach(var cell in GetCellsForBounds(oldBounds))
            {
                if (_cells.TryGetValue(cell, out var list))
                    list.Remove(obj);
            }
        }

        /// <summary>
        /// Gets the cells a rectangle intersects.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public List<T> Query(List<Rectangle> bounds)
        {
            _queryCache.Clear();

            foreach (var cell in GetCellsForBounds(bounds))
            {
                if (_cells.TryGetValue(cell, out var list))
                    foreach (var obj in list)
                        _queryCache.Add(obj);
            }

            return _queryCache.ToList();
        }

        /// <summary>
        /// Updates hashes.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="oldBounds"></param>
        public void Update(T obj, List<Rectangle> oldBounds)
        {
            var oldCells = GetCellsForBounds(oldBounds).ToHashSet();
            var newCells = GetCellsForBounds(obj.Bounds).ToHashSet();

            foreach (var cell in oldCells.Except(newCells))
            {
                if (_cells.TryGetValue(cell, out var list))
                    list.Remove(obj);
            }

            foreach (var cell in newCells.Except(oldCells))
            {
                if (!_cells.TryGetValue(cell, out var list))
                {
                    list = new List<T>();
                    _cells[cell] = list;
                }
                list.Add(obj);
            }
        }
    }
    

    public interface IHashAble
    {
        List<Rectangle> Bounds { get; }
    }
}