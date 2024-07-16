using PlayerVitals.Components;

namespace PlayerVitals.Patches;

internal static class PanelFirstAidPatches
{
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshStatusLabels))]
    private static class ApplyHeatingLocalizationKeys
    {
        private static void Postfix(Panel_FirstAid __instance)
        {
            var heatingComponent = Heating.GetHeatingComponent();
            if (heatingComponent.CurrentHeating > 0)
            {
                __instance.m_ColdStatusLabel.text = Heating.GetHeatingLevelLocalizationKey(heatingComponent.GetHeatingLevel());
            }
        }
    }
}