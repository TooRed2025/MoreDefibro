using BepInEx.Configuration;

namespace MoreDefibro
{
    internal class SetConfig
    {
        public static ConfigEntry<int> minPrice { get; private set; }
        public static ConfigEntry<int> maxPrice { get; private set; }
        public static ConfigEntry<int> minSpawnCount { get; private set; }
        public static ConfigEntry<int> maxSpawnCount { get; private set; }

        public static void config(ConfigFile config)
        {
            minSpawnCount = config.Bind("Settings", "MinSpawnCount", 0, new ConfigDescription("最小生成数量", new AcceptableValueRange<int>(0, 10)));
            maxSpawnCount = config.Bind("Settings", "MaxSpawnCount", 4, new ConfigDescription("最大生成数量", new AcceptableValueRange<int>(1, 10)));
            minPrice = config.Bind("Settings", "MinPrice", 2, new ConfigDescription("最低价格(千)", new AcceptableValueRange<int>(1, 30)));
            maxPrice = config.Bind("Settings", "MaxPrice", 10, new ConfigDescription("最高价格(千)", new AcceptableValueRange<int>(10, 60)));
        }
    }
}
