using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace MoreDefibro
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MoreDefibro : BaseUnityPlugin
    {
        private Harmony _harmony;
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            SetConfig.config(Config);

            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            Log.LogInfo($"加载完毕");
        }

        [HarmonyPatch(typeof(ShopManager), "ShopInitialize")]
        static class ShopInitializePatch
        {
            static void Postfix(ShopManager __instance)
            {
                Manager.SpawnItem(__instance);
            }
        }

        [HarmonyPatch(typeof(ItemAttributes), "GetValue")]
        static class GetValuePatch
        {
            static void Prefix(ItemAttributes __instance)
            {
                Manager.GetValue( __instance );
            }
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "toored.moredefibro";
        public const string PLUGIN_NAME = "MoreDefibro";
        public const string PLUGIN_VERSION = "1.0.3";
    }
}