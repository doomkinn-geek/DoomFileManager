using System;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Collections.Generic;
using System.IO;

namespace DoomFileManager
{
    
    class Program
    {
        public static string currentCommand;
        public static List<FileSystemInfo> CurrentFolders = new List<FileSystemInfo>();
        public static List<FileSystemInfo> CurrentFiles = new List<FileSystemInfo>();
        static void Main(string[] args)
        {
            //фиксируем размеры окна, чтобы нельзя было его изменить
            WindowUtility.FixeConsoleWindow(Configuration.ConsoleHeight, Configuration.ConsoleWidth);                       
            
            //рисуем рамку для основной панели с папками/файлами
            ConsoleDrawings.PrintFrameLines(0, 0, Console.WindowWidth, Configuration.MainPanelHeight);
            ConsoleDrawings.PrintFrameLines(0, Configuration.MainPanelHeight, Console.WindowWidth, Configuration.InfoPanelHeight);
            ConsoleDrawings.PrintString("Drives", Configuration.ConsoleWidth / 2 - 3, 0);

            SetCurrentPath("C:\\");

            bool exit = false;
            List<string> command = new List<string>();


            while (!exit)
            {
                ConsoleDrawings.TakeNewCommand();
                currentCommand = Console.ReadLine();
                command.AddRange(currentCommand.Split(' '));

                string theCommand = command[0];
                command.Remove(theCommand);
                switch (theCommand.ToLower())
                {
                    case "exit":
                        exit = true;
                        break;
                    case "cd":
                        break;
                    default:
                        ConsoleDrawings.PrintWarning($"Could not find the command {theCommand}");
                        break;
                }
            }           
        } 
        
        static void SetCurrentPath(string path)
        {
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
