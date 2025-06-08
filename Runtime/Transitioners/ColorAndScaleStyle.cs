using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A reusable apperaance block for Tween Selectables.
    /// </summary>
    [CreateAssetMenu(fileName = "InteractStyle", menuName = "Crossguard UI/Color and Scale Style", order = 1)]
    public class ColorAndScaleStyle : ScriptableObject
	{
        public float TransitionTime = 0.1f;

        [Header("States")]
        public ColorAndScaleStyleEntry[] NormalState;
        public ColorAndScaleStyleEntry[] HighlightedState;
        public ColorAndScaleStyleEntry[] SelectedState;
        public ColorAndScaleStyleEntry[] PressedState;
        public ColorAndScaleStyleEntry[] IsolatedState;
        public ColorAndScaleStyleEntry[] CheckedState;
        public ColorAndScaleStyleEntry[] DisabledState;
    }
}
