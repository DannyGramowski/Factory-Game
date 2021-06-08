using System.Collections.Generic;
using UnityEngine;
using Factory.Buildings;
using Factory.Core;

namespace Factory.UI {
    public class DisplayItemsUI : MonoBehaviour, UI {
        [SerializeField] ItemDisplay ItemDisplayPrefab;

        List<ItemDisplay> itemDisplays = new List<ItemDisplay>();
        List<Item> displayItems = new List<Item>();
        ProducableBuildings type;

        void Awake() {
            for (int i = 0; i < GlobalPointers.itemPrefabs.Count; i++) {
                ItemDisplay temp = Instantiate(ItemDisplayPrefab, transform);
                itemDisplays.Add(temp);
            }
        }

        public void SetDisplayType(ProducableBuildings type) {
            this.type = type;
            UpdateUI();
        }
        public void UpdateUI() {
            displayItems.Clear();
            foreach (Item i in GlobalPointers.itemPrefabs) {
                if (i.ValidBuilding(type)) {
                    displayItems.Add(i);
                }
            }
            for (int i = 0; i < itemDisplays.Count; i++) {
                if (i < displayItems.Count) {
                    itemDisplays[i].SetActive(true);
                    itemDisplays[i].SetItemDisplay(displayItems[i]);
                } else {
                    itemDisplays[i].SetActive(false);
                }
            }
        }
    }
}
