using HarmonyLib;
using UnityEngine;
using static KillBind.Patches.UIHandler;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatch
    {
        public static MenuManager Instance;
        private static bool HeadCreatedList = false;

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void OnAwake(MenuManager __instance)
        {
            if (__instance.isInitScene) { return; } //To avoid issues
            Instance = __instance;
            CreateInScene();

            if (!HeadCreatedList) //Create list with real values
            {
                StartOfRound tempSOR = new StartOfRound();
                HeadTypeDropdownList.Clear(); //Remove preset values

                foreach (GameObject ragdoll in tempSOR.playerRagdolls)
                {
                    string ragdollName = CleanRagdollName(ragdoll.name);
                    HeadTypeDropdownList.Add(ragdollName);
                }
                HeadCreatedList = true;
                GameObject.Destroy(tempSOR);
                return;
            }
            return;
        }

        private static string CleanRagdollName(string ragdollName)
        {
            Initialise.modLogger.LogInfo(ragdollName);
            if (ragdollName == "PlayerRagdoll") //Normal ragdoll
            {
                ragdollName = "Normal";
            }
            else if (ragdollName == "PlayerRagdollWithComedyMask Variant")
            {
                ragdollName = "Comedy Mask";
            }
            else if (ragdollName == "PlayerRagdollWithTragedyMask Variant")
            {
                ragdollName = "Tragedy Mask";
            }
            else
            {
                ragdollName = ragdollName.Replace("PlayerRagdoll", "");
                ragdollName = ragdollName.Replace(" Variant", "");
            }
            return ragdollName;
        }
    }
}