using System;
using UnityEngine;

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
        }

        public string name;
        public Prop[] props;
    }
}
