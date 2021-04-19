using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class Grabber : Building, ISelectItem {
    [SerializeField] Transform model;
    [SerializeField] EmptySpace emptySpace;
    [SerializeField] float timePerCell;

    static Vector3 modelLocalScale;
    Belt connectedBelt;
    GrabberSpot grabberSpot;
    ProductionBuilding connectedBuilding;
    Timer timer;
    bool toBuilding;//if true belt to building

    Item movingItem = null;
    Item filterItem;

    bool showDebug;
    Cell currHover;

    protected override void Awake() {
        timer = GetComponent<Timer>();
        base.Awake();
        if (modelLocalScale.Equals(Vector3.zero)) {
            print("model scale is " + model.transform.localScale);
            modelLocalScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, model.transform.localScale.z);
            print("set local scale to " + modelLocalScale);
        }
    }
    private void Update() {
        UpdateItemLogic();  
    }

    private void UpdateItemLogic() {
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

    public override void SetShowDebug(bool showDebug) {
        this.showDebug = showDebug;
    }
    
    public void SetModel(Cell currHover) {
        //print("set model"); 
        Cell from = null;
        Cell to = null;
        this.currHover = currHover;

        if (toBuilding) {
            from = connectedBelt?.baseCell ?? currHover;
            to = grabberSpot?.cell ?? currHover;
        } else {
            from = grabberSpot?.cell ?? currHover;
            to = connectedBelt?.baseCell ?? currHover;
        }
       // print("form " + from + ": to " + to);
        if(!HasGrabberSpot() && !HasConnectedBuilding()) {
         //   print("set grabber to curr hover");
            transform.position = currHover.itemPos;
            return;
        }



        //print("set model loc to the middle");
        model.transform.position = Vector3.Lerp(from.itemPos, to.itemPos, 0.5f);

        Vector2Int offSet = from.pos - to.pos;
        float angle = Utils.Angle(from.pos, to.pos);
        print("angle " + angle);
        model.eulerAngles = Utils.Vector3SetY(transform.eulerAngles, angle);

        model.transform.localScale = new Vector3(modelLocalScale.x * offSet.magnitude, modelLocalScale.y, modelLocalScale.z * 0.6f);
       /* transform.position = cell1.itemPos;
        Vector2Int offSet = cell1.pos - cell2.pos;
        Vector3 rot = Utils.Vector3SetY(transform.eulerAngles, Vector2.Angle());

        transform.position = (cell1.itemPos + cell2.itemPos) / 2;
        Vector2Int scaleFactor = Vector2Int.one + Utils.Vector2IntAbs(offSet);
        int x = scaleFactor.x, y = scaleFactor.y;
       
        scaleFactor = new Vector2Int(x, y);
        Vector3 newScale = new Vector3(scaleFactor.x * modelLocalScale.x - 0.5f, modelLocalScale.y, modelLocalScale.z * scaleFactor.y + 0.5f);
        //print("new scale = " + newScale);
        model.localScale = newScale;*/
    }
    

    public void AddBuilding(Cell clickedLoc, ProductionBuilding b, bool input) {
        connectedBuilding = b;
        toBuilding = input;
        grabberSpot = connectedBuilding.AddGrabber(clickedLoc, this, input);
        if (grabberSpot != null && connectedBelt) {
            // print("added grabber spot");
            emptySpace.transform.position = connectedBelt.itemPos;
           // print("empty space set to " + b.baseCell.pos);
            timer.StartUp(timePerCell * (Vector2Int.Distance(connectedBelt.baseCell.pos, grabberSpot.cell.pos)));
        }
    }

    public void AddBelt(Belt b, bool input) {
        connectedBelt = b;
        emptySpace.transform.position = b.itemPos;
       // print("empty space set to " + b.baseCell.pos);
        if (grabberSpot != null) {
           timer.StartUp(timePerCell * Vector2Int.Distance(connectedBelt.baseCell.pos, grabberSpot.cell.pos));
        }
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

    public int UINum() {
        return 0;
    }

    public ProducableBuildings ProducableBuildingsType() {
        return ProducableBuildings.all;
    }

    public void SetItem(Item item) {
        filterItem = item;
    }
}
