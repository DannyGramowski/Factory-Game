using Factory.Buildings;
using Factory.Core;
using System;
using UnityEngine;

namespace Factory.UI {
    public class SpawnButtons : MonoBehaviour {
        Building[] buildingPrefabs;

        private void Awake() {
            buildingPrefabs = GlobalPointers.buildingPrefabs;
        }

        public void SpawnBuilding(int spawnNum) {
            if (spawnNum >= buildingPrefabs.Length) throw new Exception(spawnNum + " is larger than buildingPrefabs with a length of " + buildingPrefabs.Length);
            SpawnBuilding(buildingPrefabs[spawnNum]);
        }

        private Building SpawnBuilding(Building building) {
            Building b = Instantiate(building, Input.mousePosition, Quaternion.identity, GlobalPointers.buildingParent);
            b.SetShowDebug(GlobalPointers.showDebug);
            InputManager.Instance.PlacingBuilding = b;
            return b;
        }
    }
}