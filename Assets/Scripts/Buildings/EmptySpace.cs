using Factory.Core;
using UnityEngine;

namespace Factory.Buildings {
    [RequireComponent(typeof(BoxCollider))]
    public class EmptySpace : MonoBehaviour {
        new Collider collider;
        private bool isEmpty;
        private Item item;
        bool change;
        void Start() {
            collider = GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (change) isEmpty = true;
            change = true;
            item = null;

        }

        private void OnTriggerStay(Collider other) {
            isEmpty = false;
            change = false;
            item = other.GetComponent<Item>();
        }

        public bool IsEmpty() {
            return isEmpty;
        }

        public Item GetItem() {
            return item;
        }
    }
}
