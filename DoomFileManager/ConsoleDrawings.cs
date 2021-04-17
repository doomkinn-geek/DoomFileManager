using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DoomFileManager
{   
    static class ConsoleDrawings
    {
        private const ConsoleColor frameForegroundColor = ConsoleColor.Green;
        private const ConsoleColor frameBackgroundColor = ConsoleColor.Black;
        private const ConsoleColor textColor = ConsoleColor.White;
        private const ConsoleColor textBackgroundColor = ConsoleColor.Black;
        private const ConsoleColor warningColor = ConsoleColor.Yellow;
        private const ConsoleColor errorColor = ConsoleColor.Red;


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
                        Console.Write("╔");
                    }

                    if (y == positionY && x > positionX && x < SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("═");
                    }

                    if (y == positionY && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("╗");
                    }

                    if (y > positionY && y < SizeY - 1 && x == positionX || y > positionY && y < SizeY - 1 && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("║");
                    }

                    if (y == SizeY - 1 && x == positionX)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("╚");
                    }

                    if (y == SizeY - 1 && x > positionX && x < SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("═");
                    }

                    if (y == SizeY - 1 && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("╝");
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

        public static void TakeNewCommand()
        {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = textBackgroundColor;
                       
            PrintMessage(Configuration.CommandPosition, "Command>");
        }

        public static void PrintWarning(string text)
        {
            Console.ForegroundColor = warningColor;
            Console.BackgroundColor = textBackgroundColor;

            PrintMessage(Configuration.MessagesPosition, text);
        }

        public static void PrintError(string text)
        {
            Console.ForegroundColor = errorColor;
            Console.BackgroundColor = textBackgroundColor;

            PrintMessage(Configuration.MessagesPosition, text);
        }

        private static void PrintMessage(int position, string text)
        {
            Console.SetCursorPosition(0, position);
            Console.Write(new String(' ', Configuration.ConsoleWidth));
            Console.SetCursorPosition(0, position);
            Console.Write(text);
        }

        public static void PrintCurrentFoldersTree(List<FileSystemInfo> folders)
        {

        }
        
        private static void PrintOneFSObject(FileSystemInfo element)
        {
            Console.Write("{0}", element.Name);
            Console.SetCursorPosition(currentCursorLeftPosition + this.width / 2, currentCursorTopPosition);
            if (element is DirectoryInfo)
            {
                Console.Write("{0}", ((DirectoryInfo)element).LastWriteTime);
            }
            else
            {
                Console.Write("{0}", ((FileInfo)element).Length);
            }
        }
    }
}
