using Factory.Core;
using UnityEngine;

namespace Factory.Buildings {
    public interface IPickupable {
        
        public Item[] Pickup(Item itemtype);
        public Vector3 GetPosition();
    }
}