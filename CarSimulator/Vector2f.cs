using System;
using System.Diagnostics;
using FarseerPhysics;
using Microsoft.Xna.Framework;

namespace CarSimulator
{
    public struct Vector2f
    {
        public float X;
        public float Y;

        public Vector2f(float x, float y)
        {
            X = x;
            Y = y;
            Debug.Assert(!float.IsNaN(X) && !float.IsNaN(Y) && !float.IsNegativeInfinity(X) &&
                         !float.IsNegativeInfinity(Y) && !float.IsPositiveInfinity(X) && !float.IsPositiveInfinity(Y));
        }

        public Vector2f(SFML.System.Vector2f v1)
        {
            X = v1.X;
            Y = v1.Y;
            Debug.Assert(!float.IsNaN(X) && !float.IsNaN(Y) && !float.IsNegativeInfinity(X) &&
                         !float.IsNegativeInfinity(Y) && !float.IsPositiveInfinity(X) && !float.IsPositiveInfinity(Y));
        }

        public Vector2f(Vector2 v1)
        {
            X = v1.X;
            Y = v1.Y;
            Debug.Assert(!float.IsNaN(X) && !float.IsNaN(Y) && !float.IsNegativeInfinity(X) &&
                         !float.IsNegativeInfinity(Y) && !float.IsPositiveInfinity(X) && !float.IsPositiveInfinity(Y));
        }

        public Vector2f InverseX
        {
            get { return new Vector2f(-X, Y); }
        }

        public Vector2f InverseY
        {
            get { return new Vector2f(X, -Y); }
        }

        public Vector2f Normalize()
        {
            Debug.Assert(!float.IsNaN(X) && !float.IsNaN(Y) && !float.IsNegativeInfinity(X) &&
                         !float.IsNegativeInfinity(Y) && !float.IsPositiveInfinity(X) && !float.IsPositiveInfinity(Y));
            var len = (float) Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            return new Vector2f(len != 0f ? X/len : 0f, len != 0f ? Y/len : 0f);
        }

        public float Length()
        {
            return (float) Math.Sqrt(X*X + Y*Y);
        }

        public Vector2f RotateDegrees(float angle)
        {
            angle = MathExtender.DegreeToRadian(angle);
            return RotateRadians(angle);
        }

        public Vector2f RotateRadians(float angle)
        {
            var newVec = new Vector2f(0f, 0f);
            newVec.X = (float) (X*Math.Cos(angle) - Y*Math.Sin(angle));
            newVec.Y = (float) (X*Math.Sin(angle) + Y*Math.Cos(angle));
            return newVec;
        }

        public Vector2f ToDisplay()
        {
            return new Vector2f(ConvertUnits.ToDisplayUnits(X), ConvertUnits.ToDisplayUnits(Y));
        }

        public Vector2f ToSim()
        {
            return new Vector2f(ConvertUnits.ToSimUnits(X), ConvertUnits.ToSimUnits(Y));
        }

        public static bool operator ==(Vector2f v1, Vector2f v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2f v1, Vector2f v2)
        {
            return !(v1 == v2);
        }

        public static Vector2f operator +(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2f operator -(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2f operator -(Vector2f v1)
        {
            return new Vector2f(-v1.X, -v1.Y);
        }

        public static Vector2f operator *(Vector2f v1, float mult)
        {
            return new Vector2f(v1.X*mult, v1.Y*mult);
        }

        public static Vector2f operator *(float mult, Vector2f v1)
        {
            return new Vector2f(v1.X*mult, v1.Y*mult);
        }

        public static Vector2f operator /(Vector2f v1, float mult)
        {
            return new Vector2f(v1.X/mult, v1.Y/mult);
        }

        public static Vector2f operator /(float mult, Vector2f v1)
        {
            return new Vector2f(mult/v1.X, mult/v1.Y);
        }

        public static explicit operator Vector2i(Vector2f v1)
        {
            return new Vector2i((int) v1.X, (int) v1.Y);
        }

        public static implicit operator SFML.System.Vector2f(Vector2f v1)
        {
            return new SFML.System.Vector2f(v1.X, v1.Y);
        }

        public static implicit operator Vector2(Vector2f v1)
        {
            return new Vector2(v1.X, v1.Y);
        }

        public override string ToString()
        {
            return "[" + X + "f, " + Y + "f]";
        }
    }
}