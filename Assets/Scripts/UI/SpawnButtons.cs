using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnButtons : MonoBehaviour {
    Building[] buildingPrefabs;

    private void Awake() {
        buildingPrefabs = GlobalPointers.buildingPrefabs;
    }

    public void SpawnBuilding(int spawnNum) {
        if (spawnNum >= buildingPrefabs.Length) throw new Exception(spawnNum + " is larger than buildingPrefabs with a length of " + buildingPrefabs.Length);
        //print("spawn " + buildingPrefabs[spawnNum].name + " button pressed");
        SpawnBuilding(buildingPrefabs[spawnNum]);
    }

    private Building SpawnBuilding(Building building) {
        Building b = Instantiate(building,Input.mousePosition,Quaternion.identity,GlobalPointers.buildingParent);
        b.ReduceNamingNum();
        b.SetShowDebug(GlobalPointers.showDebug);
        InputManager.Instance.placingBuilding = b;
        b.transform.eulerAngles = InputManager.Instance.buildingRot;
        return b;
    }
}
