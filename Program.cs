using System.Threading.Tasks;

namespace ConFPS
{
    // https://www.youtube.com/watch?v=HEb2akswCcw
    public class Program
    {
        public static async Task<int> Main()
        {
            // await CreateWallSprite();
            // await CreateLampSprite();
            
            var game = new Game();
            return await game.Run();
        }

        private static async Task CreateWallSprite()
        {
            var wall = new Sprite(32, 32);

            var colors = new[]
            {
                4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,4,4,4,4,
                4,4,4,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,
                8,8,8,8,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4,4,4,4,8,
                4,4,4,4
            };

            for (var x = 0; x < 32; x++)
            {
                for (var y = 0; y < 32; y++)
                {
                    wall.SetColor(x,y,colors[y*32+x]);
                    wall.SetGlyph(x, y, 0x2588);
                }
            }

            await wall.Save("wall.sprite");
        }

        private static async Task CreateLampSprite()
        {
            var lamp = new Sprite(8, 32);

            var y = 0;
            lamp.SetColor(3, y, Color.FG_GREY);
            lamp.SetColor(4, y, Color.FG_GREY);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(1, y, Color.FG_GREY);
            lamp.SetColor(2, y, Color.FG_GREY);
            lamp.SetColor(3, y, Color.FG_WHITE);
            lamp.SetColor(4, y, Color.FG_WHITE);
            lamp.SetColor(5, y, Color.FG_GREY);
            lamp.SetColor(6, y, Color.FG_GREY);
            lamp.SetGlyph(1, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(2, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(5, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(6, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(0, y, Color.FG_GREY);
            lamp.SetColor(1, y, Color.FG_WHITE);
            lamp.SetColor(2, y, Color.FG_WHITE);
            lamp.SetColor(3, y, Color.FG_WHITE);
            lamp.SetColor(4, y, Color.FG_WHITE);
            lamp.SetColor(5, y, Color.FG_WHITE);
            lamp.SetColor(6, y, Color.FG_WHITE);
            lamp.SetColor(7, y, Color.FG_GREY);
            lamp.SetGlyph(0, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(1, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(2, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(5, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(6, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(7, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(0, y, Color.FG_GREY);
            lamp.SetColor(1, y, Color.FG_WHITE);
            lamp.SetColor(2, y, Color.FG_WHITE);
            lamp.SetColor(3, y, Color.FG_WHITE);
            lamp.SetColor(4, y, Color.FG_WHITE);
            lamp.SetColor(5, y, Color.FG_WHITE);
            lamp.SetColor(6, y, Color.FG_WHITE);
            lamp.SetColor(7, y, Color.FG_GREY);
            lamp.SetGlyph(0, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(1, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(2, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(5, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(6, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(7, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(0, y, Color.FG_GREY);
            lamp.SetColor(1, y, Color.FG_WHITE);
            lamp.SetColor(2, y, Color.FG_WHITE);
            lamp.SetColor(3, y, Color.FG_YELLOW);
            lamp.SetColor(4, y, Color.FG_YELLOW);
            lamp.SetColor(5, y, Color.FG_WHITE);
            lamp.SetColor(6, y, Color.FG_WHITE);
            lamp.SetColor(7, y, Color.FG_GREY);
            lamp.SetGlyph(0, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(1, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(2, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(5, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(6, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(7, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(1, y, Color.FG_GREY);
            lamp.SetColor(2, y, Color.FG_WHITE);
            lamp.SetColor(3, y, Color.FG_YELLOW);
            lamp.SetColor(4, y, Color.FG_YELLOW);
            lamp.SetColor(5, y, Color.FG_WHITE);
            lamp.SetColor(6, y, Color.FG_GREY);
            lamp.SetGlyph(1, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(2, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(5, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(6, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(2, y, Color.FG_GREY);
            lamp.SetColor(3, y, Color.FG_YELLOW);
            lamp.SetColor(4, y, Color.FG_YELLOW);
            lamp.SetColor(5, y, Color.FG_GREY);
            lamp.SetGlyph(2, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(5, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(2, y, Color.FG_GREY);
            lamp.SetColor(3, y, Color.FG_YELLOW);
            lamp.SetColor(4, y, Color.FG_YELLOW);
            lamp.SetColor(5, y, Color.FG_GREY);
            lamp.SetGlyph(2, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(5, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(3, y, Color.FG_GREY);
            lamp.SetColor(4, y, Color.FG_GREY);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(3, y, Color.FG_GREY);
            lamp.SetColor(4, y, Color.FG_GREY);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(3, y, Color.FG_GREY);
            lamp.SetColor(4, y, Color.FG_GREY);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(0, y, Color.FG_GREY);
            lamp.SetColor(1, y, Color.FG_GREY);
            lamp.SetColor(2, y, Color.FG_GREY);
            lamp.SetColor(3, y, Color.FG_GREY);
            lamp.SetColor(4, y, Color.FG_GREY);
            lamp.SetColor(5, y, Color.FG_GREY);
            lamp.SetColor(6, y, Color.FG_GREY);
            lamp.SetColor(7, y, Color.FG_GREY);
            lamp.SetGlyph(0, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(1, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(2, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(5, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(6, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(7, y, PixelType.PIXEL_SOLID);
            y++;

            for (var i = 0; i < 18; i++)
            {
                lamp.SetColor(3, y, Color.FG_GREY);
                lamp.SetColor(4, y, Color.FG_GREY);
                lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
                lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
                y++;
            }

            lamp.SetColor(0, y, Color.FG_GREY);
            lamp.SetColor(1, y, Color.FG_GREY);
            lamp.SetColor(2, y, Color.FG_GREY);
            lamp.SetColor(3, y, Color.FG_GREY);
            lamp.SetColor(4, y, Color.FG_GREY);
            lamp.SetColor(5, y, Color.FG_GREY);
            lamp.SetColor(6, y, Color.FG_GREY);
            lamp.SetColor(7, y, Color.FG_GREY);
            lamp.SetGlyph(0, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(1, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(2, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(5, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(6, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(7, y, PixelType.PIXEL_SOLID);
            y++;
            lamp.SetColor(3, y, Color.FG_GREY);
            lamp.SetColor(4, y, Color.FG_GREY);
            lamp.SetGlyph(3, y, PixelType.PIXEL_SOLID);
            lamp.SetGlyph(4, y, PixelType.PIXEL_SOLID);

            await lamp.Save("lamp.sprite");
        }
    }
}
