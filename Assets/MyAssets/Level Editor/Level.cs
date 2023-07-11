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
            public int x, y;
        }

        public Prop[] props = new Prop[32];
        public int propCount;
    }
}
