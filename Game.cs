using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConFPS
{
    public class Game : ConsoleGameEngine
    {
        private Map _map;
        private Player _player;
        private bool _showMap;
        
        private Sprite _wallSprite;
        private Sprite _lampSprite;
        private List<MapObject> _objects = new List<MapObject>();

        public Game()
        {
            AppName = "Shoot'em up";
        }

        public async Task<int> Run()
        {
            ConstructConsole(320, 240, 4, 4);
            await Start();
            return 0;
        }

        protected override async Task<bool> OnUserCreate()
        {
            _map = new Map
            {
                Width = 32,
                Height = 32
            };

            _player = new Player
            {
                X = 8,
                Y = 8
            };

            await LoadSprites();
            InitializeMap(_map);

            return true;
        }

        private void InitializeMap(Map map)
        {
            var layout = new StringBuilder();
            layout
                .Append("#########.......#########.......")
                .Append("#...............#...............")
                .Append("#.......#########.......########")
                .Append("#..............##..............#")
                .Append("#......##......##......##......#")
                .Append("#......##..............##......#")
                .Append("#..............##..............#")
                .Append("###............####............#")
                .Append("##.............###.............#")
                .Append("#............####............###")
                .Append("#..............................#")
                .Append("#..............##..............#")
                .Append("#..............##..............#")
                .Append("#...........#####...........####")
                .Append("#..............................#")
                .Append("###..####....########....#######")
                .Append("####.####.......######..........")
                .Append("#...............#...............")
                .Append("#.......#########.......##..####")
                .Append("#..............##..............#")
                .Append("#......##......##.......#......#")
                .Append("#......##......##......##......#")
                .Append("#..............##..............#")
                .Append("###............####............#")
                .Append("##.............###.............#")
                .Append("#............####............###")
                .Append("#..............................#")
                .Append("#..............................#")
                .Append("#..............##..............#")
                .Append("#...........##..............####")
                .Append("#..............##..............#")
                .Append("################################");

            map.Layout = layout.ToString();

            _objects.Add(new MapObject(8.5f, 8.5f, _lampSprite));
            _objects.Add(new MapObject(7.5f, 7.5f, _lampSprite));
            _objects.Add(new MapObject(10.5f, 3.5f, _lampSprite));
        }

        private async Task LoadSprites()
        {
            var wallTask = Sprite.FromFile("wall.sprite")
                .ContinueWith(t => _wallSprite = t.Result);
            var lampTask = Sprite.FromFile("lamp.sprite")
                .ContinueWith(t => _lampSprite = t.Result);

            await Task.WhenAll(wallTask, lampTask);
        }

        protected override bool OnUserUpdate(float elapsedTime)
        {
            var cornerDetection = new List<(float distance, float dot)>(4);

            _player.HandleInput(Keys, elapsedTime, _map);
            UpdateShowMap();

            for (var x = 0; x < ScreenWidth; x++)
            {
                // cast a ray
                var rayAngle = GetRayAngle(x);

                var distanceToWall = 0f;
                const float depthIncrease = .01f;
                var hitWall = false;

                var eyeX = (float)Math.Sin(rayAngle);
                var eyeY = (float)Math.Cos(rayAngle);
                var sampleX = 0f;

                while (!hitWall && distanceToWall < _map.Depth)
                {
                    distanceToWall += depthIncrease;

                    var testX = (int)(_player.X + eyeX * distanceToWall);
                    var testY = (int)(_player.Y + eyeY * distanceToWall);

                    // test if ray is out of bounds
                    if (_map.OutOfBounds(testX, testY))
                    {
                        hitWall = true;
                        distanceToWall = _map.Depth;
                    }
                    else if (_map.HitsWall(testX, testY))
                    {
                        hitWall = true;

                        // Determine where the ray hits the wall.
                        var blockMidX = testX + 0.5f;
                        var blockMidY = testY + 0.5f;

                        var testPointX = _player.X + eyeX * distanceToWall;
                        var testPointY = _player.Y + eyeY * distanceToWall;

                        var testAngle = (float)Math.Atan2(testPointY - blockMidY, testPointX - blockMidX);
                        if (testAngle >= -PI_OVER_4 && testAngle < PI_OVER_4)
                        {
                            sampleX = testPointY - testY;
                        }
                        if (testAngle >= PI_OVER_4 && testAngle < THREE_PI_OVER_4)
                        {
                            sampleX = testPointX - testX;
                        }
                        if (testAngle < -PI_OVER_4 && testAngle >= -THREE_PI_OVER_4)
                        {
                            sampleX = testPointX - testX;
                        }
                        if (testAngle >= THREE_PI_OVER_4 || testAngle < -THREE_PI_OVER_4)
                        {
                            sampleX = testPointY - testY;
                        }
                    }
                }

                // Calculate distance to ceiling and floor
                var ceiling = (int)(ScreenHeight / 2.0f - ScreenHeight / distanceToWall);
                var floor = ScreenHeight - ceiling;

                for (var y = ScreenHeight - 1; y >= 0; y--)
                {
                    if (y <= ceiling)
                    {
                        // ceiling
                        Draw(x, y, ' ');
                    }
                    else if (y > ceiling && y <= floor)
                    {
                        // wall
                        if (distanceToWall < _map.Depth)
                        {
                            var sampleY = ((float) y - ceiling) / (floor - ceiling);
                            Draw(x, y,
                                _wallSprite.SampleGlyph(sampleX, sampleY),
                                _wallSprite.SampleColor(sampleX, sampleY));
                        }
                        else
                        {
                            Draw(x, y, (int) PixelType.PIXEL_SOLID, Color.BG_BLACK);
                        }
                    }
                    else
                    {
                        // floor
                        Draw(x, y, (int) PixelType.PIXEL_SOLID, Color.FG_DARK_GREEN);
                    }
                }
            }

            // Draw objects
            foreach (var obj in _objects)
            {
                // can the player see the object?
                var vecX = obj.X - _player.X;
                var vecY = obj.Y - _player.Y;
                var distanceFromPlayer = (float)Math.Sqrt(vecX * vecX + vecY * vecY);

                // calculate angle between object and player
                // to see if the object is in the FOV
                var eyeX = (float)Math.Sin(_player.Angle);
                var eyeY = (float) Math.Cos(_player.Angle);
                var objAngle = (float)Math.Atan2(eyeY, eyeX) - (float)Math.Atan2(vecY, vecX);
                if (objAngle < -PI)
                {
                    objAngle += 2 * PI;
                }
                if (objAngle > PI)
                {
                    objAngle -= 2 * PI;
                }

                var inPlayerFov = Math.Abs(objAngle) < Player.Fov / 2;
                if (inPlayerFov && distanceFromPlayer >= .5f && distanceFromPlayer < _map.Depth)
                {
                    var objCeiling = (ScreenHeight / 2f) - (ScreenHeight / distanceFromPlayer);
                    var objFloor = ScreenHeight - objCeiling;
                    var objHeight = objFloor - objCeiling;
                    var objAspectRatio = 1f * obj.Sprite.Height / obj.Sprite.Width;
                    var objWidth = objHeight / objAspectRatio;

                    var middleOfObject  = (0.5f * (objAngle / (Player.Fov / 2f)) + 0.5f) * ScreenWidth;

                    for (float lx = 0; lx < objWidth; lx++)
                    {
                        for (float ly = 0; ly < objHeight; ly++)
                        {
                            var sampleX = lx / objWidth;
                            var sampleY = ly / objHeight;
                            var c = obj.Sprite.SampleGlyph(sampleX, sampleY);
                            var objColumn = (int)(middleOfObject + lx - (objWidth / 2f));
                            if (objColumn >= 0 && objColumn < ScreenWidth)
                            {
                                if (c != ' ')
                                {
                                    var col = obj.Sprite.SampleColor(sampleX, sampleY);
                                    Draw(objColumn, (int) (objCeiling + ly), c, col);
                                }
                            }
                        }
                    }
                }
            }

            // WriteStats(_player);
            DrawMap(_map, _player);

            return true;
        }

        private void UpdateShowMap()
        {
            if (Keys[VirtualKeys.VK_TAB].Pressed)
            {
                _showMap = !_showMap;
            }
        }

        private void WriteStats(Player player)
        {
            var stats = $"X={player.X:F}, Y={player.Y:F}, A={player.Angle:F}";
            DrawString(0, 0, stats);
        }

        private void DrawMap(Map map, Player player)
        {
            if (!_showMap)
            {
                return;
            }

            for (var x = 0; x < map.Width; x++)
            {
                for (var y = 0; y < map.Height; y++)
                {
                    Draw(x + 1, y + 1, map.Layout[y * map.Width + x]);
                }
            }

            Draw((int)player.X + 1, (int)player.Y + 1, 'P');
        }

        public float GetRayAngle(in int x)
        {
            return (_player.Angle - Player.Fov / 2) + ((float)x / ScreenWidth) * Player.Fov;
        }
    }
}