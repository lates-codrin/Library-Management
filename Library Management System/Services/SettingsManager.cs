using System.Configuration;
using Library_Management_System.Properties;

namespace Library_Management_System.Services
{
    public static class SettingsManager
    {
        private const string KeyName = "OpenAIKey";

        public static string LoadOpenAIKey()
        {
            return Properties.Settings.Default[KeyName]?.ToString() ?? string.Empty;
        }

        public static void SaveOpenAIKey(string key)
        {
            Properties.Settings.Default[KeyName] = key;
            Properties.Settings.Default.Save();
        }

        internal static void DeleteOpenAIKey()
        {
            Properties.Settings.Default[KeyName] = "";
            Properties.Settings.Default.Save();
        }
    }
}