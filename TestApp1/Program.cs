using Newtonsoft.Json;
using System.IO;
using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

class Program
{
    static void Main(string[] args)
    {
        ReplaceFiles();  
    }

    /// <summary>
    /// Файл настроек
    /// </summary>
    public class Settings
    {
        public string[] fromDir { get; set; }
        public string toDir { get; set; }
    }
    
    /// <summary>
    /// Чтение файла настроек
    /// </summary>
    public static Settings ReadSettings()
    {
        JsonSerializer serializer = new JsonSerializer();
        Settings sets = new Settings();
            var fileJson = File.Open(
                Path.Combine(Environment.CurrentDirectory, "Settings\\settings.json"), FileMode.Open, FileAccess.Read, FileShare.Read);
            var streamReader = new StreamReader(fileJson);
            JsonReader reader = new JsonTextReader(streamReader);
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        sets = serializer.Deserialize<Settings>(reader);
                    }
                }
            }
        return sets;
    }

    /// <summary>
    /// Перемещение файлов
    /// </summary>
    public static void ReplaceFiles()
    {
        string toDirectory = ReadSettings().toDir + "\\" + DateTime.Now.ToString("dd_MM_yy_HH_mm_ss");
        string logPath = toDirectory + "\\Log.txt";
        try
        {
            Directory.CreateDirectory(toDirectory);
            WriteLog("Info: Создана папка " + toDirectory, logPath);
            foreach (string dir in ReadSettings().fromDir)
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    string fileName = toDirectory + "\\" + Path.GetFileName(file);
                    try
                    {
                        File.Copy(file, fileName);
                    }
                    catch (Exception ex)
                    {
                        WriteLog("Info: " + ex.Message, logPath);
                    }
                }
            }
            WriteLog("Info: Копирование файлов завершено", logPath);
        }
        catch (Exception ex)
        {
            WriteLog("Error: " + ex.Message, logPath);
        }
    }

    public static void WriteLog(string message, string directory)
    {
        StreamWriter sw = new StreamWriter(directory, true);
        sw.WriteLine(String.Format("{0,-10} {1}", DateTime.Now.ToString(), message));
        sw.Close();
    }
}