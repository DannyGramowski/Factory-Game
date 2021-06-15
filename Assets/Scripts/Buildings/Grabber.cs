using UnityEngine;
using Factory.Core;


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
        Belt connectedBelt;
        GrabberSpot grabberSpot;
        ProductionBuilding connectedBuilding;
        Timer timer;
        bool toBuilding;//if true belt to building
        bool endPlaced = false;

        Item movingItem = null;
        Item filterItem;

        bool showDebug;
        //Cell baseCell;
        Building hoverBuilding;

        protected override void Awake() {
            timer = GetComponent<Timer>();
            base.Awake();
            if (modelLocalScale.Equals(Vector3.zero)) {
                modelLocalScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, model.transform.localScale.z);
            }
            endPlaced = false;
            ResetModel();
        }

        private void Update() {
            UpdateItem();
        }
 
        public bool HasConnectedBuilding() {
            return connectedBuilding != null;
        }

        public bool HasConnectedBelt() {
            return connectedBelt != null;
        }

        public bool HasGrabberSpot() {
            return grabberSpot != null;
        }

        public bool HasBothConnections() {
            return HasConnectedBelt() && HasGrabberSpot();
        }

        public bool EndPlaced() {
            return endPlaced;
        }

        public override void Place(Direction direc) {
            if (hoverBuilding == null) return;

            if (ValidPlacment(baseCell.pos)) {
                Belt belt = hoverBuilding as Belt;
                if (belt != null) {
                    if (belt.grabber == null) {
                        AddBelt(belt, HasConnectedBuilding());
                    }
                } else {
                    AddBuilding(baseCell, hoverBuilding as ProductionBuilding, HasConnectedBelt());
                }
            }
        }

        public override void OnHover(Cell hoverCell) {
            if(baseCell == hoverCell) return; //you dont need to update it if hover is the same
            //base.OnHover(hoverCell);
            baseCell = hoverCell;
            //print("on hover " + baseCell);
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
                grabberSpot.cell.building = null;
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
            arrow.color = movingItem != null ? Color.green : Color.red;

            if (movingItem) {
                if (timer.TimerDone()) {
                    timer.StartTimer();
                    if (toBuilding) {
                        if (connectedBuilding.ItemInValid(movingItem)) {
                            connectedBuilding.ItemIn(movingItem);
                            movingItem = null;
                        }
                    } else {
                        movingItem.AddToBeltSystem(connectedBelt);
                        movingItem.transform.position = connectedBelt.itemPos;
                        movingItem.Activate();
                        movingItem = null;
                    }
                }
            } else if (toBuilding) {
                if (emptySpace.item && (filterItem == null || emptySpace.item.Equals(filterItem))) {
                    movingItem = emptySpace.item;
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
            //      this.baseCell = currHover;
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
            //print("angle " + angle);
            position.eulerAngles = Utils.Vector3SetY(transform.eulerAngles, angle);
            model.transform.localScale = new Vector3(modelLocalScale.x * offSet.magnitude, modelLocalScale.y, modelLocalScale.z * 0.6f);
        }

        private bool ValidPlacment(Vector2Int currHover) {
            if (!EndPlaced()) {
               // print("no end placed");
                return true;//allows it to be placed if it does not have one end placed
            }
            if (HasBothConnections()) return false;
            Cell hover = Core.Grid.Instance.GetCell(currHover);
            Building b = hover.building;
            Vector2 placedPos;
            GrabberSpot spot;

            if (b == null) return false;

            if (b is ProductionBuilding) {
                print("connected building check");
                if(connectedBuilding != null) return false;
                ProductionBuilding production = b as ProductionBuilding;

                GrabberSpot g = production.HasGrabberSpot(hover);
                if (g.connectedGrabber != null) return false;
                // placedPos = g.cell.pos;
                placedPos = connectedBelt.GetBaseCell().pos;
                spot = g;
            } else if (b is Belt) {
                print("connected belt check");
                if(connectedBelt != null) return false;
                Belt belt = b as Belt;
               
                if (belt.grabber != null) {
                    print("grabber not null");
                    return false;
                }

                print("set placed pos");
                //placedPos = belt.GetBaseCell().pos;
                placedPos = grabberSpot.cell.pos;
                spot = grabberSpot;
            } else {
                return false;
            }
            
            Vector2 offSet = placedPos - currHover;
            //print("spot is " + spot);
            foreach (Direction direc in spot.directions) {
                Vector2 direcVector = Utils.Vector2FromDirection(direc);

                //  print($"dot offset{Vector2.Dot(direcVector, offSet)} {direcVector} {offSet}");
                // if (Vector2.Dot(direcVector, offSet) == 1) {
                print($"offset {offSet.normalized} direc {direcVector}");
                if (offSet.normalized.Equals(direcVector)) {
                    print("returned " + (offSet.magnitude <= range));
                    return offSet.magnitude <= range;
                }
            }
            print("no valid directions");
            return false;
        }

        private void AddBuilding(Cell clickedLoc, ProductionBuilding b, bool input) {
            connectedBuilding = b;
            grabberSpot = connectedBuilding.AddGrabber(clickedLoc, this, input);
            if (grabberSpot != null && connectedBelt) {
                print("add building " + name);
                endPlaced = true;
                toBuilding = input;
                emptySpace.transform.position = connectedBelt.itemPos;
                timer.StartUp(timePerCell * (Vector2Int.Distance(connectedBelt.GetBaseCell().pos, grabberSpot.cell.pos)));
            } else {
                connectedBuilding = null;//if there is no valid grabber it will not allow it to be placed
                grabberSpot = null;
            }
        }

        private void AddBelt(Belt b, bool input) {
            print("add belt " + name);
            endPlaced = true;
            connectedBelt = b;
            b.grabber = this;
            print($"set {b} grabber to {b.grabber}");
            emptySpace.transform.position = b.itemPos;
            if (grabberSpot != null) {
                timer.StartUp(timePerCell * Vector2Int.Distance(connectedBelt.GetBaseCell().pos, grabberSpot.cell.pos));
            }
        }

        private void OnDrawGizmos() {//cyan for the input and magenta for the output
            if (showDebug) {
                /*  Gizmos.color = Color.white;
                  Gizmos.DrawSphere(currHover.itemPos, 0.5f);*/
                if (HasConnectedBelt()) {
                    if (toBuilding) {
                        Gizmos.color = Color.cyan;
                    } else {
                        Gizmos.color = Color.magenta;
                    }
                    Gizmos.DrawCube(connectedBelt.itemPos, Vector3.one * 0.5f);
                }

                if (HasGrabberSpot()) {
                    if (toBuilding) {
                        Gizmos.color = Color.magenta;
                    } else {
                        Gizmos.color = Color.cyan;
                    }
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

