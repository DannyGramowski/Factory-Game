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

        new void Start() {
            print("start");
            //base.Start();
            if (_validActions == null) {
                _validActions = new Dictionary<string, IAction>();
                foreach (var action in GlobalActionList.Actions) {
                    if (actionName.Contains(action.ActionName())) {
                        _validActions[action.ActionName()] = action;
                    }
                }
            }

            //ExecuteAction(new ADroneMove(), new object[][] { new object[]{transform, endPt, speed, turnSpeed}, EmptyArgs, EmptyArgs});
        }

        public int MaxStackSize() => stackCapacity - _cargo.GetSize();

        public bool ValidItem(Item item) {
            if (_cargo.IsEmpty()) return true;
            return _cargo.StackType is null || _cargo.ValidItem(item);
        }


        public void PickUpItem(ItemStack itemStack) {
            while (_cargo.GetSize() < stackCapacity) {
                if(itemStack.IsEmpty()) return;
                _cargo.AddItem(itemStack.GetItem());
            }                
        }

        

        public Item[] DeliverItem(int maxSize) {
            int deliverySize = maxSize > stackCapacity ? stackCapacity : maxSize;
            Item[] output = new Item[deliverySize];
            for (int i = 0; i < deliverySize; i++) {
                output[i] = _cargo.GetItem();
            }

            return output;
        }
         
        public override bool ValidAction(IAction action) => _validActions != null && _validActions.ContainsKey(action.ActionName());
    }
}