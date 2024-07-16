using PlayerVitals.Components;

namespace PlayerVitals.Patches;

internal static class StatusBarPatches
{
    [HarmonyPatch(typeof(StatusBar), nameof(StatusBar.Awake))]
    private static class CloneFillSprite
    {
        private static void Postfix(StatusBar __instance)
        {
            if (__instance.m_StatusBarType != StatusBar.StatusBarType.Cold) return;

            var overlay = UnityEngine.Object.Instantiate(__instance.m_FillSprite.gameObject, __instance.m_FillSprite.transform);
            overlay.name = "Foreground2";
            overlay.GetComponent<UISprite>().color = Color.red;
            overlay.SetActive(false);
        }
    }
    
    [HarmonyPatch(typeof(StatusBar), nameof(StatusBar.GetFillValuesCold))]
    private static class UpdateFillValuesForHeating
    {
        private static void Postfix(StatusBar __instance, ref StatusBar.FillValues __result)
        {
            var heatingComponent = Heating.GetHeatingComponent();

            if (heatingComponent != null && heatingComponent.CurrentHeating > 0)
            {
                var heatingEffect = heatingComponent.CurrentHeating / Heating.MaxHeating;
                __result.m_NormalizedValue = Mathf.Min(1f, __result.m_NormalizedValue + heatingEffect);
                __result.m_Fill = __result.m_NormalizedValue;
            }
        }
    }

    [HarmonyPatch(typeof(StatusBar), nameof(StatusBar.SetArrowBools))]
    private static class UpdateArrowsForHeating
    {
        private static void Postfix(StatusBar __instance, float rateOfChange)
        {
            if (__instance.m_StatusBarType != StatusBar.StatusBarType.Cold) return;

            var heatingComponent = Heating.GetHeatingComponent();
            if (heatingComponent != null && heatingComponent.CurrentHeating > 0)
            {
                var heatingRate = heatingComponent.GetRateOfChange();

                Array.Clear(__instance.m_UpArrowsEnabled, 0, __instance.m_UpArrowsEnabled.Length);
                Array.Clear(__instance.m_DownArrowsEnabled, 0, __instance.m_DownArrowsEnabled.Length);

                if (heatingRate > 0)
                {
                    if (heatingRate > __instance.m_HighThreshold)
                        __instance.m_UpArrowsEnabled[2] = true;
                    else if (heatingRate > __instance.m_MediumThreshold)
                        __instance.m_UpArrowsEnabled[1] = true;
                    else
                        __instance.m_UpArrowsEnabled[0] = true;
                }
                else if (heatingRate < 0)
                {
                    var absHeatingRate = Mathf.Abs(heatingRate);
                    if (absHeatingRate > __instance.m_HighThreshold)
                        __instance.m_DownArrowsEnabled[2] = true;
                    else if (absHeatingRate > __instance.m_MediumThreshold)
                        __instance.m_DownArrowsEnabled[1] = true;
                    else
                        __instance.m_DownArrowsEnabled[0] = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(StatusBar), nameof(StatusBar.Update))]
    private static class UpdateClonedSprite
    {
        private static void Postfix(StatusBar __instance)
        {
            if (__instance.m_StatusBarType != StatusBar.StatusBarType.Cold) return;

            var overlay = __instance.m_FillSprite.transform.Find("Foreground2")?.gameObject;
            if (overlay == null) return;

            overlay.SetActive(true);
            overlay.GetComponent<UISprite>().fillAmount = Mathf.Lerp(__instance.m_FillSpriteOffset, 1f - __instance.m_FillSpriteOffset, 1f - Heating.GetHeatingComponent().GetFillValueHeat());
        }
    }
}