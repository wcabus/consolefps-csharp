using System;

namespace ConFPS
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Layout { get; set; }

        public float Depth => Math.Max(Width, Height) / 2f;

        public bool OutOfBounds(in int x, in int y)
        {
            return x < 0 || x >= Width || y < 0 || y >= Height;
        }

        public bool HitsWall(in int x, in int y)
        {
            return Layout[y * Width + x] == '#';
        }
    }
}