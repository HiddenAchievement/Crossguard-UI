using System;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A component that represents a group of IToggles, used as radio buttons.
    /// </summary>
    public class CrossRadioGroup : CrossToggleGroupBase
    {
        public const int NoSelection = -1;
        
        [Serializable]
        public class RadioGroupEvent : UnityEvent<CrossToggleBase> {}
        
        [SerializeField]
        private bool _allowSwitchOff = false;
        /// <summary>
        /// Is it allowed that no toggle is switched on?
        /// </summary>
        /// <remarks>
        /// If this setting is enabled, pressing the toggle that is currently switched on will switch it off, so that no toggle is switched on. If this setting is disabled, pressing the toggle that is currently switched on will not change its state.
        /// Note that even if allowSwitchOff is false, the Toggle Group will not enforce its constraint right away if no toggles in the group are switched on when the scene is loaded or when the group is instantiated. It will only prevent the user from switching a toggle off.
        /// </remarks>
        public override bool AllowSwitchOff
        {
            get => _allowSwitchOff;
            set => _allowSwitchOff = value;
        }

        public RadioGroupEvent OnValueChanged;

        /// <summary>
        /// The ID of the selected toggle.  (Negative values mean none is selected.)
        /// </summary>
        public int Id => SelectedToggle == null ? NoSelection : SelectedToggle.Id;

        public CrossToggleBase SelectedToggle { get; private set; } = null;
        
        public bool HasSelectedToggle => SelectedToggle != null;

        private bool _suppressNotifyOff = false;

        protected CrossRadioGroup() { }

        /// <summary>
        /// Notify the group that the given toggle is enabled.
        /// </summary>
        /// <param name="toggle">The toggle that got triggered on.</param>
        /// <param name="sendCallback">If other toggles should send onValueChanged.</param>
        public override void NotifyToggleOn(CrossToggleBase toggle, bool sendCallback = true)
        {
#if UNITY_EDITOR
            ValidateToggleIsInGroup(toggle);
#endif
            CrossToggleBase oldToggle = SelectedToggle;
            if (oldToggle == toggle) return;

            if (oldToggle != null)
            {
                _suppressNotifyOff = true;
                if (sendCallback)
                    oldToggle.IsOn = false;
                else
                    oldToggle.SetIsOnWithoutNotify(false);
                _suppressNotifyOff = false;
            }

            SelectedToggle = toggle;
            if (sendCallback)
            {
                OnValueChanged?.Invoke(toggle);
            }
        }

        /// <summary>
        /// Notify the group that the given toggle is disabled.
        /// </summary>
        /// <param name="toggle">The toggle that got triggered on.</param>
        public override void NotifyToggleOff(CrossToggleBase toggle, bool sendCallback = true)
        {
            if (_suppressNotifyOff) return;
#if UNITY_EDITOR
            ValidateToggleIsInGroup(toggle);
#endif
            if (SelectedToggle == toggle)
            {
                SelectedToggle = null;
                if (sendCallback)
                {
                    OnValueChanged?.Invoke(null);
                }
            }
        }

        /// <summary>
        /// Ensure that the toggle group still has a valid state. This is only relevant when a ToggleGroup is Started
        /// or a Toggle has been deleted from the group.
        /// </summary>
        public override void EnsureValidState()
        {
            if (!_allowSwitchOff && !AnyTogglesOn() && _toggles.Count != 0)
            {
                _toggles.Sort((t1, t2) => t1.Id.CompareTo(t2.Id));
                _toggles[0].IsOn = true;
            }
        }

        /// <summary>
        /// Switch all toggles off.
        /// </summary>
        /// <remarks>
        /// This method can be used to switch all toggles off, regardless of whether the allowSwitchOff property is enabled or not.
        /// </remarks>
        public override void SetAllTogglesOff(bool sendCallback = true)
        {
            bool oldAllowSwitchOff = _allowSwitchOff;
            _allowSwitchOff = true;

            if (sendCallback)
            {
                for (var i = 0; i < _toggles.Count; i++)
                    _toggles[i].IsOn = false;
            }
            else
            {
                for (var i = 0; i < _toggles.Count; i++)
                    _toggles[i].SetIsOnWithoutNotify(false);
            }

            _allowSwitchOff = oldAllowSwitchOff;
            SelectedToggle = null;
        }
    }
}
