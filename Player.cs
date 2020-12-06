using System;

namespace ConFPS
{
    public class Player
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Angle { get; set; }
        public static float Fov => ConsoleGameEngine.PI_OVER_4;

        private const float Speed = 5f;

        public void HandleInput(in KeyState[] keys, float elapsed, Map map)
        {
            if (keys['A'].Held)
            {
                Angle -= .75f * Speed * elapsed;
            }

            if (keys['D'].Held)
            {
                Angle += .75f * Speed * elapsed;
            }

            if (keys['W'].Held)
            {
                Move(1, elapsed, map);
            }

            if (keys['S'].Held)
            {
                Move(-1, elapsed, map);
            }

            if (keys['Q'].Held)
            {
                Strafe(-1, elapsed, map);
            }

            if (keys['E'].Held)
            {
                Strafe(1, elapsed, map);
            }
        }

        private void Move(int direction, float elapsed, Map map)
        {
            var dx = (float)Math.Sin(Angle) * Speed * elapsed;
            var dy = (float)Math.Cos(Angle) * Speed * elapsed;

            X += direction * dx;
            Y += direction * dy;

            if (map.HitsWall((int)X, (int)Y))
            {
                X -= direction * dx;
                Y -= direction * dy;
            }
        }

        private void Strafe(int direction, float elapsed, Map map)
        {
            var dx = (float)Math.Cos(Angle) * Speed * elapsed;
            var dy = -1 * (float)Math.Sin(Angle) * Speed * elapsed;

            X += direction * dx;
            Y += direction * dy;

            if (map.HitsWall((int)X, (int)Y))
            {
                X -= direction * dx;
                Y -= direction * dy;
            }
        }
    }
}