using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HiddenAchievement.CrossguardUi
{
    [CustomPropertyDrawer(typeof(ModularStyleEntry))]
    public class ModularStyleEntryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty styleProperty = property.FindPropertyRelative("Style");
            
            VisualElement container = new VisualElement();
            
            // Add component path section.
            PropertyField componentField = new PropertyField(property.FindPropertyRelative("ComponentPath"), "Component");
            container.Add(componentField);
            
            // Add list of module data.
            ListView items = new ListView
            {
                showAddRemoveFooter     = false,
                showBorder              = true,
                showFoldoutHeader       = false,
                reorderable             = true,
                virtualizationMethod    = CollectionVirtualizationMethod.FixedHeight,
                reorderMode             = ListViewReorderMode.Animated,
                bindingPath             = styleProperty.propertyPath,
                showBoundCollectionSize = false,
                selectionType           = SelectionType.Single,
                style =
                {
                    backgroundColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f))
                }
            };
            container.Add(items);
            
            StyleModuleRuleDropdown dropdown = new StyleModuleRuleDropdown(new AdvancedDropdownState());
            dropdown.OnTypeSelected += type =>
            {
                int arraySize = styleProperty.arraySize;
                styleProperty.InsertArrayElementAtIndex(styleProperty.arraySize);
                SerializedProperty newProp = styleProperty.GetArrayElementAtIndex(arraySize);
                newProp.managedReferenceValue = CreateDefaultInstance(type);
                property.serializedObject.ApplyModifiedProperties();
            };

            var buttons = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row, flexGrow = 1f }
            };
            Button addNewItemButton = new()
            {
                text = "Add Rule",
                style = { flexGrow = 1f }
            };
            addNewItemButton.clicked += () => dropdown.Show(addNewItemButton.worldBound);
            buttons.Add(addNewItemButton);
            GUIContent icon = EditorGUIUtility.IconContent("TreeEditor.Trash");
            Button removeItemButton = new();
            removeItemButton.Add(new Image {
                image = icon.image,
            });
            removeItemButton.clicked += () =>
            {
                if (items.selectedIndex < 0) return;
                styleProperty.DeleteArrayElementAtIndex(items.selectedIndex);
                property.serializedObject.ApplyModifiedProperties();
            };
            buttons.Add(removeItemButton);
            container.Add(buttons);
            return container;
        }

        private static object CreateDefaultInstance(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (type == typeof(string)) return "";
            if (type.IsSubclassOf(typeof(UnityEngine.Object))) return null;
            return Activator.CreateInstance(type);
        }
    }
}