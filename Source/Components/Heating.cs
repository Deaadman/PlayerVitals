using PlayerVitals.Enums;

namespace PlayerVitals.Components;

[RegisterTypeInIl2Cpp(false)]
internal class Heating : MonoBehaviour
{
    internal float CurrentHeating { get; private set; }
    
    internal const float MaxHeating = 100f;
    
    private float CurrentHeatingPerHour;
    private float CurrentCoolingPerHour;
    
    private const float TemperatureHeatingStarts = 5f;
    
    private const float HeatingIncreasePerHourPerDegreeCelsius = 6f;
    private const float HeatingDecreasePerHourPerDegreeCelsius = 6f;
    
    private const float OverheatingThreshold = 99.9f;
    private const float VeryHotThreshold = 80f;
    private const float HotThreshold = 65f;
    private const float SlightlyHotThreshold = 40f;
    
    private void Awake() => CurrentHeating = Mathf.Clamp(CurrentHeating, 0, MaxHeating);

    internal float GetFillValueHeat()
    {
        return 1f - CurrentHeating / MaxHeating;
    }
    
    internal static Heating GetHeatingComponent() => GameManager.Instance().m_ConditionSystems.GetComponent<Heating>();
    
    internal HeatingLevel GetHeatingLevel()
    {
        return CurrentHeating switch
        {
            >= OverheatingThreshold => HeatingLevel.Overheating,
            >= VeryHotThreshold => HeatingLevel.VeryHot,
            >= HotThreshold => HeatingLevel.Hot,
            >= SlightlyHotThreshold => HeatingLevel.SlightlyHot,
            _ => HeatingLevel.Warm
        };
    }
    
    internal static string GetHeatingLevelLocalizationKey(HeatingLevel heatingLevel)
    {
        return heatingLevel switch
        {
            HeatingLevel.Overheating => Localization.Get("GAMEPLAY_Overheating"),
            HeatingLevel.VeryHot => Localization.Get("GAMEPLAY_HeatLevelVeryHot"),
            HeatingLevel.Hot => Localization.Get("GAMEPLAY_HeatLevelHot"),
            HeatingLevel.SlightlyHot => Localization.Get("GAMEPLAY_HeatLevelSlightlyHot"),
            _ => Localization.Get("GAMEPLAY_ColdLevelWarm")
        };
    }
    
    internal float GetRateOfChange()
    {
        if (Utils.IsZero(CurrentHeatingPerHour, 0.0001f))
        {
            if (CurrentHeating == 0f || Mathf.Approximately(CurrentHeating, MaxHeating))
            {
                return 0f;
            }
            return -1f * CurrentCoolingPerHour;
        }
        else
        {
            if (CurrentHeating == 0f || Mathf.Approximately(CurrentHeating, MaxHeating))
            {
                return 0f;
            }
            return CurrentHeatingPerHour;
        }
    }
    
    private void Update()
    {
        if (GameManager.m_IsPaused || GameManager.GetPlayerManagerComponent().m_SuspendConditionUpdate || GameManager.s_IsGameplaySuspended)
        {
            return;
        }
        
        UpdateHeatingStatus();
    }
    
    private void UpdateHeatingStatus()
    {
        var freezingComponent = GameManager.GetFreezingComponent();
        var bodyTemperature = freezingComponent.CalculateBodyTemperature();
        var todHours = GameManager.GetTimeOfDayComponent().GetTODHours(Time.deltaTime);
        
        if (bodyTemperature >= TemperatureHeatingStarts)
        {
            var tempDiff = bodyTemperature - TemperatureHeatingStarts;
            CurrentHeatingPerHour = tempDiff * HeatingIncreasePerHourPerDegreeCelsius;
            CurrentCoolingPerHour = 0f;
        }
        else
        {
            var tempDiff = TemperatureHeatingStarts - bodyTemperature;
            CurrentCoolingPerHour = tempDiff * HeatingDecreasePerHourPerDegreeCelsius;
            CurrentHeatingPerHour = 0f;
        }

        if (CurrentHeatingPerHour > 0)
        {
            CurrentHeating += CurrentHeatingPerHour * todHours;
        }
        else
        {
            CurrentHeating -= CurrentCoolingPerHour * todHours;
        }

        CurrentHeating = Mathf.Clamp(CurrentHeating, 0f, MaxHeating);
    }
}