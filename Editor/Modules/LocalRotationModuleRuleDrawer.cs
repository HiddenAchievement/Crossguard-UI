using UnityEditor;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [CustomPropertyDrawer(typeof(LocalRotationModuleRule))]
    public class LocalRotationModuleRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = "Rotation";
            position   = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Rotation"), GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}