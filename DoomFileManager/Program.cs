using System;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Collections.Generic;
using System.IO;

namespace DoomFileManager
{

    class Program
    {
        private static string _currentPath;
        public static string currentCommand;
        //public static List<FileSystemInfo> CurrentFolders = new List<FileSystemInfo>();
        //public static List<FileSystemInfo> CurrentFiles = new List<FileSystemInfo>();
        public static List<FileSystemInfo> CurrentFoldersFiles = new List<FileSystemInfo>();
        public static string CurrentPath
        {
            get { return _currentPath; }
            set { SetCurrentPath(value); }
        }
        static void Main(string[] args)
        {
            //фиксируем размеры окна, чтобы нельзя было его изменить
            WindowUtility.FixeConsoleWindow(Configuration.ConsoleHeight, Configuration.ConsoleWidth);

            //рисуем рамку для основной панели с папками/файлами
            ConsoleDrawings.PrintFrameLines(0, 0, Console.WindowWidth, Configuration.MainPanelHeight);
            ConsoleDrawings.PrintFrameLines(0, Configuration.MainPanelHeight, Console.WindowWidth, Configuration.InfoPanelHeight);


            //считывание сохраненного пути с последнего сеанса работы
            if (File.Exists("last.state"))
            {
                using (StreamReader sr = new StreamReader("last.state")) //File.OpenText(inputFile))
                {
                    string strState = "";
                    try
                    {
                        strState = sr.ReadLine();
                    }
                    catch (Exception)
                    {
                        CurrentPath = "C:\\";
                    }
                    CurrentPath = strState;
                }
            }
            else
            {
                CurrentPath = "C:\\";
            }
            ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFoldersFiles, 1);
            //справка:
            ConsoleDrawings.PrintInformation("help - список всех поддерживаемых комманд");

            bool exit = false;
            List<string> arguments = new List<string>();


            while (!exit)
            {
                ConsoleDrawings.TakeNewCommand();
                currentCommand = Console.ReadLine();
                ConsoleDrawings.ClearMessageLine();
                arguments.Clear();//по завершении обработки команды очищаем список аргументов, чтобы корректно обработать следующую команду            
                arguments.AddRange(currentCommand.Split(' '));

                string theCommand = arguments[0];
                arguments.Remove(theCommand);
                switch (theCommand.ToLower().Trim())
                {
                    case "exit":
                        SaveLastState();
                        exit = true;
                        break;
                    case "ls":
                        LSCommand(arguments);
                        break;
                    case "cp":
                        CPCommand(arguments);
                        break;
                    case "rm":
                        RMCommand(arguments);
                        break;
                    case "file":
                        FILECommand(arguments);
                        break;
                    case "help":
                        ConsoleDrawings.PrintInformation("Поддерживаемые команды: exit; ls; cp; rm; file. Введите команду с ключом /? чтобы получить подробное описание");
                        break;
                    case "":
                        arguments.Clear();
                        break;
                    default:
                        ConsoleDrawings.PrintWarning($"Невозможно обработать команду {theCommand}");
                        arguments.Clear();
                        break;
                }
            }
        }

        static void SaveLastState()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("last.state", false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(CurrentPath);
                }
            }
            catch (Exception) { }
        }
        static void LSCommand(List<string> arguments)
        {
            int pageNum;
            if (arguments.Count == 0)
            {
                ConsoleDrawings.PrintError("Неверные параметры...");
                return;
            }
            else if (arguments.Count == 1)
            {
                if (arguments[0] == "/?")
                {
                    ConsoleDrawings.PrintInformation("ls <путь> <номер страницы> - команда выводит дерево указанного каталога с указанным номером страницы");
                }
                else if (arguments[0] == "..")
                {
                    DirectoryInfo parentDirectory = Directory.GetParent(CurrentPath);
                    if (parentDirectory == null)
                    {
                        ConsoleDrawings.PrintWarning("Не существует родительской директории для указанного каталога");
                    }
                    else
                    {
                        CurrentPath = parentDirectory.FullName;
                        ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFoldersFiles, 1);
                    }
                }
                else //if(Directory.Exists(arguments[0]))//если не указан номер страницы, выводим первую
                {
                    CurrentPath = arguments[0];
                    ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFoldersFiles, 1);
                }

            }
            else if (arguments.Count == 2)//если два аргумента в команде, значит можно реализовать команду
            {
                CurrentPath = arguments[0];//первый аргумент должен быть путь                
                try
                {
                    pageNum = Convert.ToInt32(arguments[1]);
                }
                catch (FormatException)
                {
                    pageNum = 1;
                    ConsoleDrawings.PrintWarning("Некорректный аргумент <номер страницы> - выводим 1ю страницу...");
                }
                if (pageNum == 0)
                    pageNum = 1;
                ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFoldersFiles, pageNum);
            }
            else//если аргументов более 2х, возможно путь разделен пробелами
            {
                //пытаемся собрать путь, предположив, что он разделен пробелами
                string resPath = "";
                for (int i = 0; i < arguments.Count - 1; i++)
                {
                    resPath += arguments[i];
                    resPath += " ";
                }
                resPath = resPath.Trim();
                try
                {
                    pageNum = Convert.ToInt32(arguments[arguments.Count - 1]);
                }
                catch (FormatException)
                {
                    resPath += " ";
                    resPath += arguments[arguments.Count - 1];
                    pageNum = 1;
                }
                CurrentPath = resPath;
                ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFoldersFiles, pageNum);
                //ConsoleDrawings.PrintError("Невозможно распознать команду, - аргументов более 2х");
            }
        }

        static void CPCommand(List<string> arguments)
        {
            if (arguments.Count == 0)
            {
                ConsoleDrawings.PrintError("Неверные параметры...");
                return;
            }
            else if (arguments.Count == 1)
            {
                if (arguments[0].Trim() == "/?")
                {
                    ConsoleDrawings.PrintInformation("cp <путь - откуда копируем> <путь куда копируем> - копирование файла/каталога");
                }
                else
                {
                    ConsoleDrawings.PrintError("Неправильный вызов команды cp");
                }
            }
            else if (arguments.Count == 2)
            {
                string source = arguments[0];
                string dest = arguments[1];
                if (source.Trim() == dest.Trim())
                {
                    ConsoleDrawings.PrintError("Нельзя скопировать файл/папку саму в себя");
                }
                else if (source.Trim() == "" || dest.Trim() == "")
                {
                    ConsoleDrawings.PrintError("Неверные аргументы");
                }
                else
                {
                    Copy(source, dest);
                }
            }
            else
            {
                string resSourcePath = arguments[0];
                string resDestPath = "";
                int indexOfStartDest = 0;//индекс начала второго параметра
                for (int i = 1; i < arguments.Count; i++)
                {
                    resSourcePath += " ";
                    if (arguments[i].Length > 1)
                    {
                        if (arguments[i].Substring(1, 1) != ":")
                        {
                            resSourcePath += arguments[i];
                        }
                        else
                        {
                            indexOfStartDest = i;
                            break;
                        }
                    }
                    else
                    {
                        resSourcePath += arguments[i];
                    }
                }
                resSourcePath = resSourcePath.Trim();
                for (int i = indexOfStartDest; i < arguments.Count; i++)
                {
                    resDestPath += arguments[i];
                    resDestPath += " ";
                }
                resDestPath = resDestPath.Trim();
                Copy(resSourcePath, resDestPath);
            }
        }

        //Если в параметрах существует путь с пробелом, то нужно сначала разобрать пути, потому запускать копирование
        //поэтому вынес непосредственно копирование в отдельную процедуру
        static void Copy(string source, string dest)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(source);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    if (CopyDirectory(source, dest))
                        ConsoleDrawings.PrintInformation("Копирование завершено...");
                }
                else
                {
                    try
                    {
                        File.Copy(source, dest, true);
                        ConsoleDrawings.PrintInformation("Копирование завершено...");
                    }
                    catch (Exception e)
                    {
                        ConsoleDrawings.PrintError($"Ошибка копирования: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleDrawings.PrintError(e.Message);
            }
        }

        static bool CopyDirectory(string sourceDirName, string destDirName)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                DirectoryInfo[] dirs = dir.GetDirectories();

                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, true);
                    ConsoleDrawings.PrintInformation($"Копируем {file.Name}");
                }

                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath);
                }
                return true;
            }
            catch (Exception e)
            {
                ConsoleDrawings.PrintError($"Ошибка копирования: {e.Message}");
                return false;
            }
        }

        static void RMCommand(List<string> arguments)
        {
            if (arguments.Count == 0)
            {
                ConsoleDrawings.PrintError("Неверные параметры...");
                return;
            }
            else if (arguments.Count == 1)
            {
                if (arguments[0].Trim() == "/?")
                {
                    ConsoleDrawings.PrintInformation("rm <путь к папке/файлу, которую/который необходимо удалить>");
                }
                else
                {
                    Delete(arguments[0]);
                }
            }            
            else
            {
                string resPath = arguments[0];
                for (int i = 1; i < arguments.Count; i++)
                {
                    resPath += " ";
                    resPath += arguments[i];
                }
                resPath = resPath.Trim();                
                Delete(resPath);
            }
        }

        static void Delete(string path)
        {            
            FileInfo file;
            DirectoryInfo directories;
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    directories = new DirectoryInfo(path);
                    foreach (DirectoryInfo dir in directories.GetDirectories())
                    {
                        ConsoleDrawings.PrintInformation($"Удаляем {dir.Name}");
                        dir.Delete(true);
                    }
                    if (directories.FullName == CurrentPath)
                    {
                        CurrentPath = directories.Parent.FullName;//присваиваем, чтобы сработала процедура, которая обновит список
                        ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFoldersFiles, 1);
                    }
                    directories.Delete(true);
                }
                else
                {
                    file = new FileInfo(path);
                    file.Delete();
                    if (file.Directory.FullName == CurrentPath)
                    {
                        CurrentPath = file.Directory.FullName;//присваиваем, чтобы сработала процедура, которая обновит список
                        ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFoldersFiles, 1);
                    }
                }
                ConsoleDrawings.PrintInformation("Удаление завершено");
            }
            catch (Exception e)
            {
                ConsoleDrawings.PrintError(e.Message);
                return;
            }
        }

        static void FILECommand(List<string> arguments)
        {
            if (arguments.Count == 0)
            {
                ConsoleDrawings.PrintError("Неверные параметры...");
                return;
            }
            else if (arguments.Count == 1)
            {
                if (arguments[0].Trim() == "/?")
                {
                    ConsoleDrawings.PrintInformation("file <путь к файлу, который необходимо вывести>");
                }
                else
                {
                    ReadFile(arguments[0]);
                }
            }
            else
            {
                string resPath = arguments[0];
                for (int i = 1; i < arguments.Count; i++)
                {
                    resPath += " ";
                    resPath += arguments[i];
                }
                resPath = resPath.Trim();
                ReadFile(resPath);
            }
        }

        static void ReadFile(string path)
        {
            try
            {
                if(!File.Exists(path))
                {
                    ConsoleDrawings.PrintError($"Файл {path} не существует/не доступен");
                }
                else
                {
                    ConsoleDrawings.PrintFileContent(path);
                }
            }
            catch(Exception e)
            {
                ConsoleDrawings.PrintError($"Ошибка чтения файла: {e.Message}");
            }
        }

        static void SetCurrentPath(string path)
        {
            if(!Directory.Exists(path))
            {
                ConsoleDrawings.PrintError("Указанная директория не существует");
                _currentPath = "";
                CurrentFoldersFiles.Clear();
                return;
            }
            _currentPath = path;            
            if (CurrentFoldersFiles.Count != 0)
            {
                CurrentFoldersFiles.Clear();
            }

            try
            {
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    DirectoryInfo di = new DirectoryInfo(directory);
                    CurrentFoldersFiles.Add(di);
                }
            }
            catch(Exception e)
            {
                ConsoleDrawings.PrintError(e.Message);
                CurrentFoldersFiles.Clear();
                return;
            }

            try
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    CurrentFoldersFiles.Add(fi);
                }
            }
            catch (Exception e)
            {
                ConsoleDrawings.PrintError(e.Message);
                CurrentFoldersFiles.Clear();
                return;
            }
        }


    }
}
