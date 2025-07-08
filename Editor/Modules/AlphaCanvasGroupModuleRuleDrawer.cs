using UnityEditor;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [CustomPropertyDrawer(typeof(AlphaCanvasGroupModuleRule))]
    public class AlphaCanvasGroupModuleRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = "Alpha (CanvasGroup)";
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Alpha"), label);
            EditorGUI.EndProperty();
        }
    }
}