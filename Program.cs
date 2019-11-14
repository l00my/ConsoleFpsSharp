using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleFpsSharp
{
    class Program
    {
        const int STD_OUTPUT_HANDLE = -11;
        const int ScreenWidth = 120;
        const int ScreenHeight = 40;

        float playerX = 0f;
        float playerY = 0f;
        float playerAngle = 0f;

        const int MapHeight = 16;
        const int MapWidth = 16;

        static void Main(string[] args)
        {
            Console.SetBufferSize(ScreenWidth, ScreenHeight);
            IntPtr ptrStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            StringBuilder textBuffer = new StringBuilder(ScreenWidth * ScreenHeight);
            uint bytesWritten = 0;

            string map;

            while (true)
            {


                textBuffer[ScreenWidth * ScreenHeight - 1] = '\0';
                WriteConsoleOutputCharacter(ptrStdOut, textBuffer.ToString(), ScreenWidth * ScreenHeight, new Coord(0, 0), out bytesWritten);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateConsoleScreenBuffer(
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwFlags,
            IntPtr lpScreenBufferData
        );

        [DllImport("kernel32.dll")]
        static extern bool WriteConsoleOutputCharacter(
            IntPtr hConsoleOutput,
            string lpCharacter, uint nLength, Coord dwWriteCoord,
            out uint lpNumberOfCharsWritten
        );

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };
    }
}
