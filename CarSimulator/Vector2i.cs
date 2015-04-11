using System.Collections.Generic;

namespace CarSimulator
{
    public struct Vector2i : IEqualityComparer<Vector2i>
    {
        public int X;
        public int Y;

        public Vector2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2i(SFML.System.Vector2i v1)
        {
            X = v1.X;
            Y = v1.Y;
        }

        /*
        public Vector2i ToDisplay()
        {
            return new Vector2i(ConvertUnits.ToDisplayUnits(X), ConvertUnits.ToDisplayUnits(Y));
        }

        public Vector2i ToSim()
        {
            return new Vector2i(ConvertUnits.ToSimUnits(X), ConvertUnits.ToSimUnits(Y));
        }
        */

        public Vector2i InverseX
        {
            get { return new Vector2i(-X, Y); }
        }

        public Vector2i InverseY
        {
            get { return new Vector2i(X, -Y); }
        }

        public bool Equals(Vector2i v1, Vector2i v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public int GetHashCode(Vector2i obj)
        {
            unchecked
            {
                int hash = 23;
                hash = hash * 37 + obj.X;
                hash = hash * 37 + obj.Y;
                return hash;
            }
        }

        public static Vector2i operator +(Vector2i v1, Vector2i v2)
        {
            return new Vector2i(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2i operator -(Vector2i v1, Vector2i v2)
        {
            return new Vector2i(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2i operator -(Vector2i v1)
        {
            return new Vector2i(-v1.X, -v1.Y);
        }

        public static Vector2i operator *(Vector2i v1, int mult)
        {
            return new Vector2i(v1.X*mult, v1.Y*mult);
        }

        public static Vector2i operator *(int mult, Vector2i v1)
        {
            return new Vector2i(v1.X*mult, v1.Y*mult);
        }

        public static Vector2i operator /(Vector2i v1, int mult)
        {
            return new Vector2i(v1.X/mult, v1.Y/mult);
        }

        public static Vector2i operator /(int mult, Vector2i v1)
        {
            return new Vector2i(mult/v1.X, mult/v1.Y);
        }

        public static implicit operator SFML.System.Vector2i(Vector2i v1)
        {
            return new SFML.System.Vector2i(v1.X, v1.Y);
        }

        public static explicit operator Vector2f(Vector2i v1)
        {
            return new Vector2f(v1.X, v1.Y);
        }

        public override string ToString()
        {
            return "[" + X + ", " + Y + "]";
        }
    }
}