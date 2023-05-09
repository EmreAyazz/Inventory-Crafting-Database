using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TT.DB
{
    public class InventoryManager : MonoBehaviour
    {
        public Transform inventoryPanel;

        public List<Item> inventory;
        public List<Item> dataBase;

        public static List<Item> inventoryMeta;
        public static List<Item> dataBaseMeta;

        private void Awake()
        {
            inventoryMeta = inventory;
            dataBaseMeta = dataBase;

            DontDestroyOnLoad(gameObject);

            Inventory.OnItemAdd += InventoryControl;
            Inventory.OnItemRemoved += InventoryControl;
            InventoryControl();
        }

        void Update()
        {
            inventory = inventoryMeta;
        }

        private void InventoryControl()
        {
            if (inventoryPanel)
            {
                for (int i = 0; i < inventoryPanel.childCount; i++)
                {
                    inventoryPanel.GetChild(i).GetChild(0).GetComponent<Image>().color =
                        inventory[i].image == null ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
                    inventoryPanel.GetChild(i).GetChild(0).GetComponent<Image>().sprite = inventory[i].image;

                    inventoryPanel.GetChild(i).GetChild(1).gameObject.SetActive(inventory[i].count > 1 ? true : false);
                    inventoryPanel.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{inventory[i].count}";
                }
            }
        }

        public static Item FindItem_InInventory(ItemNames name)
        {
            foreach (var item in inventoryMeta)
            {
                if (item.name == name.ToString() && item.count < item.maxStack)
                {
                    return item;
                }
            }

            return null;
        }
        public static Item FindAllItem_InInventory(ItemNames name)
        {
            foreach (var item in inventoryMeta)
            {
                if (item.name == name.ToString())
                {
                    return item;
                }
            }

            return null;
        }
        public static Item FindSlot_InInventory()
        {
            foreach (var item in inventoryMeta)
            {
                if (item.count == 0)
                {
                    return item;
                }
            }

            return null;
        }
        public static Item FindItem_InDataBase(ItemNames name)
        {
            foreach (var item in dataBaseMeta)
            {
                if (item.name == name.ToString())
                {
                    Item ite = new Item();
                    ite.name = item.name;
                    ite.count = item.count;
                    ite.maxStack = item.maxStack;
                    ite.image = item.image;
                    return ite;
                }
            }

            return null;
        }

        public static void SetItem_InInventory(Item item)
        {
            for (int i = 0; i < inventoryMeta.Count; i++)
            {
                if (inventoryMeta[i].count <= 0)
                {
                    inventoryMeta[i] = item;
                    break;
                }
            }
        }

        public static void AddItem_InInventory(Item item)
        {
            for (int i = 0; i < inventoryMeta.Count; i++)
            {
                if (inventoryMeta[i].name == item.name)
                {
                    inventoryMeta[i] = item;
                    break;
                }
            }
        }

        public static void RemoveItem_InInventory(Item item)
        {
            for (int i = 0; i < inventoryMeta.Count; i++)
            {
                if (inventoryMeta[i].name == item.name)
                {
                    inventoryMeta[i] = item;
                    break;
                }
            }
        }

        public static ItemNames ToItemNames(string name)
        {
            ItemNames parsed_enum = (ItemNames)System.Enum.Parse(typeof(ItemNames), name);
            return parsed_enum;
        }
    }
}
