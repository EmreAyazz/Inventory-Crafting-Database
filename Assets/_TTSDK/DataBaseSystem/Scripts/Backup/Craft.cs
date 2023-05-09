// This file is auto-generated. Modifications are not saved.

using System.Collections.Generic;
using System;
using UnityEngine;

namespace TT.DB
{
    [Serializable]
    public class Craft
    {
        public ItemNames name;
        public Sprite image;
        public float craftTime;
        public bool listOpen = false;
        public List<CraftMaterial> items = new List<CraftMaterial>();
    }
    [Serializable]
    public class CraftMaterial
    {
        public ItemNames name;
        public Sprite image;
        public int count;
    }
}
