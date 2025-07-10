using UnityEditor;
using UnityEditor.UI;


namespace HiddenAchievement.CrossguardUi
{
    [CustomEditor(typeof(CrossUguiToggle), true)]
    [CanEditMultipleObjects]
    public class CrossUguiToggleEditor : ToggleEditor
    {
        private SerializedProperty _crossGroupProperty;
        private SerializedProperty _idProperty;
        private SerializedProperty _eventProperty;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            _crossGroupProperty = serializedObject.FindProperty("_crossGroup");
            _idProperty = serializedObject.FindProperty("_id");
            _eventProperty = serializedObject.FindProperty("OnValueChangedForId");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Crossguard Toggle Features", EditorStyles.boldLabel);
            serializedObject.Update();
            EditorGUILayout.PropertyField(_crossGroupProperty);
            EditorGUILayout.PropertyField(_idProperty);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_eventProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
