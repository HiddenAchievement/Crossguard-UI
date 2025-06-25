using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    [CreateAssetMenu(fileName = "InteractStyle", menuName = "Crossguard UI/Modular Style", order = 2)]
    public class ModularStyle : ScriptableObject
    {
        public float TransitionTime = 0.1f;
        
        [Header("States")]
        public ModularStyleEntry[] NormalState;
        public ModularStyleEntry[] HighlightedState;
        public ModularStyleEntry[] SelectedState;
        public ModularStyleEntry[] PressedState;
        public ModularStyleEntry[] IsolatedState;
        public ModularStyleEntry[] CheckedState;
        public ModularStyleEntry[] DisabledState;
    }
}