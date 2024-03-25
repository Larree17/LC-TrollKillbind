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
                modLogger.LogInfo("KillBind has been bound");
            }
        }

        public static void OnKeyPress(InputAction.CallbackContext callbackContext)
        {
            //Check if situation is valid
            if (!callbackContext.performed) { modLogger.LogInfo($"1 {callbackContext.action.name}"); return; };
            if (PlayerControllerBInstance != GameNetworkManager.Instance.localPlayerController) { modLogger.LogInfo("2"); return; };
            if (PlayerControllerBInstance.isPlayerDead) { modLogger.LogInfo("3"); return; };
            if (HUDManager.Instance.typingIndicator.enabled || PlayerControllerBInstance.isTypingChat) { modLogger.LogInfo("4"); return; };
            if (UnityEngine.Object.FindObjectOfType<Terminal>().terminalInUse && PlayerControllerBInstance.inTerminalMenu) { modLogger.LogInfo("5"); return; };

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

            CoroutineHelper.Start(Testing());
            
            //new WaitForFixedUpdate(); //To fix the body spawning under the map
            //PlayerControllerBInstance.KillPlayer(Vector3.zero, true, (CauseOfDeath)ModSettings.DeathCause.Value, ModSettings.HeadType.Value);
        }

        private static IEnumerator Testing()
        {
            yield return null; //To fix the body spawning under the map
            PlayerControllerBInstance.KillPlayer(Vector3.zero, true, (CauseOfDeath)ModSettings.DeathCause.Value, ModSettings.HeadType.Value);
        }
    }
}