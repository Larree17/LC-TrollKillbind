using HarmonyLib;
using static KillBind.Patches.UIHandler;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(QuickMenuManager))]
    public class QuickMenuManagerPatch
    {
        public static QuickMenuManager Instance;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void OnStart(QuickMenuManager __instance)
        {
            Instance = __instance;
            CreateInScene();
        }
    }
}