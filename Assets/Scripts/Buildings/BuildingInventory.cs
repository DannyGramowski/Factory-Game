using System;
using System.Collections.Generic;
using UnityEngine;
using Factory.Core;
using Factory.Saving;
using Sirenix.Serialization;

namespace Factory.Buildings {
    [Serializable]
    public class BuildingInventory {
        [SerializeField] private List<ItemStack> inventory = new List<ItemStack>();
        [SerializeField] private int maxStacks;
        private Filter _filer = new Filter();
        public BuildingInventory(int itemStackSlots) {
            // Debug.Log("constructed building inventory");
            maxStacks = itemStackSlots;
            for (int i = 0; i < maxStacks; i++) {
                inventory.Add(new ItemStack());
            }
        }

        public bool AddItems(ItemStack stack, int amount) {
            bool Selector(ItemStack itemStack) => !itemStack.IsFull();
            if (amount == -1) amount = stack.GetSize();
            while (amount > 0) {
                var itemStack = GetItemStack(stack.StackType, selector: Selector, includeEmptyIfNone: true);
                int startSize = itemStack.GetSize();
                if (itemStack is null || !itemStack.AddItem(stack, amount)) return false;
                amount -= itemStack.GetSize() - startSize;
            }

            return true;
        }

        public bool AddItems(Item item) {
            bool Selector(ItemStack itemStack) => !itemStack.IsFull();

            var itemStack = GetItemStack(item.itemName, selector: Selector, includeEmptyIfNone: true);
            return itemStack is not null && itemStack.AddItem(item);
        }

        //if stack is null will fill up with first items
        public bool GetItems(ItemStack stack, int amount) {
            bool Selector(ItemStack itemStack) => !itemStack.IsEmpty();
            if (amount == -1) amount = stack.GetCapacityLeft();
            while (0 < amount) {
                var itemStack = GetItemStack(stack.StackType, selector: Selector);
                int startAmount = stack.GetSize();
                if (itemStack is null || !itemStack.GetItems(stack, amount)) return false;
                amount -= stack.GetSize() - startAmount;
            }

            return true;
        }

        public Item GetItems(string itemType) {
            bool Selector(ItemStack itemStack) => !itemStack.IsEmpty();
            var itemStack = GetItemStack(itemType, Selector);
            return itemStack?.GetItems();
        }
        
        /*public Item GetFirstItemType() {
            foreach (var stack in inventory) {
                if(stack.IsEmpty()) continue;
                return stack.StackType;
            }

            return null;
        }*/
        private ItemStack GetItemStack(string itemType, Func<ItemStack, bool> selector = null,  bool includeEmptyIfNone = false, int startNum = 0) {
            ItemStack emptyStack = null;
            if (itemType is "") return GetEmptyStack();
            for (var i = startNum; i < inventory.Count; i++) {
                var itemStack = inventory[i];
                if (itemStack.IsEmpty()) {
                    if (includeEmptyIfNone && emptyStack is null) emptyStack = itemStack;
                    continue;
                }
                if (itemStack.StackType.Equals(itemType) && (selector is null || selector(itemStack))) return itemStack;
            }

            return emptyStack; //will return null if include empty if none is false
        }

        private ItemStack GetEmptyStack() {
            foreach (var stack in inventory) {
                if (stack.IsEmpty()) return stack;
            }

            return null;
        }
        
        public object Save() {
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

    public class Filter {
        
    }
    
    [Serializable]
    public class ItemStack {
        [SerializeField] List<Item> stack;
        public string StackType => _stackType;
        [SerializeField]private string _stackType = "";

        public bool ValidItem(string itemName) => _stackType is "" || (_stackType.Equals(itemName) && stack.Count < stack[0].stackSize);
        
        public bool AddItem(Item item) {
             if (_stackType is "") {
                _stackType = item.itemName;
                stack = new List<Item>(item.stackSize);
                stack.Add(item);
                return true;
             } else if (ValidItem(item.itemName)) {
                 stack.Add(item);
                 return true;
             }
            return false;
        }

        public bool AddItem(ItemStack newStack, int amount) {
            if (!ValidItem(newStack.StackType)) return false;
            if (_stackType is "") {
                _stackType = newStack.stack[0].itemName;
                stack = new List<Item>(newStack.stack[0].stackSize);
                stack.Add(newStack.GetItems());//gives stack an item to get size from
                amount--;
            }

            var min = Math.Min(newStack.GetSize(), amount);
            var capacity = GetCapacityLeft();
            var addAmount = Math.Min(min, capacity);
            for (int i = 0; i < addAmount; i++) {   
                stack.Add(newStack.GetItems());
            }
            return true;
        }

        private Item Pop() {
            var temp = stack[^1];
            stack.RemoveAt(stack.Count - 1);
            return temp;
        }
        
        
        public Item GetItems() {
            if (stack.Count > 0) {
                Item temp = Pop();
                if (stack.Count == 0) {
                    Reset();
                }
                return temp;
            }
            return null;
        }

        // if amount is -1 will pull as many items as it can to stack
        public bool GetItems(ItemStack incomingStack, int amount) {
            if (!ValidItem(incomingStack.StackType) || _stackType is null) return false;
            var takeAmount =  Math.Min(Math.Min(incomingStack.GetCapacityLeft(), amount == -1 ? int.MaxValue : amount), GetSize());
            
            for (int i = 0; i < takeAmount; i++) {   
                incomingStack.stack.Add(Pop());
            }
            if(stack.Count == 0) Reset();
            return true;
        }

        public int GetSize() {
            if (stack is null) return 0;
            return stack.Count;
        }

        public int GetCapacityLeft() {
            if (_stackType is "") return 0;
            if (stack.Count == 0) throw new Exception("no item in stack");
            return stack[0].stackSize - GetSize();
        } 
        public bool IsFull() {
            if (IsEmpty()) {
                return false;
            }
            return (stack.Count == stack[0].stackSize);
        }

        public bool IsEmpty() {
            return stack is null;
        }
        
        private void Reset() {
            _stackType = "";
        }

        public SVector2 Save() {
            return new SVector2(Item.ItemIndex(stack[0]),stack.Count);
        }

        public void Load(SVector2 v2) {
            Vector2Int temp = v2.ToVectorInt();
            for(int i = 0; i < temp.y; i++) {
                Item item = Item.SpawnItem(temp.x);
                AddItem(item);
            }
        }
        
        public override string ToString() {
            if (_stackType == null) return "empty";

            return _stackType + " item stack";
        }
    }
    
    /*public int GetEmptyStackIndex() {
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

        public Stack<Item> GetItems(Item itemType, int amount) {
            var output = new Stack<Item>(amount);
            var index = 0;
            do {
                var stack = GetItemStack(itemType);
                if (stack is null) break;
                while (!stack.IsEmpty()) {
                    output.Push(stack.GetItem());
                    index++;
                    if (index == amount) return output;
                }
            } while (true);

            return index == 0 ? null : output;
        }
        public Item GetItem(Item item) {
            return GetItemStack(item).GetItem();
        }

        public void TakeItems(Stack<Item> items) {
            do {
                var stack = GetItemStack(items.Peek());
                if(stack == null) return;
                while (!stack.IsFull()) {
                    stack.AddItem(items.Pop());
                    if (items.Count == 0) return;
                }
            } while (true);


        }
        
        public ItemStack GetEmptyItemStack() {
            foreach (ItemStack itemStack in inventory) {
                if (itemStack.StackType is not null) {
                    return itemStack;
                }
            }
            return null;
        }

        public ItemStack GetItemStack(Item item) {
            return GetItemStack(item, 0);
        }

        public ItemStack GetItemStack(Item item, int start) {
            foreach (var stack in inventory) {
                if (!stack.IsEmpty() && stack.StackType.Equals(item)) return stack;
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
        
        public bool HasSpace(Item item, int amount = 1) {
            foreach (var stack in inventory) {
                if (stack.StackType == item && stack.GetCapacityLeft >= amount) return true;
            }

            return HasEmptyStack();
        }

        public bool HasEmptyStack() {
            return GetEmptyItemStack() != null;
        }

        public bool AddItemToStack(Item[] items) {
            ItemStack itemStack = GetItemStack(items[0]) ?? GetEmptyItemStack();
            if (itemStack != null) {
                int i = 0;
                while (i < items.Length && itemStack.AddItem(items[i])) {
                    i++;
                }
                return items.Length == i;//returns true if it added all of the items
            }
           
            return false;
        }
        
        public bool AddItemToStack(Item item) {
            ItemStack itemStack = GetItemStack(item);
            if (itemStack != null) {
                return itemStack.AddItem(item);
            }
            itemStack = GetEmptyItemStack();
            if (itemStack != null) {
                return itemStack.AddItem(item);
            }
            return false;
        }

        public int GetNumberStacks => inventory.Count;
        
        
    }

    [System.Serializable]
    public class ItemStack {
        [SerializeField] Stack<Item> stack = new Stack<Item>();
        public Item StackType { get; private set; }
        private Item _stackType;

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
        public int GetCapacityLeft => _stackType.stackSize - GetSize();

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
        }*/
}