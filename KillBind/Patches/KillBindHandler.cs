using GameNetcodeStuff;
using HarmonyLib;
using KillBindNS;
using System;
using UnityEngine;
using static KillBindNS.Initialise;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class KillBindHandler
    {
        [HarmonyPatch("LateUpdate")]
        [HarmonyPrefix]
        public static void OnPressKill(PlayerControllerB __instance)
        {
            if (ModSettings.ModEnabled.Value && Initialise.InputActionInstance.ExplodeKey.triggered && __instance == GameNetworkManager.Instance.localPlayerController && !__instance.isPlayerDead && !__instance.isTypingChat && !HUDManager.Instance.typingIndicator.enabled && (!UnityEngine.Object.FindObjectOfType<Terminal>().terminalInUse && !__instance.inTerminalMenu))
            {
                if (Initialise.DeathCause.Value > Enum.GetValues(typeof(CauseOfDeath)).Length || Initialise.DeathCause.Value < 0) { Initialise.DeathCause.Value = (int)Initialise.DeathCause.DefaultValue; Initialise.mls.LogInfo("Your config for HeadType is invalid, reverting to default"); } //If your choice is invalid, set to default (unknown death cause)
                if (Initialise.HeadType.Value > 3 || Initialise.HeadType.Value < 0) { Initialise.HeadType.Value = (int)Initialise.HeadType.DefaultValue; Initialise.mls.LogInfo("Your config for HeadType is invalid, reverting to default"); } //If your choice is invalid, set to default (explode head)

                __instance.KillPlayer(Vector3.zero, true, (CauseOfDeath)Initialise.DeathCause.Value, Initialise.HeadType.Value);
            }
        }
    }
}
