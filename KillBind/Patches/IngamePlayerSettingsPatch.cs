using HarmonyLib;
using static KillBind.Initialise;
using static KillBind.Patches.UIHandler;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class IngamePlayerSettingsPatch
    {
        public static IngamePlayerSettings IngamePlayerSettingsInstance;

        [HarmonyPatch("Awake")]
        private static void Postfix(IngamePlayerSettings __instance)
        {
            IngamePlayerSettingsInstance = __instance;
        }

        [HarmonyPatch("SaveChangedSettings")]
        [HarmonyPostfix]
        private static void OnSaveChanges()
        {
            ModSettings.DeathCause.Value = UnsetDeathCause;
            ModSettings.RagdollType.Value = UnsetRagdollType;
            return;
        }

        [HarmonyPatch("DiscardChangedSettings")]
        [HarmonyPostfix]
        private static void OnRestoreChanges()
        {
            DeathDropdownComponent.SetValueWithoutNotify(ModSettings.DeathCause.Value);
            HeadDropdownComponent.SetValueWithoutNotify(ModSettings.RagdollType.Value);

            UnsetDeathCause = ModSettings.DeathCause.Value;
            UnsetRagdollType = ModSettings.RagdollType.Value;

            return;
        }

        [HarmonyPatch("ResetSettingsToDefault")]
        [HarmonyPostfix]
        private static void ResetSettings()
        {
            ModSettings.RagdollType.Value = (int)ModSettings.RagdollType.DefaultValue;
            ModSettings.DeathCause.Value = (int)ModSettings.DeathCause.DefaultValue;

            UnsetRagdollType = ModSettings.RagdollType.Value;
            UnsetDeathCause = ModSettings.DeathCause.Value;

            DeathDropdownComponent.SetValueWithoutNotify(UnsetDeathCause);
            HeadDropdownComponent.SetValueWithoutNotify(UnsetRagdollType);
            return;
        }
    }
}