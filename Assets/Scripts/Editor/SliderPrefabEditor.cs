using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(SliderPrefab))]
public class SliderPrefabEditor : Editor {

    public override void OnInspectorGUI() {
        SliderPrefab slider = (SliderPrefab)target;
        slider.slider = EditorGUILayout.ObjectField("slider", slider.slider, typeof(Slider), true) as Slider;
        slider.foreground = EditorGUILayout.ObjectField("foreground", slider.foreground, typeof(Image), true) as Image;
        slider.background = EditorGUILayout.ObjectField("background", slider.background, typeof(Image), true) as Image;
        slider.backgroundColor = EditorGUILayout.ColorField("slider color", slider.backgroundColor);
        slider.foregroundColor = EditorGUILayout.ColorField("slider color", slider.foregroundColor);
        slider.interactable = EditorGUILayout.Toggle("interactable", slider.interactable);
        slider.SetSliderColor();
        slider.SetInteractable();

        EditorGUILayout.Space();
        slider.useTimer = EditorGUILayout.Toggle("use timer", slider.useTimer);
        if (slider.useTimer) {
            slider.time = EditorGUILayout.FloatField("timer time", slider.time);
            slider.loop = EditorGUILayout.Toggle("looping", slider.loop);
            if (GUILayout.Button("set timer stats")) slider.SetTimer();
        }
    }

}
