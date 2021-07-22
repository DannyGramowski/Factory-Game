using Factory.Core;
using UnityEditor;
using UnityEngine;

public delegate void CloseWindow();
public delegate void OnCreate(Item newItem);

public class CreateItemWindow : EditorWindow {
    Item newItem;
    MeshRenderer renderer;
    Material mat;

    string itemName = "";
    Sprite sprite;
    Color color;
    int stackSize;
    float productionCost;
    ProducableBuildings producableBuildings;
    Transform itemTransform;

    static CloseWindow closeWindow;
    static OnCreate onCreate;
    public static CreateItemWindow Open(CloseWindow closeWindow1, OnCreate onCreate1) {
        closeWindow = closeWindow1;
        onCreate = onCreate1;
        CreateItemWindow window = GetWindow<CreateItemWindow>("create Item");
        window.minSize = new Vector2(85, 85);
        return window;
    }

    [MenuItem("Window/Item editor")]

    public static CreateItemWindow Open() {
        CreateItemWindow window = GetWindow<CreateItemWindow>("create Item");
        window.minSize = new Vector2(85, 85);
        
        return window;
    }

    private void OnEnable() { 
        GlobalPointers.testMap.gameObject.SetActive(true);
        itemTransform = GameObject.FindGameObjectWithTag("Testing").transform.GetChild(1);
        if(newItem == null) {
            CreateItem(GlobalPointers.Instance.templateItem);
        }
    }

    private void OnDisable() {
        Debug.Log("on disable");
        if (closeWindow != null) closeWindow();
        AssetDatabase.DeleteAsset($"Assets/Prefabs/Resources/Items/Materials/{mat.name}.mat");
        foreach (Item t in itemTransform.GetComponentsInChildren<Item>()) {
            DestroyImmediate(t.gameObject);
        }

    }

    public void CreateItem(Item item) {
        newItem = Instantiate(item, itemTransform.position, Quaternion.identity, itemTransform);
        renderer = newItem.GetComponentInChildren<MeshRenderer>();
        CreateNewMat();

    }

    private void OnGUI() {
        Debug.Log("active mat " + mat);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        GUILayout.Label(Utils.GetAssets<RenderTexture>("", new[] { "Assets/Prefabs/Editor" })[0]);
        EditorGUILayout.EndVertical();

        //item input fields
        EditorGUILayout.BeginVertical();
        string oldName = itemName;
        itemName = EditorGUILayout.TextField("item name", itemName);
        if (!string.IsNullOrEmpty(itemName) && oldName != itemName) {
            Debug.Log("changed name of " + mat + " to " + mat.name);
            mat.name = itemName + " Mat";
            
        }

        sprite = EditorGUILayout.ObjectField("item sprite", sprite, typeof(Sprite)) as Sprite;
        newItem.SetSprite(sprite);
        color = EditorGUILayout.ColorField("item color", color);
        mat.color = color;

        stackSize = EditorGUILayout.IntField("stack size", stackSize);
        productionCost = EditorGUILayout.FloatField("production cost", productionCost);
        producableBuildings = (ProducableBuildings)EditorGUILayout.EnumPopup("producable buildings", producableBuildings);
        if (GUILayout.Button("create new prefab")) {
            CreateNewItem();
            //Destroy(this);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void CreateNewItem() {
        if (itemName == null) {
            Debug.LogError("you need to add an item name ");
            return;
        } else if (sprite == null) {
            Debug.LogError("you need to add an item sprite");
            return;
        }

        if (Utils.GetAssets<Item>(itemName, new[] { Utils.ITEM_FOLDER_PATH }).Count != 0) {
            Debug.LogError($"{itemName} already exists");
            return;
        }

        //Debug.Log($"before create new item there were {graphView.items.Count} items");
        //  GameObject obj = MonoBehaviour.Instantiate(newItem.gameObject, itemTransform);

        GameObject obj = MonoBehaviour.Instantiate(newItem.gameObject);
        Debug.Log(obj);
        Debug.Log(obj + " with type " + obj.GetType());
        Item item = obj.GetComponentInParent<Item>();
        if (item) {
            item.itemName = itemName;
            item.sprite = sprite;
            Debug.Log($"input sprite is {sprite}: item sprite is {item.sprite}");
        } else {
            Debug.LogError("the object you are trying to instantiate is not an item");
            //Destroy(obj);
            return;
        }
        Object obj1 = PrefabUtility.SaveAsPrefabAsset(obj, $"{Utils.ITEM_FOLDER_PATH}/{itemName}.prefab", out bool output1);
        // Destroy(obj);
        //Debug.Log($"after create new item there were {graphView.items.Count} items");
       // Debug.Log("on create " + onCreate.ToString() + " item " + item);

        if(onCreate != null)onCreate(item);
        CreateNewMat();
    }

    private void CreateNewMat() {
        Debug.Log("create new Mat");
        string matPath = $"Assets/Prefabs/Resources/Items/Materials/{itemName}Mat.mat";
       // Material newMat = 
        mat = Instantiate(renderer.sharedMaterial);
      //  Debug.Log("mat " + mat + " new mat " + newMat);
        renderer.material = mat;
        AssetDatabase.CreateAsset(mat, matPath);
    }
    /* private void CreateNewMat() {
         Debug.Log("create new Mat");
         string matPath = $"Assets/Prefabs/Resources/Items/Materials/{itemName}Mat.mat";
         Material newMat = Instantiate(renderer.sharedMaterial);
         mat = newMat;
         Debug.Log("mat " + mat + " new mat " + newMat);
         renderer.material = mat;
         AssetDatabase.CreateAsset(newMat, matPath);
     }*/
}
