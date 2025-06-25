using UnityEditor;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [CustomPropertyDrawer(typeof(ColorRgbaRendererModuleRule))]
    public class ColorRgbaRendererModuleRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = "Color (RGBA)";
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Color"), label);
            EditorGUI.EndProperty();
        }
    }
}