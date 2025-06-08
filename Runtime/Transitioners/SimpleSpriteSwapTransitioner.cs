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
        private Sprite _pressedState     = null;
        [SerializeField]
        private Sprite _isolatedState    = null;
        [SerializeField]
        private Sprite _checkedState     = null;
        [SerializeField]
        private Sprite _disabledState    = null;

        [Header("Target")]
        [SerializeField]
        private Image _targetImage = null;

        private Sprite[] _stateSprites = null;
        

        public override float TransitionTime => 0.15f;

        protected override void Awake()
        {
            InitializeSprites();
            base.Awake();
        }

        private void InitializeSprites()
        {
            if (_stateSprites is { Length: > 0 }) return;
            _stateSprites = new[]
            {
                _normalState, _highlightedState, _selectedState, _pressedState, _isolatedState, _checkedState, _disabledState
            };
        }

        protected override void ClearAllComponents()
        {
            if (_targetImage == null) return;
            _targetImage.overrideSprite = null;
        }

        protected override void ForceAppearance(InteractState state)
        {
            InitializeSprites();
            SetSprite((int)state);
        }

        protected override void TransitionOn(InteractState state, bool immediate)
        {
            // Find the highest priority active state that has a sprite associated with it.
            for (int i = (int)InteractState.Count - 1; i >= 0; i--)
            {
                if (!_stateFlags[i]) continue;
                if (SetSprite(i)) return;
            }
            
            // If we get here, somehow, just set it to normal.
            SetSprite((int)InteractState.Normal);
        }

        protected override void TransitionOff(InteractState state, bool immediate)
        {
            // Find the highest priority active state that has a sprite associated with it.
            for (int i = (int)InteractState.Count - 1; i >= 0; i--)
            {
                if (!_stateFlags[i]) continue;
                if (SetSprite(i)) return;
            }
            
            // If we get here, somehow, just set it to normal.
            SetSprite((int)InteractState.Normal);
        }

        private bool SetSprite(int state)
        {
            if (_targetImage == null) return true;
            Sprite sprite = _stateSprites[state];
            if (sprite == null) return false;
            _targetImage.overrideSprite = sprite;
            return true;
        }
    }
}
