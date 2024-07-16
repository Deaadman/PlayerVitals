using PlayerVitals.Components;

namespace PlayerVitals.Patches;

internal static class StatusBarPatches
{
    [HarmonyPatch(typeof(StatusBar), nameof(StatusBar.GetFillValuesCold))]
    private static class UpdateFillValuesForHeating
    {
        private static void Postfix(StatusBar __instance, ref StatusBar.FillValues __result)
        {
            var heatingComponent = GameManager.GetFreezingComponent().GetComponent<Heating>();

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

            var heatingComponent = GameManager.GetFreezingComponent().GetComponent<Heating>();
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
    
    /* --- */
    /* Below here is code I was working on for updating the status bar UI, the circle thingy */
    
    // [HarmonyPatch(typeof(StatusBar), nameof(StatusBar.Awake))]
    // private static class StatusBarAwakePatch
    // {
    //     private static void Postfix(StatusBar __instance)
    //     {
    //         if (__instance.m_StatusBarType != StatusBar.StatusBarType.Cold) return;
    //         
    //         // Check if ForegroundHeating already exists
    //         Transform existingHeating = __instance.transform.Find("ForegroundHeating");
    //         if (existingHeating != null)
    //         {
    //             Logging.LogDebug($"ForegroundHeating already exists for Cold StatusBar: {__instance.gameObject.name}");
    //             return;
    //         }
    //         
    //         var heatingFillObj = GameObject.Instantiate(__instance.m_FillSprite.gameObject, __instance.m_FillSprite.transform.parent);
    //         heatingFillObj.name = "ForegroundHeating";
    //         var heatingFillSprite = heatingFillObj.GetComponent<UISprite>();
    //         
    //         heatingFillSprite.color = new Color(1f, 0.5f, 0f);
    //         heatingFillObj.SetActive(false);
    //         
    //         Logging.LogDebug($"Created ForegroundHeating for Cold StatusBar: {__instance.gameObject.name}");
    //     }
    // }
    //
    // [HarmonyPatch(typeof(StatusBar), nameof(StatusBar.Update))]
    // private static class StatusBarUpdatePatch
    // {
    //     private static void Postfix(StatusBar __instance)
    //     {
    //         if (__instance.m_StatusBarType != StatusBar.StatusBarType.Cold) return;
    //
    //         var heatingComponent = GameManager.GetFreezingComponent().GetComponent<Heating>();
    //         if (heatingComponent == null)
    //         {
    //             Logging.LogDebug("Heating component not found");
    //             return;
    //         }
    //
    //         // Search for ForegroundHeating in children
    //         var heatingFillObj = __instance.transform.Find("ForegroundHeating");
    //         if (heatingFillObj == null)
    //         {
    //             Logging.LogDebug($"ForegroundHeating object not found for StatusBar: {__instance.gameObject.name}");
    //             return;
    //         }
    //
    //         var heatingFillSprite = heatingFillObj.GetComponent<UISprite>();
    //         if (heatingFillSprite == null)
    //         {
    //             Logging.LogDebug("UISprite component not found on ForegroundHeating");
    //             return;
    //         }
    //
    //         Logging.LogDebug($"StatusBar: {__instance.gameObject.name}, Current Heating: {heatingComponent.m_CurrentHeating}, Max Heating: {heatingComponent.m_MaxHeating}");
    //
    //         if (heatingComponent.m_CurrentHeating > 0)
    //         {
    //             var heatingFillAmount = heatingComponent.m_CurrentHeating / heatingComponent.m_MaxHeating;
    //             heatingFillSprite.fillAmount = heatingFillAmount;
    //             heatingFillObj.gameObject.SetActive(true);
    //             Logging.LogDebug($"Set ForegroundHeating active for StatusBar: {__instance.gameObject.name}. Fill amount: {heatingFillAmount}");
    //         }
    //         else
    //         {
    //             heatingFillObj.gameObject.SetActive(false);
    //             Logging.LogDebug($"Set ForegroundHeating inactive for StatusBar: {__instance.gameObject.name}");
    //         }
    //     }
    // }
}