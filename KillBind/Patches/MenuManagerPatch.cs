using HarmonyLib;
using static KillBind.Patches.UIHandler;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void OnAwake(MenuManager __instance)
        {
            if (__instance.isInitScene) { return; } //To avoid issues
            CreateInMemory();
            CreateInScene();
        }
    }
}