using System;
using System.Collections.Generic;
using System.Text;

namespace DoomFileManager
{
    public struct DoubleLinesToPrint
    {
        public static string TopLeft { get { return "╔"; } }
        public static string TopRight { get { return "╗"; } }
        public static string BottomLeft { get { return "╚"; } }
        public static string BottomRight { get { return "╝"; } }
        public static string LineX { get { return "═"; } }
        public static string LineY { get { return "║"; } }
    }
    static class ConsoleDrawings
    {
        private const ConsoleColor frameForegroundColor = ConsoleColor.Green;
        private const ConsoleColor frameBackgroundColor = ConsoleColor.Black;
        private const ConsoleColor textColor = ConsoleColor.White;
        private const ConsoleColor textBackgroundColor = ConsoleColor.Black;


        public static void PrintFrameLines(int positionX, int positionY, int sizeX, int sizeY)//рисуем линиями очертания панели по заданным размерам и кординатам
        {
            int SizeX = positionX + sizeX;
            int SizeY = positionY + sizeY;
            Console.ForegroundColor = frameForegroundColor;
            Console.BackgroundColor = frameBackgroundColor;
            for (int y = positionY; y < SizeY; y++)
            {
                for (int x = positionX; x < SizeX; x++)
                {
                    if (y == positionY && x == positionX)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(DoubleLinesToPrint.TopLeft);
                    }

                    if (y == positionY && x > positionX && x < SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(DoubleLinesToPrint.LineX);
                    }

                    if (y == positionY && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(DoubleLinesToPrint.TopRight);
                    }

                    if (y > positionY && y < SizeY - 1 && x == positionX || y > positionY && y < SizeY - 1 && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(DoubleLinesToPrint.LineY);
                    }

                    if (y == SizeY - 1 && x == positionX)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(DoubleLinesToPrint.BottomLeft);
                    }

                    if (y == SizeY - 1 && x > positionX && x < SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(DoubleLinesToPrint.LineX);
                    }

                    if (y == SizeY - 1 && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(DoubleLinesToPrint.BottomRight);
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void PrintString(string str, int X, int Y)
        {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = textBackgroundColor;

            Console.SetCursorPosition(X, Y);
            Console.Write(str);

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
