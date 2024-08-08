using PlayerVitals.Utilities;

namespace PlayerVitals;

internal sealed class Mod : MelonMod
{
    public override void OnInitializeMelon() => LocalizationLoader.Load();
}