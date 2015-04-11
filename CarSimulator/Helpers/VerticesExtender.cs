using System.Collections.Generic;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using SFML.Graphics;

namespace CarSimulator
{
    public static class VerticesExtender
    {
        public static List<float> ToFloatList(this Vertices verts)
        {
            var floats = new List<float>();

            foreach (var vert in verts)
            {
                floats.Add(vert.X);
                floats.Add(vert.Y);
            }

            return floats;
        }

        public static Vertices ToVertices(this List<float> floats)
        {
            var verts = new Vertices();

            for (var i = 0; i < floats.Count; i += 2)
            {
                verts.Add(new Vector2(floats[i], floats[i + 1]));
            }

            return verts;
        }

        public static List<Vertices> ToVertices(this List<List<float>> floats)
        {
            var verts = new List<Vertices>();

            for (var i = 0; i < floats.Count; i++)
            {
                verts.Add(floats[i].ToVertices());
            }

            return verts;
        }

        /*
        public static List<Vertices> ToMarchingSquares(this List<Vertices> verts, float cellSize)
        {
            float realCell = cellSize.DisplayToSim();

            AABB aabb = verts.GetAABB();
            int xCount = (int)(aabb.Width / realCell);
            int yCount = (int)(aabb.Height / realCell);

            sbyte[,] f = new sbyte[xCount + 1, yCount + 1];

            const int cellDivCount = 2;
            float cellMargin = realCell / (float)cellDivCount;

            int pointCount = (cellDivCount - 1) * (cellDivCount - 1);
            float add = (float)byte.MaxValue / (float)pointCount;

            // Loop all cells
            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    Vector2 cellPos = new Vector2(aabb.LowerBound.X + x * realCell, aabb.LowerBound.Y + y * realCell);
                    AABB cell = new AABB(
                        new Vector2(
                            cellPos.X + cellMargin, 
                            cellPos.Y + cellMargin), 
                        new Vector2(
                            cellPos.X + realCell - cellMargin, 
                            cellPos.Y + realCell - cellMargin));

                    float value = (float)sbyte.MaxValue;

                    // Loop inside the cell
                    for (int j = 0; j < cellDivCount - 1; j++)
                    {
                        for (int i = 0; i < cellDivCount - 1; i++)
                        {
                            Vector2 point = new Vector2(cell.LowerBound.X + i * cellMargin, cell.LowerBound.Y + j * cellMargin);

                            bool inside = false;
                            foreach (var vert in verts)
                            {
                                int test = vert.PointInPolygon(ref point);

                                if (test == 1 || test == 0)
                                {
                                    inside = true;
                                    break;
                                }
                            }

                            if (inside)
                                value -= add;
                        }
                    }

                    f[x, y] = Convert.ToSByte(value);
                    //f[x, y] = sbyte.MinValue;
                }
            }

            return MarchingSquares.DetectSquares(aabb, realCell, realCell, f, 3, false);
        }
         */

        /*
        public static AABB GetAABB(this List<Vertices> allVerts)
        {
            AABB totalAABB = new AABB();
            bool first = true;

            foreach (var vert in allVerts)
            {
                AABB vertAABB = vert.GetAABB();

                if (first)
                {
                    totalAABB = vertAABB;
                    first = false;
                }
                else
                {
                    totalAABB.LowerBound = new Microsoft.Xna.Framework.Vector2(Math.Min(totalAABB.LowerBound.X, vertAABB.LowerBound.X), Math.Min(totalAABB.LowerBound.Y, vertAABB.LowerBound.Y));
                    totalAABB.UpperBound = new Microsoft.Xna.Framework.Vector2(Math.Max(totalAABB.UpperBound.X, vertAABB.UpperBound.X), Math.Max(totalAABB.UpperBound.Y, vertAABB.UpperBound.Y));
                }
            }

            return totalAABB;
        }
         */

        /*
        public static sbyte[,] ToTerrainMap(this Vertices verts, int xCount, int yCount)
        {
            AABB aabb = verts.GetAABB();

            sbyte[,] map = new sbyte[xCount, yCount];
            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    map[x, y] = 1;
                }
            }

            Vector2 topLeft = new Vector2(aabb.LowerBound.X, aabb.LowerBound.Y);
            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    Vector2 point = topLeft + new Vector2(
                        ((float)x / (float)xCount) * aabb.Width, 
                        ((float)y / (float)yCount) * aabb.Height);
                    bool test = verts.PointInPolygonAngle(ref point);

                    if (test)
                        map[x, y] = -1;
                }
            }


            return map;
        }

         */

        public static Vertices ToVertices(this ConvexShape convex)
        {
            var verts = new Vertices();
            for (uint i = 0; i < convex.GetPointCount(); i++)
            {
                verts.Add(new Vector2f(convex.GetPoint(i)).ToSim());
            }
            return verts;
        }
    }
}