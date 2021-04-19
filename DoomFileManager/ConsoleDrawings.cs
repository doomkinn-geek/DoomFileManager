using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private const ConsoleColor infoColor = ConsoleColor.Yellow;
        private const ConsoleColor warningColor = ConsoleColor.DarkYellow;
        private const ConsoleColor errorColor = ConsoleColor.Red;
        private const ConsoleColor fileColor = ConsoleColor.Cyan;

        static CancellationTokenSource cancelTokenSource;
        static CancellationToken token;
        private static Task CalculateFolderSize;
        private static string CurrentFolder;

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

            /*Console.SetCursorPosition(0, Configuration.CommandPosition);
            Console.Write(new string(' ', Configuration.ConsoleWidth));
            Console.SetCursorPosition(0, Configuration.CommandPosition);*/
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
            Console.SetCursorPosition(0, position+1);
            Console.Write(new String(' ', Configuration.ConsoleWidth));
            Console.SetCursorPosition(0, position);
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void PrintFoldersTree(string rootFolder, List<FileSystemInfo> content, int pageNum)//вовод дерева каталогов
        {
            if (content.Count == 0)
            {
                //если список файлов/папок пуст, ничего не выводим, просто оставляем старое дерево
                if (!Directory.Exists(rootFolder))
                    return;
                //если директория существует, но она пуста, то дерево все равно надо вывести
            }
            //рисуем в заголовке текущую директорию + номер страницы
            int maxPage;
            string header = rootFolder;
            if (content.Count != 0)
            {
                float pageNumCoefficent = (float)content.Count / (float)Configuration.ElementsOnPage;
                maxPage = (int)Math.Ceiling(pageNumCoefficent);
            }
            else // если папки отсутствуют в текущей директории вручную пишем количество страниц
            {
                maxPage = 1;
            }
            if (pageNum > maxPage)
            {
                pageNum = maxPage;
                ConsoleDrawings.PrintWarning($"Некорректный аргумент <номер страницы> максимально допустимая страница - {maxPage}");
            }
            header += $" - [{pageNum}/{maxPage}]";
            PrintMainPanelHeader(header);



            int positionX, positionY;
            positionX = 2;
            positionY = 1;
            ClearMainPanel();

            Console.SetCursorPosition(positionX, positionY);
            Console.WriteLine(rootFolder);
            positionY++;
            if (pageNum != 1)//если выводим не 1ю страницу, то рисуем дополнительно "<..>", чтобы было понятно, то это не начало дерева
            {
                Console.SetCursorPosition(positionX, positionY);
                Console.Write(TreeLines.DownContinue);
                Console.Write(TreeLines.Right);
                Console.Write("<..>");
                positionY++;
            }

            for (int i = (pageNum - 1) * Configuration.ElementsOnPage; i < content.Count; i++)
            {
                Console.SetCursorPosition(positionX, positionY);
                positionY++;
                if (i + 1 == content.Count)//если последний элемент дерева, то рисуем завершающую закорючку
                {
                    Console.Write(TreeLines.DownLast);
                }
                else//иначе продолжающаяся закорючка. В этом случае будет понятно, что существует дополнительная страница
                {
                    Console.Write(TreeLines.DownContinue);
                }
                Console.Write(TreeLines.DoubleRight);

                if (content[i] is DirectoryInfo)
                {
                    Console.ForegroundColor = textColor;
                }
                else
                {
                    Console.ForegroundColor = fileColor;
                }
                Console.Write("{0, -70}", content[i].Name);
                Console.ForegroundColor = textColor;

                if (content[i] is DirectoryInfo)
                {
                    /*try
                    {
                        string[] directories = Directory.GetDirectories(content[i].FullName);
                        //если в конкретной папке есть подпапки, помечаем её '[+]', чтобы указать, что есть смысл её просматривать:
                        if (directories.Length != 0)
                        {
                            Console.ForegroundColor = warningColor;
                            Console.WriteLine("[+]");                            
                        }
                    }
                    catch (Exception)
                    {
                        Console.Write("\n");
                        continue;
                    }*/
                    Console.ForegroundColor = textColor;
                    Console.Write("{0, -15}", "<ПАПКА>");
                }
                else if (content[i] is FileInfo)
                {
                    Console.ForegroundColor = textColor;
                    string memSize;
                    try
                    {
                        memSize = ToPrettySize(((FileInfo)content[i]).Length);
                    }
                    catch (Exception)
                    {
                        memSize = "0";
                    }
                    //для более читабельного вывода, дополняем строку проблеми
                    string sep = new string(' ', 7 - memSize.Length);
                    memSize = sep + memSize;
                    Console.Write("{0, -15}", memSize);
                }
                Console.Write("{0, -5}", content[i].LastAccessTime.ToString("dd.MM.yyyy"));
                Console.Write("\t");
                //вывод атрибутов
                if ((content[i].Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    Console.Write("r");
                else
                    Console.Write(".");
                if ((content[i].Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                    Console.Write("a");
                else
                    Console.Write(".");
                if ((content[i].Attributes & FileAttributes.System) == FileAttributes.System)
                    Console.Write("s");
                else
                    Console.Write(".");
                if ((content[i].Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    Console.Write("h");
                else
                    Console.Write(".");

                //    Console.Write("{0, 0}", content[i].Attributes.ToString());
                Console.Write("\n");
                if (positionY > Configuration.ElementsOnPage)
                    break;
            }
            if (rootFolder != CurrentFolder)//делаем эту проверку, чтобы не перерасчитывать общий объем папки
            {
                PrintInformation(rootFolder, content);
            }
            CurrentFolder = rootFolder;
        }

        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;
        private static string ToPrettySize(this long value, int decimalPlaces = 0)//функция округляет числа с размерами файлов, чтобы удобнее их было читать
        {
            var asTb = Math.Round((double)value / OneTb, decimalPlaces);
            var asGb = Math.Round((double)value / OneGb, decimalPlaces);
            var asMb = Math.Round((double)value / OneMb, decimalPlaces);
            var asKb = Math.Round((double)value / OneKb, decimalPlaces);
            string chosenValue = asTb > 1 ? string.Format("{0} Tb", asTb)
                : asGb > 1 ? string.Format("{0} Gb", asGb)
                : asMb > 1 ? string.Format("{0} Mb", asMb)
                : asKb > 1 ? string.Format("{0} Kb", asKb)
                : string.Format("{0}  B", Math.Round((double)value, decimalPlaces));
            return chosenValue;
        }

        public static void PrintInformation(string rootFolder, List<FileSystemInfo> content)
        {
            ClearInformationPanel();
            int positionX, positionY;
            positionX = 2;
            positionY = Configuration.MainPanelHeight + 1;
            Console.SetCursorPosition(positionX, positionY);
            positionY++;
            Console.ForegroundColor = infoColor;
            Console.BackgroundColor = textBackgroundColor;
            Console.WriteLine($"                   Путь: {rootFolder}");
            DirectoryInfo di = new DirectoryInfo(rootFolder);
            Console.SetCursorPosition(positionX, positionY);
            positionY++;
            Console.WriteLine($"          Дата создания: {di.CreationTime.ToString("dd.MM.yyyy")}");
            Console.SetCursorPosition(positionX, positionY);
            positionY++;
            Console.WriteLine($"    Последнее изменение: {di.LastWriteTime.ToString("dd.MM.yyyy")}");

            string attributes;
            if (rootFolder.Length > 4)
            {
                attributes = "               Атрибуты: ";
                if (di.Attributes.HasFlag(FileAttributes.ReadOnly))
                    attributes += "Только для чтения | ";
                if (di.Attributes.HasFlag(FileAttributes.Archive))
                    attributes += "Архивный | ";
                if (di.Attributes.HasFlag(FileAttributes.System))
                    attributes += "Системный | ";
                if (di.Attributes.HasFlag(FileAttributes.Hidden))
                    attributes += "Скрытый | ";
                if (attributes != "               Атрибуты: ")
                {
                    attributes = attributes.Substring(0, attributes.Length - 3);
                    Console.SetCursorPosition(positionX, positionY);
                    positionY++;
                    Console.WriteLine(attributes);
                }
            }
            else
            {
                try
                {
                    DriveInfo drive = new DriveInfo(rootFolder);
                    Console.SetCursorPosition(positionX, positionY);
                    positionY++;
                    Console.WriteLine($"Доступно места на диске: {ToPrettySize(drive.AvailableFreeSpace)}");
                    Console.SetCursorPosition(positionX, positionY);
                    positionY++;
                    Console.WriteLine($"            Метка диска: {drive.VolumeLabel}");
                }
                catch (Exception) { }
            }



            //Подсчитываем размер текущей директории в потоке, чтобы можно было остановить его, в случае смены директории
            long fullDirSize = 0;
            if (CalculateFolderSize != null)
            {
                cancelTokenSource.Cancel();
            }
            Action<object> action = (object obj) =>
            {
                GetTotalSize(rootFolder, ref fullDirSize, token);
                string size;
                try
                {
                    size = ToPrettySize(fullDirSize);
                }
                catch (Exception)
                {
                    size = "0";
                }
                if (fullDirSize != -1)
                {
                    Console.SetCursorPosition(positionX, positionY);
                    Console.Write(new string(' ', Configuration.ConsoleWidth - 3));
                    Console.SetCursorPosition(positionX, positionY);
                    positionY++;
                    Console.ForegroundColor = infoColor;
                    Console.BackgroundColor = textBackgroundColor;
                    Console.WriteLine($"            Общий объем: {size}");
                }
                ConsoleDrawings.TakeNewCommand();
            };

            try
            {
                cancelTokenSource = new CancellationTokenSource();
                token = cancelTokenSource.Token;
                Console.SetCursorPosition(positionX, positionY);
                Console.ForegroundColor = infoColor;
                Console.BackgroundColor = textBackgroundColor;
                Console.WriteLine($"            Общий объем: <расчитывается...>");
                CalculateFolderSize = Task.Factory.StartNew(action, token);
                ConsoleDrawings.TakeNewCommand();
            }
            catch (Exception e)
            {
                //PrintError(e.Message);
            }
        }

        public static void PrintFileContent(string file)
        {
            PrintMainPanelHeader(file);
            int positionX, positionY;
            positionX = 2;
            positionY = 1;
            ClearMainPanel();

            Console.SetCursorPosition(positionX, positionY);            

            using (StreamReader sr = new StreamReader(file, Encoding.Unicode))
            {
                string oneString = String.Empty;
                while ((oneString = sr.ReadLine()) != null)
                {
                    if (positionY > Configuration.ElementsOnPage)
                        break;
                    oneString = oneString.Substring(0, Configuration.ConsoleWidth - 3);
                    Console.SetCursorPosition(positionX, positionY);
                    Console.WriteLine(oneString);
                    positionY++;
                }
                Console.WriteLine();
            }
        }

        public static void GetTotalSize(string directory, ref long totalSize, CancellationToken cancelation)
        {
            string[] files;
            try
            {
                files = System.IO.Directory.GetFiles(directory);
            }
            catch(Exception)
            {                
                return;
            }

            //string folderSize = string.Empty;

            foreach (string file in files)
            {
                if (cancelation.IsCancellationRequested)
                {
                    totalSize = -1;
                    return;
                }
                totalSize += GetFileSize(file);
            }

            string[] subDirs = System.IO.Directory.GetDirectories(directory);
            foreach (string dir in subDirs)
            {
                if (cancelation.IsCancellationRequested)
                {
                    totalSize = -1;
                    return;
                }
                GetTotalSize(dir, ref totalSize, cancelation);
            }
            return;
        }

        private static long GetFileSize(string path)
        {
            try
            {
                FileInfo fi = new System.IO.FileInfo(path);
                return fi.Length;
            }
            catch
            {
                return 0;
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
            int posistionX = 1;
            int positionY = 1;            
            for(int i = 0; i < Configuration.MainPanelHeight - 2; i++)
            {
                Console.SetCursorPosition(posistionX, positionY + i);
                Console.Write(new string(' ', Configuration.ConsoleWidth - 2));
            }
        }

        private static void ClearInformationPanel()//очистка дерева каталогов
        {
            int positionX;
            int positionY;
            positionX = 1;
            positionY = Configuration.MainPanelHeight + 1;
            for (int i = 0; i < Configuration.InfoPanelHeight - 2; i++)
            {
                Console.SetCursorPosition(positionX, positionY + i);
                Console.Write(new string(' ', Configuration.ConsoleWidth - 3));
            }
        }
    }
}
