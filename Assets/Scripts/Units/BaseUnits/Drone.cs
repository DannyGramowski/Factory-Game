using System.Collections.Generic;
using Factory.Buildings;
using Factory.Core;
using Factory.Units.Actions;
using UnityEngine;

namespace Factory.Units.BaseUnits
{
    public class Drone : Unit, IItemCarrier {
        [SerializeField] protected int stackCapacity = 1;
        [SerializeField] private float speed = 5;
        [SerializeField] private float turnSpeed = 360;
        
        [SerializeField] protected ItemStack _cargo = new ItemStack();

        protected static Dictionary<string, IAction> _validActions;

        public int MaxStackSize() => stackCapacity - _cargo.GetSize();

        public bool ValidItem(string itemName) {
            if (_cargo.IsEmpty()) return true;
            return _cargo.StackType is null || _cargo.ValidItem(itemName);
        }


        /*public void PickUpItem(ItemStack items) {
            _cargo.AddItem(items, MaxStackSize());
        }*/

        public ItemStack GetPickUpStack() => _cargo;
        public int GetPickupSize() => MaxStackSize();

        public int GetDeliverySize() => _cargo.GetSize();

        public ItemStack GetDeliverStack() => _cargo;
        public Transform GetParent() => transform;

        public string GetItemType() => _cargo.StackType;

        /*public void DeliverItem(ItemStack stock,int maxSize) {
            int deliverySize = maxSize > _cargo.GetSize() ? _cargo.GetSize() : maxSize;
            stock.AddItem(_cargo, deliverySize);
        }*/

        public float GetSpeed() => speed;
        public float GetTurnSpeed() => turnSpeed;
    }
}