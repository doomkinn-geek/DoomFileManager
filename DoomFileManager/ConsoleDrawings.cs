using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DoomFileManager
{
    public struct DoubleLinesToPrint //линии для изображения рамки панелей
    {
        public static string TopLeft { get { return "╔"; } }
        public static string TopRight { get { return "╗"; } }
        public static string BottomLeft { get { return "╚"; } }
        public static string BottomRight { get { return "╝"; } }
        public static string LineX { get { return "═"; } }
        public static string LineY { get { return "║"; } }
    }
    
    public struct TreeLines //линии для создания дерева
    {
        public static string DownContinue { get { return "├"; } }
        public static string DownLast { get { return "└"; } }
        public static string Right { get { return "─"; } }
        public static string DoubleRight { get { return "──"; } }
    }

    static class ConsoleDrawings
    {
        private const ConsoleColor frameForegroundColor = ConsoleColor.Green;
        private const ConsoleColor frameBackgroundColor = ConsoleColor.Black;
        private const ConsoleColor textColor = ConsoleColor.White;
        private const ConsoleColor textBackgroundColor = ConsoleColor.Black;
        private const ConsoleColor infoColor = ConsoleColor.Green;
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

        public static void TakeNewCommand()
        {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = textBackgroundColor;

            //ClearMessageLine();
            PrintMessage(Configuration.CommandPosition, "Command>");            
        }

        public static void ClearMessageLine()//очищаем поле с информационным сообщением:
        {            
            Console.SetCursorPosition(0, Configuration.CommandPosition + 1);
            Console.Write(new String(' ', Configuration.ConsoleWidth));
        }

        public static void PrintWarning(string text)//вывод предупредительного сообщения
        {
            Console.ForegroundColor = warningColor;
            Console.BackgroundColor = textBackgroundColor;

            PrintMessage(Configuration.MessagesPosition, text);
        }

        public static void PrintError(string text)//вывод сообщения об ошибке
        {
            Console.ForegroundColor = errorColor;
            Console.BackgroundColor = textBackgroundColor;

            PrintMessage(Configuration.MessagesPosition, text);
        }

        public static void PrintInformation(string text)//вывод информационного сообщения
        {
            Console.ForegroundColor = infoColor;
            Console.BackgroundColor = textBackgroundColor;

            PrintMessage(Configuration.MessagesPosition, text);
        }

        private static void PrintMessage(int position, string text)//непосредственно печать сообщения
        {
            Console.SetCursorPosition(0, position);
            Console.Write(new String(' ', Configuration.ConsoleWidth));
            Console.SetCursorPosition(0, position);
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void PrintFoldersTree(string rootFolder, List<FileSystemInfo> folders, int pageNum)//вовод дерева каталогов
        {
            //рисуем в заголовке текущую директорию + номер страницы
            if (folders.Count != 0)
            {
                string header = rootFolder;
                float pageNumCoefficent = (float)folders.Count / (float)Configuration.ElementsOnPage;
                header += $" - [{pageNum}/{Math.Ceiling(pageNumCoefficent)}]";
                PrintMainPanelHeader(header);
            }
            else // если папки отсутствуют в текущей директории вручную пишем количество страниц
            {
                PrintMainPanelHeader($"{rootFolder} - [1/1]");
            }
            
                      
            int posistionX, positionY;
            posistionX = 2;
            positionY = 1;
            ClearMainPanel();

            Console.SetCursorPosition(posistionX, positionY);
            Console.WriteLine(rootFolder);
            positionY++;
            if(pageNum != 1)//если выводим не 1ю страницу, то рисуем дополнительно "<..>", чтобы было понятно, то это не начало дерева
            {
                Console.SetCursorPosition(posistionX, positionY);
                Console.Write(TreeLines.DownContinue);
                Console.Write(TreeLines.Right);
                Console.Write("<..>");
                positionY++;
            }
            
            for (int i = (pageNum - 1) * Configuration.ElementsOnPage; i < folders.Count; i++)
            {                
                Console.SetCursorPosition(posistionX, positionY);
                positionY++;
                if (i + 1 == folders.Count)//если последний элемент дерева, то рисуем завершающую закорючку
                {
                    Console.Write(TreeLines.DownLast);
                }
                else//иначе продолжающаяся закорючка. В этом случае будет понятно, что существует дополнительная страница
                {
                    Console.Write(TreeLines.DownContinue);
                }
                Console.Write(TreeLines.DoubleRight);
                Console.Write($"{folders[i].Name}");               

                try
                {
                    string[] directories = Directory.GetDirectories(folders[i].FullName);
                    //если в конкретной папке есть подпааки, помечаем её '[+]', чтобы указать, что есть смысл её просматривать:
                    if (directories.Length != 0)
                    {                        
                        Console.WriteLine("[+]");
                        Console.Write("\n");
                    }                  
                }
                catch (Exception)
                {
                    Console.Write("\n");
                    continue;
                }                
                if (positionY > Configuration.ElementsOnPage)
                    break;
            }            
        }

        public static void PrintMainPanelHeader(string header)
        {
            Console.ForegroundColor = frameForegroundColor;
            Console.BackgroundColor = frameBackgroundColor;
            Console.SetCursorPosition(0, 0);
            Console.Write(DoubleLinesToPrint.TopLeft);
            Console.Write(new string(DoubleLinesToPrint.LineX[0],
                                     Configuration.ConsoleWidth - 2));
            Console.Write(DoubleLinesToPrint.TopRight);
            ConsoleDrawings.PrintString(header, Configuration.ConsoleWidth / 2 - header.Length / 2, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void ClearMainPanel()//очистка дерева каталогов
        {
            int posistionX = 2;
            int positionY = 1;            
            for(int i = 0; i < Configuration.MainPanelHeight - 2; i++)
            {
                Console.SetCursorPosition(posistionX, positionY + i);
                Console.Write(new string(' ', Configuration.ConsoleWidth - 3));
            }
        }
    }
}
