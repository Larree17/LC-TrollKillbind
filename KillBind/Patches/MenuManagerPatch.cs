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
    }
}