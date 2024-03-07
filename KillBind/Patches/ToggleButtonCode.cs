using HarmonyLib;
using KillBindNS;
using UnityEngine;
using UnityEngine.UI;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class ToggleButtonCode
    {
        public static GameObject MenuToggle; //Toggle button that will be found in settings panel
        public static GameObject SettingsUI; //Settings UI of this mod

        public static void onToggleButtonClick()
        {
            if (SettingsUI.activeSelf)
            {
                BasePlugin.mls.LogInfo("hid settings ui");
                SettingsUI.SetActive(false);
            }
            else
            {
                BasePlugin.mls.LogInfo("show settings ui");
                SettingsUI.SetActive(true);
            }
        }


        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void onAwake()
        {
            //Toggle Button Creation
            MenuToggle = Object.Instantiate<GameObject>(BasePlugin.Menu1);
            Object.DontDestroyOnLoad(MenuToggle); //Do not destroy on scene change
            MenuToggle.SetActive(false);

            Button ToggleButton = MenuToggle.transform.Find("ToggleButton").GetComponent<Button>();
            ToggleButton.onClick.AddListener(onToggleButtonClick);

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
