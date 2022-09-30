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
                if (inventory[i].StackType.Equals(item)) return i;
            }
            return -1;
        }

        public Item GetFirstItem() {
            foreach (ItemStack itemStack in inventory) {
                if (itemStack.StackType != null) return itemStack.GetItem();
            }
            return null;
        }

        public Item GetItem(Item item) {
            return GetItemStack(item).GetItem();
        }

        public ItemStack GetEmptyItemStack() {
            foreach (ItemStack itemStack in inventory) {
                if (!itemStack.StackType) {
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
                if (!inventory[i].IsEmpty() && inventory[i].StackType.Equals(item)) return inventory[i];
            }
            return null;
        }

        public ItemStack GetItemStack(int index) {
            Debug.Log("get stack item index " + index);
            return inventory[index];
        }

        public bool HasItem(Item item) {
            foreach (ItemStack itemStack in inventory) {
                if (itemStack.StackType == item) return true;
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
        [SerializeField] Stack<Item> stack = new Stack<Item>();
        public Item StackType { get; private set; }
        private Item _stackType = null;

        public bool ValidItem(Item item) {
            if (_stackType is not null) Debug.Assert(_stackType.stackSize > 0, "you need to give " + _stackType.itemName + " a stack size greater than 0");
            return _stackType is not null && _stackType.Equals(item) && stack.Count < _stackType.stackSize;
        }

        public bool AddItem(Item item) {
            if (ValidItem(item)) {
                stack.Push(item);
                return true;
            } else if (stack is not null) {
                stack.Push(item);
                _stackType = stack.Peek();
                return true;
            }
            return false;
        }

        public int GetSize() => stack.Count;

        public Item GetItem() {
            if (stack.Count > 0) {
                Item temp = stack.Pop();
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
            return (stack.Count == _stackType.stackSize);
        }

        public bool IsEmpty() {
            return stack is null;
        }

        //item type, number of items in list
        public SVector2 Save() {
            return new SVector2(Item.ItemIndex(_stackType),stack.Count);
        }

        public void Load(SVector2 v2) {
            Vector2Int temp = v2.ToVectorInt();
            for(int i = 0; i < temp.y; i++) {
                Item item = Item.SpawnItem(temp.x);
                AddItem(item);
            }
        }

        private void Reset() {
            StackType = null;
        }

        public override string ToString() {
            if (_stackType == null) return "empty";

            return _stackType.itemName + " item stack";
        }
    }
}