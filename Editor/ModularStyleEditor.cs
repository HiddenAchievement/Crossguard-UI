using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HiddenAchievement.CrossguardUi
{
    [CustomEditor(typeof(ModularStyle))]
    public class ModularStyleEditor : Editor
    {
        private static readonly (string, string)[] _states =
        {
            ("Normal", "NormalState"),
            ("Highlighted", "HighlightedState"),
            ("Selected", "SelectedState"),
            ("Pressed", "PressedState"),
            ("Isolated", "IsolatedState"),
            ("Checked", "CheckedState"),
            ("Disabled", "DisabledState")
        };

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            PropertyField transitionTimeField = new PropertyField(serializedObject.FindProperty("TransitionTime"));
            container.Add(transitionTimeField);

            for (int i = 0; i < _states.Length; i++)
            {
                SerializedProperty listProperty = serializedObject.FindProperty(_states[i].Item2);
                ListView list = new ListView
                {
                    headerTitle             = _states[i].Item1,
                    showAddRemoveFooter     = true,
                    showBorder              = true,
                    showFoldoutHeader       = true,
                    reorderable             = true,
                    virtualizationMethod    = CollectionVirtualizationMethod.DynamicHeight,
                    reorderMode             = ListViewReorderMode.Animated,
                    bindingPath             = listProperty.propertyPath,
                    showBoundCollectionSize = false,
                    selectionType           = SelectionType.Single,
                    style =
                    {
                        backgroundColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f))
                    }
                };
                container.Add(list);
                // Hijacking the add button, since there isn't a good override yet.
                list.Q<Button>("unity-list-view__add-button").clickable = new Clickable(() =>
                {
                    int oldArraySize = listProperty.arraySize;
                    listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
                    SerializedProperty newProp = listProperty.GetArrayElementAtIndex(oldArraySize);
                    newProp.FindPropertyRelative("ComponentPath").stringValue = string.Empty;
                    newProp.FindPropertyRelative("Style").ClearArray();
                    listProperty.serializedObject.ApplyModifiedProperties();
                });
            }
            return container;
        }
    }
}