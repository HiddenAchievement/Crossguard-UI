using UnityEditor;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [CustomPropertyDrawer(typeof(ColorRgbRendererModuleRule))]
    public class ColorRgbRendererModuleRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = "Color (RGB)";
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Color"), label);
            EditorGUI.EndProperty();
        }
    }
}