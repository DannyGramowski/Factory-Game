using Factory.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
public class ItemHierarchyView : GraphView {
    public List<Item> items { get; private set; }

    private List<ItemNode> itemNodes = new List<ItemNode>();
    readonly Vector2 defaultNodeSize = new Vector2(25, 25);
    int numNodes = 0;

    public ItemHierarchyView() {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        UpdateItems();
        for (int i = 0; i < items.Count; i++) {
            GenerateNode(i);
        }
        SetUpPorts();
    }

    public void OnDestroy() {
        foreach (Item i in items) {
            PrefabUtility.RecordPrefabInstancePropertyModifications(i.gameObject);
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        List<Port> compatiblePorts = new List<Port>();
        Port startPortView = startPort;

        ports.ForEach((port) => {
            Port portView = port;
            if (startPortView != portView && startPortView.node != portView.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    public ItemNode GenerateNode(int itemNum) {
        return GenerateNode(items[itemNum]);
    }

    public ItemNode GenerateNode(Item item) {
        ItemNode node = new ItemNode(item, true) {
            title = item.itemName
        };
        node.inputContainer.SetEnabled(true);
        node.outputContainer.SetEnabled(true);

        UnityEngine.UIElements.Button inputButton = new UnityEngine.UIElements.Button ();
        inputButton.text = "Add";
        inputButton.clicked += delegate { AddRecipe(node); };
        node.titleContainer.Add(inputButton);

        UnityEngine.UIElements.Button outputButton = new UnityEngine.UIElements.Button();
        outputButton.text = "Remove";
        outputButton.clicked += delegate { RemoveRecipe(node); };
        node.titleContainer.Add(outputButton);

        //add image
        /* var image = new UnityEngine.UIElements.Image();
         image.sprite = item.sprite;
         node.contentContainer.Add(image);*/

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(numNodes * defaultNodeSize, defaultNodeSize));
        numNodes++;

        itemNodes.Add(node);
        AddElement(node);
        return node;
    }

    private void AddRecipe(ItemNode node) {
        foreach(ISelectable selectable in selection) {
            if(selectable is ItemNode) {
                ItemNode node1 = selectable as ItemNode;
                node.item.recipe.Add(node1.item);

               Port port = AddPort(node, UnityEditor.Experimental.GraphView.Direction.Input);
               Port port1 = AddPort(node1, UnityEditor.Experimental.GraphView.Direction.Output);
                Edge edge = port.ConnectTo(port1);
                port.Add(edge);
                AddElement(edge);
            }
        }
    }

    private void RemoveRecipe(ItemNode node) {
        foreach(ISelectable selectable in selection) {
            if(selectable is ItemNode) {
                ItemNode itemNode = selectable as ItemNode;

                //remove from final item
                int x = 0;
                while (x < node.inputContainer.childCount) {
                    Port port = node.inputContainer.ElementAt(x) as Port;
                    if (port != null && port.portName.Equals(itemNode.item.itemName)) {                     
                       node.inputContainer.RemoveAt(x);
                    } else {
                        x++;
                    }
                }

                //remove reciepe items from container
                int i = 0;
                while(i < itemNode.outputContainer.childCount) { 
                Port port = itemNode.outputContainer.ElementAt(i) as Port;
                    if(port != null && port.portName.Equals(node.item.itemName)) {
                        itemNode.outputContainer.RemoveAt(i);
                    } else {
                        i++;
                    }
                }

                node.item.recipe.Remove(itemNode.item);
            }
        }
    }

   /* public void SetRecepies() {
        Debug.Log("set recepies");
        foreach (ItemNode node in itemNodes) {
            node.item.recipe.Clear();
            for (int i = 0; i < node.inputContainer.childCount; i++) {
                Port port = node.inputContainer.ElementAt(i) as Port;
                if (port != null) {

                }
            }
        }
    }*/

    private Port GeneratePort(ItemNode node, UnityEditor.Experimental.GraphView.Direction portDirection, string portName, Port.Capacity capacity = Port.Capacity.Single) {
        Port port = node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        port.portName = portName;
        return port;
    }

    private Port AddPort(ItemNode node, UnityEditor.Experimental.GraphView.Direction direction) {
        Port newPort = GeneratePort(node, direction, node.item.itemName);
        int outputPortCount = node.outputContainer.Query("connector").ToList().Count;

        if (direction == UnityEditor.Experimental.GraphView.Direction.Input) {
            node.inputContainer.Add(newPort);
        } else {
            node.outputContainer.Add(newPort);
        }
        node.RefreshExpandedState();
        node.RefreshPorts();

        return newPort;
    }

    public void UpdateItems() {
        items = Utils.GetAssets<Item>("", new[] { Utils.ITEM_FOLDER_PATH });
    }

    private void SetUpPorts() {
        //set up input
        foreach (ItemNode node in itemNodes) {
            foreach (Item i in node.item.recipe) {
                Port port = GeneratePort(node, UnityEditor.Experimental.GraphView.Direction.Input, i.itemName);
                port.portName = i.itemName;
                node.inputContainer.Add(port);

                ItemNode node2 = GetNode(i);

                /*                Edge edge = new Edge();
                                port.Connect(edge);
                                AddPort(node2).Connect(edge);
                                AddElement(edge);*/
                Port port2 = AddPort(node2, UnityEditor.Experimental.GraphView.Direction.Output);
                port2.portName = node.item.itemName;
                Edge edge = port.ConnectTo(port2);
                port.Add(edge);
                AddElement(edge);
            }
        }
    }

    private ItemNode GetNode(Item i) {
        foreach (ItemNode node in itemNodes) {
            if (node.item.Equals(i)) return node;
        }
        return null;
    }

}
