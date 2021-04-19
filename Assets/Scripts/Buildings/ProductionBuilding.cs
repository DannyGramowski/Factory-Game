using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProductionBuilding : Building {
    [SerializeField] bool NotGrabberSpots;//if true will create offsets for all grabberspots except the one inputed
    public List<GrabberSpot> grabberSpots = new List<GrabberSpot>();

    [SerializeField] protected BuildingInventory inventory;

    [SerializeField] int numberOfStacks;

    List<Grabber> inGrabbers = new List<Grabber>();
    List<Grabber> outGrabbers = new List<Grabber>();

    protected override void Awake() {
        Debug.Assert(numberOfStacks > 0 || this is Assembler, "you need to set the number of inventory slots");
            inventory = new BuildingInventory(numberOfStacks);
        
        base.Awake();
    }
    public override void Placed() {
        SetGrabberSpots();
    }
    private void SetGrabberSpots() {
        if (!NotGrabberSpots) {
            foreach (GrabberSpot g in grabberSpots) {
                g.SetGrabberCell(this);
            }
        } else {
            CreateGrabberSpots();
        }
    }

    private void CreateGrabberSpots() {
        List<GrabberSpot> newGrabberSpots = new List<GrabberSpot>();
        for(int y = 0; y < dimensions.y; y++) {
            for(int x = 0; x < dimensions.x; x++) {
                bool dontUse = false ;
                Vector2Int temp = new Vector2Int(x, y);
                foreach(GrabberSpot grabberSpot in grabberSpots) {
                    if(grabberSpot.offSet == temp) {
                        dontUse = true;
                        break;
                    }
                }
                if(!dontUse) {
                    GrabberSpot tempGrabber = new GrabberSpot(temp);
                    tempGrabber.SetGrabberCell(this);
                    newGrabberSpots.Add(tempGrabber);
                }
            }
        }
        grabberSpots = newGrabberSpots;
    }

    public GrabberSpot AddGrabber(Cell loc, Grabber g,bool input) {
        GrabberSpot grabberSpot = HasGrabberSpot(loc);
        if (grabberSpot != null) {
            grabberSpot.connectedGrabber = g;
            if (input && grabberSpot.spotType != Type.output) {
                inGrabbers.Add(g);
                return grabberSpot;
            } else if (!input && grabberSpot.spotType != Type.input) {
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
    }

    public virtual void ItemIn(Item item) { }

    public virtual bool ItemInValid(Item item) { return false;}

    public virtual Item ItemOut(Item filterItem) { return null; }

    public virtual bool ItemOutValid(Item filterItem) { return false;}

}

[System.Serializable]
public class GrabberSpot {
    public Vector2Int offSet;
    public Cell cell;
    public Type spotType = Type.both;
    public Grabber connectedGrabber;
    public Direction direction;

    private ProductionBuilding productionBuilding;

    public override string ToString() {
        return productionBuilding.ToString() + ": "
            + offSet.ToString();
    }

    public void SetGrabberCell(ProductionBuilding productionBuilding) {
        this.productionBuilding = productionBuilding;
        cell = Grid.Instance.GetCell(productionBuilding.baseCell.pos + offSet);
        direction = Utils.AddDirection(direction, productionBuilding.direction);
    }  

    public GrabberSpot(Vector2Int offSet) {
        this.offSet = offSet;
    } 
}

public enum Type {
    input = 1,
    output = -1,
    both = 0
}
