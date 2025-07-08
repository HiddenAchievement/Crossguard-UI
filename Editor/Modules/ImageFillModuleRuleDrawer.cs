using UnityEditor;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [CustomPropertyDrawer(typeof(ImageFillModuleRule))]
    public class ImageFillModuleRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = "Fill Amount";
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Fill"), label);
            EditorGUI.EndProperty();
        }
    }
}