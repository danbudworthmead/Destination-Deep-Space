using System;

namespace MyAssets.UI.CommunityMaps
{
    [Serializable]
    public class Level
    {
        public int id = int.MinValue;
        public string name = string.Empty;
        public int score = int.MinValue;
    }


    [Serializable]
    public class LevelData
    {
        public Level[] maps;
    }
}
