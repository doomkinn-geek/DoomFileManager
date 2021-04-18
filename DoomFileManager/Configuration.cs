using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace DoomFileManager
{
    static class Configuration
    {
        public static int ElementsOnPage
        {
            get
            {
                int res;
                try
                {
                    res = Convert.ToInt32(ReadSetting("ElementsOnPage"));
                }
                catch(Exception)
                {
                    res = 20;
                }
                if (res <= 0) res = 20;
                return res;
            }
        }

        public static int ConsoleHeight
        {
            get
            {
                if(Convert.ToInt32(ReadSetting("ConsoleHeight")) < ElementsOnPage)
                {
                    return ElementsOnPage + 12;
                }
                else
                {
                    return Convert.ToInt32(ReadSetting("ConsoleHeight"));
                }
            }
        }

        public static int ConsoleWidth
        {
            get
            {
                return Convert.ToInt32(ReadSetting("ConsoleWidth"));                
            }
        }

        public static int MainPanelHeight //размер главной панели
        {
            get { return ElementsOnPage + 2; }
        }

        public const int InfoPanelHeight = 10;//размер информационной панели

        public static int MessagesPosition
        {
            get { return Configuration.MainPanelHeight + Configuration.InfoPanelHeight + 1; }
        }

        public static int CommandPosition
        {
            get { return Configuration.MainPanelHeight + Configuration.InfoPanelHeight; }
        }


        private static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "";
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                //;
            }
        }
    }
}
