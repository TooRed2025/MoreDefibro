using HarmonyLib;
using Photon.Pun;
using System.Reflection;
using UnityEngine;

namespace MoreDefibro
{
    internal class Manager
    {
        private static readonly FieldInfo _itemValueMin = AccessTools.Field(typeof(ItemAttributes), "itemValueMin");
        private static readonly FieldInfo _itemValueMax = AccessTools.Field(typeof(ItemAttributes), "itemValueMax");
        public static void GetValue(ItemAttributes __instance) 
        {
            if (__instance.item != null && __instance.item.name == "Item ReviveItem")
            {
                int minP = SetConfig.minPrice.Value;
                int maxP = SetConfig.maxPrice.Value;

                if (minP <= 0 || maxP <= 0 || maxP < minP)
                {
                    MoreDefibro.Log.LogWarning($"价格配置无效(min:{minP}, max:{maxP})，使用默认值");
                    minP = 2;
                    maxP = 10;
                }

                float minVal = (minP * 1000f) / 4f;
                float maxVal = (maxP * 1000f) / 4f;
                _itemValueMin.SetValue(__instance, minVal);
                _itemValueMax.SetValue(__instance, maxVal);
            }
        }

        public static void SpawnItem(ShopManager __instance) 
        {
            if (!SemiFunc.RunIsShop()) return;
            if (!SemiFunc.IsMasterClientOrSingleplayer()) return;

            int minSpawn = SetConfig.minSpawnCount.Value;
            int maxSpawn = SetConfig.maxSpawnCount.Value;

            if (minSpawn < 0 || maxSpawn < 0 || maxSpawn < minSpawn)
            {
                MoreDefibro.Log.LogWarning($"生成数量配置无效(min:{minSpawn}, max:{maxSpawn})，使用默认值");
                minSpawn = 1;
                maxSpawn = 4;
            }

            int spawnCount = Random.Range(minSpawn, maxSpawn + 1);
            if (spawnCount <= 0)
            {
                MoreDefibro.Log.LogInfo("生成数量为0直接跳过");
                return;
            }

            if (!StatsManager.instance.itemDictionary.TryGetValue("Item ReviveItem", out Item defibroItem))
            {
                return;
            }

            Transform extractionPoint = __instance.extractionPoint;

            MoreDefibro.Log.LogInfo($"正在生成{spawnCount}个除颤器...");

            Vector3 forward = Vector3.ProjectOnPlane(extractionPoint.forward, Vector3.up).normalized;
            Vector3 right = Vector3.Cross(forward, Vector3.up);

            bool isMultiplayer = SemiFunc.IsMultiplayer();

            for (int i = 0; i < spawnCount; i++)
            {
                float distance = 1.2f + (i % 2) * 0.8f;
                float offsetX = ((i / 2) % 2 == 0 ? -1 : 1) * ((i / 2) + 1) * 0.7f;

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
            MoreDefibro.Log.LogInfo($"生成完毕");
        }
    }
}
