using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A base class for various sorts of spinner components.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(AxisControlHelper))]
    public class CrossSpinnerBase : Selectable, IAxisControl
    {
        [Serializable]
        public class SpinnerEvent : UnityEvent<int> { }

        [SerializeField]
        private int _value = 0;

        public int Value
        {
            get => _value;
            set => Set(value);
        }

        [SerializeField]
        private bool _wrapValues = false;
        public bool WrapValues => _wrapValues;

        [SerializeField]
        private bool _horizontal = true;
        public bool Horizontal => _horizontal;

        [SerializeField]
        private bool _alwaysRequireIsolation = false;
        public bool AlwaysRequireIsolation => _alwaysRequireIsolation;
        
        public SpinnerEvent OnValueChanged = new SpinnerEvent();
        
        public ITransitioner Transitioner { get; private set; }

        public bool IsAtMin => Value == Min;

        public bool IsAtMax => Value == Max;
        
        public bool ReverseValue => false;

        protected AxisControlHelper _axisControlHelper;
        
        
#if UNITY_EDITOR
        protected override void Reset()
        {
            // base.Reset();
            transition = Transition.None;
        }
#endif
        
        protected CrossSpinnerBase() {}
        
        protected override void Awake()
        {
            // base.Awake();
            Transitioner = GetComponent<ITransitioner>();

            if (Application.isPlaying)
            {
                if (Transitioner == null)
                {
                    Debug.LogWarning(name + " needs a Transitioner component added.");
                }
            
                _axisControlHelper = GetComponent<AxisControlHelper>();
            }
        }
        
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if (Transitioner == null)
            {
                Transitioner = GetComponent<ITransitioner>();
                if (Transitioner == null) return;
            }
            if (Transitioner.IsInteractable != interactable)
            {
                Transitioner.IsInteractable = interactable;
                _axisControlHelper.OnInteractableChanged(interactable);
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Set(_value, false);
            UpdateVisuals();
        }

        public override void OnMove(AxisEventData eventData)
        {
            // Bypass default behavior, because Axis Control Helper will manage this.
        }

        public virtual void SetValueWithoutNotify(int input)
        {
            Set(input, false);
        }
        
        protected virtual void Set(int input, bool sendCallback = true)
        {
            int newValue = ClampValue(input);
            if (_value == newValue) return;
            _value = newValue;
            UpdateVisuals();
            if (_axisControlHelper != null)
            {
                _axisControlHelper.UpdateButtons();
            }
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Spinner.value", this);
                OnValueChanged.Invoke(newValue);
            }
        }

        protected int ClampValue(int input) => (input < Min) ? Min : (input > Max) ? Max : input;
        
        public void Decrement()
        {
            int newValue = Value - Step;

            if (newValue < Min)
            {
                newValue = _wrapValues ? Max : Min;
            }

            Value = newValue;
        }
        
        public void Increment()
        {
            int newValue = Value + Step;

            if (newValue > Max)
            {
                newValue = _wrapValues ? Min : Max;
            }

            Value = newValue;
        }

        #region Override These
        public virtual int Step => 1;

        public virtual int Min
        {
            get => 0;
            set { }
        }

        public virtual int Max
        {
            get => int.MaxValue;
            set { }
        }

        protected virtual void UpdateVisuals() { }
        #endregion
    }
}
