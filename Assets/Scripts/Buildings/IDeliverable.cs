using Factory.Core;
using UnityEngine;

namespace Factory.Buildings {
    public interface IDeliverable {
        public void Deliver(Item[] items);
        public Vector3 GetPosition();
        public int GetMaxDeliverySize();
    }
}