using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// This will allow you to make Legacy UGUI toggles that are fully Crossguard-compatible.
    /// </summary>
    public class CrossToggle : Toggle, ICrossToggle
    {
        public const int NoId = int.MinValue;
        
        [Serializable]
        public class ToggleIdEvent : UnityEvent<int, bool> {}
        
        [Serializable]
        public class NavSelectedEvent : UnityEvent<int> {}
        
        [Tooltip("A Cross Toggle Group, in case you want to use it instead of the standard Toggle Group")]
        [SerializeField]
        protected CrossToggleGroupBase _crossGroup;
        public CrossToggleGroupBase CrossGroup
        {
            get => _crossGroup;
            set
            {
                SetCrossToggleGroup(value, true);
                PlayEffect(true);
                UpdateCheckedFlag(true);
            }
        }
        
        [Tooltip("A number for uniquely identifying this toggle.")]
        [SerializeField]
        protected int _id = NoId;
        public int Id { get => _id; set => _id = value; }

        public bool IsOn
        {
            get => isOn;
            set => Set(value);
        }
        
        public ToggleIdEvent OnValueChangedForId = new();

        /// <summary>
        /// It's very important to understand that this is not when the toggle is turned on. This is when the toggle is
        /// selected by gamepad/keyboard/tab. This is primarily for ensuring visibility in pick lists, like dropdowns.
        /// </summary>
        [Tooltip("This is when a toggle is selected BY A GAME CONTROLLER OR KEYBOARD, and not when it is toggled.")]
        public NavSelectedEvent OnNavSelected = new();
        
        private ITransitioner _transitioner;
        
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            transition = Transition.None;
        }
#endif
        protected override void Awake()
        {
            base.Awake();
            _transitioner = GetComponent<ITransitioner>();
            if (!Application.isPlaying) return;
            if (_transitioner != null) return;
            Debug.LogWarning($"{name} needs a Transitioner component added.");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                if ((_id == NoId) && (transform.parent.childCount > 0))
                {
                    _id = transform.GetSiblingIndex();
                }                
            }

            SetCrossToggleGroup(_crossGroup, false);
            UpdateCheckedFlag(true);
            onValueChanged.AddListener(ProcessInternalToggle);
        }
        
        protected override void OnDisable()
        {
            onValueChanged.RemoveListener(ProcessInternalToggle);
            SetCrossToggleGroup(null, false);
            base.OnDisable();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnNavSelected?.Invoke(_id);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }

        private void InternalToggle()
        {
            IsOn = !IsOn;
        }

        private void Set(bool value, bool sendCallback = true, bool immediate = false)
        {
            if (value == isOn) return;

            if (_crossGroup != null)
            {
                if (!value && !_crossGroup.AllowSwitchOff && !_crossGroup.AnyTogglesOn())
                {
                    // Nope.
                    isOn = true;
                    return;
                }
            }

            if (sendCallback)
            {
                isOn = value;
            }
            else
            {
                base.SetIsOnWithoutNotify(value);
            }
            
            if (_crossGroup != null && IsActive())
            {
                if (value)
                {
                    _crossGroup.NotifyToggleOn(this, sendCallback);
                }
                else
                {
                    _crossGroup.NotifyToggleOff(this, sendCallback);
                }
            }
            
            UpdateCheckedFlag(immediate);
            if (sendCallback)
            {
                OnValueChangedForId?.Invoke(_id, value);
            }
        }

        public new void SetIsOnWithoutNotify(bool value)
        {
            Set(value, false);
        }

        public void SetIsOnImmediate(bool value)
        {
            Set(value, true, true);
        }
        
        private void SetCrossToggleGroup(CrossToggleGroupBase newGroup, bool setMemberValue)
        {
            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (_crossGroup != null)
                _crossGroup.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                _crossGroup = newGroup;

            // Only register to the new group if this Toggle is active.
            if (newGroup != null && IsActive())
                newGroup.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to _crossGroup here as it's not guaranteed to have been set.
            if (newGroup != null && isOn && IsActive())
                newGroup.NotifyToggleOn(this, false);
        }

        private void ProcessInternalToggle(bool _)
        {
            UpdateCheckedFlag(false);
        }
        
        private void UpdateCheckedFlag(bool immediate)
        {
            if (_transitioner == null) return;
            if (isOn)
            {
                _transitioner.SetStateFlag(InteractState.Checked, immediate);
            }
            else
            {
                _transitioner.ClearStateFlag(InteractState.Checked, immediate);
            }
        }
        
        private void PlayEffect(bool instant) 
        {
            if (graphic == null)
                return;

            // WARNING: These can interfere with the Transition system.
#if UNITY_EDITOR
            if (!Application.isPlaying)
                graphic.canvasRenderer.SetAlpha(isOn ? 1f : 0f);
            else
#endif
            {
                if (toggleTransition == ToggleTransition.None) instant = true;
                graphic.CrossFadeAlpha(isOn ? 1f : 0f, instant ? 0f : 0.1f, true);
            }
        }
        
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            // We don't call the base DoStateTransition here because it does cosmetic things we don't need.
            if (_transitioner == null)
            {
                _transitioner = GetComponent<ITransitioner>();
                if (_transitioner == null) return;
            }
            
            if (_transitioner.IsInteractable != interactable)
            {
                _transitioner.IsInteractable = interactable;
            }
        }
    }
}