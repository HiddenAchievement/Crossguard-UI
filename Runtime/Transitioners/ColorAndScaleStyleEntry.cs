using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A component's appearance during a particular state.
    /// </summary>
    [System.Serializable]
    public class ColorAndScaleStyleEntry
    {
        public string ComponentPath;
        public bool UseColor;
        public Color Color = Color.white;
        public bool UseAlpha;
        public float Alpha = 1;
        public bool UseScale;
        public Vector2 Scale = Vector2.one;
    }
}
