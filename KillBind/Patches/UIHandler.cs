using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KillBind.Patches
{
    public class UIHandler
    {
        //Memory

        private static GameObject mButton;
        private static TextMeshProUGUI mButtonTextComp;

        private static GameObject mMenu;
        private static GameObject mMenuPanel;
        private static GameObject mMenuButton;
        private static GameObject mMenuText;

        private static GameObject mSettingsPanel;
        private static Transform mSettingsPanelTransform;

        private static string textTitle = "Kill Bind Settings";

        //Shared

        private static bool ExistsInMemory = false;

        private static GameObject TalkMode;
        private static GameObject MenuContainer;

        //Scene

        private static GameObject sceneButton;
        private static Transform sceneButtonTransform;

        private static GameObject sceneSettingsPanel;
        private static Transform sceneSettingsPanelTransform;

        private static GameObject sceneMenu;
        private static Transform sceneMenuTransform;
        private static GameObject sceneMenuButton;
        private static GameObject sceneMenuPanel;

        private static Vector3 ButtonLocalPosition = new Vector3(9.0871f, 235.4723f, -4.7728f);
        private static Quaternion zeroRotation = new Quaternion(0, 0, 0, 0);

        /* Copy TalkMode (put in SettingsPanel) (done)
         * Remove SettingsOption (done)
         * Add own button functionality (done)
         * Create UI background from box around mic settings (unsure) (set as last sibling) (put in SettingsPanel) (done)
         * fix image/background not being visible masking the rest of the menu
         * Use dropdowns for HeadType and DeathCause (maybe code from slider mod)
         * voilà
         */

        public static void CreateInMemory()
        {
            if (ExistsInMemory) { return; } //To avoid potential memory leaks

            MenuContainer = GetSettingsPanel();

            mMenu = MenuContainer.transform.Find("MenuNotification").gameObject;
            mSettingsPanel = MenuContainer.transform.Find("SettingsPanel").gameObject;

            //Create button

            mSettingsPanelTransform = mSettingsPanel.transform;
            TalkMode = mSettingsPanelTransform.Find("MicSettings").gameObject.transform.Find("TalkMode").gameObject;

            mButton = GameObject.Instantiate(TalkMode);
            mButton.name = "KillBindButton";
            GameObject.DestroyImmediate(mButton.GetComponent<SettingsOption>());

            mButtonTextComp = mButton.transform.Find("Text").gameObject.transform.GetComponent<TextMeshProUGUI>();
            mButtonTextComp.text = textTitle;
            mButtonTextComp.horizontalAlignment = HorizontalAlignmentOptions.Center;

            //Create menu
            mMenu = GameObject.Instantiate(mMenu);
            mMenu.name = "KillBindMenu";

            mMenuPanel = mMenu.transform.Find("Panel").gameObject;

            mMenuButton = mMenuPanel.transform.Find("ResponseButton").gameObject;

            GameObject.DestroyImmediate(mMenuButton.GetComponent<Button>()); //remove persistent listeners
            mMenuButton.AddComponent<Button>();
            mMenuButton.GetComponent<Button>().transition = Selectable.Transition.Animation; //rebind animation to button
            mMenuButton.transform.localPosition = new Vector3(0, -70f, 1.3f);

            mMenuText = mMenuPanel.transform.Find("NotificationText").gameObject;

            mMenuText.GetComponent<TextMeshProUGUI>().text = textTitle;
            mMenuText.transform.localPosition = new Vector3(0, 80f, -3f);

            //Store all in memory
            Object.DontDestroyOnLoad(mButton);
            Object.DontDestroyOnLoad(mMenu);
            ExistsInMemory = true;
            return;
        }

        public static void CreateInScene()
        {
            if (!ExistsInMemory) { CreateInMemory(); return; } //If it isn't in memory, create one in memory

            MenuContainer = GetSettingsPanel();

            sceneSettingsPanel = MenuContainer.transform.Find("SettingsPanel").gameObject;
            sceneSettingsPanelTransform = sceneSettingsPanel.transform;

            TalkMode = sceneSettingsPanelTransform.Find("MicSettings").gameObject.transform.Find("TalkMode").gameObject;

            //Button code
            sceneButton = Object.Instantiate(mButton);
            sceneButton.SetActive(true);

            sceneButtonTransform = sceneButton.transform;
            sceneButtonTransform.SetParent(sceneSettingsPanelTransform);
            sceneButtonTransform.SetAsFirstSibling(); //Avoid overlapping
            sceneButtonTransform.localPosition = ButtonLocalPosition;
            sceneButtonTransform.rotation = zeroRotation;
            sceneButtonTransform.localScale = Vector3.one;

            //Menu code
            sceneMenu = Object.Instantiate(mMenu);
            sceneMenu.SetActive(false);

            sceneMenuTransform = sceneMenu.transform;
            sceneMenuTransform.SetParent(MenuContainer.transform);
            sceneMenuTransform.SetAsLastSibling(); //Avoid overlapping
            sceneMenuTransform.localPosition = MenuContainer.transform.localPosition;
            sceneMenuTransform.rotation = zeroRotation;
            sceneMenuTransform.localScale = Vector3.one;

            sceneMenuPanel = sceneMenuTransform.Find("Panel").gameObject;

            sceneMenuButton = sceneMenuPanel.transform.Find("ResponseButton").gameObject;

            //Add listeners
            sceneButton.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClicked(true); });
            sceneMenuButton.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClicked(false); });
            return;
        }

        private static void OnButtonClicked(bool setActive)
        {
            MenuManagerPatch.MenuManagerInstance.MenuAudio.PlayOneShot(GameNetworkManager.Instance.buttonSelectSFX);
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