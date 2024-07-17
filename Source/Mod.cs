using PlayerVitals.Utilities;

namespace PlayerVitals;

internal sealed class Mod : MelonMod
{
    public override void OnInitializeMelon()
    {
        LocalizationLoader.Load();
    }

    [HarmonyPatch(nameof(StatusBar), nameof(StatusBar.IsDebuffActive))]
    private static class Testing
    {
        private static void Postfix(StatusBar __instance, ref bool __result)
        {
            if (__instance.m_StatusBarType != StatusBar.StatusBarType.Condition) return;
            __result = true;
        }
    }
    
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasRiskAffliction))]
    private static class Testing2
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            __result = true;
        }
    }
    
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshPaperDoll))]
    private static class Testing3
    {
        private static void Postfix(Panel_FirstAid __instance)
        {
            Utils.SetActive(__instance.m_PaperDollMale, PlayerManager.m_VoicePersona == VoicePersona.Male);
            Utils.SetActive(__instance.m_PaperDollFemale, PlayerManager.m_VoicePersona == VoicePersona.Female);

            if (InterfaceManager.GetPanel<Panel_Clothing>().IsEnabled())
            {
                InterfaceManager.GetPanel<Panel_Clothing>().Enable(false);
            }
            
            var chestIconIndex = -1;
            for (var i = 0; i < __instance.m_BodyIconList.Length; i++)
            {
                if (!__instance.m_BodyIconList[i].name.ToLower().Contains("chest")) continue;
                chestIconIndex = i;
                break;
            }

            if (chestIconIndex == -1) return;
            
            __instance.m_BodyIconList[chestIconIndex].gameObject.SetActive(true);
            __instance.m_BodyIconList[chestIconIndex].color = new Color(0.7352941f, 0.5191095f, 0.2324827f, 1f);
            __instance.m_BodyIconList[chestIconIndex].spriteName = __instance.m_BodyIconSpriteNameAffliction;
            
            var chestIconObject = __instance.m_BodyIconList[chestIconIndex].gameObject;
            Component[] childComponents = chestIconObject.GetComponentsInChildren<Component>();
            foreach (var component in childComponents)
            {
                if (component is not UISprite childSprite) continue;
                var colorWithAlpha = __instance.m_ColorAffliction;
                colorWithAlpha.a = childSprite.color.a;
                childSprite.color = colorWithAlpha;
            }
        }
    }
    
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshScrollList))]
    private static class Testing4
    {
        private static void Postfix(Panel_FirstAid __instance)
        {
            var scrollList = __instance.m_ScrollListEffects;
            
            var customItemExists = false;
            for (var i = 0; i < scrollList.m_ScrollObjects.Count; i++)
            {
                if (scrollList.m_ScrollObjects[i].name == "CustomScrollListItem")
                {
                    customItemExists = true;
                    break;
                }
            }
            
            if (!customItemExists)
            {
                var customObj = UnityEngine.Object.Instantiate(scrollList.m_PrefabObject, scrollList.transform);
                customObj.name = "CustomScrollListItem";
        
                var customButton = customObj.GetComponentInChildren<AfflictionButton>();

                if (customButton != null)
                {
                    //customButton.m_AfflictionType = AfflictionType.Generic;
                    customButton.m_AfflictionLocation = AfflictionBodyArea.Chest;
                    customButton.m_LabelCause.text = "Custom Cause";
                    customButton.m_LabelEffect.text = "Custom Affliction";
                    customButton.m_LabelEffect.color = new Color(0.7352941f, 0.5191095f, 0.2324827f, 1f);
                    customButton.m_SpriteEffect.color = new Color(0.7352941f, 0.5191095f, 0.2324827f, 1f);
                    customButton.m_SpriteEffect.spriteName = "status_major";
                
                    scrollList.CleanUp();
                    scrollList.m_ScrollObjects.Insert(0, customObj);
                    scrollList.CreateList(scrollList.m_ScrollObjects.Count);
                    scrollList.Update();
                }
            }
        }
    }
    
    // [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.GetColorBasedOnAffliction))]
    // private static class Testing5
    // {
    //     private static void Postfix(AfflictionButton __instance, ref Color __result, bool isHovering)
    //     {
    //         __result = isHovering ? __instance.m_RiskColorHover : __instance.m_RiskColor;
    //     }
    // }
}