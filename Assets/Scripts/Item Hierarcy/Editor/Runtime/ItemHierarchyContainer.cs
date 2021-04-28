using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ItemHierarchyContainer : ScriptableObject{
    public List<NodeLinkData> nodeLinks = new List<NodeLinkData>();
    public List<ItemNodeData> itemNodeData = new List<ItemNodeData>();
}
