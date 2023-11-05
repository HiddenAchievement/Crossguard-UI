using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A transitioner that swaps between different sprites for various states.
    /// </summary>
    public class SimpleSpriteSwapTransitioner : AbstractTransitioner
    {
        [Header("States")]
        [SerializeField]
        private Sprite _normalState      = null;
        [SerializeField]
        private Sprite _highlightedState = null;
        [SerializeField]
        private Sprite _selectedState    = null;
        [SerializeField]
        private Sprite _isolatedState    = null;
        [SerializeField]
        private Sprite _pressedState     = null;
        [SerializeField]
        private Sprite _disabledState    = null;

        [Header("Target")]
        [SerializeField]
        private Image _targetImage = null;


        public override float TransitionTime => 0.15f;

        protected override void ClearAllComponents()
        {
            if (_targetImage == null) return;
            _targetImage.overrideSprite = null;
        }

        protected override void ForceAppearance(InteractState state)
        {
            SetSprite(state);
        }

        protected override void TransitionOn(InteractState state, bool immediate)
        {
            // If there's a higher priority state, don't bother changing the appearance.
            for (int i = (int) InteractState.Count - 1; i > (int)state; i--)
            {
                if (_stateFlags[i]) return;
            }

            SetSprite(state);
        }

        protected override void TransitionOff(InteractState state, bool immediate)
        {
            // If there's a higher priority state, don't bother changing the appearance.
            for (int i = (int) InteractState.Count - 1; i > (int)state; i--)
            {
                if (_stateFlags[i]) return;
            }

            // Find the highest priority state after the one being turned off.
            for (int i = (int) state - 1; i >= 0; i--)
            {
                if (!_stateFlags[i]) continue;
                SetSprite((InteractState) i);
                return;
            }
            
            // If we get here, somehow, just set it to normal.
            SetSprite(InteractState.Normal);
        }

        private void SetSprite(InteractState state)
        {
            if (_targetImage == null) return;
            switch (state)
            {
                case InteractState.Normal:
                    _targetImage.overrideSprite = _normalState;
                    break;
                case InteractState.Highlighted:
                    _targetImage.overrideSprite = _highlightedState;
                    break;
                case InteractState.Selected:
                    _targetImage.overrideSprite = _selectedState;
                    break;
                case InteractState.Isolated:
                    _targetImage.overrideSprite = _isolatedState;
                    break;
                case InteractState.Pressed:
                    _targetImage.overrideSprite = _pressedState;
                    break;
                case InteractState.Disabled:
                    _targetImage.overrideSprite = _disabledState;
                    break;
            }
        }
    }
}
