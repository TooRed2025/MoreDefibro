using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Photon.Pun;

namespace MoreDefibro
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MoreDefibro : BaseUnityPlugin
    {
        private Harmony _harmony;
        internal static ManualLogSource Log;

        private static ConfigEntry<int> _minSpawnCount;
        private static ConfigEntry<int> _maxSpawnCount;
        private static ConfigEntry<int> _minPrice;
        private static ConfigEntry<int> _maxPrice;

        private void Awake()
        {
            Log = Logger;

            _minSpawnCount = Config.Bind("Settings", "MinSpawnCount", 1, "最小数量");
            _maxSpawnCount = Config.Bind("Settings", "MaxSpawnCount", 4, "最大数量");
            _minPrice = Config.Bind("Settings", "MinPrice", 2000, "最低价格");
            _maxPrice = Config.Bind("Settings", "MaxPrice", 8000, "最高价格");

            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            Log.LogInfo($"加载完毕");
        }

        [HarmonyPatch(typeof(ShopManager), "ShopInitialize")]
        static class ShopManager_ShopInitialize_Patch
        {
            static void Postfix(ShopManager __instance)
            {
                if (!SemiFunc.RunIsShop()) return;
                if (!SemiFunc.IsMasterClientOrSingleplayer()) return;

                if (_minSpawnCount.Value < 0 || _maxSpawnCount.Value < 0)
                {
                    Log.LogWarning("生成数量不能为负数");
                    return;
                }

                if (_minPrice.Value <= 1000 || _maxPrice.Value <= 1000)
                {
                    Log.LogWarning("价格不得小于等于1000");
                    return;
                }

                if (!StatsManager.instance.itemDictionary.TryGetValue("Item ReviveItem", out Item defibroItem))
                {
                    return;
                }

                Transform extractionPoint = __instance.extractionPoint;
                if (extractionPoint == null)
                {
                    return;
                }

                Value originalValue = defibroItem.value;
                float savedValueMin = originalValue.valueMin;
                float savedValueMax = originalValue.valueMax;

                defibroItem.value.valueMin = _minPrice.Value / 4f;
                defibroItem.value.valueMax = _maxPrice.Value / 4f;

                Vector3 forward = Vector3.ProjectOnPlane(extractionPoint.forward, Vector3.up).normalized;
                Vector3 right = Vector3.Cross(forward, Vector3.up);

                bool isMultiplayer = SemiFunc.IsMultiplayer();
                int spawnCount = Random.Range(_minSpawnCount.Value, _maxSpawnCount.Value + 1);
                if (spawnCount <= 0)
                {
                    originalValue.valueMin = savedValueMin;
                    originalValue.valueMax = savedValueMax;
                    Log.LogInfo("生成数量为0直接跳过");
                    return;
                }

                Log.LogInfo($"正在生成{spawnCount}个除颤器...");

                for (int i = 0; i < spawnCount; i++)
                {
                    float distance = 2f + (i % 2) * 1.5f;
                    float offsetX = ((i / 2) % 2 == 0 ? -1 : 1) * ((i / 2) + 1);

                    Vector3 position = extractionPoint.position + forward * distance + right * offsetX;
                    position.y = 0.5f;

                    if (defibroItem.prefab?.Prefab != null)
                    {
                        if (isMultiplayer)
                        {
                            PhotonNetwork.InstantiateRoomObject(defibroItem.prefab.ResourcePath, position, Quaternion.identity, 0, null);
                        }
                        else
                        {
                            Object.Instantiate(defibroItem.prefab.Prefab, position, Quaternion.identity);
                        }
                    }
                }

                originalValue.valueMin = savedValueMin;
                originalValue.valueMax = savedValueMax;

                Log.LogInfo($"生成完毕");
            }
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "toored.moredefibro";
        public const string PLUGIN_NAME = "MoreDefibro";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}