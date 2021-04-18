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
        public static List<FileSystemInfo> CurrentFolders = new List<FileSystemInfo>();
        public static List<FileSystemInfo> CurrentFiles = new List<FileSystemInfo>();
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

            CurrentPath = "C:\\";
            ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFolders, 1);

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
                        exit = true;
                        break;
                    case "cd":
                        CDCommand(arguments);
                        break;
                    case "help":
                        ConsoleDrawings.PrintInformation("Поддерживаемые команды: exit; cd; ls; cp; rm; file. Введите команду с ключом /? чтобы получить подробное описание");
                        break;
                    case "":
                        break;
                    default:
                        ConsoleDrawings.PrintWarning($"Could not find the command {theCommand}");
                        break;
                }
            }           
        } 

        static void CDCommand(List<string> arguments)
        {
            if(arguments.Count == 0)
            {
                ConsoleDrawings.PrintError("Wrong parameters...");
                return;
            }
            if(arguments.Count == 1)
            {
                if(arguments[0] == "/?")
                {
                    ConsoleDrawings.PrintInformation("cd <путь> <номер страницы> - команда выводит дерево указанного каталога с указанным номером страницы");
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
                        ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFolders, 1);
                    }
                }
                else if(Directory.Exists(arguments[0]))//если не указан номер страницы, выводим первую
                {
                    CurrentPath = arguments[0];
                    ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFolders, 1);
                }    
                
            }
            if(arguments.Count == 2)//если два аргумента в команде, значит можно реализовать команду
            {
                CurrentPath = arguments[0];//первый аргумент должен быть путь
                int pageNum;
                try
                {
                    pageNum = Convert.ToInt32(arguments[1]);
                }
                catch(FormatException)
                {
                    pageNum = 1;
                    ConsoleDrawings.PrintWarning("Некорректный аргумент <номер страницы> - выводим 1ю страницу...");
                }
                float pageNumCoefficent = (float)CurrentFolders.Count / (float)Configuration.ElementsOnPage;
                int maxPage = (int)Math.Ceiling(pageNumCoefficent);
                if (pageNum > maxPage)
                {
                    pageNum = maxPage;
                    ConsoleDrawings.PrintWarning($"Некорректный аргумент <номер страницы> максимально допустимая страница - {maxPage}");
                }
                if (pageNum == 0)
                    pageNum = 1;
                ConsoleDrawings.PrintFoldersTree(CurrentPath, CurrentFolders, pageNum);
            }
            arguments.Clear();//по завершении обработки команды очищаем список аргументов, чтобы корректно обработать следующую команду            
        }
        
        static void SetCurrentPath(string path)
        {
            if(!Directory.Exists(path))
            {
                ConsoleDrawings.PrintError("Указанная директория не существует");
                return;
            }
            _currentPath = path;
            if (CurrentFolders.Count != 0)
            {
                CurrentFolders.Clear();
            }
            if (CurrentFiles.Count != 0)
            {
                CurrentFiles.Clear();
            }
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                CurrentFolders.Add(di);
            }

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                CurrentFiles.Add(fi);
            }
        }


    }
}
