using Factory.Buildings;
using Factory.Core;
using UnityEngine;

namespace Factory.Units.BaseUnits {
    public interface IItemCarrier {
        public int MaxStackSize();
        public bool ValidItem(string item);
        
        //public void PickUpItem(ItemStack items);
        public ItemStack GetPickUpStack();
        public int GetPickupSize();
        public int GetDeliverySize();
        public ItemStack GetDeliverStack();

        public Transform GetParent();
        //public ItemStack DeliverItem(int maxSize);
    }
}