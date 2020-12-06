namespace ConFPS
{
    public class GameObject
    {
        public GameObject(float x, float y, Sprite s)
        {
            X = x;
            Y = y;
            Sprite = s;

            VX = VY = 0;
            Remove = false;
        }

        public float X;
        public float Y;
        public float VX;
        public float VY;
        public bool Remove;
        public Sprite Sprite;
    }
}