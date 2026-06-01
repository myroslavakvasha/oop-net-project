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
        private readonly string _configPath = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "..", "DataLayer", "Resources", "Config", "config.txt")
        );

        public ConfigService()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
                    File.WriteAllText(_configPath, "FILE");
                    IsAPI = false;
                    return;
                }

                string val = File.ReadAllText(_configPath).Trim().ToUpper();
                System.Diagnostics.Debug.WriteLine($"File content: '{val}' - Result: {val.Equals("API")}");
                IsAPI = val.Equals("API");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CONFIG ERROR: {ex.Message}");
                IsAPI = false;
            }
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
