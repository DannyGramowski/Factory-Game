using System.Collections.Generic;
using UnityEngine;
using Factory.Core;
using Factory.MapFeatures;
using Factory.Units.BaseUnits;

namespace Factory.Buildings {
    public abstract class ProductionBuilding : Building, IPickupable, IDeliverable {
        public List<DroneLine> droneLines = new List<DroneLine>();
        [SerializeField] protected BuildingInventory inventory;
        [SerializeField] int numberOfStacks = 3;
        [SerializeField] private Transform droneTarget;


        protected override void Awake() {
            //Debug.Assert(numberOfStacks > 0 || this is Assembler, "you need to set the number of inventory slots");
            inventory = new BuildingInventory(numberOfStacks);
            base.Awake();

            
        }

        public override void Place(Direction placeDirection) {
            //SetUpGrabberSpots();
            base.Place(placeDirection);
        }

        protected override void OverrideLoad(Dictionary<string, object> dict) {
            inventory.Load(dict["buildingInventory"]);
            base.OverrideLoad(dict);
        }

        protected override void OverrideSave(Dictionary<string, object> dict) {
            dict["buildingInventory"] = inventory.Save();
            base.OverrideSave(dict);
        }

        protected void AddDroneLine(DroneLine droneLine) {
            droneLines.Add(droneLine);
        }

        /*private void SetUpGrabberSpots() {
            foreach (GrabberSpot g in grabberSpots) {
                g.SetGrabberCell(this);
            }
        }

        public void SetGrabberSpots(List<GrabberSpot> grabberSpots) {
            this.grabberSpots.Clear();
            this.grabberSpots = grabberSpots;
        }

        public GrabberSpot AddGrabber(Cell loc, Grabber g, bool input) {
            GrabberSpot grabberSpot = HasGrabberSpot(loc);
            
            if (grabberSpot != null && grabberSpot.connectedGrabber == null) {
                grabberSpot.connectedGrabber = g;
                if (input && grabberSpot.spotType != IOType.output) {
                    inGrabbers.Add(g);
                    return grabberSpot;
                } else if (!input && grabberSpot.spotType != IOType.input) {
                    outGrabbers.Add(g);
                    return grabberSpot;
                }
            }
            return null;
        }

        public GrabberSpot HasGrabberSpot(Cell checkCell) {
            Vector2Int offSet = checkCell.pos - baseCell.pos;
            foreach (GrabberSpot g in grabberSpots) {
                if (offSet.Equals(g.offSet)) {
                    return g;
                }
            }
            return null;
        }*/

        #region pickup and deliver
        public Vector3 GetPosition() => droneTarget.position;
        public void Pickup(ItemStack itemStack, int amount) {
            inventory.GetItems(itemStack, amount);
        }
        
        public void Deliver(ItemStack items, int amount) {
            inventory.AddItems(items, amount);
        }
        #endregion
        
        public virtual void ItemIn(Item item) { }

        public virtual bool ItemInValid(Item item) { return false; }

        public virtual Item ItemOut(Item filterItem) { return null; }

        public virtual bool ItemOutValid(Item filterItem) { return false; }
        
    }

    /*[System.Serializable]
    public class GrabberSpot {
        public Vector2Int offSet;
        public Cell cell;
        public IOType spotType;
        public Grabber connectedGrabber;
        public List<Direction> directions;

        private ProductionBuilding productionBuilding;

        public override string ToString() {
            return productionBuilding.ToString() + ": "
                + offSet.ToString();
        }

        public object Save() {
            return null;
        }
        public void SetGrabberCell(ProductionBuilding productionBuilding) {
            this.productionBuilding = productionBuilding;
            cell = Core.Grid.Instance.GetCell(productionBuilding.GetBaseCell().pos + offSet);
            for (int i = 0; i < directions.Count; i++) {
                directions[i] = Utils.AddDirection(directions[i], productionBuilding.direction);
            }
        }

        public GrabberSpot(Vector2Int offSet, List<Direction> setDirections, IOType ioType) {
            this.offSet = offSet;
            directions = setDirections;
            spotType = ioType;
        }
    }*/

    public enum IOType {
        input = -1,
        output = 1,
        both = 0
    }
}
