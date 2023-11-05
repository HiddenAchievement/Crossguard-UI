using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A Slider slightly customized to work with TweenTransitioner.
    /// </summary>
    [RequireComponent(typeof(AxisControlHelper))]
    public class CrossSlider : Slider, IAxisControl
    {
        public bool WrapValues => false;

        public bool Horizontal => direction == Direction.LeftToRight || direction == Direction.RightToLeft;
        public bool ReverseValue => direction == Direction.RightToLeft || direction == Direction.TopToBottom;
        protected float _stepSize => wholeNumbers ? 1 : (maxValue - minValue) * 0.1f;

        public ITransitioner Transitioner { get; private set; }

        public bool IsAtMin => value < minValue + 0.01f;
        public bool IsAtMax => value > maxValue - 0.01f;

        private AxisControlHelper _axisControlHelper;

        protected CrossSlider() { }

#if UNITY_EDITOR
        protected override void Reset()
        {
            // base.Reset();
            transition = Transition.None;
        }
#endif
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
        

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_axisControlHelper == null) return;
            onValueChanged.AddListener(_axisControlHelper.UpdateButtons);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_axisControlHelper == null) return;
            onValueChanged.AddListener(_axisControlHelper.UpdateButtons);
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
        
        public override void OnMove(AxisEventData eventData)
        {
            // _axisControlHelper.OnMove(eventData);
        }
        
        
        public virtual void Increment()
        {
            value = Mathf.Clamp01(value + _stepSize);
        }

        public virtual void Decrement()
        {
            value = Mathf.Clamp01(value - _stepSize);
        }
    }
}
