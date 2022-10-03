using System.Collections.Generic;
using Factory.Core;
using UnityEngine;

namespace Factory.Buildings {
    public interface IDeliverable: IPosition {
        public void Deliver(Stack<Item> items);
        public int GetMaxDeliverySize(Item itemType);
    }
}