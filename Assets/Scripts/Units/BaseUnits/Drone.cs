using System.Collections.Generic;
using Factory.Buildings;
using Factory.Core;
using Factory.Units.Actions;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;

namespace Factory.Units.BaseUnits
{
    public class Drone : Unit, IItemCarrier {
        [SerializeField] protected int stackCapacity = 1;
        [SerializeField] private float speed = 5;
        [SerializeField] private float turnSpeed = 360;
        
        protected ItemStack _cargo = new ItemStack();

        protected static Dictionary<string, IAction> _validActions;

        public int MaxStackSize() => stackCapacity - _cargo.GetSize();

        public bool ValidItem(Item item) {
            if (_cargo.IsEmpty()) return true;
            return _cargo.StackType is null || _cargo.ValidItem(item);
        }


        public void PickUpItem(Item[] items) {
            int i = 0;
            while (_cargo.GetSize() < stackCapacity) {
                if(items.Length <= i) return;
                _cargo.AddItem(items[i]);
                i++;
            }                
        }

        public Item GetItemType() => _cargo.StackType;

        public Item[] DeliverItem(int maxSize) {
            int deliverySize = maxSize > stackCapacity ? stackCapacity : maxSize;
            Item[] output = new Item[deliverySize];
            for (int i = 0; i < deliverySize; i++) {
                output[i] = _cargo.GetItem();
            }

            return output;
        }

        public float GetSpeed() => speed;
        public float GetTurnSpeed() => turnSpeed;
    }
}