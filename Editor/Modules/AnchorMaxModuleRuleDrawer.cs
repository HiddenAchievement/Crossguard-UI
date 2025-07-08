using UnityEditor;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [CustomPropertyDrawer(typeof(AnchorMaxModuleRule))]
    public class AnchorMaxModuleRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = "AnchorMax";
            position   = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("AnchorMax"), GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}