using System;

namespace MyAssets.Level_Editor
{
    [Serializable]
    public class Level
    {
        [Serializable]
        public class Prop
        {
            public string id;
            public int x, y;
            public int scale;
            public int gravityRadius;
            public int mass;
            public int r;
            public int g;
            public int b;
        }

        public int dataVersion = 1;
        public string name;
        public Prop[] props;
    }
}
