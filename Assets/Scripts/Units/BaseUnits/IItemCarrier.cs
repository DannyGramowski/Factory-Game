using System.Collections;
using System.Collections.Generic;
using Factory.Buildings;
using Factory.Core;

namespace Factory.Units.BaseUnits {
    public interface IItemCarrier {
        public int MaxStackSize();
        public bool ValidItem(Item item);
        
        public void PickUpItem(Stack<Item> items);
        public Stack<Item> DeliverItem(int maxSize);
    }
}