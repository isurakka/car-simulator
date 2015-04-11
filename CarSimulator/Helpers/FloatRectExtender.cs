using FarseerPhysics.Collision;
using SFML.Graphics;

namespace CarSimulator
{
    public static class FloatRectExtender
    {
        public static FloatRect ToFloatRect(this AABB aabb)
        {
            return new FloatRect(
                aabb.LowerBound.X.SimToDisplay(),
                aabb.LowerBound.Y.SimToDisplay(),
                (aabb.UpperBound.X - aabb.LowerBound.X).SimToDisplay(),
                (aabb.UpperBound.Y - aabb.LowerBound.Y).SimToDisplay());
        }
    }
}