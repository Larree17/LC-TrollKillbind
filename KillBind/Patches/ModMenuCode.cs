using HarmonyLib;
using KillBindNS;
using UnityEngine;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(QuickMenuManager))]
    public class ModMenuCode
    {
        //WHAT
        [HarmonyPatch("OpenQuickMenu")]
        [HarmonyPrefix]
        public static void test()
        {

        }
    }
}
