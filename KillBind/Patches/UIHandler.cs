using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static KillBind.Initialise;

namespace KillBind.Patches
{
    public class UIHandler
    {
        //Variables

        //TO DO: MERGE ALL MEMORY AND SCENE VARIABLES INTO ONE
        //and maybe the code for creating the dropdowns into a method (only for the property changes that both dropdowns have)

        //Memory

        private static GameObject mMenu;
        private static Transform mMenuTransform;

        private static GameObject mDeathDropdown;
        private static Transform mDeathDropdownTransform;
        private static GameObject mDeathDropdownText;
        private static TMP_Dropdown DeathDropdownComponent;
        private static readonly Vector3 DeathDropdownLocalPosition = new Vector3(54.4909f, 7.9495f, -0.9875f);
        private static readonly Vector3 DeathDropdownTextLocalPosition = new Vector3(-113.8745f, 0, 1.3978f);

        private static GameObject mHeadDropdown;
        private static Transform mHeadDropdownTransform;
        private static GameObject mHeadDropdownText;
        private static TMP_Dropdown HeadDropdownComponent;
        private static readonly Vector3 HeadDropdownLocalPosition = new Vector3(54.3413f, -26.5335f, 0.5443f);
        private static readonly Vector3 HeadDropdownTextLocalPosition = new Vector3(-100.7262f, 0, -0.3203f); // slightly different so the ':' of both texts align

        private static GameObject TitleMenu;
        private static Transform TitleMenuTransform;
        private static TextMeshProUGUI TitleMenuComponent;
        private static readonly Vector3 TitleLocalPosition = new Vector3(-56.2559f, 34.7314f, -1.0344f);

        private static GameObject mSettingsPanel;
        private static Transform mSettingsPanelTransform;

        private static readonly string textTitle = "KILL BIND SETTINGS"; //all caps to match vanilla
        private static readonly string deathcauseTitle = "Cause of death:"; // Cause of Death enums
        private static readonly string headtypeTitle = "Type of head:"; // Normal, Decapitated, Spring head

        private static readonly Vector2 MenuSize = new Vector2(273.7733f, 96.7017f);
        private static readonly Vector2 DropdownSize = new Vector2(156, 30);

        //Shared

        private static bool ExistsInMemory = false;

        private static GameObject MenuContainer;
        private static Array CauseOfDeathValues;

        //Scene

        private static GameObject sceneSettingsPanel;
        private static Transform sceneSettingsPanelTransform;

        private static GameObject sceneMenu;
        private static Transform sceneMenuTransform;

        private static readonly Vector3 MenuLocalPosition = new Vector3(-158.3053f, 93.1761f, 3.5f); //new Vector3(43.1f, 176, -2.6f);

        private static readonly Vector3 NormalScale = Vector3.one;
        private static readonly Quaternion zeroRotation = new Quaternion(0, 0, 0, 0);

        //Methods

        private static void CreateInMemory()
        {
            if (ExistsInMemory) { return; } //To avoid potential memory leaks

            CauseOfDeathValues = Enum.GetValues(typeof(CauseOfDeath)); //Put result in variable for later use

            MenuContainer = GetSettingsPanel();

            mSettingsPanel = MenuContainer.transform.Find("SettingsPanel").gameObject;
            mSettingsPanelTransform = mSettingsPanel.transform;

            //Create menu

            mMenu = mSettingsPanel.transform.Find("MicSettings").gameObject.transform.Find("BoxFrame").gameObject;
            mMenu = GameObject.Instantiate(mMenu);
            mMenu.name = "KillBindMenu";
            mMenu.GetComponent<RectTransform>().sizeDelta = MenuSize;

            mMenuTransform = mMenu.transform;

            modLogger.LogInfo("menu");
            //Create Cause of Death Dropdown

            mDeathDropdown = mSettingsPanelTransform.Find("FullscreenMode").gameObject;
            mDeathDropdown = GameObject.Instantiate(mDeathDropdown);
            mDeathDropdown.name = "DeathCauseDropdown";
            mDeathDropdown.GetComponent<RectTransform>().sizeDelta = DropdownSize;

            DeathDropdownComponent = mDeathDropdown.GetComponent<TMP_Dropdown>();
            DeathDropdownComponent.ClearOptions(); //Clear values from FullscreenMode
            DeathDropdownComponent.AddOptions(SetDropdownList(true));

            GameObject.DestroyImmediate(mDeathDropdown.GetComponent<SettingsOption>()); //Remove unneeded component

            mDeathDropdownTransform = mDeathDropdown.transform;
            mDeathDropdownTransform.SetParent(mMenuTransform);
            mDeathDropdownTransform.localPosition = DeathDropdownLocalPosition;
            mDeathDropdownTransform.rotation = zeroRotation;
            mDeathDropdownTransform.localScale = NormalScale;

            mDeathDropdownText = mDeathDropdownTransform.Find("Label2").gameObject;
            mDeathDropdownText.GetComponent<TextMeshProUGUI>().text = deathcauseTitle;
            mDeathDropdownText.transform.localPosition = DeathDropdownTextLocalPosition;

            modLogger.LogInfo("deathcause dropdown");
            //Create Head Type (HeadType) Dropdown

            mHeadDropdown = GameObject.Instantiate(mDeathDropdown);
            mHeadDropdown.name = "HeadTypeDropdown";

            HeadDropdownComponent = mHeadDropdown.GetComponent<TMP_Dropdown>();
            HeadDropdownComponent.ClearOptions(); //Clear values from DeathCauseDropdown
            HeadDropdownComponent.AddOptions(SetDropdownList(false));

            mHeadDropdownTransform = mHeadDropdown.transform;
            mHeadDropdownTransform.SetParent(mMenuTransform);
            mHeadDropdownTransform.localPosition = HeadDropdownLocalPosition;
            mHeadDropdownTransform.rotation = zeroRotation;
            mHeadDropdownTransform.localScale = NormalScale;

            mHeadDropdownText = mHeadDropdownTransform.Find("Label2").gameObject;
            mHeadDropdownText.GetComponent<TextMeshProUGUI>().text = headtypeTitle;
            mHeadDropdownText.transform.localPosition = HeadDropdownTextLocalPosition;

            modLogger.LogInfo("headtype dropdown");
            //Create Menu Title

            TitleMenu = mSettingsPanelTransform.Find("Headers").gameObject.transform.Find("Display").gameObject;
            TitleMenu = GameObject.Instantiate(TitleMenu);
            TitleMenu.name = "Title";

            TitleMenuTransform = TitleMenu.transform;
            TitleMenuTransform.SetParent(mMenuTransform);

            TitleMenuComponent = TitleMenu.GetComponent<TextMeshProUGUI>();
            TitleMenuComponent.enableWordWrapping = false;
            TitleMenuComponent.fontSize = 24;
            TitleMenuComponent.text = textTitle;

            modLogger.LogInfo("menu title");
            //Store menu in memory

            UnityEngine.Object.DontDestroyOnLoad(mMenu);
            ExistsInMemory = true;
            modLogger.LogInfo("Succesfully created and stored the menu in memory");
            return;
        }

        public static void CreateInScene()
        {
            if (!ExistsInMemory) { CreateInMemory(); } //If it isn't in memory, create one in memory, then continue creating it in scene

            MenuContainer = GetSettingsPanel();

            sceneSettingsPanel = MenuContainer.transform.Find("SettingsPanel").gameObject;
            sceneSettingsPanelTransform = sceneSettingsPanel.transform;

            sceneMenu = GameObject.Instantiate(mMenu);
            sceneMenu.SetActive(true);

            sceneMenuTransform = sceneMenu.transform;
            sceneMenuTransform.SetParent(sceneSettingsPanelTransform);
            sceneMenuTransform.localPosition = MenuLocalPosition;
            sceneMenuTransform.rotation = zeroRotation;
            sceneMenuTransform.localScale = NormalScale;
            sceneMenuTransform.SetAsFirstSibling();

            //set start value for dropdowns

            TitleMenuTransform = sceneMenuTransform.Find("Title").gameObject.transform; //do this when in scene
            TitleMenuTransform.localPosition = TitleLocalPosition;
            TitleMenuTransform.rotation = zeroRotation;
            TitleMenuTransform.localScale = NormalScale * 0.8144f; //essentially default Display scale

            modLogger.LogInfo("Created menu in scene");
            //add listeners to dropdowns for value changes
            return;
        }

        private static List<string> SetDropdownList(bool isCauseOfDeathDropdown)
        {
            List<string> dropdownList = new List<string> { "Normal", "Decapitated", "Spring head" };

            if (isCauseOfDeathDropdown)
            {
                dropdownList.Clear(); //Remove preset values

                foreach (CauseOfDeath enumValue in CauseOfDeathValues)
                {
                    dropdownList.Add(enumValue.ToString());
                }
                return dropdownList;
            }
            else //if not Cause of Death, return preset list (list for Head Type)
            {
                return dropdownList;
            }
        }

        private static GameObject GetSettingsPanel()
        {
            if (IsInMainMenu())
            {
                return GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject;
            }
            else
            {
                return GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject;
            }
        }

        private static bool IsInMainMenu()
        {
            if (SceneManager.GetSceneByName("SampleSceneRelay").isLoaded)
            {
                return false;
            }
            return true;
        }
    }
}