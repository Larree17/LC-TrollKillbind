using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;
using System.Reflection;
using System.Text;
using System.IO;

namespace KillBindNS
{
    public class KillBind_Inputs : LcInputActions
    {
        [InputAction("<Keyboard>/k", Name = "Suicide", ActionType = InputActionType.Button)]
        public InputAction ActionKillBind { get; set; }
    }

    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    public class Initialise : BaseUnityPlugin
    {

        private const string modGUID = "com.Confusified.KillBind";
        private const string modName = "Kill Bind";
        private const string modVersion = "2.0.0";

        public static readonly KillBind_Inputs InputAction = new KillBind_Inputs();
        private readonly Harmony _harmony = new Harmony(modGUID);
        public static ManualLogSource modLogger;

        private static readonly string configLocation = Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Replace(".", "\\"));
        public static string privateConfigLocation = configLocation + ".private";
        private static ConfigFile modConfig = new ConfigFile(configLocation + ".cfg", false);
        
        public class DefaultModSettings
        {
            public static bool ModEnabled = true;
            public static int DeathCause = 0;
            public static int HeadType = 1;
            public static int ConfigVersion = 1;
        }
        public class ModSettings
        {
            public static ConfigEntry<bool> ModEnabled;
            public static ConfigEntry<int> DeathCause;
            public static ConfigEntry<int> HeadType;
            public static int ConfigVersion;
        }

        

        public void Awake()
        {
            modLogger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            modLogger = Logger;
            ES3.Init();
            
            InitialiseConfig();

            ES3.CacheFile(privateConfigLocation);

            UpdateConfig();
            if (ModSettings.ModEnabled.Value)
            {
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                modLogger.LogInfo($"{modName} {modVersion} has loaded");
                return;
            }
            modLogger.LogInfo($"{modName} {modVersion} did not load, it is disabled in the config");
            return;
        }

        private void InitialiseConfig()
        {
            LoadPrivateConfig();
            ModSettings.ModEnabled = modConfig.Bind<bool>("Mod Settings", "Enabled", true, "Toggle the mod");
            ModSettings.DeathCause = modConfig.Bind<int>("Mod Settings", "DeathType", 0, "Decide what cause of death you will have when pressing your kill bind");
            ModSettings.HeadType = modConfig.Bind<int>("Mod Settings", "HeadType", 1, "Decide what will happen with your head");
        }

        private void UpdateConfig()
        {
            if (ModSettings.ConfigVersion == DefaultModSettings.ConfigVersion) { return; }
            int[] oldInts = { ModSettings.HeadType.Value, ModSettings.DeathCause.Value, ModSettings.ConfigVersion};
            bool oldModEnabled = ModSettings.ModEnabled.Value;

            //Clear files and variables
            modConfig.Clear();
            modConfig = null;
            File.WriteAllText(configLocation + ".cfg", ""); //Clears config (private one is unnecessary because you can't edit it manually
            modConfig = new ConfigFile(configLocation + ".cfg", false);

            ModSettings.ModEnabled = null;
            ModSettings.DeathCause = null;
            ModSettings.HeadType = null;
            ModSettings.ConfigVersion = -999;

            //Set default values
            InitialiseConfig();

            //Restore old values
            ModSettings.HeadType.Value = oldInts[0];
            ModSettings.DeathCause.Value = oldInts[1];
            ModSettings.ModEnabled.Value = oldModEnabled;
            ModSettings.ConfigVersion = DefaultModSettings.ConfigVersion;
            ES3.Save("ConfigVersion", ModSettings.ConfigVersion, privateConfigLocation); //not really needed because it won't change mid-game

            StringBuilder updatedConfigOutput = new StringBuilder("Updated config file from version 0 to 1");
            updatedConfigOutput.Replace("0", oldInts[2].ToString());
            updatedConfigOutput.Replace("1", ModSettings.ConfigVersion.ToString());
            modLogger.LogInfo(updatedConfigOutput);
            return;
        }

        private static void LoadPrivateConfig()
        {
            ModSettings.ConfigVersion = ES3.Load("ConfigVersion", privateConfigLocation, DefaultModSettings.ConfigVersion);
            return;
        }
    }
}
