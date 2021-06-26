using System.Collections.Generic;
using UnityEngine;
using Factory.Buildings;
using Factory.Saving;

namespace Factory.Core {

    public class Item : MonoBehaviour {
        public string itemName;
        public Sprite sprite;
        public int stackSize = 100;
        public float productionCost;

        [SerializeField] float errorDistance = 0.5f;

        public ProducableBuildings producableBuilding;
        public List<Item> recipe;

        BeltSystem beltSystem;
        Belt nextBelt;
        Belt currBelt;

        float time;
        bool onBelt;
        bool moving;

        public static Item SpawnItem(int index) {
            return SpawnItem(index, Vector3.zero);
        }

        public static Item SpawnItem(int index, Vector3 position) {
            return index != -1 ? Instantiate(GlobalPointers.itemPrefabs[index], GlobalPointers.ItemParent) : null;
        }

        public static int ItemIndex(Item item) {
            return GlobalPointers.itemPrefabs.IndexOf(item);
        }

        private void FixedUpdate() {
            if (onBelt && moving && nextBelt) {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, nextBelt.itemPos, nextBelt.speed / 60 * time);
                float distance = Vector3.Distance(transform.position, nextBelt.itemPos);
                if (distance < errorDistance) {
                    currBelt = nextBelt;
                    nextBelt = beltSystem.NextBelt(currBelt);
                    time = 0;
                }
            }
            moving = true;
        }


        private void OnTriggerStay(Collider other) {
            if (other.GetComponent<Item>()) {
                moving = false;
            }
        }


        public bool ValidBuilding(ProducableBuildings type) {
            if (type == ProducableBuildings.all || producableBuilding == type) return true;

            return false;
        }

        public override bool Equals(object other) {
            //Debug.Assert(itemName != null, "you need to give " + name + " an item name");
          //  Debug.Assert((other as Item).itemName != null, "you need to give " + (other as Item).name + " an item name");
            if (!(other is Item)) return false;
            return itemName.Equals((other as Item).itemName);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public void SetSprite(Sprite sprite) {
            this.sprite = sprite;
            GetComponentInChildren<SpriteRenderer>().sprite = this.sprite;
        }

        public void AddToBeltSystem(Belt belt) {
            this.beltSystem = belt.beltSystem;
            currBelt = belt;
            nextBelt = beltSystem.NextBelt(belt);
            onBelt = true;
            moving = true;
        }

        public void RemoveFromBeltSystem() {
            onBelt = false;
            moving = false;
        }

        public void Deactivate() {
            //transform.position = Vector3.zero;
            gameObject.SetActive(false);
        }

        public void Activate() {
            gameObject.SetActive(true);
        }
    }

    //type of building its produced in
    public enum ProducableBuildings {
        miner,
        assember,
        all,
        none
    }
}