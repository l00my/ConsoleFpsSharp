using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ConsoleFpsSharp
{
    class Program
    {
        const int STD_OUTPUT_HANDLE = -11;
        const int ScreenWidth = 120;
        const int ScreenHeight = 40;

        static float playerX = 8f;
        static float playerY = 8f;
        static float playerAngle = 0f;

        const int MapHeight = 16;
        const int MapWidth = 16;

        static float fov = 3.14159f / 4f;
        static float depth = 16f;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.SetBufferSize(ScreenWidth, ScreenHeight);
            IntPtr ptrStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            byte[] textBuffer = new byte[ScreenWidth * ScreenHeight];
            uint bytesWritten = 0;

            string map = "";

            map += "################";
            map += "#..............#";
            map += "#..............#";
            map += "#..............#";
            map += "########.......#";
            map += "#..............#";
            map += "#.........#....#";
            map += "#..............#";
            map += "#.........#....#";
            map += "#..............#";
            map += "#..............#";
            map += "#..............#";
            map += "#.......########";
            map += "#..............#";
            map += "#..............#";
            map += "################";


            var tp1 = DateTime.Now;
            var tp2 = DateTime.Now;

            while (true)
            {
                tp2 = DateTime.Now;
                var elapsed = tp2 - tp1;
                var deltaTime = elapsed.Milliseconds / 1000f;

                // Controls
                // Handle CCW Rotation
                // A
                if ((GetAsyncKeyState((short)'A') & 1) == 1)
                {
                    playerAngle -= (0.1f);
                    //playerAngle -= (0.03f);
                }

                if ((GetAsyncKeyState((short)'D') & 1) == 1)
                {
                    playerAngle += (0.01f);
                    //playerAngle += (0.03f);
                }

                if ((GetAsyncKeyState((short)'W') & 1) == 1)
                {
                    playerX += (float)Math.Sin(playerAngle) * 0.1f * deltaTime;
                    playerY += (float)Math.Cos(playerAngle) * 0.1f * deltaTime;
                }

                if ((GetAsyncKeyState((short)'S') & 1) == 1)
                {
                    playerX -= (float)Math.Sin(playerAngle) * 0.1f * deltaTime;
                    playerY -= (float)Math.Cos(playerAngle) * 0.1f * deltaTime;
                }

                for (int x = 0; x < ScreenWidth; x++)
                {
                    // For each column, calculate the projected ray angle into world space
                    float rayAngle = (playerAngle - fov / 2f) + (x / ScreenWidth) * fov;

                    float distanceToWall = 0f;
                    bool hitWall = false;

                    float eyeX = (float)Math.Sin(rayAngle); // Unit vector for ray in player space
                    float eyeY = (float)Math.Cos(rayAngle);

                    while (!hitWall && distanceToWall < depth)
                    {
                        distanceToWall += 0.1f;

                        int testX = (int)(playerX + eyeX * distanceToWall);
                        int testY = (int)(playerY + eyeY * distanceToWall);

                        // Test if ray is out of bounds
                        if (testX < 0 || testX >= MapWidth || testY < 0 || testY >= MapHeight)
                        {
                            hitWall = true; // Just set distance to maximum depth
                            distanceToWall = depth;
                        }
                        else
                        {
                            // Ray is inbounds so test to see if the ray cell is a wall block
                            if (map[testY * MapWidth + testX] == '#')
                            {
                                hitWall = true;
                            }
                        }
                    }

                    // Calculate distance to ceiling and floor
                    int ceiling = (int)((ScreenHeight / 2f) - ScreenHeight / distanceToWall);
                    int floor = ScreenHeight - ceiling;

                    char shade;
                    if (distanceToWall <= depth / 4f) { shade = '#'; } // Very close
                    else if (distanceToWall <= depth / 3f) { shade = '*'; }
                    else if (distanceToWall <= depth / 2f) { shade = '~'; }
                    else if (distanceToWall <= depth) { shade = '-'; }
                    else { shade = ' '; } // Too far away

                    for (int y = 0; y < ScreenHeight; y++)
                    {
                        if (y <= ceiling)
                        {
                            textBuffer[y * ScreenWidth + x] = (byte)' ';
                        }
                        else if (y > ceiling && y <= floor)
                        {
                            textBuffer[y * ScreenWidth + x] = (byte)shade;
                        }
                        else
                        {
                            // Shade floor based on distance
                            float distance = 1f - ((y - ScreenHeight / 2f) / (ScreenHeight / 2f));
                            if (distance < 0.25f) { shade = 'X'; }
                            else if (distance < 0.5f) { shade = '+'; }
                            else if (distance < 0.75f) { shade = ':'; }
                            else if (distance < 0.9f) { shade = '.'; }
                            else { shade = ' '; }
                            textBuffer[y * ScreenWidth + x] = (byte)shade;
                        }
                    }
                }

                textBuffer[ScreenWidth * ScreenHeight - 1] = (byte)'\0';
                WriteConsoleOutputCharacter(ptrStdOut, textBuffer, ScreenWidth * ScreenHeight, new Coord(0, 0), out bytesWritten);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool WriteConsoleOutputCharacter(
            IntPtr hConsoleOutput,
            byte[] lpCharacter, uint nLength, Coord dwWriteCoord,
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

        [DllImport("user32.dll", SetLastError = true)]

        public static extern short GetAsyncKeyState(int vKey);
    }
}
