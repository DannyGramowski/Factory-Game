using System.Collections.Generic;
using UnityEngine;
using Factory.Buildings;

namespace Factory.Core {

    [ExecuteAlways]
    public class GlobalPointers : Singleton<GlobalPointers> {
        public static Camera mainCamera;
        public static Transform ItemParent;
        public static Transform buildingParent;
        public static float itemHeight = 1.34f;
        public static Building[] buildingPrefabs;
        public static BeltSystem beltSystemPrefab;
        public static List<Item> itemPrefabs;
        public static bool showDebug;

        [SerializeField] Transform itemParentInput;
        [SerializeField] Transform buildingParentInput;
        [SerializeField] Transform testMap;
        [SerializeField] Building[] buildingPrefabsInput;
        [SerializeField] BeltSystem beltSystemPrefabInput;
        [SerializeField] List<Item> itemPrefabsInput;
        [SerializeField] bool showDebugInput;

        void Awake() {
            testMap.gameObject.SetActive(false);

            mainCamera = Camera.main;
            ItemParent = itemParentInput;
            buildingParent = buildingParentInput;
            showDebug = showDebugInput;
            buildingPrefabs = buildingPrefabsInput;
            beltSystemPrefab = beltSystemPrefabInput;
            ReloadItems();
            for (int i = 0; i < buildingPrefabs.Length; i++) {
                buildingPrefabs[i].buildingType = i;
            }
        }

        public void ReloadItems() {
            itemPrefabs = Utils.GetAssets<Item>("", new[] { Utils.ITEM_FOLDER_PATH });
            itemPrefabsInput = itemPrefabs;
        }
    }
}