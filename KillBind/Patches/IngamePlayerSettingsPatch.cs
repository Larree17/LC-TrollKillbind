using HarmonyLib;
using static KillBind.Initialise;
using static KillBind.Patches.UIHandler;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class IngamePlayerSettingsPatch
    {
        [HarmonyPatch("SaveChangedSettings")]
        [HarmonyPostfix]
        private static void OnSaveChanges()
        {
            ModSettings.DeathCause.Value = UnsetDeathCause;
            ModSettings.HeadType.Value = UnsetHeadType;
            return;
        }

        [HarmonyPatch("DiscardChangedSettings")]
        [HarmonyPostfix]
        private static void OnRestoreChanges()
        {
            DeathDropdownComponent.SetValueWithoutNotify(ModSettings.DeathCause.Value);
            HeadDropdownComponent.SetValueWithoutNotify(ModSettings.HeadType.Value);

            UnsetDeathCause = ModSettings.DeathCause.Value;
            UnsetHeadType = ModSettings.HeadType.Value;

            return;
        }
    }
}