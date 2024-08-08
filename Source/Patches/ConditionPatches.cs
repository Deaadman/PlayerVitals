using AfflictionComponent.Components;
using PlayerVitals.Afflictions;

namespace PlayerVitals.Patches;

internal static class ConditionPatches
{
    [HarmonyPatch(nameof(Condition), nameof(Condition.Update))]
    private static class Test
    {
        private static void Postfix(Condition __instance)
        {
            if (GameManager.GetFatigueComponent().IsExhausted() && !AfflictionManager.GetAfflictionManagerInstance().HasAfflictionOfType(typeof(FaintingAffliction)))
                _ = new FaintingAffliction("Fainting Risk", "Being too tired.", "Sleep boy.",
                    "Have to get more sleep boy.", AfflictionBodyArea.Head, "", true, false, 0f, true, false, [], []);
        }
    }
}