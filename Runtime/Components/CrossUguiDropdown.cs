using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    public class CrossUguiDropdown : TMP_Dropdown
    {
        private ITransitioner _transitioner;
        private ScrollRect _scrollRect;

        public UnityEvent OnShowDropdown;
        
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
            if (_transitioner == null)
            {
                Debug.LogWarning($"{name} needs a Transitioner component added.");
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
        
        /// <summary>
        /// Handling for when the dropdown is initially 'clicked'. Typically shows the dropdown
        /// </summary>
        /// <param name="eventData">The associated event data.</param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            EnsureSelectedItemVisible();
            OnShowDropdown?.Invoke();
        }
        
        /// <summary>
        /// Handling for when the dropdown is selected and a submit event is processed. Typically shows the dropdown
        /// </summary>
        /// <param name="eventData">The associated event data.</param>
        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            EnsureSelectedItemVisible();
            OnShowDropdown?.Invoke();
        }

        public virtual void OnToggleSelected()
        {
            EnsureSelectedItemVisible();
        }

        /// <summary>
        /// UGUI's dropdown has a bug that causes it to not scroll, if one navigates through the list of items, by
        /// keyboard or gamepad. The intent of this logic is to fix this bug.
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnItemSelected(DropdownItem item)
        {
            if (_scrollRect == null)
            {
                _scrollRect = item.GetComponentInParent<ScrollRect>();
            }

            if (_scrollRect.content.rect.width > 0)
            {
                EnsureItemVisible(item);
            }
            else
            {
                StartCoroutine(EnsureItemVisibleEventually(item));
            }
        }
        
        protected void EnsureSelectedItemVisible()
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            if (go == null) return;
            DropdownItem item = go.GetComponent<DropdownItem>();
            if (item == null) return;
            OnItemSelected(item);
        }
        
        protected virtual void EnsureItemVisible(DropdownItem item)
        {
            UiUtilities.EnsureItemVisible(_scrollRect, (RectTransform)item.transform);
        }

        private IEnumerator EnsureItemVisibleEventually(DropdownItem item)
        {
            while (_scrollRect.content.rect.width < 0)
            {
                yield return null;
            }
            EnsureItemVisible(item);
        }
    }
}