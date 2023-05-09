// This file is auto-generated. Modifications are not saved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TT.DB
{
    public class Inventory : InventoryManager
    {
        public delegate void AddItemEvent();
        public static event AddItemEvent OnItemAdd;

        public delegate void RemoveItemEvent();
        public static event RemoveItemEvent OnItemRemoved;

        public static void AddItem(ItemNames name, int value) => ItemStatus(name, value);
        public static void RemoveItem(ItemNames name, int value) => ItemStatus(name, -value);
        public static void RemoveAllItem() => ItemStatus();
        public static bool Full(ItemNames name) => FullStatus(name);


        private static void ClearAllItems()
        {
            for (int i = 0; i < inventoryMeta.Count; i++)
            {
                Item ite = new Item();

                ite.name = inventoryMeta[i].name;
                ite.image = inventoryMeta[i].image;
                ite.count = inventoryMeta[i].count;
                ite.maxStack = inventoryMeta[i].maxStack;
                ite.test = inventoryMeta[i].test;

                inventoryMeta[i] = ite;
            }
        }

        private static void ItemStatus()
        {
            for (int i = 0; i < inventoryMeta.Count; i++)
            {
                inventoryMeta[i] = new Item();
            }

            ClearAllItems();
        }

        private static void ItemStatus(ItemNames name, int value)
        {
            Item dataBaseItem = FindItem_InDataBase(name);

            if (value > 0)
            {
                while (value > 0)
                {
                    Item item = FindItem_InInventory(name) == null ? FindSlot_InInventory() : FindItem_InInventory(name);
                    int stack = 0;
                    if (item.count > 0)
                    {
                        stack = item.maxStack - item.count <= value ? item.maxStack - item.count : value;
                        item = item.count <= 0 ? dataBaseItem : item;
                        item.count += stack;
                        value -= stack;
                        AddItem_InInventory(item);
                    }
                    else
                    {
                        stack = dataBaseItem.maxStack <= value ? dataBaseItem.maxStack : value;
                        item = item.count <= 0 ? dataBaseItem : item;
                        item.count = stack;
                        value -= stack;
                        SetItem_InInventory(item);
                    }
                }

                ClearAllItems();

                OnItemAdd?.Invoke();
            }

            if (value < 0)
            {
                for (int i = inventoryMeta.Count - 1; i >= 0; i--)
                {
                    if (inventoryMeta[i].name == name.ToString())
                    {
                        int stack = inventoryMeta[i].count <= Mathf.Abs(value) ? inventoryMeta[i].count : Mathf.Abs(value);
                        inventoryMeta[i].count -= stack;
                        inventoryMeta[i] = inventoryMeta[i].count <= 0 ? new Item() : inventoryMeta[i];
                        value += stack;

                        if (value >= 0) break;
                    }
                }

                ClearAllItems();

                OnItemRemoved?.Invoke();
            }

        }
        private static bool FullStatus(ItemNames name)
        {
            Item item = FindItem_InInventory(name) == null ? FindSlot_InInventory() : FindItem_InInventory(name);

            if (item == null) return true;
            else return false;
        }
    }
}

