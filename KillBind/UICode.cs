using HarmonyLib;
using KillBindNS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KillBind.Patches
{
    public class UICode
    {

        public static GameObject MenuToggleButton; //Toggle button that will be found in settings panel
        public static GameObject SettingsUI; //Settings UI of this mod
        public static bool UIActive = false; //State of visibility of UI
        public static GameObject ObjectMenu; //Canvas of UI

        public static void OnToggleButtonClick()
        {
            UIActive = !SettingsUI.activeSelf;
            SettingsUI.SetActive(UIActive);
        }
        public static bool IsUIEnabled()
        {
            return BasePlugin.UseCustomUI.Value;
        }

        public static void OnDDValueChanged(TMP_Dropdown origin)
        {
            if (origin == SettingsUI.transform.Find("Drop_DeathCause").gameObject.GetComponent<TMP_Dropdown>()) //Death Cause Value
            {
                BasePlugin.DeathCause.Value = origin.value;
                BasePlugin.mls.LogInfo($"Saved new CauseOfDeath value: {origin.value}");
            }
            else if (origin == SettingsUI.transform.Find("Drop_HeadType").gameObject.GetComponent<TMP_Dropdown>()) //Head Type Value
            {
                BasePlugin.HeadType.Value = origin.value;
                BasePlugin.mls.LogInfo($"Saved new HeadType value: {origin.value}");
            }
        }

        public static void OnResetButtonClick()
        {
            BasePlugin.DeathCause.Value = (int)BasePlugin.DeathCause.DefaultValue;
            BasePlugin.HeadType.Value = (int)BasePlugin.HeadType.DefaultValue;

            SettingsUI.transform.Find("Drop_HeadType").gameObject.GetComponent<TMP_Dropdown>().value = BasePlugin.HeadType.Value;
            SettingsUI.transform.Find("Drop_DeathCause").gameObject.GetComponent<TMP_Dropdown>().value = BasePlugin.DeathCause.Value;
        }

        public static void CreateUI()
        {
            ObjectMenu = UnityEngine.Object.Instantiate(BasePlugin.Menu);
            ObjectMenu.SetActive(true);
            ObjectMenu.hideFlags = HideFlags.None;

            //Toggle Button
            MenuToggleButton = ObjectMenu.transform.Find("ToggleButton").gameObject;
            MenuToggleButton.SetActive(false);

            //Settings UI
            SettingsUI = ObjectMenu.transform.Find("SettingsUI").gameObject;
            SettingsUI.SetActive(false);

            BasePlugin.mls.LogInfo("Finished menu creation");
        }

        public static void ConfigureUI()
        {
            //Toggle Button Functionality
            Button ToggleButton = MenuToggleButton.GetComponent<Button>();
            ToggleButton.onClick.AddListener(OnToggleButtonClick);

            TMP_Dropdown CoD_Dropdown = SettingsUI.transform.Find("Drop_DeathCause").gameObject.GetComponent<TMP_Dropdown>();
            CoD_Dropdown.ClearOptions();

            List<string> codlist = new List<string>();

            foreach (CauseOfDeath cod in Enum.GetValues(typeof(CauseOfDeath)))
            {
                codlist.Add(cod.ToString());
            }

            CoD_Dropdown.AddOptions(codlist);
            CoD_Dropdown.value = BasePlugin.DeathCause.Value;
            CoD_Dropdown.onValueChanged.AddListener(delegate { OnDDValueChanged(CoD_Dropdown); });

            TMP_Dropdown HT_Dropdown = SettingsUI.transform.Find("Drop_HeadType").gameObject.GetComponent<TMP_Dropdown>();
            HT_Dropdown.ClearOptions();

            List<string> htlist = new List<string> { "Normal", "Decapitated", "Coil" }; //idk if there is a list of the values in the game's code or an easy way to fetch them (this is the temp solution)

            HT_Dropdown.AddOptions(htlist);
            HT_Dropdown.value = BasePlugin.HeadType.Value;
            HT_Dropdown.onValueChanged.AddListener(delegate { OnDDValueChanged(HT_Dropdown); });

            Button ResetButton = SettingsUI.transform.Find("ResetValues").gameObject.GetComponent<Button>();
            ResetButton.onClick.AddListener(OnResetButtonClick);

            BasePlugin.mls.LogInfo("Finished menu configuration");
        }

        [HarmonyPatch(typeof(MenuManager))]

        public class MenuPatches
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void ExecOnStart_Menu() //Create the UI on Start
            {
                CreateUI();
                ConfigureUI();
            }

            [HarmonyPatch("EnableUIPanel")]
            [HarmonyPostfix]
            public static void OnEnable(GameObject enablePanel) //Show toggle when in Settings
            {
                if (enablePanel.name == "SettingsPanel")
                {
                    MenuToggleButton.SetActive(true);
                    SettingsUI.SetActive(UIActive);
                }
            }

            [HarmonyPatch("DisableUIPanel")]
            [HarmonyPostfix]
            public static void OnDisable(GameObject enablePanel) //Hide UI when out of Settings
            {
                if (enablePanel.name == "SettingsPanel")
                {
                    MenuToggleButton.SetActive(false);
                    SettingsUI.SetActive(false);
                }
            }
        }

        [HarmonyPatch(typeof(QuickMenuManager))]
        public class QuickMenuPatches
        {
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            public static void ExecOnStart_QuickMenu() //Create the UI on Start
            {
                CreateUI();
                ConfigureUI();
            }

            [HarmonyPatch("EnableUIPanel")]
            [HarmonyPostfix]
            public static void OnEnable(GameObject enablePanel) //Show toggle when in Settings
            {
                if (enablePanel.name == "SettingsPanel")
                {
                    MenuToggleButton.SetActive(true);
                    SettingsUI.SetActive(UIActive);
                }
            }

            [HarmonyPatch("DisableUIPanel")]
            [HarmonyPostfix]
            public static void OnDisable(GameObject enablePanel) //Hide UI when out of Settings
            {
                if (enablePanel.name == "SettingsPanel")
                {
                    MenuToggleButton.SetActive(false);
                    SettingsUI.SetActive(false);
                }
            }

            [HarmonyPatch("CloseQuickMenuPanels")]
            [HarmonyPostfix]
            public static void OnClose() //Hide UI when out of Settings
            {
                MenuToggleButton.SetActive(false);
                SettingsUI.SetActive(false);
            }
        }
    }
}
