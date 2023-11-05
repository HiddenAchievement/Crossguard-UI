using UnityEditor;
using UnityEditorInternal;


#if UNITY_EDITOR

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// For editing the Tween Transition Styles.
    /// </summary>
    [CustomEditor(typeof(ColorAndScaleStyle))]
    public class ColorAndScaleStyleEditor : Editor
    {
        private static string[] _colHeaders = {"Component", "✓", "Color", "✓", "Alpha", "✓", "Scale"};
        private static float?[] _colSizes = {null, 10, 50, 10, 35, 10, 100};

        private static (string, string)[] _states =
        {
            ("Normal", "NormalState"), ("Highlighted", "HighlightedState"), ("Selected", "SelectedState"),
            ("Pressed", "PressedState"), ("Isolated", "IsolatedState"), ("Disabled", "DisabledState")
        };

        private SerializedProperty _transitionTime;
        private ReorderableList[] _lists = new ReorderableList[_states.Length];

        private void OnEnable()
        {
            _transitionTime = serializedObject.FindProperty("TransitionTime");

            for (int i = 0; i < _states.Length; i++)
            {
                _lists[i] = ReorderableListUtility.CreateAutoLayout(serializedObject.FindProperty(_states[i].Item2), _colHeaders, _colSizes);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_transitionTime);

            for (int i = 0; i < _states.Length; i++)
            {
                EditorGUILayout.Space();
                // EditorGUILayout.LabelField(_states[i].Item1, EditorStyles.boldLabel);
                ReorderableListUtility.DoLayoutListWithFoldout(_lists[i]);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
