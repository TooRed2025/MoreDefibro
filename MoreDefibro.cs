using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace MoreDefibro
{
    [BepInPlugin("TooRed.MoreDefibro", "MoreDefibro", "1.0.3")]
    public class MoreDefibro : BaseUnityPlugin
    {
        internal static MoreDefibro Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance.Log;
        private ManualLogSource Log => base.Logger;
        internal Harmony Harmony { get; set; }
        public static ConfigEntry<bool> EnableMod { get; private set; }
        public static ConfigEntry<int> MinPrice { get; private set; }
        public static ConfigEntry<int> MaxPrice { get; private set; }
        public static ConfigEntry<int> MinSpawnCount { get; private set; }
        public static ConfigEntry<int> MaxSpawnCount { get; private set; }
        public static ConfigEntry<bool> DebugMode { get; private set; }

        private void Awake()
        {
            Instance = this;

            // Prevent the plugin from being deleted
            this.gameObject.transform.parent = null;
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;
            MinSpawnCount = Config.Bind("Settings", "MinSpawnCount", 1, new ConfigDescription("最小生成数量", new AcceptableValueRange<int>(0, 10)));
            MaxSpawnCount = Config.Bind("Settings", "MaxSpawnCount", 4, new ConfigDescription("最大生成数量", new AcceptableValueRange<int>(1, 10)));
            MinPrice = Config.Bind("Settings", "MinPrice", 2, new ConfigDescription("最低价格(千)", new AcceptableValueRange<int>(1, 30)));
            MaxPrice = Config.Bind("Settings", "MaxPrice", 10, new ConfigDescription("最高价格(千)", new AcceptableValueRange<int>(10, 60)));
            EnableMod = Config.Bind("Settings", "EnableMod", true, new ConfigDescription("启用Mod"));
            DebugMode = Config.Bind("Settings", "DebugMode", false, new ConfigDescription("启用调试日志"));

            EnableMod.SettingChanged += OnEnableModChanged;
            Patch();

            Log.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        }

        internal void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
        }

        internal void Unpatch()
        {
            Harmony?.UnpatchSelf();
        }

        internal static void DeBug(string message)
        {
            if (DebugMode.Value) Logger.LogInfo($"{message}");
        }

        private void OnEnableModChanged(object sender, System.EventArgs e)
        {
            if (EnableMod.Value)
            {
                Patch();
                Log.LogInfo("Mod enabled.");
            }
            else
            {
                Unpatch();
                Log.LogInfo("Mod disabled.");
            }
        }
    }
}