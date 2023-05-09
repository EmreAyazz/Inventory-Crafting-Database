using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TT.DB
{
    public class CraftingManager : MonoBehaviour
    {
        public static CraftingManager instance;

        public Transform craftPanel;
        public Image craftProgressBar;

        public List<Craft> dataBase;

        public static Image craftProgressBarMeta;
        public static List<Craft> dataBaseMeta;
        public static ItemNames missingMaterialName;
        public static int missingMaterialCount;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            dataBaseMeta = dataBase;
            craftProgressBarMeta = craftProgressBar;

            DontDestroyOnLoad(gameObject);
        }

        public static bool isEnough(Craft craftItem)
        {
            int count = 0;
            foreach (CraftMaterial cMaterial in craftItem.items)
            {
                count = cMaterial.count;
                foreach (Item item in Inventory.inventoryMeta)
                {
                    if (item.name == cMaterial.name.ToString())
                    {
                        count -= item.count >= count ? count : item.count;

                        if (count <= 0) break;
                    }
                }

                if (count > 0)
                {
                    missingMaterialName = cMaterial.name;
                    missingMaterialCount = count;
                    return false;
                }
            }

            return true;
        }
        public static Craft FindItem_InDataBase(ItemNames name)
        {
            foreach (Craft craft in dataBaseMeta)
            {
                if (craft.name == name) return craft;
            }
            return null;
        }
        public static IEnumerator Crafting(ItemNames name, float time)
        {
            float fullTime = 0;
            while (time > fullTime)
            {
                fullTime += Time.deltaTime;

                if (craftProgressBarMeta)
                    craftProgressBarMeta.fillAmount = fullTime / time;

                yield return new WaitForFixedUpdate();
            }

            CraftSystem.Crafted(name);
        }
    }
}
