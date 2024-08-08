using AfflictionComponent.Components;

namespace PlayerVitals.Afflictions;

public class FaintingAffliction(string afflictionName, string cause, string desc, string noHealDesc, AfflictionBodyArea location, string spriteName, bool risk, bool buff, float duration, bool noTimer, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems) : CustomAffliction(afflictionName, cause, desc, noHealDesc, location, spriteName, risk, buff, duration, noTimer, instantHeal, remedyItems, altRemedyItems)
{
    protected override void CureSymptoms() { }

    protected override void OnCure() { }

    public override void OnUpdate()
    {
        if (!GameManager.GetFatigueComponent().IsExhausted() && HasAfflictionRisk())
            Cure();
    }
}