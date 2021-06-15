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

        //Building hoverBuilding;
        //bool hoverOnBuilding = false;

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
            if (hoverBuilding == null)
                return;

            if (hoverBuilding is ProductionBuilding) {
                if (!HasConnectedBuilding()) {
                    //print($"connected building valid{ValidPlacment(currHover.pos)}");
                    // print("cur hover pos " + baseCell.pos);
                    if (ValidPlacment(baseCell.pos)) {
                        AddBuilding(baseCell, hoverBuilding as ProductionBuilding, HasConnectedBelt());
                    }
                    //else    print("not valid placement");
                }
            } else if (hoverBuilding is Belt) {
                Belt belt = hoverBuilding as Belt;
                if (!HasConnectedBelt()) {
                    //    print($"connected belt valid{ValidPlacment(baseCell.pos)}");

                    if (ValidPlacment(baseCell.pos) && belt.grabber == null) {
                        AddBelt(belt, HasConnectedBuilding());
                    }
                    //else print("not valid placment");
                }
            }
        }

        public override void OnHover(Cell hoverCell) {
            //base.OnHover(hoverCell);
            baseCell = hoverCell;
            //print("on hover " + baseCell);
            hoverBuilding = hoverCell.building;
            SetModel(hoverCell);

            //if(hoverBuilding == null) return;
            /*
                        if (hoverBuilding is ProductionBuilding) {
                            if (!HasConnectedBuilding()) {
                                //print($"connected building valid{ValidPlacment(currHover.pos)}");
                               // print("cur hover pos " + baseCell.pos);
                                if (ValidPlacment(baseCell.pos)) {
                                    AddBuilding(baseCell, hoverBuilding as ProductionBuilding, HasConnectedBelt());
                                } else print("not valid placement");
                            }
                        } else if (hoverBuilding is Belt) {
                            if (!HasConnectedBelt()) {
                            //    print($"connected belt valid{ValidPlacment(baseCell.pos)}");

                                if (ValidPlacment(baseCell.pos))
                                    AddBelt(hoverBuilding as Belt, HasConnectedBuilding());
                                else
                                    print("not valid placment");
                            }
                        }*/
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
            print("connected belt " + connectedBelt);
            connectedBuilding = null;
            grabberSpot = null;
            model.localScale = modelLocalScale;
            print("reset model");

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

        private bool ValidPlacment(Vector2 currHover) {
            if (!EndPlaced())
                return true;//allows it to be placed if it does not have one end placed
            if (HasBothConnections())
                return false;

            Vector2Int placedPos = (Vector2Int)(connectedBelt?.GetBaseCell()?.pos ?? grabberSpot?.cell.pos);
            Vector2 offSet = (currHover - placedPos);
            foreach (Direction direc in grabberSpot.directions) {
                Vector2 direcVector = Utils.Vector2FromDirection(direc);
                if (direcVector.Equals(offSet.normalized)) {
                    return offSet.magnitude <= range;
                }
            }
            return false;
        }

        private void AddBuilding(Cell clickedLoc, ProductionBuilding b, bool input) {
            endPlaced = true;
            connectedBuilding = b;
            toBuilding = input;
            grabberSpot = connectedBuilding.AddGrabber(clickedLoc, this, input);
            if (grabberSpot != null && connectedBelt) {
                emptySpace.transform.position = connectedBelt.itemPos;
                timer.StartUp(timePerCell * (Vector2Int.Distance(connectedBelt.GetBaseCell().pos, grabberSpot.cell.pos)));
            }
        }

        private void AddBelt(Belt b, bool input) {
            endPlaced = true;
            connectedBelt = b;
            b.grabber = this;
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
                    Gizmos.DrawCube(grabberSpot.
                        cell.
                        itemPos, Vector3.one * 0.5f);
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

//void AddGrabber() {
/*   Building hoverBuilding = currHover.building;

   if (hoverBuilding is ProductionBuilding) {
       if (!g.HasConnectedBuilding()) {
           print($"connected building valid{g.ValidPlacment(currHover.pos)}");
           if (g.ValidPlacment(currHover.pos)) g.AddBuilding(currHover, hoverBuilding as ProductionBuilding, g.HasConnectedBelt());
           else print("not valid placement");
       }
   } else if (hoverBuilding is Belt) {
       if (!g.HasConnectedBelt()) {
           print($"connected belt valid{g.ValidPlacment(currHover.pos)}");

           if (g.ValidPlacment(currHover.pos)) g.AddBelt(hoverBuilding as Belt, g.HasConnectedBuilding());
           else print("not valid placment");
       }
   }*/

/*  if (g.HasBothConnections()) {
      placingBuilding = Instantiate(placingBuilding, GlobalPointers.buildingParent);
      (placingBuilding as Grabber).ResetModel();
      placingBuilding.SetShowDebug(GlobalPointers.showDebug);
  }*/
//}