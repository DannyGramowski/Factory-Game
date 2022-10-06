using System.Collections.Generic;
using UnityEngine;
using Factory.Core;
using Factory.Saving;
using System.Linq;

namespace Factory.Buildings {
    public class Assembler : ProductionBuilding, ISelectableBuilding {
        [SerializeField] float productionPerSec = 200;

        new Dictionary<string, ItemStack> inventory = new Dictionary<string, ItemStack>();
        Item producingItem;
        Item[] recipeItems;
        float currProduction;
    //    int newItemIndex;

        private void Update() {
            if (currProduction > 0) {
                currProduction -= productionPerSec * Time.deltaTime;
            }
            if (currProduction <= 0) {
                //print(GetRecipeItems() + " " + !GetFromInventory(producingItem).IsFull());
                if (GetRecipeItems() && !GetFromInventory(producingItem).IsFull()) {//has recipie items and there is space for the new item to go
                    CreateItem();
                    currProduction = producingItem.productionCost;
                    Debug.Assert(currProduction > 0, "you need to set the production value of " + producingItem);
                }
            }
        }

        public void SetItem(Item item) {
            //print("set item " + item);
            producingItem = item;
            //newItemIndex = producingItem.recipe.Count;
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
                    Item test2 = test1.GetItems();
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
            if (!inventory.ContainsKey(i.itemName)) return null;
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
            if ((filterItem != null && temp.StackType == filterItem.itemName) || filterItem == null) return temp.GetItems();

            return null;
        }

        public override bool ItemOutValid(Item filterItem) {
            if (producingItem == null) return false;

            ItemStack temp = GetFromInventory(producingItem);
            if ((filterItem != null && temp.StackType == filterItem.itemName) || filterItem == null) return !temp.IsEmpty();

            return false;
        }

        protected override void OverrideLoad(Dictionary<string, object> dict) {
            base.OverrideLoad(dict);

           // producingItem = ];
            currProduction = (float)dict["currProduction"];
            SetItem(GlobalPointers.itemPrefabs[(int)dict["production item"]]);
            LoadInventory(dict["buildingInventory"]);

            int[] recipieNums = (int[])dict["recipieNums"];
            for(int i = 0; i < recipeItems.Length; i++) {
                recipeItems[i] = Item.SpawnItem(recipieNums[i]);
            }
        }

        protected override void OverrideSave(Dictionary<string, object> dict) {
            base.OverrideSave(dict);

            dict["production item"] = Item.ItemIndex(producingItem);
            dict["currProduction"] = currProduction;
            dict["buildingInventory"] = SaveInventory();//replaces the building inventory in production building;

            int[] recipieItemNums = new int[recipeItems.Length];
            for(int i = 0; i < recipieItemNums.Length; i++) recipieItemNums[i] = Item.ItemIndex(recipeItems[i]);
            dict["recipieNums"] = recipieItemNums;
        }

        private object SaveInventory() {
            //List<KeyValuePair<string, >> pairs = new List<KeyValuePair<string, object>>; 
            List<SVector2> items = new List<SVector2>();
            foreach(var stack in inventory) {
                items.Add(stack.Value.Save());
            }
            return items;
        }

        private void LoadInventory(object obj) {
            List<SVector2> items = (List<SVector2>)obj;
            for(int i = 0; i < items.Count ; i++) {
                Vector2Int temp = items[i].ToVectorInt();
                Item item = Item.SpawnItem(temp.x);
                print("item " + item);
                if(item != null) inventory[item.itemName].AddItem(item);
            }
        }
        

        public int UINum() {
            return 2;
        }

        public ProducableBuildings ProducableBuildingsType() {
            return ProducableBuildings.assember;
        }
    }
}