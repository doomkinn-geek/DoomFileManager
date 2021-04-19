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
            ConsoleDrawings.PrintString("C:\\", Configuration.ConsoleWidth / 2, 0);

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

            bool exit = false;
            List<string> arguments = new List<string>();


            while (!exit)
            {
                ConsoleDrawings.TakeNewCommand();
                currentCommand = Console.ReadLine();
                ConsoleDrawings.ClearMessageLine();
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
                        CDCommand(arguments);
                        break;
                    case "help":
                        ConsoleDrawings.PrintInformation("Поддерживаемые команды: exit; ls; cp; rm; file. Введите команду с ключом /? чтобы получить подробное описание");
                        break;
                    case "":
                        arguments.Clear();
                        break;                    
                    default:
                        ConsoleDrawings.PrintWarning($"Could not find the command {theCommand}");
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
        static void CDCommand(List<string> arguments)
        {
            int pageNum;
            if (arguments.Count == 0)
            {
                ConsoleDrawings.PrintError("Неверные параметры...");
                return;
            }
            else if(arguments.Count == 1)
            {
                if(arguments[0] == "/?")
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
                else if(Directory.Exists(arguments[0]))//если не указан номер страницы, выводим первую
                {
                    CurrentPath = arguments[0];
                    ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFoldersFiles, 1);
                }    
                
            }
            else if(arguments.Count == 2)//если два аргумента в команде, значит можно реализовать команду
            {
                CurrentPath = arguments[0];//первый аргумент должен быть путь                
                try
                {
                    pageNum = Convert.ToInt32(arguments[1]);
                }
                catch(FormatException)
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
            arguments.Clear();//по завершении обработки команды очищаем список аргументов, чтобы корректно обработать следующую команду            
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
