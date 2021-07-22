using System.Collections.Generic;
using UnityEngine;
using Factory.Buildings;
using Factory.Saving;

namespace Factory.Core {

    [SelectionBase][RequireComponent(typeof(SavingEntity))]
    public class Item : MonoBehaviour, ISaveable {
        public string itemName;
        public Sprite sprite;
        public int stackSize = 100;
        public float productionCost = 300;

        [SerializeField] float errorDistance = 0.5f;
        [SerializeField] float checkDistance = 0.2f;
        [SerializeField] LayerMask itemLayer;

        public ProducableBuildings producableBuilding;
        public List<Item> recipe;

        static int namingNum = 0;//reset on load

        BeltSystem beltSystem;// from curr belt;
        Belt nextBelt;
        Belt currBelt;//

        float time;//
        bool onBelt;//if it is saved this is true
        bool moving;//set in fixed update
        Direction movingDirec;//from currBelt

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
            namingNum++;
            name = itemName +" " + namingNum.ToString();
        }

        private void FixedUpdate() {
            moving = !Infront();
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
           /* Item b = other.GetComponent<Item>();
            if (b != null && !Infront(b)) {
              //  print("stop moving");
                moving = false;
            }*/
        }

        private void SetNextBelt() {
            nextBelt = beltSystem.NextBelt(currBelt);
            //if(nextBelt != null && beltSystem != nextBelt.beltSystem ) {
            if(nextBelt != null) {
                beltSystem.BeltAdded -= OnBeltAdded;
                beltSystem = nextBelt.beltSystem;
                beltSystem.BeltAdded += OnBeltAdded;
            }
            movingDirec = currBelt.direction;
        }


        void OnBeltAdded() {
            print(name + " on belt added");
            SetNextBelt();
        }

        private bool Infront() {

            Vector2 direc = Utils.Vector2FromDirection(movingDirec);
           
            return Physics.Raycast(transform.position, new Vector3(direc.x, 0, direc.y), checkDistance, itemLayer);
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
        //    Ray ray = new Ray(transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, direction);
        }

        public object Save() {
            if (!onBelt) return null;

            Dictionary<string, object> save = new Dictionary<string, object>();
            save["itemType"] = Item.ItemIndex(this);
            save["guid"] = GetComponent<SavingEntity>().GetUniqueIdentifier();
            save["pos"] = new SVector3(transform.position);
            save["beltPos"] = new SVector2(currBelt.GetBaseCell().pos);
            save["time"] = time;

            print("saved " + name);
            return save;
        }

        public void Load(object state) {
            object value = ((KeyValuePair<string, object>)state).Value;
            Dictionary<string, object> save = (Dictionary<string, object>)value;

            GetComponent<SavingEntity>().SetUniqueIdentifier(save["guid"] as string);
            transform.position =  (save["pos"] as SVector3).ToVector();
            time = (float)save["time"];

            Vector2Int beltPos = (save["beltPos"] as SVector2).ToVectorInt();
            Cell beltCell = Grid.Instance.GetCell(beltPos);
            currBelt = beltCell.building as Belt;

            beltSystem = currBelt.beltSystem;
            nextBelt = beltSystem.NextBelt(currBelt);
            movingDirec = currBelt.direction;
            onBelt = true;
            
        }

        public SavingType SaveType() {
            return SavingType.item;
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