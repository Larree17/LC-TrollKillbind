using HarmonyLib;
using UnityEngine;
using static KillBind.Patches.UIHandler;
using static KillBind.Patches.StartOfRoundPatch;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatch
    {
        public static MenuManager Instance;

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void OnAwake(MenuManager __instance)
        {
            if (__instance.isInitScene) { return; } //To avoid issues
            Instance = __instance;
            CreateInScene();
            return;
        }
    }
}