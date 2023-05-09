using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using System;
using System.IO;
using UnityEditor.PackageManager.UI;

namespace TT.DB
{
    public class CraftDataBaseWindow : EditorWindow
    {
        public Craft craft = new Craft();
        public List<Craft> craftList = new List<Craft>();
        private Vector2 scrollPos;

        [MenuItem("TT/Database/CraftingDataBase")]
        public static void ShowWindow()
        {
            var window = GetWindow<CraftDataBaseWindow>();
            window.titleContent = new GUIContent("CraftDataBase");
            window.minSize = new Vector2(700, 600);
        }

        void OnGUI()
        {
            if (craft == null)
                craft = new Craft();

            string separator = new string('-', (int)EditorGUIUtility.currentViewWidth);

            EditorGUILayout.BeginHorizontal();
            craft.image = (Sprite)EditorGUILayout.ObjectField("Image", craft.image, typeof(Sprite), false, GUILayout.Width(200));
            EditorGUILayout.BeginVertical();
            craft.name = (ItemNames)EditorGUILayout.EnumPopup("Name", craft.name, GUILayout.Width(200));
            craft.craftTime = EditorGUILayout.FloatField("Craft Time", craft.craftTime, GUILayout.Width(200));
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Add", GUILayout.Width(300), GUILayout.Height(30)))
            {
                craftList.Add(craft);
                craft = new Craft();
            }
            if (GUILayout.Button("Save To File", GUILayout.Width(300), GUILayout.Height(30)))
            {
                ItemDataWrapper itemDataWrapper = new ItemDataWrapper();
                itemDataWrapper.craftDataList = craftList;
                string jsonData = JsonUtility.ToJson(itemDataWrapper);
                File.WriteAllText(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/craftDataBase.json", jsonData);

                if (GameObject.Find("CraftDataBase") == null)
                {
                    GameObject craftDataBase = new GameObject("CraftDataBase");
                    CraftingManager crafting = craftDataBase.AddComponent<CraftingManager>();
                    crafting.dataBase = craftList;
                }
                else
                {
                    CraftingManager crafting = GameObject.Find("CraftDataBase").GetComponent<CraftingManager>();
                    crafting.dataBase = craftList;
                }
            }
            if (GUILayout.Button("Load From File", GUILayout.Width(300), GUILayout.Height(30)))
            {
                if (File.Exists(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/craftDataBase.json"))
                {
                    string jsonData = File.ReadAllText(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/craftDataBase.json");
                    ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(jsonData);
                    craftList = itemDataWrapper.craftDataList;
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(separator);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int j = 0; j < craftList.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();

                if (craftList[j].image)
                    EditorGUILayout.LabelField(new GUIContent(craftList[j].image.texture), GUILayout.Width(50), GUILayout.Height(50));
                else
                    EditorGUILayout.LabelField("", GUILayout.Width(50), GUILayout.Height(50));

                EditorGUILayout.BeginVertical();

                craftList[j].name = (ItemNames)EditorGUILayout.EnumPopup("Craft Name", craftList[j].name, GUILayout.Width(300));
                craftList[j].craftTime = EditorGUILayout.FloatField("Craft Time", craftList[j].craftTime, GUILayout.Width(300));

                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("Add Craft Material", GUILayout.Width(150), GUILayout.Height(25)))
                {
                    craftList[j].items.Add(new CraftMaterial());
                }

                if (GUILayout.Button("Delete", GUILayout.Width(150), GUILayout.Height(25)))
                {
                    craftList.RemoveAt(j);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Items:");

                if (GUILayout.Button(craftList[j].listOpen ? "v" : ">", GUILayout.Width(25), GUILayout.Height(25)))
                {
                    if (craftList[j].listOpen)
                    {
                        craftList[j].listOpen = false;
                    }
                    else
                    {
                        craftList[j].listOpen = true;
                    }
                }

                EditorGUILayout.EndHorizontal();

                if (craftList[j].listOpen)
                {
                    for (int i = 0; i < craftList[j].items.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (GameObject.Find("ItemDataBase"))
                        {
                            List<Item> dataBase = GameObject.Find("ItemDataBase").GetComponent<InventoryManager>().dataBase;
                            foreach (Item item in dataBase)
                            {
                                if (item.name == craftList[j].items[i].name.ToString())
                                {
                                    craftList[j].items[i].image = item.image;
                                }
                            }
                        }

                        EditorGUILayout.LabelField("", GUILayout.Width(50));

                        if (craftList[j].items[i].image)
                            EditorGUILayout.LabelField(new GUIContent(craftList[j].items[i].image.texture), GUILayout.Width(50), GUILayout.Height(50));
                        else
                            EditorGUILayout.LabelField("", GUILayout.Width(50), GUILayout.Height(50));

                        craftList[j].items[i].name = (ItemNames)EditorGUILayout.EnumPopup(craftList[j].items[i].name, GUILayout.Width(200));
                        craftList[j].items[i].count = EditorGUILayout.IntField("Item Count", craftList[j].items[i].count, GUILayout.Width(200));

                        if (GUILayout.Button("Delete"))
                        {
                            craftList[j].items.RemoveAt(i);
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.LabelField(separator);
            }
            EditorGUILayout.EndScrollView();
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

        [Serializable]
        private class ItemDataWrapper
        {
            public List<Craft> craftDataList;
        }
    }
}
