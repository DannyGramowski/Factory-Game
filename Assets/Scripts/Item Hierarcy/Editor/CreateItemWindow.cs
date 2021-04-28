using UnityEditor;
using UnityEngine;

public delegate void CloseWindow();
public delegate void OnCreate();

public class CreateItemWindow : EditorWindow {
    Item newItem;
    MeshRenderer renderer;
    Material mat;

    string itemName;
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


    private void OnEnable() {
        itemTransform = GameObject.FindGameObjectWithTag("Testing").transform.GetChild(1);

    }

    private void OnDisable() {
        Debug.Log("on disable");
        if (closeWindow != null) closeWindow();
        AssetDatabase.DeleteAsset($"Assets/Prefabs/Items/Materials/{mat.name}.mat");
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
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        GUILayout.Label(Utils.GetAssets<RenderTexture>("", new[] { "Assets/Prefabs/Editor" })[0]);
        EditorGUILayout.EndVertical();

        //item input fields
        EditorGUILayout.BeginVertical();
        itemName = EditorGUILayout.TextField("item name", itemName);
        mat.name = itemName + "Mat";

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
        onCreate();

        //Debug.Log($"before create new item there were {graphView.items.Count} items");
        bool output;
        GameObject obj = MonoBehaviour.Instantiate(newItem.gameObject, itemTransform);
        Item item = obj.GetComponent<Item>();
        if (item) {
            item.itemName = itemName;
            item.sprite = sprite;
            Debug.Log($"input sprite is {sprite}: item sprite is {item.sprite}");
        } else {
            Debug.LogError("the object you are trying to instantiate is not an item");
            //Destroy(obj);
            return;
        }
        CreateNewMat();
        Object obj1 = PrefabUtility.SaveAsPrefabAsset(obj,
            $"{Utils.ITEM_FOLDER_PATH}/{itemName}.prefab",
            out output);
        // Destroy(obj);
        //Debug.Log($"after create new item there were {graphView.items.Count} items");

    }

    private void CreateNewMat() {
        Debug.Log("create new Mat");
        string matPath = $"Assets/Prefabs/Items/Materials/{itemName}Mat.mat";
        Material newMat = Instantiate(renderer.sharedMaterial);
        mat = newMat;
        renderer.material = mat;
        AssetDatabase.CreateAsset(newMat, matPath);
    }
}
