using PlayerVitals.Components;

namespace PlayerVitals.Patches;

internal static class GameManagerPatches
{
    [HarmonyPatch(nameof(GameManager), nameof(GameManager.InstantiateSystems))]
    private static class AddCustomComponents
    {
        private static void Postfix(GameManager __instance)
        {
            _ = __instance.m_ConditionSystems.GetComponent<Heating>() ?? __instance.m_ConditionSystems.AddComponent<Heating>();
            _ = __instance.m_FirstAidSystems.GetComponent<Hyperthermia>() ?? __instance.m_FirstAidSystems.AddComponent<Hyperthermia>();
        }
    }
}