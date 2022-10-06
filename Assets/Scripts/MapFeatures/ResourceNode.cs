using System.Collections.Generic;
using Factory.Buildings;
using Factory.Core;
using UnityEngine;

namespace Factory.MapFeatures {
    [SelectionBase]
    public class ResourceNode : MonoBehaviour, IPickupable{
        [SerializeField] private float timeToHarvest = 1;
        [SerializeField] private Item producingItem;
        [SerializeField] private Transform droneTarget;

        public Item HarvestResource() {
            Item item = Instantiate(producingItem,transform);
            return item;
        }

        public float GetHarvestTime() => timeToHarvest;

        public Vector3 GetPosition() => droneTarget.position;
        public void Pickup(ItemStack itemStack, int amount) { }

        public Stack<Item> Pickup(Item itemType, int amount) {
            return null;
        }
    }
}