using System.Collections.Generic;
using UnityEngine;
using Factory.Core;
using Factory.Saving;

namespace Factory.Buildings {
    [System.Serializable]
    public class BuildingInventory {
        [SerializeField] List<ItemStack> inventory = new List<ItemStack>();
        [SerializeField] int maxStacks;

        public BuildingInventory(int itemStackSlots) {
           // Debug.Log("constructed building inventory");
            maxStacks = itemStackSlots;
            for (int i = 0; i < maxStacks; i++) {
                inventory.Add(new ItemStack());
            }
        }

        public int GetEmptyStackIndex() {
            for (int i = 0; i < inventory.Count; i++) {
                if (inventory[i].IsEmpty()) return i;
            }
            return -1;
        }

        public int GetStackIndex(Item item) {
            for (int i = 0; i < inventory.Count; i++) {
                if (inventory[i].stackType.Equals(item)) return i;
            }
            return -1;
        }

        public Item GetFirstItem() {
            foreach (ItemStack itemStack in inventory) {
                if (itemStack.stackType != null) return itemStack.GetItem();
            }
            return null;
        }

        public Item GetItem(Item item) {
            return GetItemStack(item).GetItem();
        }

        public ItemStack GetEmptyItemStack() {
            foreach (ItemStack itemStack in inventory) {
                if (!itemStack.stackType) {
                    return itemStack;
                }
            }
            return null;
        }

        public ItemStack GetItemStack(Item item) {
            return GetItemStack(item, 0);
        }

        public ItemStack GetItemStack(Item item, int start) {
            for (int i = 0; i < inventory.Count; i++) {
                if (!inventory[i].IsEmpty() && inventory[i].stackType.Equals(item)) return inventory[i];
            }
            return null;
        }

        public ItemStack GetItemStack(int index) {
            Debug.Log("get stack item index " + index);
            return inventory[index];
        }

        public bool HasItem(Item item) {
            foreach (ItemStack itemStack in inventory) {
                if (itemStack.stackType == item) return true;
            }
            return false;
        }

        public bool HasSpace(Item item) {
            int startNum = 0;
            do {
                ItemStack itemStack = GetItemStack(item, startNum);
                if (itemStack != null) {
                    if (!itemStack.IsFull()) {
                        return true;
                    } else {
                        startNum++;
                    }
                } else {
                    break;
                }
            } while (startNum < inventory.Count);
            return HasEmptyStack();
        }

        public bool HasEmptyStack() {
            return GetEmptyItemStack() != null;
        }

        public bool AddItemToStack(Item item) {
            ItemStack itemStack = GetItemStack(item);
            //        Debug.Log("item stack " + itemStack);
            if (itemStack != null) {
                return itemStack.AddItem(item);
            }
            itemStack = GetEmptyItemStack();
            if (itemStack != null) {
                return itemStack.AddItem(item);
            }
            return false;
        }

        public object Save() {
            //Dictionary<string, object> dict = new Dictionary<string, object>();
            List<SVector2> list = new List<SVector2>();
            foreach(ItemStack stack in inventory) {
                list.Add(stack.Save());
            }
            return list;
        }

        public void Load(object obj) {

            List<SVector2> list = (List<SVector2>) obj;
            if (inventory.Count == 0) return;
            for(int i = 0; i < list.Count; i++) {
                inventory[i].Load(list[i]);
            }
        }
    }

    [System.Serializable]
    public class ItemStack {
        [SerializeField] List<Item> stack = new List<Item>();
        public Item stackType;

        public bool ValidItem(Item item) {
            if (stackType != null) Debug.Assert(stackType.stackSize > 0, "you need to give " + stackType.itemName + " a stack size greater than 0");
            return stackType != null && stackType.Equals(item) && stack.Count < stackType.stackSize;
        }

        public bool AddItem(Item item) {
            if (ValidItem(item)) {
                stack.Add(item);
                return true;
            } else if (!stackType) {
                stack.Add(item);
                stackType = stack[0];
                return true;
            }
            return false;
        }

        public Item GetItem() {
            if (stack.Count > 0) {
                Item temp = stack[0];
                stack.RemoveAt(0);
                if (stack.Count == 0) {
                    Reset();
                }
                return temp;
            }
            return null;
        }

        public bool IsFull() {
            if (IsEmpty()) {
                return false;
            }
            return (stack.Count == stackType.stackSize);
        }

        public bool IsEmpty() {
            return stackType == null;
        }

        //item type, number of items in list
        public SVector2 Save() {
            return new SVector2(Item.ItemIndex(stackType),stack.Count);
        }

        public void Load(SVector2 v2) {
            Vector2Int temp = v2.ToVectorInt();
            for(int i = 0; i < temp.y; i++) {
                Item item = Item.SpawnItem(temp.x);
                AddItem(item);
            }
        }

        private void Reset() {
            stackType = null;
        }

        public override string ToString() {
            if (stackType == null) return "empty";

            return stackType.itemName + " item stack";
        }
    }
}