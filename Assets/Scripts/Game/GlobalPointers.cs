using System.Collections.Generic;
using UnityEngine;
using Factory.Buildings;
using System.Linq;

namespace Factory.Core {

    [ExecuteAlways]
    public class GlobalPointers : Singleton<GlobalPointers> {
        public static Transform ItemParent;
        public static Transform buildingParent;
        public static Transform testMap;
        public static bool showDebug;

        public static float itemHeight = 1.34f;
        public static Camera mainCamera;

        public static BeltSystem beltSystemPrefab;
       public static Building[] buildingPrefabs;
        public static List<Item> itemPrefabs;
        public Item templateItem;

        [SerializeField] Transform itemParentInput;
        [SerializeField] Transform buildingParentInput;
        [SerializeField] Transform testMapInput;

        [SerializeField] bool showDebugInput;

        private void OnValidate() {
            Awake();
        }
        void Awake() {
            testMap = testMapInput;
            testMap.gameObject.SetActive(false);

            mainCamera = Camera.main;
            ItemParent = itemParentInput;
            buildingParent = buildingParentInput;
            showDebug = showDebugInput;

            beltSystemPrefab = (Resources.Load("BeltSystem") as GameObject).GetComponent<BeltSystem>();
            itemPrefabs = Utils.GetAssets<Item>("", new[] { Utils.ITEM_FOLDER_PATH });
            buildingPrefabs = Utils.GetAssets<Building>("", new[] { Utils.BUILDING_FOLDER_PATH }).ToArray();
/*            itemPrefabs = Resources.LoadAll<Item>("Items").ToList();
            buildingPrefabs = Resources.LoadAll<Building>("Building");*/
            for (int i = 0; i < buildingPrefabs.Length; i++) {
                buildingPrefabs[i].buildingType = i;
            }
        }

    }
}