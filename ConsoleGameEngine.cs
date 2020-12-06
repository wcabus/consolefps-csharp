using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ConFPS
{
    public abstract class ConsoleGameEngine
    {
        private int _screenWidth;
        private int _screenHeight;
        private CHAR_INFO[] _screenBuffer;
        private COORD _bufferSize;
        private readonly COORD _bufferCoord = new COORD();
        
        private readonly IntPtr _console;
        private readonly IntPtr _originalConsole;
        private SMALL_RECT _rectWindow;

        private static volatile bool _atomActive;

        protected string AppName;
        protected KeyState[] Keys = new KeyState[256];
        private short[] _keyNewState = new short[256];
        private short[] _keyOldState = new short[256];

        public const float PI = (float)Math.PI;
        public const float THREE_PI_OVER_4 = 3 * PI_OVER_4;
        public const float PI_OVER_4 = PI / 4;
        public const float PI_OVER_2 = PI / 2;

        protected ConsoleGameEngine()
        {
            _originalConsole = IntPtr.Zero;
            _screenWidth = 80;
            _screenHeight = 30;

            AppName = "Default";

            _console = ConInterop.GetStdHandle(ConInterop.STD_OUTPUT_HANDLE);
            if (_console.ToInt32() == -1)
            {
                var hResult = Marshal.GetLastWin32Error();
                Console.Error.WriteLine($"Failed to get console handle: {hResult}");
            }

            Console.CursorVisible = false;
        }

        ~ConsoleGameEngine()
        {
            ConInterop.SetConsoleActiveScreenBuffer(_originalConsole);
        }

        public int ConstructConsole(int width, int height, int fontw, int fonth)
        {
            _screenWidth = width;
            _screenHeight = height;

            // Change console visual size to a minimum so ScreenBuffer can shrink
            // below the actual visual size
            _rectWindow = new SMALL_RECT
            {
                Right = 1,
                Bottom = 1
            };
            ConInterop.SetConsoleWindowInfo(_console, true, ref _rectWindow);

            // Set the size of the screen buffer
            _bufferSize = new COORD
            {
                X = (short)_screenWidth, 
                Y = (short)_screenHeight
            };
            if (!ConInterop.SetConsoleScreenBufferSize(_console, _bufferSize))
            {
                return Error("SetConsoleScreenBufferSize");
            }

            // Assign screen buffer to the console
            if (!ConInterop.SetConsoleActiveScreenBuffer(_console))
            {
                return Error("SetConsoleActiveScreenBuffer");
            }

            var cfi = new CONSOLE_FONT_INFOEX(fontw, fonth, "Consolas");
            cfi.cbSize = Marshal.SizeOf(cfi);
            if (!ConInterop.SetCurrentConsoleFontEx(_console, false, ref cfi))
            {
                return Error("SetCurrentConsoleFontEx");
            }

            // Get screen buffer info and check the maximum allowed window size. Return
            // error if exceeded, so user knows their dimensions/fontsize are too large
            CONSOLE_SCREEN_BUFFER_INFO csbi;
            if (!ConInterop.GetConsoleScreenBufferInfo(_console, out csbi))
            {
                return Error("GetConsoleScreenBufferInfo");
            }
            if (_screenHeight > csbi.dwMaximumWindowSize.Y)
            {
                return Error("Screen Height / Font Height Too Big");
            }
            if (_screenWidth > csbi.dwMaximumWindowSize.X)
            {
                return Error("Screen Width / Font Width Too Big");
            }

            // Set Physical Console Window Size
            _rectWindow = new SMALL_RECT
            {
                Left = 0,
                Top = 0,
                Right = (short) (_screenWidth - 1),
                Bottom = (short) (_screenHeight - 1)
            };
            
            if (!ConInterop.SetConsoleWindowInfo(_console, true, ref _rectWindow))
            {
                return Error("SetConsoleWindowInfo");
            };

            _screenBuffer = new CHAR_INFO[_screenWidth * _screenHeight];
            return 1;
        }

        public Task Start()
        {
            var t = Task.Run(GameThread);
            t.Wait();

            return t;
        }

        public int ScreenWidth => _screenWidth;
        public int ScreenHeight => _screenHeight;

        private async Task GameThread()
        {
            _atomActive = true;
            if (!await OnUserCreate())
            {
                _atomActive = false;
            }

            var sw = Stopwatch.StartNew();
            while (_atomActive)
            {
                // Run as fast as possible
                while (_atomActive)
                {
                    var elapsedTime = (float) sw.Elapsed.TotalSeconds;
                    sw.Restart();
                    HandleKeyboardInput();

                    // Handle Frame Update
                    if (!OnUserUpdate(elapsedTime))
                    {
                        _atomActive = false;
                    }

                    // Update Title & Present Screen Buffer
                    var s = $"C# - Console Game Engine - {AppName} - FPS: {1.0f / elapsedTime:F}";
                    Console.Title = s;
                    ConInterop.WriteConsoleOutput(_console, _screenBuffer, _bufferSize, _bufferCoord, ref _rectWindow);
                }

                if (!await OnUserDestroy())
                {
                    _atomActive = true;
                }
            }
        }

        protected abstract Task<bool> OnUserCreate();
        protected abstract bool OnUserUpdate(float elapsedTime);

        protected virtual Task<bool> OnUserDestroy()
        {
            return Task.FromResult(true);
        }

        private void HandleKeyboardInput()
        {
            for (var i = 0; i < 256; i++)
            {
                _keyNewState[i] = ConInterop.GetAsyncKeyState(i);

                Keys[i].Pressed = false;
                Keys[i].Released = false;

                if (_keyNewState[i] != _keyOldState[i])
                {
                    if ((_keyNewState[i] & 0x8000) == 0x8000)
                    {
                        Keys[i].Pressed = !Keys[i].Held;
                        Keys[i].Held = true;
                    }
                    else
                    {
                        Keys[i].Released = true;
                        Keys[i].Held = false;
                    }
                }

                _keyOldState[i] = _keyNewState[i];
            }
        }

        public virtual void Draw(int x, int y, int c = 0x2588c, Color col = Color.FG_WHITE)
        {
            if (x >= 0 && x < _screenWidth && y >= 0 && y < _screenHeight)
            {
                _screenBuffer[y * _screenWidth + x].Char = (char)c;
                _screenBuffer[y * _screenWidth + x].Attributes = (ushort)col;
            }
        }

        public void Fill(int x1, int y1, int x2, int y2, int c = 0x2588, Color col = Color.FG_WHITE)
        {
            Clip(ref x1, ref y1);
            Clip(ref x2, ref y2);
            for (var x = x1; x < x2; x++)
            {
                for (var y = y1; y < y2; y++)
                {
                    Draw(x, y, c, col);
                }
            }
        }

        public void DrawString(int x, int y, string s, Color col = Color.FG_WHITE)
        {
            int x1 = 0, y1 = 0;
            foreach (var c in s)
            {
                Draw(x + x1, y + y1, c, col);
                x1++;
                if (x1 > ScreenWidth)
                {
                    x1 = 0;
                    y1++;
                }
            }
        }

        public void DrawStringAlpha(int x, int y, string s, Color col = Color.FG_WHITE)
        {
            int x1 = 0, y1 = 0;
            foreach (var c in s)
            {
                if (c != ' ')
                {
                    Draw(x+x1, y+y1, c, col);
                }

                x1++;
                if (x1 > ScreenWidth)
                {
                    x1 = 0;
                    y1++;
                }
            }
        }

        public void Clip(ref int x, ref int y)
        {
            if (x < 0) x = 0;
            if (x >= _screenWidth) x = _screenWidth;
            if (y < 0) y = 0;
            if (y >= _screenHeight) y = _screenHeight;
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int c = 0x2588, Color col = Color.FG_WHITE)
        {
            int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
            dx = x2 - x1;
            dy = y2 - y1;
            dx1 = Math.Abs(dx);
            dy1 = Math.Abs(dy);
            px = 2 * dy1 - dx1;
            py = 2 * dx1 - dy1;
            if (dy1 <= dx1)
            {
                if (dx >= 0)
                {
                    x = x1;
                    y = y1;
                    xe = x2;
                }
                else
                {
                    x = x2;
                    y = y2;
                    xe = x1;
                }

                Draw(x, y, c, col);

                for (i = 0; x < xe; i++)
                {
                    x++;
                    if (px < 0)
                    {
                        px += 2 * dy1;
                    }
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            y++;
                        }
                        else
                        {
                            y--;
                        }
                        px += 2 * (dy1 - dx1);
                    }
                    Draw(x, y, c, col);
                }
            }
            else
            {
                if (dy >= 0)
                {
                    x = x1;
                    y = y1;
                    ye = y2;
                }
                else
                {
                    x = x2;
                    y = y2;
                    ye = y1;
                }

                Draw(x, y, c, col);

                for (i = 0; y < ye; i++)
                {
                    y++;
                    if (py <= 0)
                    {
                        py += 2 * dx1;
                    }
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            x++;
                        }
                        else
                        {
                            x--;
                        }
                        py += 2 * (dx1 - dy1);
                    }
                    Draw(x, y, c, col);
                }
            }
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, int c = 0x2588, Color col = Color.FG_WHITE)
        {
            DrawLine(x1, y1, x2, y2, c, col);
            DrawLine(x2, y2, x3, y3, c, col);
            DrawLine(x3, y3, x1, y1, c, col);
        }

        protected int Error(string msg)
        {
            var hresult = Marshal.GetLastWin32Error();
            ConInterop.SetConsoleActiveScreenBuffer(_originalConsole);
            Console.Error.WriteLine(msg);
            Console.Error.WriteLine($"HRESULT = {hresult}");
            return 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CHAR_INFO
        {
            public char Char;
            public ushort Attributes;
        }

        private enum ConAttributes : ushort
        {
            FOREGROUND_BLUE = 0x0001,
            FOREGROUND_GREEN = 0x0002,
            FOREGROUND_RED = 0x0004,
            FOREGROUND_INTENSITY = 0x0008,
            BACKGROUND_BLUE = 0x0010,
            BACKGROUND_GREEN = 0x0020,
            BACKGROUND_RED = 0x0040,
            BACKGROUND_INTENSITY = 0x0080,
            COMMON_LVB_LEADING_BYTE = 0x0100,
            COMMON_LVB_TRAILING_BYTE = 0x0200,
            COMMON_LVB_GRID_HORIZONTAL = 0x0400,
            COMMON_LVB_GRID_LVERTICAL = 0x0800,
            COMMON_LVB_GRID_RVERTICAL = 0x1000,
            COMMON_LVB_REVERSE_VIDEO = 0x4000,
            COMMON_LVB_UNDERSCORE = 0x8000,
        }

        private struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public COORD(short x, short y)
            {
                X = x;
                Y = y;
            }

            public short X;
            public short Y;
        }

        private const int FF_DONTCARE = 0;
        private const int FW_NORMAL = 400;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CONSOLE_FONT_INFOEX
        {
            public int cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;

            public CONSOLE_FONT_INFOEX(int fontw, int fonth, string faceName)
            {
                cbSize = sizeof(ulong) + 3*sizeof(int) + 2*sizeof(short) + LF_FACESIZE*sizeof(char);
                nFont = 0;
                dwFontSize = new COORD((short)fontw, (short)fonth);
                FontFamily = FF_DONTCARE;
                FontWeight = FW_NORMAL;
                FaceName = faceName;
            }

            const int LF_FACESIZE = 32;
        }

        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }

        public static class VirtualKeys
        {
            public const int VK_BACK = 0x08;
            public const int VK_TAB = 0x09;
            public const int VK_RETURN = 0x0D;
            public const int VK_SHIFT = 0x10;
            public const int VK_CONTROL = 0x11;
            public const int VK_MENU = 0x12; // ALT
            public const int VK_PAUSE = 0x13;
            public const int VK_CAPITAL = 0x14; // CAPS
            public const int VK_ESCAPE = 0x1B;
            public const int VK_SPACE = 0x20;
            public const int VK_PRIOR = 0x21; // page up
            public const int VK_NEXT = 0x22; // page down
            public const int VK_END = 0x23;
            public const int VK_HOME = 0x24;
            public const int VK_LEFT = 0x25;
            public const int VK_UP = 0x26;
            public const int VK_RIGHT = 0x27;
            public const int VK_DOWN = 0x28;
            public const int VK_SELECT = 0x29;
            public const int VK_PRINT = 0x2A;
            public const int VK_EXECUTE = 0x2B;
            public const int VK_SNAPSHOT = 0x2C;
            public const int VK_INSERT = 0x2D;
            public const int VK_DELETE = 0x2E;
            public const int VK_HELP = 0x2F;
            public const int VK_0 = 0x30;
            public const int VK_1 = 0x31;
            public const int VK_2 = 0x32;
            public const int VK_3 = 0x33;
            public const int VK_4 = 0x34;
            public const int VK_5 = 0x35;
            public const int VK_6 = 0x36;
            public const int VK_7 = 0x37;
            public const int VK_8 = 0x38;
            public const int VK_9 = 0x39;
            public const int VK_A = 0x41;
            public const int VK_B = 0x42;
            public const int VK_C = 0x43;
            public const int VK_D = 0x44;
            public const int VK_E = 0x45;
            public const int VK_F = 0x46;
            public const int VK_G = 0x47;
            public const int VK_H = 0x48;
            public const int VK_I = 0x49;
            public const int VK_J = 0x4A;
            public const int VK_K = 0x4B;
            public const int VK_L = 0x4C;
            public const int VK_M = 0x4D;
            public const int VK_N = 0x4E;
            public const int VK_O = 0x4F;
            public const int VK_P = 0x50;
            public const int VK_Q = 0x51;
            public const int VK_R = 0x52;
            public const int VK_S = 0x53;
            public const int VK_T = 0x54;
            public const int VK_U = 0x55;
            public const int VK_V = 0x56;
            public const int VK_W = 0x57;
            public const int VK_X = 0x58;
            public const int VK_Y = 0x59;
            public const int VK_Z = 0x5A;
            public const int VK_LWIN = 0x5B;
            public const int VK_RWIN = 0x5C;
            public const int VK_APPS = 0x5D;
            public const int VK_SLEEP = 0x5F;
            public const int VK_NUMPAD0 = 0x60;
            public const int VK_NUMPAD1 = 0x61;
            public const int VK_NUMPAD2 = 0x62;
            public const int VK_NUMPAD3 = 0x63;
            public const int VK_NUMPAD4 = 0x64;
            public const int VK_NUMPAD5 = 0x65;
            public const int VK_NUMPAD6 = 0x66;
            public const int VK_NUMPAD7 = 0x67;
            public const int VK_NUMPAD8 = 0x68;
            public const int VK_NUMPAD9 = 0x69;
            public const int VK_MULTIPLY = 0x6A;
            public const int VK_ADD = 0x6B;
            public const int VK_SEPARATOR = 0x6C;
            public const int VK_SUBTRACT = 0x6D;
            public const int VK_DECIMAL = 0x6E;
            public const int VK_DIVIDE = 0x6F;
            public const int VK_F1 = 0x70;
            public const int VK_F2 = 0x71;
            public const int VK_F3 = 0x72;
            public const int VK_F4 = 0x73;
            public const int VK_F5 = 0x74;
            public const int VK_F6 = 0x75;
            public const int VK_F7 = 0x76;
            public const int VK_F8 = 0x77;
            public const int VK_F9 = 0x78;
            public const int VK_F10 = 0x79;
            public const int VK_F11 = 0x7A;
            public const int VK_F12 = 0x7B;
            public const int VK_F13 = 0x7C;
            public const int VK_F14 = 0x7D;
            public const int VK_F15 = 0x7E;
            public const int VK_F16 = 0x7F;
            public const int VK_F17 = 0x80;
            public const int VK_F18 = 0x81;
            public const int VK_F19 = 0x82;
            public const int VK_F20 = 0x83;
            public const int VK_F21 = 0x84;
            public const int VK_F22 = 0x85;
            public const int VK_F23 = 0x86;
            public const int VK_F24 = 0x87;
        }

        private class ConInterop
        {
            public const int STD_INPUT_HANDLE = -10;
            public const int STD_OUTPUT_HANDLE = -11;

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetStdHandle(int handle);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetConsoleWindowInfo(IntPtr console, bool absolute, [In] ref SMALL_RECT consoleWindow);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetConsoleScreenBufferSize(IntPtr console, COORD size);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetConsoleActiveScreenBuffer(IntPtr console);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetCurrentConsoleFontEx(IntPtr console, bool maximumWindow, ref CONSOLE_FONT_INFOEX consoleCurrentFontEx);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool GetConsoleScreenBufferInfo(IntPtr console, out CONSOLE_SCREEN_BUFFER_INFO consoleScreenBufferInfo);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool WriteConsoleOutput(IntPtr console, CHAR_INFO[] buffer, COORD bufferSize, COORD bufferCoord, ref SMALL_RECT writeRegion);

            [DllImport("user32.dll")]
            public static extern short GetAsyncKeyState(int vKeys);
        }
    }
}