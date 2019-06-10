using System;
using System.IO;

namespace MusicRnn.Services
{
    class ConfigService
    {
        private static ConfigService _instance = new ConfigService();
        public static ConfigService GetInstance { get => _instance; }

        private string _envPath;
        public string EnvPath
        {
            get => _envPath;
            set
            {
                File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}script\Start.bat", $@"{value}\Scripts\activate.bat & python {AppDomain.CurrentDomain.BaseDirectory}script\main.py & pause");
                _envPath = value;
            }
        }


    }
}
