using UnityEngine;
using Factory.Buildings;
using Factory.UI;
using Factory.Saving;

namespace Factory.Core {

    public class InputManager : Singleton<InputManager> {
        public Building PlacingBuilding { get { return placingBuilding; } set {
                placingBuilding = value;
                if (placingBuilding != null) {
                    placingBuilding.transform.eulerAngles = buildingRot;
                }
            } }

        [SerializeField] LayerMask layerMask;

        private Building placingBuilding;
        private Vector3 buildingRot = new Vector3(0, 0, 0);

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
            } else if(Input.GetKeyDown(KeyCode.L)) {
                SaveSystem.Instance.Save();
            } 
        }

        void BuildingPos() {
            if (PlacingBuilding && Physics.Raycast(GlobalPointers.mainCamera.ScreenPointToRay(Input.mousePosition), out hit, layerMask)) {
                currHover = grid.GetCell(grid.CellNum(hit.point));
                if (grid.Placable(currHover, PlacingBuilding)) {
                    PlacingBuilding.OnHover(currHover);
                }
            }
        }

        private void MouseInput() {
            if (Input.GetMouseButton(0)) {
                if (PlacingBuilding && currHover && grid.Placable(currHover, PlacingBuilding)) {
                    PlacingBuilding.Place(Utils.AngleToDirection(buildingRot.y));
                    if (PlacingBuilding.BuildingPlaced()) {
                        PlacingBuilding = Instantiate(GlobalPointers.buildingPrefabs[PlacingBuilding.buildingType], currHover.transform.position, Quaternion.Euler(buildingRot), GlobalPointers.buildingParent);
                    }
                } else if(Input.GetMouseButtonDown(0)) {
                    SelectBuilding();
                }
            }
        }

        void SelectBuilding() {
            if (Physics.Raycast(GlobalPointers.mainCamera.ScreenPointToRay(Input.mousePosition), out hit)) {
                selection = hit.transform.GetComponent<Building>();
                if (oldSelection != null) { 
                    Unselect();
                }

                if (selection != null) {
                    Select(); 
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
                UIManager.Instance.SetUI(selection as ISelectableBuilding);
            }
        }
    }