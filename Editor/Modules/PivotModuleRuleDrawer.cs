using UnityEditor;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [CustomPropertyDrawer(typeof(PivotModuleRule))]
    public class PivotModuleRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = "Pivot";
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Pivot"), GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}