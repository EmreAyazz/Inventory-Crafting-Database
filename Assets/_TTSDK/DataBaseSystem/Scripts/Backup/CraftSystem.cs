using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TT.DB
{
    public class CraftSystem : CraftingManager
    {
        public delegate void CraftingEvent(ItemNames name);
        public static event CraftingEvent OnItemCrafting;

        public delegate void CraftedEvent(ItemNames name);
        public static event CraftedEvent OnItemCrafted;

        public delegate void MissingMaterialsEvent(ItemNames materialNames, int missingMaterialCount);
        public static event MissingMaterialsEvent OnMissingMaterials;

        public static void CraftIt(ItemNames name) => CraftItMeta(name);
        public static void Crafted(ItemNames name) => CraftedMeta(name);


        private static void CraftItMeta(ItemNames name)
        {
            Craft craftItem = FindItem_InDataBase(name);

            isEnoughCraft(craftItem);
        }
        private static void CraftedMeta(ItemNames name)
        {
            Craft craftItem = FindItem_InDataBase(name);
            OnItemCrafted?.Invoke(craftItem.name);

            foreach (CraftMaterial cMaterial in craftItem.items)
            {
                Inventory.RemoveItem(cMaterial.name, cMaterial.count);
            }

            Inventory.AddItem(craftItem.name, 1);
        }

        private static void isEnoughCraft(Craft craftItem)
        {
            if (isEnough(craftItem))
            {
                instance.StartCoroutine(Crafting(craftItem.name, craftItem.craftTime));
                OnItemCrafting?.Invoke(craftItem.name);
            }
            else
            {
                OnMissingMaterials?.Invoke(missingMaterialName, missingMaterialCount);
            }
        }
    }
}
