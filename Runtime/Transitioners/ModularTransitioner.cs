#if UNITY_IOS || UNITY_ANDROID
#define INTERACT_MOBILE
#elif UNITY_STANDALONE || UNITY_WEBGL
#define INTERACT_DESKTOP
#else
#define INTERACT_CONSOLE
#endif

using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    public class ModularTransitioner : AbstractTransitioner
    {
        [SerializeField]
        [Tooltip("Assign the scriptable object containing this element's style.")]
        private ModularStyle _style;

        private readonly ModularTransitionManager _manager = new();
        
        /// <inheritdoc />
        public override float TransitionTime => _style == null ? 0 : _style.TransitionTime;

        private void OnDestroy()
        {
            _manager.Cleanup();
        }

        /// <inheritdoc />
        protected override void InitializeStates()
        {
            if (_style == null)
            {
                Debug.LogWarning($"ModularTransitioner {name} is missing a Style.", gameObject);
                return;
            }
            _manager.Initialize(transform, _style.TransitionTime, false, new[] {
                _style.NormalState,
#if INTERACT_DESKTOP
                _style.HighlightedState,
#endif
                _style.SelectedState,
                _style.PressedState,
#if !INTERACT_MOBILE
                _style.IsolatedState,
#endif
                _style.CheckedState,
                _style.DisabledState
            });

            _manager.InitializeStates();
        }
        
        #region AbstractTransitioner
        /// <inheritdoc />
        protected override void ClearAllComponents()
        {
            if (_style == null) return;
            ModularTransitionManager.ClearAllComponents(transform, _style.NormalState);
        }

        /// <inheritdoc />
        protected override void ForceAppearance(InteractState state)
        {
            if (_style == null) return;
            ModularTransitionManager.ForceAppearance(transform, state switch
            {
                InteractState.Normal      => _style.NormalState,
                InteractState.Highlighted => _style.HighlightedState,
                InteractState.Selected    => _style.SelectedState,
                InteractState.Pressed     => _style.PressedState,
                InteractState.Isolated    => _style.IsolatedState,
                InteractState.Checked     => _style.CheckedState,
                InteractState.Disabled    => _style.DisabledState,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            });
        }

        /// <inheritdoc />
        protected override void TransitionOn(InteractState state, bool immediate)
        {
            _manager.TransitionOn((int)state, immediate);
        }
        
        /// <inheritdoc />
        protected override void TransitionOff(InteractState state, bool immediate)
        {
            _manager.TransitionOff((int)state, immediate, _stateFlags);
        }
        #endregion
    }
}