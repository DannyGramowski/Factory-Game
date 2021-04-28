using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

public class ItemHierarchyGraph : EditorWindow {
    private ItemHierarchyView graphView;
    private string fileName = "neww graph";

    CloseWindow method;
    OnCreate onCreate;

    /*    string itemName = "test";
        Sprite itemSprite;*/
    [MenuItem("Window/Item Hierarchy")]
    public static void OpenGraphWindow() {
        ItemHierarchyGraph window = GetWindow<ItemHierarchyGraph>("Item Hierarchy");
    }

    private void OnEnable() {
        ConstructGraphView();
        GenerateNewItem();

        method += ConstructGraphView;

        onCreate += ConstructGraphView;
        onCreate += graphView.UpdateItems;

    }


    private void OnDisable() {
        graphView.OnDestroy();
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView() {
        graphView = new ItemHierarchyView();

        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }


    private void GenerateNewItem() {
        Toolbar toolbar = new Toolbar();

        var fileNameTextField = new TextField("fileName: ");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)){text = "Save Data"});
        toolbar.Add(new Button(() => RequestDataOperation(false)){text = "load Data"});

        Button nodeCreateButton = new Button(() => {
            CreateItemWindow itemWindow = CreateItemWindow.Open(method, onCreate);
            itemWindow.CreateItem(graphView.items[0]);
        }) {
            text = "Create Node"
        };
        toolbar.Add(nodeCreateButton);

        rootVisualElement.Add(toolbar);

    }

    private void RequestDataOperation(bool save) {
        if(string.IsNullOrEmpty(fileName)) {
            EditorUtility.DisplayDialog("Invalid file name!" , "please enter a valid file name","ok");
        }

        var saveUtility = GraphSaveUtility.Instance(graphView);
        if(save) {
            saveUtility.SaveGraph(fileName);
        } else {
            saveUtility.LoadGraph(fileName);
        }
    }


}
