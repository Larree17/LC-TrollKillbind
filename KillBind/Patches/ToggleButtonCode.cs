using HarmonyLib;
using KillBindNS;
using UnityEngine;
using UnityEngine.UI;

namespace KillBind.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class ToggleButtonCode
    {
        public static GameObject MenuToggleButton; //Toggle button that will be found in settings panel
        public static GameObject SettingsUI; //Settings UI of this mod
        public static GameObject ObjectMenu; //Canvas of UI

        public static void OnToggleButtonClick()
        {
            SettingsUI.SetActive(!SettingsUI.activeSelf);
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void ExecOnStart(MenuManager __instance)
        {
            if (__instance.isInitScene) //avoid executing twice
            {
                return;
            }

            ObjectMenu = Object.Instantiate(BasePlugin.Menu);
            ObjectMenu.SetActive(true);
            ObjectMenu.hideFlags = HideFlags.None;

            //Toggle Button
            MenuToggleButton = ObjectMenu.transform.Find("ToggleButton").gameObject;
            MenuToggleButton.SetActive(false);

            //Toggle Button Functionality
            Button ToggleButton = MenuToggleButton.GetComponent<Button>();
            ToggleButton.onClick.AddListener(OnToggleButtonClick);

            //Settings UI
            SettingsUI = ObjectMenu.transform.Find("SettingsUI").gameObject;
            SettingsUI.SetActive(false);
            //remains visible when going out of settings panel


            BasePlugin.mls.LogInfo("Finished menu setup");
        }

        [HarmonyPatch("EnableUIPanel")]
        [HarmonyPostfix]
        public static void OnEnable(GameObject enablePanel)
        {
            if (enablePanel.name == "SettingsPanel")
            {
                MenuToggleButton.SetActive(true);
            }
        }

        [HarmonyPatch("DisableUIPanel")]
        [HarmonyPostfix]
        public static void OnDisable(GameObject enablePanel)
        {
            if (enablePanel.name == "SettingsPanel")
            {
                MenuToggleButton.SetActive(false);
            }
        }
    }

}
