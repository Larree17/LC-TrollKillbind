using HarmonyLib;
using static KillBind.Patches.UIHandler;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(QuickMenuManager))]
    public class QuickMenuManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void OnStart(QuickMenuManager __instance)
        {
            CreateInScene();
        }
    }
}