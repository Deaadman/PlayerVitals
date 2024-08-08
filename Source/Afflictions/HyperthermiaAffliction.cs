using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;

namespace PlayerVitals.Afflictions;

public class HyperthermiaAffliction(string afflictionName, string cause, string desc, string noHealDesc, AfflictionBodyArea location, string spriteName, bool risk, bool buff, float duration, bool noTimer, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems) : CustomAffliction(afflictionName, cause, desc, noHealDesc, location, spriteName, risk, buff, duration, noTimer, instantHeal, remedyItems, altRemedyItems), IRiskPercentage
{
    private float _riskPercentage;
    private float _lastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
    
    protected override void CureSymptoms() { }

    public float GetRiskValue() => _riskPercentage;
    
    protected override void OnCure() { }

    public override void OnUpdate()
    {
        if (HasAfflictionRisk())
            UpdateRiskValue();
    }

    public void UpdateRiskValue()
    {
        var currentTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
        var elapsedTime = currentTime - _lastUpdateTime;
            
        var riskIncrease = elapsedTime * 60f;
            
        _riskPercentage = Mathf.Min(_riskPercentage + riskIncrease, 100f);
        _lastUpdateTime = currentTime;
    }
}