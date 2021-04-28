using System.Collections.Generic;
using UnityEngine;

public class Assembler : ProductionBuilding, ISelectItem {
    [SerializeField] float productionPerSec;

    new Dictionary<string, ItemStack> inventory = new Dictionary<string, ItemStack>();
    Item producingItem;
    Item[] recipeItems;
    float currProduction;
    int newItemIndex;

    private void Update() {
        if (currProduction > 0) {
            currProduction -= productionPerSec * Time.deltaTime;
        }
        if (currProduction <= 0) {
            //print(GetRecipeItems() + " " + !GetFromInventory(producingItem).IsFull());
            if (GetRecipeItems() && !GetFromInventory(producingItem).IsFull()) {
                CreateItem();
                currProduction = producingItem.productionCost;
                Debug.Assert(currProduction > 0, "you need to set the production value of " + producingItem);
            }
        }
    }

    public void SetItem(Item item) {
        //print("set item " + item);
        producingItem = item;
        newItemIndex = producingItem.recipe.Count;
        recipeItems = new Item[producingItem.recipe.Count];
        inventory.Clear();
        foreach (Item i in producingItem.recipe) {
            inventory.Add(i.itemName, new ItemStack());
        }
        inventory.Add(producingItem.itemName, new ItemStack());
    }

    private void CreateItem() {
        Item temp = Instantiate(producingItem, GlobalPointers.ItemParent);
        temp.Deactivate();
        GetFromInventory(temp).AddItem(temp);
        for (int i = 0; i < recipeItems.Length; i++) {

            Destroy(recipeItems[i].gameObject);
        }
    }

    private bool GetRecipeItems() {
        if (recipeItems == null) return false;
        bool output = true;
        for (int i = 0; i < recipeItems.Length; i++) {
            if (recipeItems[i] == null) {
                ItemStack test1 = GetFromInventory(producingItem.recipe[i]);
                Item test2 = test1.GetItem();
                recipeItems[i] = test2;
                output &= recipeItems[i] != null;
            }
        }
        return output;
    }

    private int getItemIndex(Item item) {
        if (producingItem == null) return -1;

        for (int i = 0; i < producingItem.recipe.Count; i++) {
            if (producingItem.recipe[i].Equals(item)) return i;
        }
        return -1;
    }

    private ItemStack GetFromInventory(Item i) {
        return inventory[i.itemName];
    }

    public override void ItemIn(Item item) {//fix these
        if (producingItem == null) return;
        ItemStack temp = GetFromInventory(item);
        // print("item in added to " + temp.stackType);
        temp.AddItem(item);

    }

    public override bool ItemInValid(Item item) {
        ItemStack temp = GetFromInventory(item);
        return temp != null && !temp.IsFull();
    }

    public override Item ItemOut(Item filterItem) {
        if (producingItem == null) return null;

        ItemStack temp = GetFromInventory(producingItem);
        if ((filterItem != null && temp.stackType == filterItem) || filterItem == null) return temp.GetItem();

        return null;
    }

    public override bool ItemOutValid(Item filterItem) {
        if (producingItem == null) return false;

        ItemStack temp = GetFromInventory(producingItem);
        if ((filterItem != null && temp.stackType == filterItem) || filterItem == null) return !temp.IsEmpty();

        return false;
    }

    public int UINum() {
        return 2;
    }

    public ProducableBuildings ProducableBuildingsType() {
        return ProducableBuildings.assember;
    }
}
