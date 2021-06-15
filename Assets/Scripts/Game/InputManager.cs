using UnityEngine;
using Factory.Buildings;
using Factory.UI;

namespace Factory.Core {

    public class InputManager : Singleton<InputManager> {
        public Building PlacingBuilding { get { return placingBuilding; }
            set {
                placingBuilding = value;
                if (placingBuilding != null) {
                    placingBuilding.transform.eulerAngles = buildingRot;
                }
            } }

        private Building placingBuilding;
        private Vector3 buildingRot = new Vector3(0, 0, 0);

        [SerializeField] LayerMask layerMask;
        // [SerializeField] BeltSystem beltSystemPrefab;
        int[] test;
        public Building selection;
        Building oldSelection;

        bool showDebug;
        Grid grid;
        RaycastHit hit;
        Cell currHover;

        private void Awake() {
            grid = Grid.Instance;
            showDebug = GlobalPointers.showDebug;
        }
        private void Update() {
            KeyBoardInput();
            BuildingPos();
            MouseInput();
        }

        private void KeyBoardInput() {
            if (Input.GetKeyDown(KeyCode.R) && PlacingBuilding) {
                buildingRot = PlacingBuilding.Rotate(buildingRot);
            } else if (Input.GetKeyDown(KeyCode.Escape)) {
                if (PlacingBuilding) {
                    PlacingBuilding.CancelPlace(currHover);
                    PlacingBuilding = null;
                } else {
                    UIManager.Instance.gameObject.SetActive(false);
                }
            }

        }

        void BuildingPos() {
            if (PlacingBuilding &&
                Physics.Raycast(GlobalPointers.mainCamera.ScreenPointToRay(Input.mousePosition), out hit, layerMask)) {
                currHover = grid.GetCell(grid.CellNum(hit.point));
                if (grid.Placable(currHover, PlacingBuilding)) {
                    PlacingBuilding.OnHover(currHover);
                }
            }
        }

        private void MouseInput() {
            /*if (placingBuilding && currHover) {
                placingBuilding.OnHover(currHover);
            }*/

            if (Input.GetMouseButton(0)) {
                if (PlacingBuilding && currHover && grid.Placable(currHover, PlacingBuilding)) {
                    PlacingBuilding.Place(Utils.AngleToDirection(buildingRot.y));
                    if (PlacingBuilding.BuildingPlaced()) {
                        //lacingBuilding.IncreaseNamingNum();
                        //print("created new building");
                        PlacingBuilding = Instantiate(GlobalPointers.buildingPrefabs[PlacingBuilding.buildingType], currHover.transform.position, Quaternion.Euler(buildingRot), GlobalPointers.buildingParent);
                    }
                } else {
                    SelectBuilding();
                }
            }

         
        }
            void SelectBuilding() {
                if (Physics.Raycast(GlobalPointers.mainCamera.ScreenPointToRay(Input.mousePosition), out hit)) {
                    selection = hit.transform.GetComponent<Building>();
                    if (oldSelection != null) { // reset material to default
                        Unselect();
                    }

                    if (selection != null) {
                        Select(); // do stuff with selected object
                        oldSelection = selection;
                    }
                }
            }

            void Unselect() {
                oldSelection.selected = false;
                oldSelection = null;
            }

            void Select() {
                selection.selected = true;
            }



           

        }
    }

/*     if (placingBuilding is Grabber) {//grabber placing
                  Grabber g = placingBuilding as Grabber;
                  if (Input.GetMouseButtonDown(0)) {
                      AddGrabber();
                  }
                  g.SetModel(currHover);

              } else if (Input.GetMouseButton(0) && placingBuilding && currHover && grid.Placable(currHover, placingBuilding)) {//everything else
                  Building temp = Instantiate(placingBuilding, GlobalPointers.buildingParent);

                  temp.direction = Utils.AngleToDirection(buildingRot.y);
                  temp.SetPostion(currHover);
                  temp.Placed();

                  CheckBeltSystem(temp);//check beltss

              } else if (Input.GetMouseButtonDown(0)) {
                  SelectBuilding();
                  ISelectItem selectItem = selection as ISelectItem;
                  if (selectItem != null) {
                      UIManager.Instance.SetUI(selectItem);
                  }
              }*/

/* bool CheckBeltSystem(Building building) {
     Belt b = building as Belt;
     if (b) {
         bool addedBelt = false;
         Vector2Int forwardCell = Utils.Vector2FromDirection(b.direction) + currHover.pos;
         if (validCell(forwardCell)) {
             Cell cell = grid.GetCell(forwardCell);
             Belt belt = cell.building as Belt;
             if (CheckBeltConnected(
                 currHover, cell)
            && belt.beltSystem.belts[0].Equals(belt)
             ) { //to only add a belt infront if it is adding to the first belt
                 belt.beltSystem.AddBelt(b, true);
                 addedBelt = true;
             }
         }

         addedBelt |= AddToBeltSystem(90, b);
         addedBelt |= AddToBeltSystem(180, b);
         addedBelt |= AddToBeltSystem(270, b);

         if (!addedBelt) {
             BeltSystem temp = Instantiate(beltSystemPrefab, GlobalPointers.buildingParent);
             temp.AddBelt(b, false);
             temp.SetShowDebug(showDebug);

         }
         return true;
     } else {
         return false;
     }
 }*/

/* bool AddToBeltSystem(float angle, Belt b) {
     Direction direc = Utils.AngleToDirection(b.transform.eulerAngles.y + angle);
     Vector2Int addedPos = Utils.Vector2FromDirection(direc);
     Vector2Int testCell = addedPos + currHover.pos;

     if (validCell(testCell)) {
         if (CheckBeltConnected(grid.GetCell(testCell), currHover)) {
             BeltSystem beltSystem = (grid.GetCell(testCell).building as Belt).beltSystem;
             if (b.beltSystem && !(beltSystem.Equals(b.beltSystem))) {
                 beltSystem.CombineBeltSystems(b.beltSystem);
             } else if (!(beltSystem.Equals(b.beltSystem))) {
                 beltSystem.AddBelt(b, false);
             }
             return true;
         }

     }
     return false;
 }*/

/*  bool CheckBeltConnected(Cell to, Cell from) {
      Belt toBelt = to?.building as Belt;
      Belt fromBelt = from?.building as Belt;
      if (toBelt && fromBelt) {
          Vector2Int checkPos = (Utils.Vector2FromDirection(toBelt.direction) + to.pos);
          if (checkPos.Equals(from.pos)) {
              return true;
          }
      }
      return false;
  }*/

/* void AddGrabber() {
     Grabber g = placingBuilding as Grabber;
     Building hoverBuilding = currHover.building;

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
     }

     if (g.HasBothConnections()) {
         placingBuilding = Instantiate(placingBuilding, GlobalPointers.buildingParent);
         (placingBuilding as Grabber).ResetModel();
         placingBuilding.SetShowDebug(GlobalPointers.showDebug);
     }
 }*/

/*            bool validCell(Vector2Int pos) {
                return !(pos.x > grid.width - 1 || pos.x < 0 || pos.y > grid.height - 1 || pos.y < 0);
            }*/