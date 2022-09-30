using UnityEngine;
using Factory.Core;
using Factory.Saving;
using System.Collections.Generic;

namespace Factory.Buildings {
    [RequireComponent(typeof(Timer))]
    public class Grabber : Building, ISelectableBuilding {
        [SerializeField] Transform model;
        [SerializeField] Transform position;
        [SerializeField] EmptySpace emptySpace;
        [SerializeField] float timePerCell;
        [SerializeField] int range;//number of cells it can go
        [SerializeField] SpriteRenderer arrow;

        static Vector3 modelLocalScale;
        Belt connectedBelt;//
        GrabberSpot grabberSpot;//
        ProductionBuilding connectedBuilding;
        Timer timer;//
        bool toBuilding;//if true belt to building//
        bool endPlaced = false;

        Item movingItem = null;//
        Item filterItem;//

        bool showDebug;
      //  Collider collider;
        Building hoverBuilding;
        Core.Grid grid;
        protected override void Awake() {
            timer = GetComponent<Timer>();
            base.Awake();
            if (modelLocalScale.Equals(Vector3.zero)) {
                modelLocalScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, model.transform.localScale.z);
            }
            endPlaced = false;
            grid = Core.Grid.Instance; 
            ResetModel();
        }

        private void Update() {
            UpdateItem();
        }

        public bool HasConnectedBuilding() {
            return !connectedBuilding.Equals(null);
        }

        public bool HasConnectedBelt() {
            return !connectedBelt.Equals(null);
        }

        public bool HasGrabberSpot() {
            return !grabberSpot.Equals(null);
        }

        public bool HasBothConnections() {
            return HasConnectedBelt() && HasGrabberSpot();
        }

        public bool EndPlaced() {
            return endPlaced;
        }

        public override void Place(Direction direc) {
            if (hoverBuilding.Equals(null)) return;

            if (ValidPlacment(baseCell.pos)) {
                Belt belt = hoverBuilding as Belt;
                if (belt is not null) {
                    if (belt.grabber.Equals(null)) {
                        AddBelt(belt, !HasConnectedBuilding());
                    }
                } else {
                    AddBuilding(baseCell, hoverBuilding as ProductionBuilding, HasConnectedBelt());
                }
            }

            if(BuildingPlaced()) {
                timer.StartUp(timePerCell * (Vector2Int.Distance(connectedBelt.GetBaseCell().pos, grabberSpot.cell.pos)));
            }
        }

        public override void OnHover(Cell hoverCell) {
            if (baseCell == hoverCell) return; //you dont need to update it if hover is the same
            baseCell = hoverCell;
            if (ValidPlacment(hoverCell.pos)) {
                hoverBuilding = hoverCell.building;
                SetModel(hoverCell);
            }
        }

        public override bool BuildingPlaced() {
            return HasBothConnections();
        }

        public override void Deconstruct() {
            if (HasGrabberSpot()) {
                grabberSpot.connectedGrabber = null;
            }
            if (connectedBelt) {
                connectedBelt.grabber = null;
            }

            base.Deconstruct();
        }

        public override void CancelPlace(Cell currHover) {
            Deconstruct();
        }

        public override void SetShowDebug(bool showDebug) {
            this.showDebug = showDebug;
        }


        public int UINum() {
            return 0;
        }

        public ProducableBuildings ProducableBuildingsType() {
            return ProducableBuildings.all;
        }

        public void SetItem(Item item) {
            filterItem = item;
        }

        private void UpdateItem() {
            arrow.color = movingItem is not null ? Color.green : Color.red;

            if (movingItem) {
                if (timer.TimerDone()) {
                    timer.StartTimer();
                    if (toBuilding) {
                        if (connectedBuilding.ItemInValid(movingItem)) {
                            connectedBuilding.ItemIn(movingItem);
                            movingItem = null;
                        }
                    } else {
                        if (emptySpace.IsEmpty()) {
                            movingItem.AddToBeltSystem(connectedBelt);
                            movingItem.transform.position = connectedBelt.itemPos;
                            movingItem.Activate();
                            movingItem = null;
                        }
                    }
                }
            } else if (toBuilding) {
                if (emptySpace.GetItem() && (filterItem == null || emptySpace.GetItem().Equals(filterItem))) {
                    movingItem = emptySpace.GetItem();
                    movingItem.RemoveFromBeltSystem();
                    movingItem.Deactivate();
                }
            } else {
                if (connectedBelt && connectedBuilding) {
                    movingItem = connectedBuilding.ItemOut(filterItem);
                }
            }
        }

        private void ResetModel() {
            connectedBelt = null;
            connectedBuilding = null;
            grabberSpot = null;
            model.localScale = modelLocalScale;
        }

        private void SetModel(Cell currHover) {
            if (!EndPlaced()) {
                transform.position = currHover.itemPos;
                return;
            }

            Cell from = null;
            Cell to = null;
            if (toBuilding) {
                from = connectedBelt?.GetBaseCell() ?? currHover;
                to = grabberSpot?.cell ?? currHover;
            } else {
                from = grabberSpot?.cell ?? currHover;
                to = connectedBelt?.GetBaseCell() ?? currHover;
            }

            position.position = Vector3.Lerp(from.itemPos, to.itemPos, 0.5f);
            Vector2Int offSet = from.pos - to.pos;

            float angle = -Utils.Angle(offSet);


            position.eulerAngles = Utils.Vector3SetY(transform.eulerAngles, angle);
            model.transform.localScale = new Vector3(modelLocalScale.x * offSet.magnitude, modelLocalScale.y, modelLocalScale.z * 0.6f);

            GetComponent<BoxCollider>().size = new Vector3(modelLocalScale.x * offSet.magnitude, modelLocalScale.y, modelLocalScale.z * 0.6f);
            GetComponent<BoxCollider>().center = position.localPosition;
            
            //arrow.transform.LookAt(to.itemPos);
            arrow.transform.eulerAngles = toBuilding ? new Vector3(90, 180, 0) : new Vector3(90, 0, 0);
        }

        private bool ValidPlacment(Vector2Int currHover) {
            if (!EndPlaced()) return true; 
            if (HasBothConnections())return false;

            Cell hover = Core.Grid.Instance.GetCell(currHover);
            Building b = hover.building;
            Vector2 buildingPos;
            Vector2 beltPos;
            GrabberSpot spot;

            if (b == null) return false;

            if (b is ProductionBuilding) {
                ProductionBuilding production = b as ProductionBuilding;
                GrabberSpot g = production.HasGrabberSpot(hover);

                if (connectedBuilding != null || g == null || g.connectedGrabber != null)  return false;
                buildingPos = g.cell.pos;
                beltPos = connectedBelt.GetBaseCell().pos;
                spot = g;
            } else if (b is Belt) {
                if (connectedBelt != null)
                    return false;
                Belt belt = b as Belt;

                if (belt.grabber != null) return false;

                buildingPos = grabberSpot.cell.pos;
                beltPos = belt.GetBaseCell().pos;
                spot = grabberSpot;
            } else {
                return false;
            }

            Vector2 offSet = beltPos - buildingPos;
            foreach (Direction direc in spot.directions) {
                Vector2 direcVector = Utils.Vector2FromDirection(direc);
                if (offSet.normalized.Equals(direcVector)) {
                    return offSet.magnitude <= range;
                }
            }
            return false;
        }

        private void AddBuilding(Cell clickedLoc, ProductionBuilding b, bool input) {
            connectedBuilding = b;
            grabberSpot = connectedBuilding.AddGrabber(clickedLoc, this, input);
            if (grabberSpot != null) {
                toBuilding = input;
                endPlaced = true;
            } else {
                connectedBuilding = null;//if there is no valid grabber it will not allow it to be placed
                grabberSpot = null;
            }
        }

        private void AddBelt(Belt b, bool input) {
            endPlaced = true;
            connectedBelt = b;

            toBuilding = input;
            b.grabber = this;
            emptySpace.transform.position = b.itemPos;
        }
        protected override void OverrideSave(Dictionary<string, object> dict) {
            dict["belt"] = new SVector2(connectedBelt.GetBaseCell().pos);
            dict["spot"] = new SVector2(grabberSpot.cell.pos);
            dict["timer"] = timer.Save();
            dict["toBuilding"] = toBuilding;
            dict["movingItem"] = movingItem;
            dict["filterItem"] = filterItem;
        }

        protected override void OverrideLoad(Dictionary<string, object> dict) {
            toBuilding = (bool)dict["toBuilding"];
            timer.Load(dict["timer"]);
            movingItem = dict.ContainsKey("movingItem") ? dict["movingItem"] as Item : null;
            filterItem = dict.ContainsKey("filerItem") ? dict["filerItem"] as Item : null;

            Cell buildingCell = grid.GetCell(((SVector2)dict["spot"]).ToVectorInt());
            Cell beltCell = grid.GetCell(((SVector2)dict["belt"]).ToVectorInt());
           
            if(toBuilding) {
                AddBuilding(buildingCell, buildingCell.building as ProductionBuilding, true);
                OnHover(beltCell);
                AddBelt(beltCell.building as Belt, false);
            } else {
                AddBelt(beltCell.building as Belt, false);
                OnHover(buildingCell);
                AddBuilding(buildingCell, buildingCell.building as ProductionBuilding, false);
            }
        }


        private void OnDrawGizmos() {//cyan for the input and magenta for the output
            if (showDebug) {
                /*  Gizmos.color = Color.white;
                  Gizmos.DrawSphere(currHover.itemPos, 0.5f);*/
                if (HasConnectedBelt())
                {
                    Gizmos.color = toBuilding ? Color.cyan : Color.magenta;
                    Gizmos.DrawCube(connectedBelt.itemPos, Vector3.one * 0.5f);
                }

                if (HasGrabberSpot())
                {
                    Gizmos.color = toBuilding ? Color.magenta : Color.cyan;
                    Gizmos.DrawCube(grabberSpot.cell.itemPos, Vector3.one * 0.5f);
                }

                if (HasBothConnections()) {
                    Gizmos.color = Color.grey;
                    Gizmos.DrawLine(connectedBelt.itemPos, grabberSpot.cell.itemPos);
                }

                //addes cube to middle of model
                Gizmos.color = Color.white;
                Gizmos.DrawCube(model.transform.position, Vector3.one * 0.25f);

                //addes cube to empty space
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(emptySpace.transform.position, Vector3.one * 0.25f);
            }
        }
    }
}

