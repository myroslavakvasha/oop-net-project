using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataLayer.Services
{
    public class ConfigService
    {
        private readonly string _configPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Сonfig", "config.txt");

        public ConfigService()
        {
            if (!File.Exists(_configPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
                File.WriteAllText(_configPath, "FILE");
                IsAPI = false;
                return;
            }

            string val = File.ReadAllText(_configPath).Trim();
            IsAPI = val.Equals("API");
        }

        public bool IsAPI { get; private set; }
        public void SetSource(bool useApi)
        {
            IsAPI = useApi;
            Save();
        }

        private void Save() => File.WriteAllText(_configPath, IsAPI ? "API" : "FILE");
    }
}
