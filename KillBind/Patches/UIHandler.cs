using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static KillBind.Initialise;

namespace KillBind.Patches
{
    public class UIHandler
    {
        //Memory

        private static GameObject mButton;
        private static Transform mButtonTransform;
        private static TextMeshProUGUI mButtonTextComp;

        private static GameObject mSettingsPanel;

        //Both

        private static bool ExistsInMemory = false;

        private static GameObject TalkMode;
        private static Transform TalkModeTransform;

        //Scene

        private static GameObject sceneButton;
        private static Transform sceneButtonTransform;
        private static GameObject sceneSettingsPanel;

        private static Vector3 positionOffset;

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
                mSettingsPanel = GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject.transform.Find("SettingsPanel").gameObject;
            }
            else
            {
                mSettingsPanel = GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject.transform.Find("SettingsPanel").gameObject; //Shouldn't be needed
            }
            //Create button
            TalkMode = mSettingsPanel.transform.Find("MicSettings").gameObject.transform.Find("TalkMode").gameObject;
            TalkModeTransform = TalkMode.transform;

            mButton = GameObject.Instantiate(TalkMode);
            mButton.name = "KillBindButton";
            GameObject.Destroy(mButton.GetComponent<SettingsOption>());

            mButtonTransform = mButton.transform;
            mButtonTransform.position = TalkModeTransform.position;

            mButtonTextComp = mButtonTransform.Find("Text").gameObject.transform.GetComponent<TextMeshProUGUI>();
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

            sceneButton = Object.Instantiate(mButton);
            sceneButtonTransform = sceneButton.transform;

            if (IsInMainMenu())
            {
                sceneSettingsPanel = GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject.transform.Find("SettingsPanel").gameObject;
                positionOffset = new Vector3(36f, 14f, 0f);
            }
            else
            {
                sceneSettingsPanel = GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject.transform.Find("SettingsPanel").gameObject;
                positionOffset = new Vector3(2f, 0.9625f, 0f);
            }

            TalkMode = sceneSettingsPanel.transform.Find("MicSettings").gameObject.transform.Find("TalkMode").gameObject;
            TalkModeTransform = TalkMode.transform;

            sceneButtonTransform.SetParent(sceneSettingsPanel.transform);
            sceneButtonTransform.SetAsFirstSibling(); //Avoid overlapping
            sceneButtonTransform.position = TalkModeTransform.position + positionOffset;
            sceneButtonTransform.localScale = new Vector3(1f, 1f, 1f);

            return;
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