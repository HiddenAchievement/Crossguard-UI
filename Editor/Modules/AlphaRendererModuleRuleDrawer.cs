using UnityEditor;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [CustomPropertyDrawer(typeof(AlphaRendererModuleRule))]
    public class AlphaRendererModuleRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = "Alpha (Renderer)";
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Alpha"), label);
            EditorGUI.EndProperty();
        }
    }
}