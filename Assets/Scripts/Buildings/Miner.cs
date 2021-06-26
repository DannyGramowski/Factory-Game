using Factory.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Factory.Buildings {
    public class Miner : ProductionBuilding, ISelectableBuilding {
        [SerializeField] Item spawnItem;
        [SerializeField] float productionPerSec;

        float currProduction;

        private void Update() {
            if (currProduction <= 0) {
                SpawnItem();
            } else {
                currProduction -= productionPerSec * Time.deltaTime;
            }

        }

        void SpawnItem() {
            if (spawnItem != null && inventory.HasSpace(spawnItem)) {
                Item item = Instantiate(spawnItem, transform.position, Quaternion.identity, GlobalPointers.ItemParent);
                if (inventory.AddItemToStack(item)) {
                    item.Deactivate();
                    currProduction = spawnItem.productionCost;
                    Debug.Assert(currProduction > 0, "you need to set the production value of " + spawnItem);
                } else {
                    Destroy(item.gameObject);
                }
            }
        }

        public override Item ItemOut(Item filterItem) {
            if (filterItem) {
                return inventory.GetItem(filterItem);
            } else {
                return inventory.GetFirstItem();
            }
        }

        public override bool ItemOutValid(Item filterItem) {
            if (filterItem) {
                return inventory.HasItem(filterItem);
            } else {
                return inventory.GetFirstItem() != null;
            }
        }

        public int UINum() {
            return 1;
        }

        public ProducableBuildings ProducableBuildingsType() {
            return ProducableBuildings.miner;
        }

        public void SetItem(Item item) {
            spawnItem = item;
        }

        protected override void OverrideLoad(Dictionary<string, object> dict) {
            int itemNum = (int) dict["item num"];
            spawnItem = itemNum != -1 ? GlobalPointers.itemPrefabs[itemNum] : null;
            currProduction = (float) dict["curr production"];
            base.OverrideLoad(dict);
        }

        protected override void OverrideSave(Dictionary<string, object> dict) {
            dict["item num"] = GlobalPointers.itemPrefabs.IndexOf(spawnItem);
            dict["curr production"] = currProduction;
            base.OverrideSave(dict);
        }
        /* private void OnDrawGizmos() {
             if (baseCell) {
                 Vector3 pos = Grid.Instance.GetCell(baseCell.pos + Vector2Int.one + (Utils.Vector2FromDirection(direction) * 2)).transform.position;
                 Gizmos.DrawCube(pos, Vector3.one);
             }
         }*/
    }
}