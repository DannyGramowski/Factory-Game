using System.Collections.Generic;
using Factory.Core;
using UnityEngine;

namespace Factory.Buildings {
    public interface IPickupable: IPosition {
        
        public Stack<Item> Pickup(Item itemType, int amount);
    }
}