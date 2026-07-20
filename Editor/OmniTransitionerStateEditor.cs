using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// This is a stub editor for OmniTransitionerState Editor to force it into a UIElements context, so that the
    /// ModularStyleEntryDrawer works properly. This shouldn't be necessary, but it's possible that existing
    /// base inspectors (e.g. Odin) may be interfering with the context.
    /// </summary>
    [CustomEditor(typeof(OmniTransitionerState))]
    public class OmniTransitionerStateEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            return root;
        }
    }
}