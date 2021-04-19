using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Miner : ProductionBuilding, ISelectItem {
    [SerializeField] Item spawnItem;
    [SerializeField] float productionPerSec;

    float currProduction;
    /*protected override void Awake() {
        base.Awake();
    }*/

    public override void Placed() {
        base.Placed();
    }

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
            // throw new Exception("could not add " + item + "  to " + name);
            }
        }  
    }

    public override Item ItemOut(Item filterItem) {
        if(filterItem) {
            return inventory.GetItem(filterItem);
        } else {
            return inventory.GetFirstItem();
        }
    }

    public override bool ItemOutValid(Item filterItem) {
        if(filterItem) {
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



    /* private void OnDrawGizmos() {
         if (baseCell) {
             Vector3 pos = Grid.Instance.GetCell(baseCell.pos + Vector2Int.one + (Utils.Vector2FromDirection(direction) * 2)).transform.position;
             Gizmos.DrawCube(pos, Vector3.one);
         }
     }*/
}
