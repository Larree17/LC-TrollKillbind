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
        private static Transform MicSettingsTransform;

        //Scene

        private static GameObject sceneButton;
        private static Transform sceneButtonTransform;
        private static GameObject sceneSettingsPanel;

        private static Vector3 MainMenuPositionOffset = new Vector3(53.369f, 7.19f, 0);
        private static Vector3 QuickMenuOffsetScale = new Vector3(0.05714f, 0.06875f, 0); //for easier changes (maybe do it better sometime soon, with screen res n stuff)
        private static Vector3 QuickMenuPositionOffset = Vector3.Scale(MainMenuPositionOffset, QuickMenuOffsetScale);
        private static Vector3 currentOffset;

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
            MicSettingsTransform = mSettingsPanel.transform.Find("MicSettings").gameObject.transform;
            TalkMode = MicSettingsTransform.Find("TalkMode").gameObject;
            TalkModeTransform = TalkMode.transform;

            mButton = GameObject.Instantiate(TalkMode);
            mButton.name = "KillBindButton";
            GameObject.DestroyImmediate(mButton.GetComponent<SettingsOption>());

            mButtonTransform = mButton.transform;

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
                //1051.631 627.0407 -35.4911 (MicSettings)
                //1105.000 634.1907 -35.4911 (Position I want the button to be)
                currentOffset = MainMenuPositionOffset;
                sceneSettingsPanel = GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject.transform.Find("SettingsPanel").gameObject;
            }
            else
            {
                sceneSettingsPanel = GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject.transform.Find("SettingsPanel").gameObject;
                currentOffset = QuickMenuPositionOffset;
            }
            MicSettingsTransform = mSettingsPanel.transform.Find("MicSettings").gameObject.transform;
            TalkMode = MicSettingsTransform.Find("TalkMode").gameObject;
            TalkModeTransform = TalkMode.transform;

            sceneButtonTransform.SetParent(sceneSettingsPanel.transform);
            sceneButtonTransform.SetAsFirstSibling(); //Avoid overlapping
            sceneButtonTransform.position = MicSettingsTransform.position + currentOffset;
            sceneButtonTransform.rotation = new Quaternion(0, 0, 0, 0);
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