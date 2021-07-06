using Factory.Buildings;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(SetUpGrabber))]
public class SetUpGrabbersEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SetUpGrabber setUp = (SetUpGrabber)target;
        if (GUILayout.Button("generate grabbers")) {
/*            SerializedObject obj = new SerializedObject(setUp.setGrabbers);
            SerializedProperty grabberList = obj.FindProperty("setGrabbers.grabberSpots");*/
            setUp.GenerateGrabbers();
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if(prefabStage != null) EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }
}
