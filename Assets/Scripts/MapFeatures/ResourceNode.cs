using Factory.Buildings;
using Factory.Core;
using UnityEngine;

namespace Factory.MapFeatures {
    [SelectionBase]
    public class ResourceNode : MonoBehaviour, IPickupable{
        [SerializeField] private float timeToHarvest = 1;
        [SerializeField] private Item producingItem;

        public Item HarvestResource() {
            Item item = Instantiate(producingItem);
            return item;
        }

        public float GetHarvestTime() => timeToHarvest;

        public Item[] Pickup(Item itemType) {
            return null;
        }

        public Vector3 GetPosition() => transform.position;
    }
}