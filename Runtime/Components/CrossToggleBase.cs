using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A base implementation for basic Toggle behavior that can be used for additional custom Crossguard toggles.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CrossToggleBase : CrossSelectable, IPointerClickHandler, ISubmitHandler, ICanvasElement, ICrossToggle
    {
        public const int NoId = int.MinValue;
        
        [Serializable]
        public class ToggleEvent : UnityEvent<bool> {}
        
        [Serializable]
        public class ToggleIdEvent : UnityEvent<int, bool> {}
        
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        protected bool _isOn = true;
        public bool IsOn { get => _isOn; set => Set(value); }

        /// <summary>
        /// A grouping of toggles that behave in a linked way.
        /// </summary>
        [Tooltip("A grouping of toggles that behave in a linked way.")]
        [SerializeField]
        protected CrossToggleGroupBase _group;
        /// <summary>
        /// Group the toggle belongs to.
        /// </summary>
        public CrossToggleGroupBase Group
        {
            get => _group;
            set
            {
                SetToggleGroup(value, true);
                UpdateAppearance(true);
            }
        }

        [Tooltip("A number for uniquely identifying this toggle.")]
        [SerializeField]
        protected int _id = NoId;
        public int Id {  get => _id; set => _id = value; }
        
        public ToggleEvent OnValueChanged = new ToggleEvent();
        public ToggleIdEvent OnValueChangedForId = new ToggleIdEvent();
   
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }
#endif
        
        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
            {
                OnValueChanged?.Invoke(_isOn);
                OnValueChangedForId?.Invoke(_id, _isOn);
            }
#endif
        }
        
        public virtual void LayoutComplete()
        {}

        public virtual void GraphicUpdateComplete()
        {}

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

            SetToggleGroup(_group, false);
            UpdateAppearance(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        public void SetIsOnWithoutNotify(bool value)
        {
            Set(value, false);
        }
        
        protected virtual void Set(bool value, bool sendCallback = true)
        {
            if (_isOn == value)
                return;

            _isOn = value;
            
            if (_group != null)
            {
                if (!value && !_group.AllowSwitchOff && !_group.AnyTogglesOn())
                {
                    // Nope.
                    _isOn = true;
                    return;
                }

                if (IsActive())
                {
                    if (value)
                    {
                        _group.NotifyToggleOn(this, sendCallback);
                    }
                    else
                    {
                        _group.NotifyToggleOff(this, sendCallback);
                    }
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            UpdateAppearance(false);
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("CrossSlideToggle.value", this);
                OnValueChanged?.Invoke(_isOn);
                OnValueChangedForId?.Invoke(_id, _isOn);
            }
        }

        /// <summary>
        /// This plays the on/off transition for the toggle.
        /// </summary>
        /// <param name="instant">Jump immediately to the end of the transition, instead of playing the animation.</param>
        protected virtual void PlayEffect(bool instant) {}

        protected override void Start()
        {
            UpdateAppearance(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            IsOn = !IsOn;
        }
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            InternalToggle();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }
        
        
        private void SetToggleGroup(CrossToggleGroupBase newGroup, bool setMemberValue)
        {
            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (_group != null)
                _group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                _group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (newGroup != null && IsActive())
                newGroup.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && IsOn && IsActive())
                newGroup.NotifyToggleOn(this, false);
        }

        private void UpdateAppearance(bool immediate)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && _transitioner == null) return;
#endif
            PlayEffect(immediate);
            if (_isOn)
            {
                _transitioner.SetStateFlag(InteractState.Checked, immediate);
            }
            else
            {
                _transitioner.ClearStateFlag(InteractState.Checked, immediate);
            }
        }
    }
}
