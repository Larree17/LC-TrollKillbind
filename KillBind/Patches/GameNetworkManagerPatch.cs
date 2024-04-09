using HarmonyLib;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    public class GameNetworkManagerPatch
    {
        public static int currentGameVersion;

        [HarmonyPatch("Awake")]
        public static void Prefix(GameNetworkManager __instance)
        {
            currentGameVersion = __instance.gameVersionNum;
        }
    }
}