using Factory.Buildings;
using Factory.Core;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Factory.UI {
    public class SpawnButtons : MonoBehaviour {
        Building[] buildingPrefabs;
        Button buttonTemplate;

        private void Awake() {
            buildingPrefabs = GlobalPointers.buildingPrefabs;
            buttonTemplate = Resources.Load<Button>("Building Button");
            GenerateButtons();
        }

        public void SpawnBuilding(Building building) {
            Building b = Instantiate(building, Input.mousePosition, Quaternion.identity, GlobalPointers.buildingParent);
            b.SetShowDebug(GlobalPointers.showDebug);
            InputManager.Instance.PlacingBuilding = b;
        }

        void GenerateButtons() {

            foreach (Building building in buildingPrefabs) {
                CreateButton(building.GetName(), delegate { SpawnBuilding(building); });
            }
            CreateButton("save", delegate { Saving.SaveSystem.Instance.Save(); });
            CreateButton("load", delegate { Saving.SaveSystem.Instance.Load(); });

        }

        void CreateButton(string name, UnityAction action) {
            Button temp = Instantiate(buttonTemplate, transform);
            temp.transform.GetComponentInChildren<Text>().text = name;
            temp.onClick.AddListener(action);
        }
    }
}