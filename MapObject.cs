namespace ConFPS
{
    public struct MapObject
    {
        public MapObject(float x, float y, Sprite s)
        {
            X = x;
            Y = y;
            Sprite = s;
        }

        public float X;
        public float Y;
        public Sprite Sprite;
    }
}