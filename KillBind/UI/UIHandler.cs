using DunGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static KillBind.Initialise;

namespace KillBind.Patches
{
    public class UIHandler
    {
        //Variables

        //TO DO: merge the code for creating the dropdowns into a method (only for the property changes that both dropdowns have)

        public static int UnsetDeathCause = ModSettings.DeathCause.Value;
        public static int UnsetRagdollType = ModSettings.RagdollType.Value;

        private static GameObject MenuContainer;

        private static bool ExistsInMemory = false;

        private static Array CauseOfDeathValues;

        private static GameObject memoryMenu;
        private static GameObject Menu;
        private static Transform MenuTransform;
        private static readonly Vector3 MenuLocalPosition = new Vector3(-158.3053f, 93.1761f, 3.5f);
        private static readonly Vector2 MenuSize = new Vector2(273.7733f, 96.7017f);

        public static GameObject DeathDropdown;
        public static Transform DeathDropdownTransform;
        private static GameObject DeathDropdownText;
        public static TMP_Dropdown DeathDropdownComponent;
        private static readonly Vector3 DeathDropdownLocalPosition = new Vector3(54.4909f, 7.9495f, -0.9875f);
        private static readonly Vector3 DeathDropdownTextLocalPosition = new Vector3(-113.8745f, 0, 1.3978f);

        public static GameObject HeadDropdown;
        public static Transform HeadDropdownTransform;
        private static GameObject HeadDropdownText;
        public static TMP_Dropdown HeadDropdownComponent;
        private static readonly Vector3 HeadDropdownLocalPosition = new Vector3(54.3413f, -26.5335f, 0.5443f);
        private static readonly Vector3 HeadDropdownTextLocalPosition = new Vector3(-100.7262f, 0, -0.3203f); // slightly different so the ':' of both texts align

        private static List<string> CauseOfDeathDropdownList = new List<string> { };
        private static bool DeathCreatedList = false;
        public static List<string> HeadTypeDropdownList; //Premade list for when you launch the game (will be set automatically after joining a lobby once)

        private static GameObject TitleMenu;
        private static Transform TitleMenuTransform;
        private static TextMeshProUGUI TitleMenuComponent;
        private static readonly Vector3 TitleLocalPosition = new Vector3(-68.7573f, 35.4375f, -1.0543f);

        private static GameObject SettingsPanel;
        private static Transform SettingsPanelTransform;

        private static readonly string textTitle = "KILL BIND SETTINGS"; //all caps to match vanilla
        private static readonly string deathcauseTitle = "Cause of death:"; // Cause of Death enums
        private static readonly string headtypeTitle = "Ragdoll type:"; // Normal, Headburst, Spring, etc

        private static readonly Vector2 DropdownSize = new Vector2(156, 30);
        private static readonly Vector3 NormalScale = Vector3.one;
        private static readonly Quaternion zeroRotation = new Quaternion(0, 0, 0, 0);

        private static AudioSource menuAudio;

        //Methods

        private static void CreateInMemory()
        {
            if (ExistsInMemory) { return; } //To avoid potential memory leaks

            CauseOfDeathValues = Enum.GetValues(typeof(CauseOfDeath)); //Put result in variable for later use
            HeadTypeDropdownList = Initialise.RagdollTypeList;

            MenuContainer = GetSettingsPanel();

            SettingsPanel = MenuContainer.transform.Find("SettingsPanel").gameObject;
            SettingsPanelTransform = SettingsPanel.transform;

            //Create menu

            memoryMenu = SettingsPanel.transform.Find("MicSettings").gameObject.transform.Find("BoxFrame").gameObject;
            memoryMenu = GameObject.Instantiate(memoryMenu);
            memoryMenu.name = "KillBindMenu";
            memoryMenu.GetComponent<RectTransform>().sizeDelta = MenuSize;

            menuAudio = memoryMenu.AddComponent<AudioSource>();

            MenuTransform = memoryMenu.transform;

            //Create Cause of Death Dropdown

            DeathDropdown = SettingsPanelTransform.Find("FullscreenMode").gameObject;
            DeathDropdown = GameObject.Instantiate(DeathDropdown);
            DeathDropdown.name = "DeathCauseDropdown";
            DeathDropdown.GetComponent<RectTransform>().sizeDelta = DropdownSize;

            DeathDropdownComponent = DeathDropdown.GetComponent<TMP_Dropdown>();
            DeathDropdownComponent.ClearOptions(); //Clear values from FullscreenMode
            DeathDropdownComponent.AddOptions(SetDropdownList(true));

            GameObject.DestroyImmediate(DeathDropdown.GetComponent<SettingsOption>()); //Remove unneeded component

            DeathDropdownTransform = DeathDropdown.transform;
            DeathDropdownTransform.SetParent(MenuTransform);
            DeathDropdownTransform.localPosition = DeathDropdownLocalPosition;
            DeathDropdownTransform.rotation = zeroRotation;
            DeathDropdownTransform.localScale = NormalScale;

            DeathDropdownText = DeathDropdownTransform.Find("Label2").gameObject;
            DeathDropdownText.GetComponent<TextMeshProUGUI>().text = deathcauseTitle;
            DeathDropdownText.transform.localPosition = DeathDropdownTextLocalPosition;

            //Create Head Type (HeadType) Dropdown → should be renamed to RagdollType

            HeadDropdown = GameObject.Instantiate(DeathDropdown);
            HeadDropdown.name = "HeadTypeDropdown";

            HeadDropdownTransform = HeadDropdown.transform;
            HeadDropdownTransform.SetParent(MenuTransform);
            HeadDropdownTransform.localPosition = HeadDropdownLocalPosition;
            HeadDropdownTransform.rotation = zeroRotation;
            HeadDropdownTransform.localScale = NormalScale;

            HeadDropdownText = HeadDropdownTransform.Find("Label2").gameObject;
            HeadDropdownText.GetComponent<TextMeshProUGUI>().text = headtypeTitle;
            HeadDropdownText.transform.localPosition = HeadDropdownTextLocalPosition;

            //Create Menu Title

            TitleMenu = SettingsPanelTransform.Find("Headers").gameObject.transform.Find("Display").gameObject;
            TitleMenu = GameObject.Instantiate(TitleMenu);
            TitleMenu.name = "Title";

            TitleMenuTransform = TitleMenu.transform;
            TitleMenuTransform.SetParent(MenuTransform);

            TitleMenuComponent = TitleMenu.GetComponent<TextMeshProUGUI>();
            TitleMenuComponent.enableWordWrapping = false;
            TitleMenuComponent.fontSize = 24;
            TitleMenuComponent.text = textTitle;

            //Store menu in memory

            UnityEngine.Object.DontDestroyOnLoad(memoryMenu);
            ExistsInMemory = true;
            modLogger.LogInfo("Succesfully created and stored the menu in memory");
            return;
        }

        public static void CreateInScene()
        {
            if (!ExistsInMemory) { CreateInMemory(); } //If it isn't in memory, create one in memory, then continue creating it in scene

            MenuContainer = GetSettingsPanel();

            SettingsPanel = MenuContainer.transform.Find("SettingsPanel").gameObject;
            SettingsPanelTransform = SettingsPanel.transform;

            Menu = GameObject.Instantiate(memoryMenu);
            Menu.SetActive(true);

            menuAudio = Menu.GetComponent<AudioSource>();

            MenuTransform = Menu.transform;
            MenuTransform.SetParent(SettingsPanelTransform);
            MenuTransform.localPosition = MenuLocalPosition;
            MenuTransform.rotation = zeroRotation;
            MenuTransform.localScale = NormalScale;
            MenuTransform.SetAsFirstSibling();

            DeathDropdownComponent = MenuTransform.Find(DeathDropdown.name).gameObject.GetComponent<TMP_Dropdown>();
            DeathDropdownComponent.SetValueWithoutNotify(ModSettings.DeathCause.Value);

            HeadDropdownComponent = MenuTransform.Find(HeadDropdown.name).gameObject.GetComponent<TMP_Dropdown>();
            HeadDropdownComponent.ClearOptions(); //Clear values from DeathCauseDropdown
            HeadDropdownComponent.AddOptions(SetDropdownList(false));
            HeadDropdownComponent.SetValueWithoutNotify(ModSettings.RagdollType.Value);

            TitleMenuTransform = MenuTransform.Find("Title").gameObject.transform; //do this when in scene
            TitleMenuTransform.localPosition = TitleLocalPosition;
            TitleMenuTransform.rotation = zeroRotation;
            TitleMenuTransform.localScale = NormalScale * 0.8144f; //essentially default Display scale

            //add listeners to dropdowns for value changes

            DeathDropdownComponent.onValueChanged.AddListener(delegate { ValueUpdateDropdown(DeathDropdownComponent); });
            HeadDropdownComponent.onValueChanged.AddListener(delegate { ValueUpdateDropdown(HeadDropdownComponent); });

            modLogger.LogInfo("Created menu in scene");
            return;
        }

        private static List<string> SetDropdownList(bool isCauseOfDeathDropdown)
        {
            if (isCauseOfDeathDropdown && !DeathCreatedList)
            {
                foreach (CauseOfDeath enumValue in CauseOfDeathValues)
                {
                    CauseOfDeathDropdownList.Add(enumValue.ToString());
                }
                DeathCreatedList = true;
                return CauseOfDeathDropdownList;
            }
            else if (isCauseOfDeathDropdown && DeathCreatedList)
            {
                return CauseOfDeathDropdownList;
            }

            return HeadTypeDropdownList;
        }

        private static void ValueUpdateDropdown(TMP_Dropdown targetDropdownComponent)
        {
            CoroutineHelper.Start(QuickMenuManagerPlaySound()); //Play sound on change
            SetChangesNotAppliedTextVisible();

            if (targetDropdownComponent == DeathDropdownComponent) //if the dropdown is for Cause of Death
            {
                UnsetDeathCause = targetDropdownComponent.value;
                return;
            }

            UnsetRagdollType = targetDropdownComponent.value;
            return;
        }

        private static void SetChangesNotAppliedTextVisible()
        {
            TextMeshProUGUI settingsBackButton;
            string discardText;

            if (IsInMainMenu())
            {
                MenuManagerPatch.Instance.changesNotAppliedText.enabled = true;
                settingsBackButton = MenuManagerPatch.Instance.settingsBackButton;
                discardText = "DISCARD";
            }
            else
            {
                QuickMenuManagerPatch.Instance.changesNotAppliedText.enabled = true;
                settingsBackButton = QuickMenuManagerPatch.Instance.settingsBackButton;
                discardText = "Discard changes";
            }

            IngamePlayerSettings.Instance.changesNotApplied = true;
            settingsBackButton.text = discardText;
            return;
        }

        private static IEnumerator QuickMenuManagerPlaySound()
        {
            yield return null; //Wait for next frame
            menuAudio.PlayOneShot(GameNetworkManager.Instance.buttonTuneSFX);
        }

        private static GameObject GetSettingsPanel()
        {
            if (IsInMainMenu())
            {
                return GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject;
            }

            return GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject;
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