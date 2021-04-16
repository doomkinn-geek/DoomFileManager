using System;
using System.Runtime.InteropServices;
using System.Configuration;

namespace DoomFileManager
{
    
    class Program
    {
        static int mainPanelHeight;//размер главной панели
        static int infoPanelHeight;//размер информационной панели
        static void Main(string[] args)
        {
            //фиксируем размеры окна, чтобы нельзя было его изменить
            WindowUtility.FixeConsoleWindow(Configuration.ConsoleHeight, Configuration.ConsoleWidth);

            mainPanelHeight = Configuration.ElementsOnPage + 2;
            infoPanelHeight = 10;
            //рисуем рамку для основной панели с папками/файлами
            ConsoleDrawings.PrintFrameLines(0, 0, Console.WindowWidth, mainPanelHeight);
            ConsoleDrawings.PrintFrameLines(0, mainPanelHeight, Console.WindowWidth, infoPanelHeight);

            ConsoleDrawings.PrintString("Drives", Configuration.ConsoleWidth / 2, 0);

            Console.ReadLine();
        }      


    }
}
