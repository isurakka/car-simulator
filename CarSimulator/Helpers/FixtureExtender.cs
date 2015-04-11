using System;
using System.Diagnostics;
using System.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SFML.System;

namespace CarSimulator
{
    public static class FixtureExtender
    {
        public static Vertices GetLocalVertices(this Fixture fix)
        {
            switch (fix.Shape.ShapeType)
            {
                case ShapeType.Circle:
                {
                    var verts = new Vertices();

                    var shape = (CircleShape) fix.Shape;
                    for (float i = 0; i < shape.Radius; i += shape.Radius/shape.Radius)
                    {
                        var perc = i/shape.Radius;

                        //        circle angle                      radius         relative pos
                        var x = (float) Math.Sin(perc*Math.PI)*shape.Radius + shape.Position.X;
                        var y = (float) Math.Cos(perc*Math.PI)*shape.Radius + shape.Position.X;
                        verts.Add(new Vector2f(x, y).ToXNA());
                    }

                    return verts;
                }
                case ShapeType.Polygon:
                {
                    var shape = (PolygonShape) fix.Shape;
                    Debug.Assert(
                        !shape.Vertices.Any(
                            v =>
                                float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNegativeInfinity(v.X) ||
                                float.IsNegativeInfinity(v.Y) || float.IsPositiveInfinity(v.X) ||
                                float.IsPositiveInfinity(v.Y)));
                    return shape.Vertices;
                }
                default:
                {
                    return new Vertices();
                    //throw new NotImplementedException();
                }
            }
        }

        public static Vertices GetWorldVertices(this Fixture fix)
        {
            Transform tf;
            fix.Body.GetTransform(out tf);

            Debug.Assert(!float.IsNaN(tf.p.X));
            Debug.Assert(!float.IsNaN(tf.p.Y));
            Debug.Assert(!float.IsNaN(tf.q.GetAngle()));

            var local = fix.GetLocalVertices();
            var world = new Vertices(local.Count);

            for (var i = 0; i < local.Count; i++)
            {
                //world[i] = MathUtils.Mul(ref tf, local[i]);
                world.Add(MathUtils.Mul(ref tf, local[i]));
            }

            Debug.Assert(
                !world.Any(
                    v =>
                        float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNegativeInfinity(v.X) ||
                        float.IsNegativeInfinity(v.Y) || float.IsPositiveInfinity(v.X) || float.IsPositiveInfinity(v.Y)));

            return world;
        }

        /*
        public static AABB GetWorldAABB(this Fixture fix)
        {
            Transform tf;
            fix.Body.GetTransform(out tf);

            switch (fix.Shape.ShapeType)
            {
                case ShapeType.Circle:
                    {
                        AABB aabb;

                        CircleShape shape = (CircleShape)fix.Shape;
                        
                        shape.ComputeAABB(out aabb, ref tf, 0);

                        return aabb;
                    }
                case ShapeType.Polygon:
                    {
                        AABB aabb;

                        PolygonShape shape = (PolygonShape)fix.Shape;
                        
                        shape.ComputeAABB(out aabb, ref tf, 0);

                        return aabb;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
         */

        public static Vector2 GetWorldCentroid(this Fixture fix)
        {
            var local = fix.GetLocalVertices();

            return fix.Body.GetWorldPoint(local.GetCentroid());
        }
    }
}