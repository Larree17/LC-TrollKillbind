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

        //Both

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

        private static Vector3 ButtonLocalPosition = new Vector3(9.0871f, 235.4723f, -4.7728f);

        /* Copy TalkMode (put in SettingsPanel)
         * Remove SettingsOption
         * Add own button functionality
         * Create UI background from box around mic settings (unsure) (set as last sibling) (put in SettingsPanel)
         * Use dropdowns for HeadType and DeathCause (maybe code from slider mod)
         * voilà
         */

        public static void CreateInMemory()
        {
            if (ExistsInMemory) { return; } //To avoid potential memory leaks
            if (IsInMainMenu())
            {
                MenuContainer = GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject;
            }
            else
            {
                MenuContainer = GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject;  //Shouldn't be needed
            }

            mMenu = MenuContainer.transform.Find("MenuNotification").gameObject;
            mSettingsPanel = MenuContainer.transform.Find("SettingsPanel").gameObject;

            //Create button

            mSettingsPanelTransform = mSettingsPanel.transform;
            TalkMode = mSettingsPanelTransform.Find("MicSettings").gameObject.transform.Find("TalkMode").gameObject;

            mButton = GameObject.Instantiate(TalkMode);
            mButton.name = "KillBindButton";
            GameObject.DestroyImmediate(mButton.GetComponent<SettingsOption>());

            mButtonTextComp = mButton.transform.Find("Text").gameObject.transform.GetComponent<TextMeshProUGUI>();
            mButtonTextComp.text = "Kill Bind Settings";
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

            mMenuText.GetComponent<TextMeshProUGUI>().text = "Kill Bind Settings";
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
            if (IsInMainMenu())
            {
                MenuContainer = GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject;
            }
            else
            {
                MenuContainer = GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject;
            }

            sceneSettingsPanel = MenuContainer.transform.Find("SettingsPanel").gameObject;
            sceneSettingsPanelTransform = sceneSettingsPanel.transform;

            TalkMode = sceneSettingsPanelTransform.Find("MicSettings").gameObject.transform.Find("TalkMode").gameObject;

            //Button code
            sceneButton = Object.Instantiate(mButton);

            sceneButtonTransform = sceneButton.transform;
            sceneButtonTransform.SetParent(sceneSettingsPanelTransform);
            sceneButtonTransform.SetAsFirstSibling(); //Avoid overlapping
            sceneButtonTransform.localPosition = ButtonLocalPosition;
            sceneButtonTransform.rotation = new Quaternion(0, 0, 0, 0);
            sceneButtonTransform.localScale = Vector3.one;
            //modLogger.LogInfo("Created button");

            //Menu code
            sceneMenu = Object.Instantiate(mMenu);
            sceneMenu.SetActive(false);

            sceneMenuTransform = sceneMenu.transform;
            sceneMenuTransform.SetParent(sceneSettingsPanelTransform.parent);
            sceneMenuTransform.SetAsLastSibling(); //Avoid overlapping
            sceneMenuTransform.localPosition = sceneSettingsPanelTransform.parent.transform.localPosition;
            sceneMenuTransform.rotation = new Quaternion(0, 0, 0, 0);
            sceneMenuTransform.localScale = Vector3.one;

            sceneMenuButton = sceneMenuTransform.Find("Panel").gameObject.transform.Find("ResponseButton").gameObject;
            //modLogger.LogInfo("Created menu");

            //Add listeners
            sceneButton.GetComponent<Button>().onClick.AddListener(delegate { onButtonClicked(true); });
            sceneMenuButton.GetComponent<Button>().onClick.AddListener(delegate { onButtonClicked(false); });
            //modLogger.LogInfo("Added all listeners");
            return;
        }

        private static void onButtonClicked(bool setActive)
        {
            MenuManagerPatch.MenuManagerInstance.PlayConfirmSFX();
            sceneMenu.SetActive(setActive);
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