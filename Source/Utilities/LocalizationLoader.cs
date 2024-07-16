using LocalizationUtilities;

namespace PlayerVitals.Utilities;

internal static class LocalizationLoader
{
    private const string JsonFile = "PlayerVitals.Resources.Localization.json";

    public static void Load()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(JsonFile);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException($"Failed to load resource: {JsonFile}"));
            
            LocalizationManager.LoadJsonLocalization(reader.ReadToEnd());
        }
        catch (Exception ex)
        {
            Logging.LogError($"Error loading localizations: {ex.Message}");
        }
    }
}