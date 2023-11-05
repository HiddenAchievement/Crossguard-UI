using UnityEngine;
using UnityEngine.EventSystems;

#if INPUT_SYSTEM_AVAILABLE && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// This is an addendum to the UI Input Module to pass along some additional events that Crossguard UI is
    /// expecting.  If you're not using Legacy Input or the Input System Package, you'll need to implement your
    /// own version of this.
    /// </summary>
    public class CrossguardInputAdapter : MonoBehaviour
    { 
        private BaseEventData _eventData;
        
        private void Awake()
        {
            _eventData = new BaseEventData(EventSystem.current);
        }

        private void TriggerEventOnSelectedObject<T>(ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected == null) return;
            _eventData.selectedObject = selected;
            ExecuteEvents.Execute(selected, _eventData, functor);
        }
        
        
#if INPUT_SYSTEM_AVAILABLE && ENABLE_INPUT_SYSTEM
        private bool _prevFieldInProgress;
        private InputAction _prevFieldAction;
        private InputAction _nextFieldAction;
        private InputAction _navigateAction;
        
        private void OnEnable()
        {
            InputSystemUIInputModule inputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                Debug.LogError("CrossguardInputAdapter failed to find a InputSystemUIInputModule on the EventSystem object.");
                return;
            }

            InputActionAsset actionsAsset = inputModule.actionsAsset;
            if (actionsAsset == null)
            {
                Debug.LogError("CrossguardInputAdapter failed to find an Actions Asset on the InputSystemUIInputModule.");
                return;
            }
            
            InputActionMap inputMap = actionsAsset.FindActionMap("UI");
            if (inputMap == null)
            {
                Debug.LogError("CrossguardInputAdapter failed to find an input map called \"UI\".");
                return;
            }

            _prevFieldAction = inputMap.FindAction("PrevField");
            _nextFieldAction = inputMap.FindAction("NextField");
            _navigateAction  = inputMap.FindAction("Navigate");

            
            if (_prevFieldAction != null)
            {
                _prevFieldAction.started   += OnPrevFieldStart;
                _prevFieldAction.performed += OnPrevFieldPerform;
                _prevFieldAction.canceled  += OnPrevFieldCancel;
            }

            if (_nextFieldAction != null)
            {
                _nextFieldAction.performed += OnNextFieldPerform;
            }

            if (_navigateAction != null)
            {
                _navigateAction.canceled += OnNavigateCancel;
            }
        }

        private void OnDisable()
        {
            if (_prevFieldAction != null)
            {
                _prevFieldAction.started   -= OnPrevFieldStart;
                _prevFieldAction.performed -= OnPrevFieldPerform;
                _prevFieldAction.canceled  -= OnPrevFieldCancel;
                _prevFieldAction = null;
            }

            if (_nextFieldAction != null)
            {
                _nextFieldAction.performed -= OnNextFieldPerform;
                _nextFieldAction = null;
            }

            if (_navigateAction != null)
            {
                _navigateAction.canceled -= OnNavigateCancel;
                _navigateAction = null;
            }

            _prevFieldInProgress = false;
        }

        private void OnPrevFieldStart(InputAction.CallbackContext context)
        {
            _prevFieldInProgress = true;
        }

        private void OnPrevFieldPerform(InputAction.CallbackContext context)
        {
            TriggerEventOnSelectedObject(CrossExecuteEvents.PrevFieldHandler);
        }
        
        private void OnPrevFieldCancel(InputAction.CallbackContext context)
        {
            _prevFieldInProgress = false;
        }
        
        private void OnNextFieldPerform(InputAction.CallbackContext context)
        {
            if (_prevFieldInProgress) return;
            TriggerEventOnSelectedObject(CrossExecuteEvents.NextFieldHandler);
        }

        private void OnNavigateCancel(InputAction.CallbackContext context)
        {
            TriggerEventOnSelectedObject(CrossExecuteEvents.MoveEndHandler);
        }
#endif

        
#if ENABLE_LEGACY_INPUT_MANAGER
        private bool _wasMoving;
        private bool _isMoving =>
            (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f) ||
            (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f);
        private void Update()
        {
            if (_wasMoving && !_isMoving)
            {
                TriggerEventOnSelectedObject(CrossExecuteEvents.MoveEndHandler);
            }

            _wasMoving = _isMoving;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                GameObject selected = EventSystem.current.currentSelectedGameObject;
                if (selected != null)
                {
                    _eventData.selectedObject = selected;
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        TriggerEventOnSelectedObject(CrossExecuteEvents.PrevFieldHandler);
                    }
                    else
                    {
                        TriggerEventOnSelectedObject(CrossExecuteEvents.NextFieldHandler);
                    }
                }
            }
        }
#endif        
    }
}
