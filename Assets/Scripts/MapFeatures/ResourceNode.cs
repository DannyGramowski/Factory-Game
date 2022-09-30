using Factory.Buildings;
using Factory.Core;
using UnityEngine;

namespace Factory.MapFeatures {
    [SelectionBase]
    public class ResourceNode : MonoBehaviour {
        [SerializeField] private float timeToHarvest = 1;
        [SerializeField] private Item producingItem;

        public Item HarvestResource() {
            Item item = Instantiate(producingItem);
            return item;
        }

        public float GetHarvestTime() => timeToHarvest;
    }
}