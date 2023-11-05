using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
#if INPUT_SYSTEM_AVAILABLE && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

#endif

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// CrossguardUI's Input Field
    /// </summary>
    public class CrossInputField : TMP_InputField
    {
        private ITransitioner _transitioner;
        private bool _isolationMode = false;
        private BaseEventData _eventData;

        private bool _tabPressed;

        public bool IsEditing => _isolationMode;
        
        
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
            if (Application.isPlaying && _transitioner == null)
            {
                Debug.LogWarning(name + " needs a Transitioner component added.");
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if (_transitioner == null)
            {
                _transitioner = GetComponent<ITransitioner>();
            }
            if (_transitioner == null) return;
            if (_transitioner.IsInteractable != interactable)
            {
                _transitioner.IsInteractable = interactable;
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (eventData is AxisEventData) return; // We entered via an axis control. Do not activate.
            _tabPressed = false;
            BeginEditing(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (!_isolationMode) return;
            EnableIsolationMode(false);
            base.OnDeselect(eventData);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            BeginEditing(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!_isolationMode)
            {
                EnableIsolationMode(true);
            }
        }

        private void OnGUI()
        {
            if (!isFocused) return;
            Event evt = Event.current;
            
            if (evt == null) return;
            if (evt.keyCode != KeyCode.Tab) return;

            if (evt.rawType == EventType.KeyDown)
            {
                // We need to make sure that this element is the one that pressed tab, so that we don't
                // advance based on a key up from a tab on another element.
                _tabPressed = true;
            }
            else if (_tabPressed && evt.rawType == EventType.KeyUp)
            {
                _tabPressed = false;
                if (evt.modifiers.HasFlag(EventModifiers.Shift))
                {
                    TriggerEvent(CrossExecuteEvents.PrevFieldHandler);
                }
                else
                {
                    TriggerEvent(CrossExecuteEvents.NextFieldHandler);
                }
            }
        }

        private void BeginEditing(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable() || _isolationMode)
                return;
            EnableIsolationMode(true);
            base.OnSelect(eventData);
        }

        private void EnableIsolationMode(bool on)
        {
            if (on)
            {
                _isolationMode = true;
                _transitioner.SetStateFlag(InteractState.Isolated, false);
#if INPUT_SYSTEM_AVAILABLE && ENABLE_INPUT_SYSTEM
                // Suppress all keyboard processing outside of the Input Field, to avoid whacky behavior.
                InputSystem.DisableDevice(Keyboard.current);
#endif
                onEndEdit.AddListener(DisableIsolationMode);
            }
            else
            {
                _isolationMode = false;
                _transitioner.ClearStateFlag(InteractState.Isolated, false);
#if INPUT_SYSTEM_AVAILABLE && ENABLE_INPUT_SYSTEM
                // Restore keyboard processing.
                InputSystem.EnableDevice(Keyboard.current);
#endif
                onEndEdit.RemoveListener(DisableIsolationMode);
            }
        }

        private void DisableIsolationMode(string unused)
        {
            EnableIsolationMode(false);
        }
        
        private void TriggerEvent<T>(ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
        {
            if (_eventData == null)
            {
                _eventData = new BaseEventData(EventSystem.current);
            }
            
            _eventData.selectedObject = gameObject;
            ExecuteEvents.Execute(gameObject, _eventData, functor);
        }
    }
}
