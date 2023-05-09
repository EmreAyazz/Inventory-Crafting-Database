using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TT.DB
{
    public class ItemDataResetWindow : EditorWindow
    {
        [Serializable]
        private class ItemDataWrapper
        {
            public List<Items> itemDataList;
        }

        [Serializable]
        private class ItemDataBaseWrapper
        {
            public List<Item> itemDataList;
        }

        public Items item;
        private List<Items> itemList = new List<Items>();

        [MenuItem("TT/Database/ItemData")]
        public static void ShowWindow()
        {
            var window = GetWindow<ItemDataResetWindow>();
            window.titleContent = new GUIContent("ItemData");
            window.minSize = new Vector2(700, 600);
        }

        private void OnGUI()
        {
            if (item == null)
                item = new Items();

            if (File.Exists(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemDataMeta.json"))
            {
                if (itemList.Count <= 0)
                {
                    string jsonData = File.ReadAllText(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemDataMeta.json");
                    ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(jsonData);
                    itemList = itemDataWrapper.itemDataList;
                }
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("public class Item", GUILayout.Width(500));
            EditorGUILayout.LabelField("{", GUILayout.Width(500));
            if (itemList.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("", GUILayout.Width(50));
                    EditorGUILayout.LabelField("public", GUILayout.Width(50));
                    EditorGUILayout.LabelField($"{itemList[i].valueType.ToString().ToLower()}", GUILayout.Width(60));
                    EditorGUILayout.LabelField($"{itemList[i].valueName}", GUILayout.Width(150));

                    if (itemList[i].valueName != "name" && itemList[i].valueName != "image" && itemList[i].valueName != "count" && itemList[i].valueName != "maxStack")
                    {
                        if (GUILayout.Button("Delete", GUILayout.Width(100)))
                        {
                            itemList.RemoveAt(i);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.LabelField("}", GUILayout.Width(700));

            item.valueName = EditorGUILayout.TextField("Value Name", item.valueName, GUILayout.Width(250));
            item.valueType = (Values)EditorGUILayout.EnumPopup("Value Type", item.valueType, GUILayout.Width(250));

            // Save button
            if (GUILayout.Button("Add", GUILayout.Width(250), GUILayout.Height(100)))
            {
                bool found = false;
                foreach (Items items in itemList)
                {
                    if (items.valueName == item.valueName)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    itemList.Add(item);
                    item = new Items();
                }
            }

            if (GUILayout.Button("Save to File", GUILayout.Width(250)))
            {
                ItemDataWrapper itemDataWrapper = new ItemDataWrapper();
                itemDataWrapper.itemDataList = itemList;
                string jsonData = JsonUtility.ToJson(itemDataWrapper);
                File.WriteAllText(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemDataMeta.json", jsonData);
            }

            if (GUILayout.Button("Load from File", GUILayout.Width(250)))
            {
                if (File.Exists(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemDataMeta.json"))
                {
                    string jsonData = File.ReadAllText(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemDataMeta.json");
                    ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(jsonData);
                    itemList = itemDataWrapper.itemDataList;
                }
            }

            // Save button
            if (GUILayout.Button("Write", GUILayout.Width(250), GUILayout.Height(100)))
            {
                ChangeItemScript();

                ChangeItemDataBaseScript();

                ChangeInventoryMetaScript();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

        }

        public void ChangeItemScript()
        {
            AssetDatabase.Refresh();

            // Try to find an existing file in the project called "ProjectConstants.cs"
            string filePath = FindPath("Item", "cs");

            // If no such file exists already, use the save panel to get a folder in which the file will be placed.
            if (string.IsNullOrEmpty(filePath))
            {
                string directory =
                    EditorUtility.OpenFolderPanel("Choose location for file ItemDataBase.cs", Application.dataPath, "");

                // Canceled choose? Do nothing.
                if (string.IsNullOrEmpty(directory))
                {
                    return;
                }

                filePath = Path.Combine(directory, "ProjectConstants.cs");
            }

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("// This file is auto-generated. Modifications are not saved.");
                writer.WriteLine();
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using UnityEngine.UI;");
                writer.WriteLine();
                writer.WriteLine("namespace TT.DB");
                writer.WriteLine("{");

                writer.WriteLine("    [System.Serializable]");
                writer.WriteLine("    public class Item");
                writer.WriteLine("    {");
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (itemList[i].valueType == ItemDataResetWindow.Values.String || itemList[i].valueType == ItemDataResetWindow.Values.Int || itemList[i].valueType == ItemDataResetWindow.Values.Float)
                        writer.WriteLine($"          public {itemList[i].valueType.ToString().ToLower()} {itemList[i].valueName};");
                    else
                        writer.WriteLine($"          public {itemList[i].valueType.ToString()} {itemList[i].valueName};");
                }
                writer.WriteLine("    }");

                writer.WriteLine("    [System.Serializable]");
                writer.WriteLine("    public enum ItemNames");
                writer.WriteLine("    {");

                GameObject itemDataBase = GameObject.Find("ItemDataBase");

                if (itemDataBase != null)
                {
                    InventoryManager inventory = itemDataBase.GetComponent<InventoryManager>();

                    writer.WriteLine($"         None,");
                    for (int i = 0; i < inventory.dataBase.Count; i++)
                    {
                        if(inventory.dataBase[i].name != "")
                        {
                            writer.WriteLine($"         {inventory.dataBase[i].name},");
                        }
                    }
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }
        public void ChangeItemDataBaseScript()
        {
            AssetDatabase.Refresh();

            // Try to find an existing file in the project called "ProjectConstants.cs"
            string filePath = FindPath("ItemDataBaseWindow", "cs");

            // If no such file exists already, use the save panel to get a folder in which the file will be placed.
            if (string.IsNullOrEmpty(filePath))
            {
                string directory =
                    EditorUtility.OpenFolderPanel("Choose location for file ItemDataBase.cs", Application.dataPath, "");

                // Canceled choose? Do nothing.
                if (string.IsNullOrEmpty(directory))
                {
                    return;
                }

                filePath = Path.Combine(directory, "ProjectConstants.cs");
            }

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("// This file is auto-generated. Modifications are not saved.");
                writer.WriteLine();
                writer.WriteLine("using UnityEditor;");
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.IO;");
                writer.WriteLine("using System;");
                writer.WriteLine();

                writer.WriteLine("namespace TT.DB");
                writer.WriteLine("{");

                writer.WriteLine("public class ItemDataBaseWindow : EditorWindow");
                writer.WriteLine("{");

                writer.WriteLine("    [Serializable]");
                writer.WriteLine("    private class ItemDataWrapper");
                writer.WriteLine("    {");
                writer.WriteLine("          public List<Item> itemDataList;");
                writer.WriteLine("    }");
                writer.WriteLine();

                writer.WriteLine("    private Item itemData;");
                writer.WriteLine("    private List<Item> itemList = new List<Item>();");
                writer.WriteLine();
                writer.WriteLine("    private Vector2 scrollPos;");
                writer.WriteLine("    private int sortOption = 0;");
                writer.WriteLine("    private string[] sortOptions = { \"All\", \"Gun\", \"Melee\", \"Ammo\", \"NoneImage\" };");
                writer.WriteLine();
                writer.WriteLine("    [MenuItem(\"TT/Database/ItemDataBase\")]");
                writer.WriteLine("    public static void ShowWindow()");
                writer.WriteLine("    {");
                writer.WriteLine("          var window = GetWindow<ItemDataBaseWindow>();");
                writer.WriteLine("          window.titleContent = new GUIContent(\"ItemDataBase System\");");
                writer.WriteLine("          window.minSize = new Vector2(1200, 700);");
                writer.WriteLine("    }");
                writer.WriteLine();

                writer.WriteLine("    private void OnGUI()");
                writer.WriteLine("    {");
                writer.WriteLine("          if (itemData == null)");
                writer.WriteLine("              itemData = new Item();");
                writer.WriteLine();
                writer.WriteLine("          if (File.Exists(Application.dataPath + \"/_TTSDK/DataBaseSystem/Json/itemData.json\"))");
                writer.WriteLine("          {");
                writer.WriteLine("              if (itemList.Count <= 0)");
                writer.WriteLine("              {");
                writer.WriteLine("                  string jsonData = File.ReadAllText(Application.dataPath + \"/_TTSDK/DataBaseSystem/Json/itemData.json\");");
                writer.WriteLine("                  ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(jsonData);");
                writer.WriteLine("                  itemList = itemDataWrapper.itemDataList;");
                writer.WriteLine("              }");
                writer.WriteLine("          }");
                writer.WriteLine();
                writer.WriteLine("          EditorGUILayout.BeginHorizontal();");
                writer.WriteLine();
                writer.WriteLine("          itemData.image = (Sprite)EditorGUILayout.ObjectField(\"Item Image\", itemData.image, typeof(Sprite), false, GUILayout.Width(250));");
                writer.WriteLine();
                writer.WriteLine("          EditorGUILayout.BeginVertical();");
                writer.WriteLine();

                for (int i = 0; i < itemList.Count; i++)
                {
                    switch (itemList[i].valueType)
                    {
                        case Values.String:
                            writer.WriteLine($"          itemData.{itemList[i].valueName} = EditorGUILayout.TextField(\"Item {itemList[i].valueName}\", itemData.{itemList[i].valueName}, GUILayout.Width(500));");
                            break;
                        case Values.Int:
                            writer.WriteLine($"          itemData.{itemList[i].valueName} = EditorGUILayout.IntField(\"Item {itemList[i].valueName}\", itemData.{itemList[i].valueName}, GUILayout.Width(500));");
                            break;
                        case Values.Float:
                            writer.WriteLine($"          itemData.{itemList[i].valueName} = EditorGUILayout.FloatField(\"Item {itemList[i].valueName}\", itemData.{itemList[i].valueName}, GUILayout.Width(500));");
                            break;
                        case Values.Vector2:
                            writer.WriteLine($"          itemData.{itemList[i].valueName} = EditorGUILayout.Vector2Field(\"Item {itemList[i].valueName}\", itemData.{itemList[i].valueName}, GUILayout.Width(500));");
                            break;
                        case Values.Vector3:
                            writer.WriteLine($"          itemData.{itemList[i].valueName} = EditorGUILayout.Vector3Field(\"Item {itemList[i].valueName}\", itemData.{itemList[i].valueName}, GUILayout.Width(500));");
                            break;
                        case Values.Sprite:
                            if (itemList[i].valueName != "image")
                                writer.WriteLine($"          itemData.{itemList[i].valueName} = (Sprite)EditorGUILayout.ObjectField(\"Item {itemList[i].valueName}\", itemData.{itemList[i].valueName}, typeof(Sprite), false, GUILayout.Width(500));");
                            break;
                    }
                }

                writer.WriteLine();
                writer.WriteLine("          EditorGUILayout.EndVertical();");
                writer.WriteLine();
                writer.WriteLine("          EditorGUILayout.BeginVertical();");
                writer.WriteLine("          if (GUILayout.Button(\"Add\", GUILayout.Width(300), GUILayout.Height(100)))");
                writer.WriteLine("          {");
                writer.WriteLine("              itemList.Add(itemData);");
                writer.WriteLine("              itemData = new Item();");
                writer.WriteLine("          }");
                writer.WriteLine();
                writer.WriteLine("          if (GUILayout.Button(\"Save to File\", GUILayout.Width(300)))");
                writer.WriteLine("          {");
                writer.WriteLine("              ItemDataWrapper itemDataWrapper = new ItemDataWrapper();");
                writer.WriteLine("              itemDataWrapper.itemDataList = itemList;");
                writer.WriteLine("              string jsonData = JsonUtility.ToJson(itemDataWrapper);");
                writer.WriteLine("              File.WriteAllText(Application.dataPath + \"/_TTSDK/DataBaseSystem/Json/itemData.json\", jsonData);");
                writer.WriteLine();
                writer.WriteLine("              if (GameObject.Find(\"ItemDataBase\") == null)");
                writer.WriteLine("              {");
                writer.WriteLine("                  GameObject itemDataBase = new GameObject(\"ItemDataBase\");");
                writer.WriteLine("                  InventoryManager inventory = itemDataBase.AddComponent<InventoryManager>();");
                writer.WriteLine("                  inventory.dataBase = itemList;");
                writer.WriteLine("              }");
                writer.WriteLine("              else");
                writer.WriteLine("              {");
                writer.WriteLine("                  InventoryManager inventory = GameObject.Find(\"ItemDataBase\").GetComponent<InventoryManager>();");
                writer.WriteLine("                  inventory.dataBase = itemList;");
                writer.WriteLine("              }");
                writer.WriteLine("          }");
                writer.WriteLine();
                writer.WriteLine("          if (GUILayout.Button(\"Load from File\", GUILayout.Width(300)))");
                writer.WriteLine("          {");
                writer.WriteLine("              if (File.Exists(Application.dataPath + \"/_TTSDK/DataBaseSystem/Json/itemData.json\"))");
                writer.WriteLine("              {");
                writer.WriteLine("                  string jsonData = File.ReadAllText(Application.dataPath + \"/_TTSDK/DataBaseSystem/Json/itemData.json\");");
                writer.WriteLine("                  ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(jsonData);");
                writer.WriteLine("                  itemList = itemDataWrapper.itemDataList;");
                writer.WriteLine("              }");
                writer.WriteLine("          }");
                writer.WriteLine();
                writer.WriteLine("          EditorGUILayout.EndVertical();");
                writer.WriteLine();
                writer.WriteLine("          EditorGUILayout.EndHorizontal();");
                writer.WriteLine();
                writer.WriteLine("          EditorGUILayout.Space(25);");
                writer.WriteLine();
                writer.WriteLine("          sortOption = EditorGUILayout.Popup(\"Sort by\", sortOption, sortOptions);");
                writer.WriteLine();
                writer.WriteLine("          EditorGUILayout.BeginHorizontal();");
                writer.WriteLine();

                writer.WriteLine("          EditorGUILayout.LabelField(\"\", GUILayout.Width(50));");
                for (int i = 0; i < itemList.Count; i++)
                {
                    writer.WriteLine($"          EditorGUILayout.LabelField(\"{itemList[i].valueName}\", GUILayout.Width(100));");
                }
                writer.WriteLine();

                writer.WriteLine("          EditorGUILayout.EndHorizontal();");
                writer.WriteLine();
                writer.WriteLine("          scrollPos = EditorGUILayout.BeginScrollView(scrollPos);");
                writer.WriteLine("          for (int i = 0; i < itemList.Count; i++)");
                writer.WriteLine("          {");
                writer.WriteLine("              EditorGUILayout.BeginHorizontal();");
                writer.WriteLine();
                writer.WriteLine("              if (sortOption == 0)");
                writer.WriteLine("              {");
                writer.WriteLine("                  if (itemList[i].image)");
                writer.WriteLine("                      EditorGUILayout.LabelField(new GUIContent(itemList[i].image.texture), GUILayout.Width(50), GUILayout.Height(50));");
                writer.WriteLine("                  else");
                writer.WriteLine("                      EditorGUILayout.LabelField(\"\", GUILayout.Width(50), GUILayout.Height(50));");
                writer.WriteLine();

                for (int i = 0; i < itemList.Count; i++)
                {
                    switch (itemList[i].valueType)
                    {
                        case Values.String:
                            writer.WriteLine($"                 itemList[i].{itemList[i].valueName} = EditorGUILayout.TextField(itemList[i].{itemList[i].valueName}, GUILayout.Width(100));");
                            break;
                        case Values.Int:
                            writer.WriteLine($"                 itemList[i].{itemList[i].valueName} = EditorGUILayout.IntField(itemList[i].{itemList[i].valueName}, GUILayout.Width(100));");
                            break;
                        case Values.Float:
                            writer.WriteLine($"                 itemList[i].{itemList[i].valueName} = EditorGUILayout.FloatField(itemList[i].{itemList[i].valueName}, GUILayout.Width(100));");
                            break;
                        case Values.Vector2:
                            writer.WriteLine($"                 itemList[i].{itemList[i].valueName} = EditorGUILayout.Vector2Field(\"\", itemList[i].{itemList[i].valueName}, GUILayout.Width(100));");
                            break;
                        case Values.Vector3:
                            writer.WriteLine($"                 itemList[i].{itemList[i].valueName} = EditorGUILayout.Vector3Field(\"\", itemList[i].{itemList[i].valueName}, GUILayout.Width(100));");
                            break;
                        case Values.Sprite:
                            writer.WriteLine($"                 itemList[i].{itemList[i].valueName} = (Sprite)EditorGUILayout.ObjectField(itemList[i].{itemList[i].valueName}, typeof(Sprite), false, GUILayout.Width(100));");
                            break;
                    }
                }

                writer.WriteLine();
                writer.WriteLine("                 if (GUILayout.Button(\"Delete\"))");
                writer.WriteLine("                 {");
                writer.WriteLine("                      itemList.RemoveAt(i);");
                writer.WriteLine();
                writer.WriteLine("                      ItemDataWrapper itemDataWrapper = new ItemDataWrapper();");
                writer.WriteLine("                      itemDataWrapper.itemDataList = itemList;");
                writer.WriteLine("                      string jsonData = JsonUtility.ToJson(itemDataWrapper);");
                writer.WriteLine("                      File.WriteAllText(Application.dataPath + \"/_TTSDK/DataBaseSystem/Json/itemData.json\", jsonData);");
                writer.WriteLine("                 }");
                writer.WriteLine("              }");
                writer.WriteLine();
                writer.WriteLine("              EditorGUILayout.EndHorizontal();");
                writer.WriteLine();
                writer.WriteLine("          }");
                writer.WriteLine();
                writer.WriteLine("          EditorGUILayout.EndScrollView();");

                writer.WriteLine("    }");
                writer.WriteLine("}");
                writer.WriteLine("}");
            }
        }
        public void ChangeInventoryMetaScript()
        {
            AssetDatabase.Refresh();

            // Try to find an existing file in the project called "ProjectConstants.cs"
            string filePath = FindPath("InventoryMeta", "cs");

            // If no such file exists already, use the save panel to get a folder in which the file will be placed.
            if (string.IsNullOrEmpty(filePath))
            {
                string directory =
                    EditorUtility.OpenFolderPanel("Choose location for file ItemDataBase.cs", Application.dataPath, "");

                // Canceled choose? Do nothing.
                if (string.IsNullOrEmpty(directory))
                {
                    return;
                }

                filePath = Path.Combine(directory, "ProjectConstants.cs");
            }

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("// This file is auto-generated. Modifications are not saved.\r\n" +
                "\r\n" +
                "using System.Collections;\r\n" +
                "using System.Collections.Generic;\r\n" +
                "using UnityEngine;\r\n" +
                "\r\n" +
                "namespace TT.DB\r\n" +
                "{\r\n" +
                "    public class Inventory : InventoryManager\r\n" +
                "    {\r\n" +
                "        public delegate void AddItemEvent();\r\n" +
                "        public static event AddItemEvent OnItemAdd;\r\n" +
                "\r\n" +
                "        public delegate void RemoveItemEvent();\r\n" +
                "        public static event RemoveItemEvent OnItemRemoved;\r\n" +
                "\r\n" +
                "        public static void AddItem(ItemNames name, int value) => ItemStatus(name, value);\r\n" +
                "        public static void RemoveItem(ItemNames name, int value) => ItemStatus(name, -value);\r\n" +
                "        public static void RemoveAllItem() => ItemStatus();\r\n" +
                "        public static bool Full(ItemNames name) => FullStatus(name);\r\n" +
                "\r\n" +
                "\r\n" +
                "        private static void ClearAllItems()\r\n" +
                "        {\r\n" +
                "            for (int i = 0; i < inventoryMeta.Count; i++)\r\n" +
                "            {\r\n" +
                "                Item ite = new Item();\r\n");
                                 for (int i = 0; i < itemList.Count; i++)
                                 {
                                     writer.WriteLine($"                ite.{itemList[i].valueName} = inventoryMeta[i].{itemList[i].valueName};");
                                 }
                writer.WriteLine("\r\n" + 
                "                inventoryMeta[i] = ite;\r\n" +
                "            }\r\n" +
                "        }\r\n" +
                "\r\n" +
                "        private static void ItemStatus()\r\n" +
                "        {\r\n" +
                "            for (int i = 0; i < inventoryMeta.Count; i++)\r\n" +
                "            {\r\n" +
                "                inventoryMeta[i] = new Item();\r\n" +
                "            }\r\n" +
                "\r\n" +
                "            ClearAllItems();\r\n" +
                "        }\r\n" +
                "\r\n" +
                "        private static void ItemStatus(ItemNames name, int value)\r\n" +
                "        {\r\n" +
                "            Item dataBaseItem = FindItem_InDataBase(name);\r\n" +
                "\r\n" +
                "            if (value > 0)\r\n" +
                "            {\r\n" +
                "                while (value > 0)\r\n" +
                "                {\r\n" +
                "                    Item item = FindItem_InInventory(name) == null ? FindSlot_InInventory() : FindItem_InInventory(name);\r\n" +
                "                    int stack = 0;\r\n" +
                "                    if (item.count > 0)\r\n" +
                "                    {\r\n" +
                "                        stack = item.maxStack - item.count <= value ? item.maxStack - item.count : value;\r\n" +
                "                        item = item.count <= 0 ? dataBaseItem : item;\r\n" +
                "                        item.count += stack;\r\n" +
                "                        value -= stack;\r\n" +
                "                        AddItem_InInventory(item);\r\n" +
                "                    }\r\n" +
                "                    else\r\n" +
                "                    {\r\n" +
                "                        stack = dataBaseItem.maxStack <= value ? dataBaseItem.maxStack : value;\r\n" +
                "                        item = item.count <= 0 ? dataBaseItem : item;\r\n" +
                "                        item.count = stack;\r\n" +
                "                        value -= stack;\r\n" +
                "                        SetItem_InInventory(item);\r\n" +
                "                    }\r\n" +
                "                }\r\n" +
                "\r\n" +
                "                ClearAllItems();\r\n" +
                "\r\n" +
                "                OnItemAdd?.Invoke();\r\n" +
                "            }\r\n" +
                "\r\n" +
                "            if (value < 0)\r\n" +
                "            {\r\n" +
                "                for (int i = inventoryMeta.Count - 1; i >= 0; i--)\r\n" +
                "                {\r\n" +
                "                    if (inventoryMeta[i].name == name.ToString())\r\n" +
                "                    {\r\n" +
                "                        int stack = inventoryMeta[i].count <= Mathf.Abs(value) ? inventoryMeta[i].count : Mathf.Abs(value);\r\n" +
                "                        inventoryMeta[i].count -= stack;\r\n" +
                "                        inventoryMeta[i] = inventoryMeta[i].count <= 0 ? new Item() : inventoryMeta[i];\r\n" +
                "                        value += stack;\r\n" +
                "\r\n" +
                "                        if (value >= 0) break;\r\n" +
                "                    }\r\n" +
                "                }\r\n" +
                "\r\n" +
                "                ClearAllItems();\r\n" +
                "\r\n" +
                "                OnItemRemoved?.Invoke();\r\n" +
                "            }\r\n" +
                "\r\n" +
                "        }\r\n" +
                "        private static bool FullStatus(ItemNames name)\r\n" +
                "        {\r\n" +
                "            Item item = FindItem_InInventory(name) == null ? FindSlot_InInventory() : FindItem_InInventory(name);\r\n" +
                "\r\n" +
                "            if (item == null) return true;\r\n" +
                "            else return false;\r\n" +
                "        }\r\n" +
                "    }\r\n" +
                "}\r\n" +
                "");
            }
        }

        private static string FindPath(string filename, string extension)
        {
            string path = string.Empty;
            foreach (var file in Directory.GetFiles(Application.dataPath, "*." + extension, SearchOption.AllDirectories))
            {
                if (Path.GetFileNameWithoutExtension(file) == filename)
                {
                    path = file;
                    break;
                }
            }

            return path;
        }

        [System.Serializable]
        public class Items
        {
            public string valueName;
            public Values valueType;
        }

        [System.Serializable]
        public enum Values
        {
            Int,
            Float,
            String,
            Vector2,
            Vector3,
            Sprite
        }
    }
}
