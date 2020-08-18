using Newtonsoft.Json;
using System.IO;

namespace DragonfruitPlugin.Client.Helpers
{
    public static class FileHelper
    {
        public static void Serialize<T>(T value, string fileName)
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(value, Formatting.Indented));
        }
    }
}
