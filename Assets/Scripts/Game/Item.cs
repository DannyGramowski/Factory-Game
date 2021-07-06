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
        [SerializeField] float checkDistance = 0.1f;

        public ProducableBuildings producableBuilding;
        public List<Item> recipe;

        BeltSystem beltSystem;
        Belt nextBelt;
        Belt currBelt;

        float time;
        bool onBelt;
        bool moving;
        Direction movingDirec;

        bool debug = false;

        public static Item SpawnItem(int index) {
            return SpawnItem(index, Vector3.zero);
        }

        public static Item SpawnItem(int index, Vector3 position) {
            return index != -1 ? Instantiate(GlobalPointers.itemPrefabs[index], GlobalPointers.ItemParent) : null;
        }

        public static int ItemIndex(Item item) {
            return GlobalPointers.itemPrefabs.IndexOf(item);
        }

        private void Start() {
            debug = GlobalPointers.showDebug;
        }

        private void FixedUpdate() {
            
            if (onBelt && moving && nextBelt) {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, nextBelt.itemPos, nextBelt.speed / 60 * time);
                float distance = Vector3.Distance(transform.position, nextBelt.itemPos);
                if (distance < errorDistance) {
                    currBelt = nextBelt;
                    SetNextBelt();
                    time = 0;
                }
            }
            moving = true;
        }


        private void OnTriggerStay(Collider other) {
            Item b = other.GetComponent<Item>();
            if (b != null && !Infront(b)) {
              //  print("stop moving");
                moving = false;
            }
        }

        private void SetNextBelt() {
            nextBelt = beltSystem.NextBelt(currBelt);
            //if(nextBelt != null && beltSystem != nextBelt.beltSystem ) {
            if(nextBelt != null) {
                beltSystem.BeltAdded -= OnBeltAdded;
                beltSystem = nextBelt.beltSystem;
                beltSystem.BeltAdded += OnBeltAdded;
            }
            //movingDirec = currBelt.direction;
        }


        void OnBeltAdded() {
            print(name + " on belt added");
            SetNextBelt();
        }

        private bool Infront(Item other) {
            /*   if (nextBelt == null) return false;
               Vector3 offset = (transform.position - other.transform.position).normalized;
               Vector2Int direcOffset = Utils.Vector2FromDirection(nextBelt.direction);
   *//*
               print($"position offset is {offset}, other pos {other.transform.position}, curr pos {transform.position}");
               print($"direc offset is {direcOffset} direc is {movingDirec}");
               print($"returned {offset.x == direcOffset.x} && {offset.z == direcOffset.y}");*//*

               return offset.x == direcOffset.x && offset.z == direcOffset.y;*/
            Vector2 direc = Utils.Vector2FromDirection(movingDirec);
            Ray ray = new Ray(transform.position, new Vector3(direc.x, 0, direc.y) * checkDistance);
            return Physics.Raycast(ray);
           // return false;
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
            SetNextBelt();
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

        private void OnDrawGizmos() {
            if (!debug) return;

            Vector2 direc = Utils.Vector2FromDirection(movingDirec);
            Vector3 direction = new Vector3(direc.x, 0, direc.y) * checkDistance;
            print("direction " + direction);
            Ray ray = new Ray(transform.position,direction);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray);
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