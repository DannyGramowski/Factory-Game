using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetUpGrabber))]
public class SetUpGrabbersEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SetUpGrabber setUp = (SetUpGrabber)target;
        if(GUILayout.Button("generate grabbers")) {
            setUp.GenerateGrabbers();
        }
    }
}
