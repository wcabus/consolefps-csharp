using System;

namespace ConFPS
{
    public class Player
    {
        private readonly Game _game;
        private readonly Random _random = new Random();

        public Player(Game game)
        {
            _game = game;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Angle { get; set; }
        public static float Fov => ConsoleGameEngine.PI_OVER_4;

        private const float Speed = 5f;

        public void HandleInput(float elapsed)
        {
            if (_game.Keys['A'].Held)
            {
                Angle -= .75f * Speed * elapsed;
            }

            if (_game.Keys['D'].Held)
            {
                Angle += .75f * Speed * elapsed;
            }

            if (_game.Keys['W'].Held)
            {
                Move(1, elapsed);
            }

            if (_game.Keys['S'].Held)
            {
                Move(-1, elapsed);
            }

            if (_game.Keys['Q'].Held)
            {
                Strafe(-1, elapsed);
            }

            if (_game.Keys['E'].Held)
            {
                Strafe(1, elapsed);
            }

            // Fire a bullet
            if (_game.Keys[VirtualKeys.VK_SPACE].Released)
            {
                var bullet = new GameObject(X, Y, _game.BulletSprite);

                var noise = ((float) _random.NextDouble() - 0.5f) * 0.1f;
                bullet.VX = (float)Math.Sin(Angle + noise) * 8f;
                bullet.VY = (float)Math.Cos(Angle + noise) * 8f;
                
                _game.AddObject(bullet);
            }
        }

        private void Move(int direction, float elapsed)
        {
            var dx = (float)Math.Sin(Angle) * Speed * elapsed;
            var dy = (float)Math.Cos(Angle) * Speed * elapsed;

            X += direction * dx;
            Y += direction * dy;

            if (_game.Map.HitsWall((int)X, (int)Y))
            {
                X -= direction * dx;
                Y -= direction * dy;
            }

            Clamp();
        }

        private void Strafe(int direction, float elapsed)
        {
            var dx = (float)Math.Cos(Angle) * Speed * elapsed;
            var dy = -1 * (float)Math.Sin(Angle) * Speed * elapsed;

            X += direction * dx;
            Y += direction * dy;

            if (_game.Map.HitsWall((int)X, (int)Y))
            {
                X -= direction * dx;
                Y -= direction * dy;
            }

            Clamp();
        }

        private void Clamp()
        {
            if (X < 0)
            {
                X = 0;
            }

            if (X > _game.Map.Width - 1)
            {
                X = _game.Map.Width - 1;
            }

            if (Y < 0)
            {
                Y = 0;
            }

            if (Y > _game.Map.Height - 1)
            {
                Y = _game.Map.Height - 1;
            }
        }
    }
}