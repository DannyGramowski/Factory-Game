/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System;*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility {
    private ItemHierarchyView targetGraphView;
    private ItemHierarchyContainer containerCache;

    private List<Edge> edges => targetGraphView.edges.ToList();
    private List<ItemNode> nodes => targetGraphView.nodes.ToList().Cast<ItemNode>().ToList();

    public static GraphSaveUtility Instance(ItemHierarchyView targetView) {
        return new GraphSaveUtility {
            targetGraphView = targetView
        };   
    }

  public void SaveGraph(string fileName) {
        if (!edges.Any()) return;
        var itemContainer = ScriptableObject.CreateInstance<ItemHierarchyContainer>();

        var connectedPorts = edges.Where(x => x.input.node != null).ToArray();
        for(int i = 0; i < connectedPorts.Length; i++) {
            var ouputNode = connectedPorts[i].output.node as ItemNode;
            var inputNode = connectedPorts[i].input.node as ItemNode;

            itemContainer.nodeLinks.Add(new NodeLinkData {
                BaseNodeGuid = ouputNode.GUID,
                OutPortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID,
                InputPortName = connectedPorts[i].input.portName
            }); 
             
        }

        foreach(var node in nodes.Where(node => !node.entryPoint)) {
            itemContainer.itemNodeData.Add(new ItemNodeData {
                NodeGUID = node.GUID,
                position = node.GetPosition().position
            }) ;
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");

        AssetDatabase.CreateAsset(itemContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();

    }

    public void LoadGraph(string fileName) {
        containerCache = Resources.Load<ItemHierarchyContainer>(fileName);
        if(containerCache == null) {
            EditorUtility.DisplayDialog("File not found", "Target item graph file does not exist!", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ClearGraph() {
        nodes.Find(x => x.entryPoint).GUID = containerCache.nodeLinks[0].BaseNodeGuid;

        foreach(var node in nodes) {
            if (node.entryPoint) return;
            edges.Where(x => x.input.node == node).ToList().ForEach(edge => targetGraphView.RemoveElement(edge));

            targetGraphView.RemoveElement(node);
        }
    }

    private void CreateNodes() {
        foreach(var nodeDat in containerCache.itemNodeData) {
           // var tempNode = targetGraphView.GenerateNode()
        }
    }

    private void ConnectNodes() {

    }
}
