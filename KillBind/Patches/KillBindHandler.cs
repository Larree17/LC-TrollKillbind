using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static KillBindNS.Initialise;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class KillBindHandler
    {
        private static PlayerControllerB PlayerControllerBInstance;

        [HarmonyPatch("Awake")]
        public static void Prefix(PlayerControllerB __instance)
        {
            PlayerControllerBInstance = __instance;
            if (PlayerControllerBInstance != GameNetworkManager.Instance.localPlayerController)
            {
                InputActionInstance.ActionKillBind.performed += OnKeyPress;
                modLogger.LogInfo("KillBind has been bound"); // Don't have to worry about unbinding it, it is taken care of in OnKeyPress (with an if statement)
            }
        }

        public static void OnKeyPress(InputAction.CallbackContext callbackContext)
        {
            //Check if situation is valid
            if (!callbackContext.performed) { return; }
            if (PlayerControllerBInstance != GameNetworkManager.Instance.localPlayerController) { return; };
            if (PlayerControllerBInstance.isPlayerDead) { return; };
            if (HUDManager.Instance.typingIndicator.enabled || PlayerControllerBInstance.isTypingChat) { return; };
            if (UnityEngine.Object.FindObjectOfType<Terminal>().terminalInUse && PlayerControllerBInstance.inTerminalMenu) { return; };

            //Check if current config is valid
            if (ModSettings.DeathCause.Value > Enum.GetValues(typeof(CauseOfDeath)).Length || ModSettings.DeathCause.Value < 0) //If your choice is invalid, set to default (unknown death cause)
            {
                ModSettings.DeathCause.Value = (int)ModSettings.DeathCause.DefaultValue;
                modLogger.LogInfo("Your config for HeadType is invalid, reverting to default");
            }

            if (ModSettings.HeadType.Value > 3 || ModSettings.HeadType.Value < 0) //If your choice is invalid, set to default (explode head)
            {
                ModSettings.HeadType.Value = (int)ModSettings.HeadType.DefaultValue;
                modLogger.LogInfo("Your config for HeadType is invalid, reverting to default");
            }

            //Run KillPlayer
            CoroutineHelper.Start(KillNextUpdate());
        }

        private static IEnumerator KillNextUpdate()
        {
            yield return null; //To fix the body spawning under the map, pauses until the next update cycle
            PlayerControllerBInstance.KillPlayer(Vector3.zero, true, (CauseOfDeath)ModSettings.DeathCause.Value, ModSettings.HeadType.Value);
        }
    }
}