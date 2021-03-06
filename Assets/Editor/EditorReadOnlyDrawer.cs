﻿using UnityEngine;
using UnityEditor;

// Create a read-only field in the Unity Inspector. Source:
// https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html

[CustomPropertyDrawer(typeof(EditorReadOnlyAttribute))]
public class EditorReadOnlyDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property,
                                GUIContent label) {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}