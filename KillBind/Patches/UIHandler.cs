using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static KillBind.Initialise;

namespace KillBind.Patches
{
    public class UIHandler
    {
        //Memory

        private static GameObject mButton;
        private static TextMeshProUGUI mButtonTextComp;

        private static GameObject mMenu;

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

            //Store all in memory
            Object.DontDestroyOnLoad(mButton);
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
            sceneButton.GetComponent<Button>().onClick.AddListener(delegate { onButtonClicked(); });

            //Menu code

            return;
        }

        private static void onButtonClicked()
        {
            modLogger.LogInfo("the button was clicked c:");
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