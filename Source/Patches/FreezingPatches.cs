using PlayerVitals.Components;

namespace PlayerVitals.Patches;

internal static class FreezingPatches
{
    [HarmonyPatch(nameof(Freezing), nameof(Freezing.UpdateFreezingStatus))]
    private static class PreventUpdateOfFreezingComponent
    {
        private static void Postfix(Freezing __instance)
        {
            var heatingComponent = __instance.GetComponent<Heating>();
            if (heatingComponent != null && heatingComponent.CurrentHeating > 0)
            {
                __instance.m_CurrentFreezing = 0;   
            }
        }
    }
}