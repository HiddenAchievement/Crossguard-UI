using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
	/// <summary>
	/// A Button slightly customized to work with TweenTransitioner.
	/// </summary>
    public class CrossButton : Button, IPointerDownHandler, IPointerUpHandler
    {
        private ITransitioner _transitioner;

        public UnityEvent OnButtonDown = new UnityEvent();
        public UnityEvent OnButtonUp = new UnityEvent();
        
        private WaitForSeconds _submitHoldWait = null;

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
            _transitioner = GetComponent<ITransitioner>();
            if (Application.isPlaying)
            {
                if (_transitioner == null)
                {
                    Debug.LogWarning(name + " needs a Transitioner component added.");
                    return;
                }
                _submitHoldWait = new WaitForSeconds(_transitioner.TransitionTime);
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            
            if (_transitioner == null)
            { 
                _transitioner = GetComponent<ITransitioner>();
                if (_transitioner == null) return;
            }

            if (_transitioner.IsInteractable != interactable)
            {
                _transitioner.IsInteractable = interactable;
                if (!interactable)
                {
                    StopAllCoroutines();
                }
            }
        }
        
        public override void OnSubmit(BaseEventData unused)
        {
            if (!IsActive() || !IsInteractable() || (_transitioner == null))
                return;
            AnimateClick();
            onClick?.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!interactable) return;
            OnButtonDown?.Invoke();
        }
 
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnButtonUp?.Invoke();
        }

        #region IDrivableControl
        
        public void AnimateClick()
        {
            AnimatePress();
            StartCoroutine(OnFinishPress());
        }

        public void AnimatePress()
        {
            if (!IsActive() || !IsInteractable() || (_transitioner == null))
                return;

            _transitioner.SetStateFlag(InteractState.Pressed, false);
        }

        public void AnimateRelease()
        {
            _transitioner.ClearStateFlag(InteractState.Pressed, false);
        }
        

        public void AnimateIsolated()
        {
            _transitioner.SetStateFlag(InteractState.Isolated, false);
        }

        public void AnimateUnisolated()
        {
            _transitioner.ClearStateFlag(InteractState.Isolated, false);
        }
        
        #endregion
        
        protected IEnumerator OnFinishPress()
        {
            yield return _submitHoldWait;
            AnimateRelease();
        }
    }
}
