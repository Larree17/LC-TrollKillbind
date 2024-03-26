using UnityEngine;
using UnityEngine.SceneManagement;
using static KillBindNS.Initialise;

namespace KillBind.Patches
{
    public class UIHandler
    {
        private static bool ExistsInMemory = false;
        private static GameObject mButton;
        private static Transform mButtonTransform;
        private static GameObject mSettingsPanel;
        private static GameObject TalkMode;
        private static Transform TalkModeTransform;
        private static Vector3 positionOffset;

        private static GameObject sceneButton;
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
                mSettingsPanel = GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject.transform.Find("SettingsPanel").gameObject;
            }
            TalkMode = mSettingsPanel.transform.Find("MicSettings").gameObject.transform.Find("TalkMode").gameObject;
            TalkModeTransform = TalkMode.transform;
            mButton = GameObject.Instantiate(TalkMode);
            GameObject.Destroy(mButton.GetComponent<SettingsOption>());
            mButton.name = "KillBindButton";
            mButtonTransform = mButton.transform;
            mButtonTransform.position = TalkModeTransform.position;

            //Code here
            Object.DontDestroyOnLoad(mButton);
            ExistsInMemory = true;
            return;
        }

        public static void CreateInScene()
        {
            if (!ExistsInMemory) { CreateInMemory(); return; } //If it isn't in memory, create one in memory
            if (IsInMainMenu())
            {
                positionOffset = new Vector3(36f, 14f, 0f);
            }
            else
            {
                positionOffset = new Vector3(2f, 0.9625f, 0f);
            }

            sceneButton = Object.Instantiate(mButton);
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