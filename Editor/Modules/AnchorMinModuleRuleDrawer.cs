using UnityEditor;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [CustomPropertyDrawer(typeof(AnchorMinModuleRule))]
    public class AnchorMinModuleRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = "AnchorMin";
            position   = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("AnchorMin"), GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}