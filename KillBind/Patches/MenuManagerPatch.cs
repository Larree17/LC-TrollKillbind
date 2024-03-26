using HarmonyLib;
using static KillBind.Patches.UIHandler;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatch
    {
        public static MenuManager MenuManagerInstance;

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void OnAwake(MenuManager __instance)
        {
            if (__instance.isInitScene) { return; } //To avoid issues
            MenuManagerInstance = __instance;
            CreateInMemory();
            CreateInScene();
        }
    }
}