using System;

namespace Game
{
    public static class Utils
    {
        private static Random rng = new Random();

        public static Position GetRandomPosition(int width, int height)
        {
            return new Position(rng.Next(width), rng.Next(height));
        }
    }
}
