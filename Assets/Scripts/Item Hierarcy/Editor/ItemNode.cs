using UnityEditor.Experimental.GraphView;

public class ItemNode : Node {
    public string GUID;
    public Item item;
    public bool entryPoint;

    public ItemNode(Item item, bool entryPoint) {
        GUID = item.itemName;
        this.item = item;
        this.entryPoint = entryPoint;
    }
}
