using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConFPS
{
    public class Sprite
    {
        private int[] _glyphs;
        private int[] _colors;

        public Sprite(int w, int h)
        {
            Create(w, h);
        }

        public static async Task<Sprite> FromFile(string path)
        {
            var sprite = new Sprite(1,1);
            if (await sprite.Load(path))
            {
                return sprite;
            }

            return null;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public async Task<bool> Save(string path)
        {
            try {
                await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                await using var bw = new BinaryWriter(fs);

                bw.Write(Width);
                bw.Write(Height);
                foreach (var color in _colors)
                {
                    bw.Write(color);
                }
                foreach (var glyph in _glyphs)
                {
                    bw.Write(glyph);
                }

                bw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Load(string path)
        {
            try
            {
                await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var br = new BinaryReader(fs);

                var w = br.ReadInt32();
                var h = br.ReadInt32();

                Create(w, h);
                var size = w * h;
                int c;
                for (var i = 0; i < size; i++)
                {
                    c = br.ReadInt32();
                    _colors[i] = c;
                }

                for (var i = 0; i < size; i++)
                {
                    c = br.ReadInt32();
                    _glyphs[i] = c;
                }

                br.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetGlyph(int x, int y, int c)
        {
            SetItemInArray(x, y, c, _glyphs);
        }

        public void SetGlyph(int x, int y, PixelType c)
        {
            SetItemInArray(x, y, (int)c, _glyphs);
        }

        public int GetGlyph(int x, int y)
        {
            var glyph = GetItemFromArray(x, y, _glyphs);
            return glyph == default ? ' ' : glyph;
        }

        public int SampleGlyph(float x, float y)
        {
            var sx = (int)(x * Width);
            var sy = (int)(y * Height - 1.0f);
            return GetGlyph(sx, sy);
        }

        public void SetColor(int x, int y, int c)
        {
            SetItemInArray(x, y, c, _colors);
        }

        public void SetColor(int x, int y, Color c)
        {
            SetItemInArray(x, y, (int)c, _colors);
        }

        public Color GetColor(int x, int y)
        {
            var color = GetItemFromArray(x, y, _colors);
            return color == default ? Color.FG_BLACK : (Color)color;
        }

        public Color SampleColor(float x, float y)
        {
            var sx = (int)(x * Width);
            var sy = (int)(y * Height - 1.0f);
            return GetColor(sx, sy);
        }

        private void SetItemInArray(int x, int y, int c, IList<int> arr)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return;
            }

            arr[y * Width + x] = c;
        }

        private int GetItemFromArray(int x, int y, IList<int> arr)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return default;
            }

            return arr[y * Width + x];
        }

        private void Create(int w, int h)
        {
            Width = w;
            Height = h;

            var size = w * h;
            _glyphs = new int[size];
            _colors = new int[size];

            for (var i = 0; i < size; i++)
            {
                _glyphs[i] = ' ';
                _colors[i] = (int)Color.FG_BLACK;
            }
        }
    }
}