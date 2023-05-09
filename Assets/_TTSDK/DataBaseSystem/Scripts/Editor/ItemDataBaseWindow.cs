// This file is auto-generated. Modifications are not saved.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace TT.DB
{
public class ItemDataBaseWindow : EditorWindow
{
    [Serializable]
    private class ItemDataWrapper
    {
          public List<Item> itemDataList;
    }

    private Item itemData;
    private List<Item> itemList = new List<Item>();

    private Vector2 scrollPos;
    private int sortOption = 0;
    private string[] sortOptions = { "All", "Gun", "Melee", "Ammo", "NoneImage" };

    [MenuItem("TT/Database/ItemDataBase")]
    public static void ShowWindow()
    {
          var window = GetWindow<ItemDataBaseWindow>();
          window.titleContent = new GUIContent("ItemDataBase System");
          window.minSize = new Vector2(1200, 700);
    }

    private void OnGUI()
    {
          if (itemData == null)
              itemData = new Item();

          if (File.Exists(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemData.json"))
          {
              if (itemList.Count <= 0)
              {
                  string jsonData = File.ReadAllText(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemData.json");
                  ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(jsonData);
                  itemList = itemDataWrapper.itemDataList;
              }
          }

          EditorGUILayout.BeginHorizontal();

          itemData.image = (Sprite)EditorGUILayout.ObjectField("Item Image", itemData.image, typeof(Sprite), false, GUILayout.Width(250));

          EditorGUILayout.BeginVertical();

          itemData.name = EditorGUILayout.TextField("Item name", itemData.name, GUILayout.Width(500));
          itemData.count = EditorGUILayout.IntField("Item count", itemData.count, GUILayout.Width(500));
          itemData.maxStack = EditorGUILayout.IntField("Item maxStack", itemData.maxStack, GUILayout.Width(500));
          itemData.test = EditorGUILayout.FloatField("Item test", itemData.test, GUILayout.Width(500));

          EditorGUILayout.EndVertical();

          EditorGUILayout.BeginVertical();
          if (GUILayout.Button("Add", GUILayout.Width(300), GUILayout.Height(100)))
          {
              itemList.Add(itemData);
              itemData = new Item();
          }

          if (GUILayout.Button("Save to File", GUILayout.Width(300)))
          {
              ItemDataWrapper itemDataWrapper = new ItemDataWrapper();
              itemDataWrapper.itemDataList = itemList;
              string jsonData = JsonUtility.ToJson(itemDataWrapper);
              File.WriteAllText(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemData.json", jsonData);

              if (GameObject.Find("ItemDataBase") == null)
              {
                  GameObject itemDataBase = new GameObject("ItemDataBase");
                  InventoryManager inventory = itemDataBase.AddComponent<InventoryManager>();
                  inventory.dataBase = itemList;
              }
              else
              {
                  InventoryManager inventory = GameObject.Find("ItemDataBase").GetComponent<InventoryManager>();
                  inventory.dataBase = itemList;
              }
          }

          if (GUILayout.Button("Load from File", GUILayout.Width(300)))
          {
              if (File.Exists(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemData.json"))
              {
                  string jsonData = File.ReadAllText(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemData.json");
                  ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(jsonData);
                  itemList = itemDataWrapper.itemDataList;
              }
          }

          EditorGUILayout.EndVertical();

          EditorGUILayout.EndHorizontal();

          EditorGUILayout.Space(25);

          sortOption = EditorGUILayout.Popup("Sort by", sortOption, sortOptions);

          EditorGUILayout.BeginHorizontal();

          EditorGUILayout.LabelField("", GUILayout.Width(50));
          EditorGUILayout.LabelField("name", GUILayout.Width(100));
          EditorGUILayout.LabelField("image", GUILayout.Width(100));
          EditorGUILayout.LabelField("count", GUILayout.Width(100));
          EditorGUILayout.LabelField("maxStack", GUILayout.Width(100));
          EditorGUILayout.LabelField("test", GUILayout.Width(100));

          EditorGUILayout.EndHorizontal();

          scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
          for (int i = 0; i < itemList.Count; i++)
          {
              EditorGUILayout.BeginHorizontal();

              if (sortOption == 0)
              {
                  if (itemList[i].image)
                      EditorGUILayout.LabelField(new GUIContent(itemList[i].image.texture), GUILayout.Width(50), GUILayout.Height(50));
                  else
                      EditorGUILayout.LabelField("", GUILayout.Width(50), GUILayout.Height(50));

                 itemList[i].name = EditorGUILayout.TextField(itemList[i].name, GUILayout.Width(100));
                 itemList[i].image = (Sprite)EditorGUILayout.ObjectField(itemList[i].image, typeof(Sprite), false, GUILayout.Width(100));
                 itemList[i].count = EditorGUILayout.IntField(itemList[i].count, GUILayout.Width(100));
                 itemList[i].maxStack = EditorGUILayout.IntField(itemList[i].maxStack, GUILayout.Width(100));
                 itemList[i].test = EditorGUILayout.FloatField(itemList[i].test, GUILayout.Width(100));

                 if (GUILayout.Button("Delete"))
                 {
                      itemList.RemoveAt(i);

                      ItemDataWrapper itemDataWrapper = new ItemDataWrapper();
                      itemDataWrapper.itemDataList = itemList;
                      string jsonData = JsonUtility.ToJson(itemDataWrapper);
                      File.WriteAllText(Application.dataPath + "/_TTSDK/DataBaseSystem/Json/itemData.json", jsonData);
                 }
              }

              EditorGUILayout.EndHorizontal();

          }

          EditorGUILayout.EndScrollView();
    }
}
}
