using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;
using System.Reflection;
using System.IO;

namespace KillBindNS
{
    public class KillBind : LcInputActions
    {
        [InputAction("<Keyboard>/k", Name = "Suicide", ActionType = InputActionType.Button)]
        public InputAction ExplodeKey { get; set; }
    }

    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    public class BasePlugin : BaseUnityPlugin
    {
        //Mod Defining Vars
        private const string modGUID = "Confusified.KillBind";
        private const string modName = "Kill Bind";
        private const string modVersion = "1.2.2";
        private readonly Harmony _harmony = new Harmony(modGUID);
        public static ManualLogSource mls;

        //Mod Config Vars
        private readonly ConfigFile Configuration = new ConfigFile(Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Replace(".", "\\") + ".cfg"), false);
        public static ConfigEntry<bool> ModEnabled;
        public static ConfigEntry<int> DeathCause;
        public static ConfigEntry<int> HeadType;
        public static ConfigEntry<bool> UseCustomUI;

        //Mod Non-Config Vars
        public static KillBind InputActionInstance = new KillBind();
        public static AssetBundle ModAssetBundle;
        public static GameObject Menu;
        //Mod Functions
        public void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls = Logger;

            //Set Default Config Values
            SetModConfig();

            //Load AssetBundle
            ModAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "confusified_killbind.ui"));
            if (ModAssetBundle == null)
            {
                mls.LogError("Error while trying to load the AssetBundle.");
                return;
            }
            //Load Prefab
            Menu = ModAssetBundle.LoadAsset<GameObject>("Assets/KillBindMod/KillbindUI.prefab");
            if (Menu == null)
            {
                mls.LogError("Error while trying to load the prefab.");
                return;
            }
            DontDestroyOnLoad(Menu);

            //Patch All Code
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            mls.LogInfo($"{modName} {modVersion} has loaded");
        }

        public void SetModConfig()
        {
            ModEnabled = Configuration.Bind<bool>("Global Settings", "Enabled", true, "Toggle the mod");
            DeathCause = Configuration.Bind<int>("Mod Settings", "DeathType", 0, "Decide what cause of death you will have when pressing your kill bind.\n" +
                "There are currently 13 death causes in the game:\n" +
                "(0) Unknown (Default)\n(1) Bludgeoning\n(2) Gravity\n(3) Blast\n(4) Strangulation\n(5) Suffocation\n(6) Mauling\n(7) Gunshots\n(8) Crushing\n(9) Drowning\n(10) Abandoned\n(11) Electrocution\n(12) Kicking");
            HeadType = Configuration.Bind<int>("Mod Settings", "HeadType", 1, "Decide what will happen with your head.\n" +
                "There are currently 3 head types in the game:\n" +
                "(0) Normal - Your head will stay on your body\n" +
                "(1) Decapitate (Default) - Your head will be blown off of your body\n" +
                "(2) Coilhead - Your head will be replaced with a coil"
                );
            UseCustomUI = Configuration.Bind<bool>("Mod Settings", "UseCustomUI", true, "Enable the custom UI for this mod (Disabling will also disable the ability to customise the config in-game");
        }
    }
}
