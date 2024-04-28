using System.Text.Json;

namespace AlwaysOnTopKeyboard;

public class AppSettings
{
    public Point WindowLocation { get; set; }
    public Size WindowSize { get; set; }
    public double WindowOpacity { get; set; }
    public required string KeyboardLayout { get; set; }
    public required string KeyboardLayoutNextGen { get; set; }

    public static string SettingsFileName = "alwaysontopkeyboard-settings.json";

    public static AppSettings? LoadSettings()
    {
        string filePath = Path.Combine(Application.StartupPath, SettingsFileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AppSettings>(json);
        }

        return null;
    }

    public static void SaveSettings(AppSettings settings)
    {
        string filePath = Path.Combine(Application.StartupPath, SettingsFileName);
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}
