using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPointers : MonoBehaviour {
    public static Camera mainCamera;
    public static Transform ItemParent;
    public static Transform buildingParent;
    public static float itemHeight = 1.34f;
    public static Building[] buildingPrefabs;
    public static Item[] itemPrefabs;
    public static bool showDebug;

    [SerializeField] Transform itemParentInput;
    [SerializeField] Transform buildingParentInput;
    [SerializeField] Transform testMap;
    [SerializeField] Building[] buildingPrefabsInput;
    [SerializeField] Item[] itemPrefabsInput;
    [SerializeField] bool showDebugInput;
    
    void Awake() {
        testMap.gameObject.SetActive(false);

        mainCamera = Camera.main;
        ItemParent = itemParentInput;
        buildingParent = buildingParentInput;
        showDebug = showDebugInput;
        buildingPrefabs = buildingPrefabsInput;
        itemPrefabs = itemPrefabsInput;
        for(int i = 0; i < buildingPrefabs.Length; i++) {
            buildingPrefabs[i].buildingType = i;
        }
    }

    //prevents placing a belt from throwing an error
    public void PlacedBelt() {}
}
