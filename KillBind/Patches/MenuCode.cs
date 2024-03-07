using HarmonyLib;
using KillBindNS;
using UnityEngine;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuCode
    {
        public static GameObject MenuToggle; //Toggle button that will be found in settings panel
        public static GameObject SettingsUI; //Settings UI of this mod
        //WHAT
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void onAwake()
        {
            //Toggle Button Creation
            MenuToggle = Object.Instantiate<GameObject>(BasePlugin.Menu1);
            Object.DontDestroyOnLoad(MenuToggle); //Do not destroy on scene change
            MenuToggle.SetActive(false);

            //Settings UI Creation
            SettingsUI = Object.Instantiate<GameObject>(BasePlugin.Menu2);
            Object.DontDestroyOnLoad(SettingsUI); //Do not destroy on scene change
            SettingsUI.SetActive(false);
        }

        [HarmonyPatch("EnableUIPanel")]
        [HarmonyPostfix]
        public static void onEnable(GameObject enablePanel)
        {
            if (enablePanel.name == "SettingsPanel")
            {
                MenuToggle.SetActive(true);
            }
        }

        [HarmonyPatch("DisableUIPanel")]
        [HarmonyPostfix]
        public static void onDisable(GameObject enablePanel)
        {
            if (enablePanel.name == "SettingsPanel")
            {
                MenuToggle.SetActive(false);
            }
        }
    }
}
