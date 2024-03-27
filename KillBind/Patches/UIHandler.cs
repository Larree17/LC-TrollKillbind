using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static KillBind.Initialise;

namespace KillBind.Patches
{
    public class UIHandler
    {
        //Variables

        //Memory

        private static GameObject mMenu;
        private static Transform mMenuTransform;

        private static GameObject mDeathDropdown;
        private static Transform mDeathDropdownTransform;
        private static GameObject mDeathDropdownText;

        private static GameObject mHeadDropdown;
        private static Transform mHeadDropdownTransform;
        private static GameObject mHeadDropdownText;

        private static GameObject mSettingsPanel;
        private static Transform mSettingsPanelTransform;

        private static readonly string textTitle = "Kill Bind Settings";
        private static readonly string deathcauseTitle = "Cause of death"; //(Cause of Death enums)
        private static readonly string headtypeTitle = "Ragdoll type:"; //(Normal, No head, Spring head)

        private static readonly Vector2 MenuSize = new Vector2(273.7733f, 96.7017f);
        private static readonly Vector2 DropdownSize = new Vector2(0, 0);

        private static readonly Vector3 DeathDropdownLocalPosition = new Vector3(54.4909f, 7.9495f, -0.9875f); // new Vector3(9.0871f, 235.4723f, -4.7728f);
        private static readonly Vector3 DeathDropdownTextLocalPosition = new Vector3(-135.5511f, 0, 1.3978f);

        private static readonly Vector3 HeadDropdownLocalPosition = new Vector3(54.3413f, -26.5335f, 0.5443f);
        private static readonly Vector3 HeadDropdownTextLocalPosition = new Vector3(-130.3006f, 0, 1.3978f); // slightly different so the ':' of both texts align

        //Shared

        private static bool ExistsInMemory = false;

        private static GameObject MenuContainer;

        //Scene

        private static GameObject sceneSettingsPanel;
        private static Transform sceneSettingsPanelTransform;

        private static GameObject sceneMenu;
        private static Transform sceneMenuTransform;

        private static readonly Vector3 MenuLocalPosition = new Vector3(-158.3053f, 93.1761f, 3.5f); //new Vector3(43.1f, 176, -2.6f);

        private static readonly Vector3 NormalScale = Vector3.one;
        private static readonly Quaternion zeroRotation = new Quaternion(0, 0, 0, 0);

        //Methods

        public static void CreateInMemory()
        {
            if (ExistsInMemory) { return; } //To avoid potential memory leaks

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
            //modLogger.LogInfo("before destroy");
            GameObject.DestroyImmediate(mDeathDropdown.GetComponent<SettingsOption>()); //Remove unneeded component

            mDeathDropdownTransform = mDeathDropdown.transform;
            mDeathDropdownTransform.SetParent(mMenuTransform);
            mDeathDropdownTransform.localPosition = DeathDropdownLocalPosition;

            mDeathDropdownText = mDeathDropdownTransform.Find("Label2").gameObject;
            mDeathDropdownText.GetComponent<TextMeshProUGUI>().text = deathcauseTitle;
            mDeathDropdownText.transform.localPosition = DeathDropdownTextLocalPosition;

            modLogger.LogInfo("deathcause dropdown");
            //Create Ragdoll Type (HeadType) Dropdown
            mHeadDropdown = GameObject.Instantiate(mDeathDropdown);
            mHeadDropdown.name = "HeadTypeDropdown";

            mHeadDropdownTransform = mHeadDropdown.transform;
            mHeadDropdownTransform.SetParent(mMenuTransform);
            mHeadDropdownTransform.localPosition = HeadDropdownLocalPosition;

            mHeadDropdownText = mHeadDropdownTransform.Find("Label2").gameObject;
            mHeadDropdownText.GetComponent<TextMeshProUGUI>().text = headtypeTitle;
            mHeadDropdownText.transform.localPosition = HeadDropdownTextLocalPosition;

            //Store menu in memory
            Object.DontDestroyOnLoad(mMenu);
            ExistsInMemory = true;
            modLogger.LogInfo("Succesfully created and stored the menu in memory");
            return;
        }

        public static void CreateInScene()
        {
            if (!ExistsInMemory) { CreateInMemory(); } //If it isn't in memory, create one in memory and then also create in scene after

            MenuContainer = GetSettingsPanel();

            sceneSettingsPanel = MenuContainer.transform.Find("SettingsPanel").gameObject;
            sceneSettingsPanelTransform = sceneSettingsPanel.transform;

            //Menu code
            //TO DO
            //PixelPerUnitMultiplier = 6.516
            //BoxFrame scale = (0.52, 0.75, 1)
            //BoxFrame localPos = (43.1, 176, -2.6)
            //parent = SettingsPanel

            sceneMenu = Object.Instantiate(mMenu);
            sceneMenu.SetActive(true);

            sceneMenuTransform = sceneMenu.transform;
            sceneMenuTransform.SetParent(sceneSettingsPanelTransform);
            sceneMenuTransform.SetAsFirstSibling();
            sceneMenuTransform.localPosition = MenuLocalPosition;
            sceneMenuTransform.rotation = zeroRotation;
            sceneMenuTransform.localScale = NormalScale;

            modLogger.LogInfo("created menu");

            return;
        }

        private static void OnButtonClicked(bool setActive)
        {
            MenuManagerPatch.MenuManagerInstance.PlayConfirmSFX();
            sceneMenu.SetActive(setActive);
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