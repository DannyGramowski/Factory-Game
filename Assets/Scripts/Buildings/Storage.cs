using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : ProductionBuilding {

    public override bool ItemInValid(Item item) {
        return inventory.HasSpace(item);
    }

    public override bool ItemOutValid(Item filterItem) {
        return base.ItemOutValid(filterItem);
    }

    public override void ItemIn(Item item) {
        inventory.AddItemToStack(item);
    }

    public override Item ItemOut(Item filterItem) {
        if(filterItem) {
            return inventory.GetItemStack(filterItem).GetItem();
        }
        Item item = inventory.GetFirstItem();
        if (item) {
            print("item out " + item);
            return item;
        } else {
            return null;
        }
    }
}
