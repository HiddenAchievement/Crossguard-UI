using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Intended to help out with axis-based components that hijack navigation controls.
    /// </summary>
    public class AxisControlHelper : MonoBehaviour, ISubmitHandler, ICancelHandler, IMoveHandler, IMoveEndHandler
    {
        [SerializeField]
        [Tooltip("Require isolation even if there is no conflicting navigation.")]
        private bool _alwaysRequireIsolation = false;
        
        [SerializeField]
        [Tooltip("A button that can decrement the value (optional).")]
        private CrossButton _decrementButton = null;
        public CrossButton DecrementButton
        {
            get => _decrementButton;
            set => SetDecrementButton(value);
        }
        
        [SerializeField]
        [Tooltip("A button that can increment the value (optional).")]
        private CrossButton _incrementButton = null;
        public CrossButton IncrementButton
        {
            get => _incrementButton;
            set => SetIncrementButton(value);
        }
        
        [SerializeField]
        [Tooltip("How long you must hold one of the buttons to start repeating.")]
        private float _repeatDelay = 0.3f;

        [SerializeField]
        [Tooltip("Repetitions per second.")]
        private float _repeatRate = 6f;
        
        private IAxisControl _control;

        private bool _isolationMode = false;
        
        private WaitForSeconds _holdStart;
        private WaitForSeconds _holdInterval;
        private bool _axisIncrementing;
        private bool _axisDecrementing;

        private void Awake()
        {
            _control = GetComponent<IAxisControl>();
            _holdStart = new WaitForSeconds(_repeatDelay);
            _holdInterval = new WaitForSeconds(1f / _repeatRate);
        }

        private void OnEnable()
        {
            if (_decrementButton != null)
            {
                _decrementButton.OnButtonDown.AddListener(OnDecrementButtonDown);
                _decrementButton.OnButtonUp.AddListener(OnDecrementButtonUp);
            }

            if (_incrementButton != null)
            {
                _incrementButton.OnButtonDown.AddListener(OnIncrementButtonDown);
                _incrementButton.OnButtonUp.AddListener(OnIncrementButtonUp);  
            }

            UpdateButtons();
        }

        private void OnDisable()
        {
            if (_decrementButton != null)
            {
                _decrementButton.OnButtonDown.RemoveListener(OnDecrementButtonDown);
                _decrementButton.OnButtonUp.RemoveListener(OnDecrementButtonUp);
            }

            if (_incrementButton != null)
            {
                _incrementButton.OnButtonDown.RemoveListener(OnIncrementButtonDown);
                _incrementButton.OnButtonUp.RemoveListener(OnIncrementButtonUp);
            }
        }


        private void Navigate(BaseEventData eventData, UIBehaviour sel)
        {
            // Copied from Selectable
            if (sel == null || !sel.IsActive()) return;
            SetIsolationMode(false);
            eventData.selectedObject = sel.gameObject;
        }

        private void Navigate(AxisEventData eventData)
        {
            // Copied from Selectable
            switch (eventData.moveDir)
            {
                case MoveDirection.Right:
                    Navigate(eventData, _control.FindSelectableOnRight());
                    break;

                case MoveDirection.Up:
                    Navigate(eventData, _control.FindSelectableOnUp());
                    break;

                case MoveDirection.Left:
                    Navigate(eventData, _control.FindSelectableOnLeft());
                    break;

                case MoveDirection.Down:
                    Navigate(eventData, _control.FindSelectableOnDown());
                    break;
            }
        }

        public void OnInteractableChanged(bool value)
        {
            if (!value)
            {
                SetIsolationMode(false);
            }

            if (_decrementButton != null)
            {
                _decrementButton.interactable = value && (_control.WrapValues || !_control.IsAtMin);
            }

            if (_incrementButton != null)
            {
                _incrementButton.interactable = value && (_control.WrapValues || !_control.IsAtMax);
            }
        }
        
#region Event Handlers
        public void OnMove(AxisEventData eventData)
        {
            if (!_control.IsActive() || !_control.IsInteractable() || (_alwaysRequireIsolation && !_isolationMode))
            {
                Navigate(eventData);
                return;
            }

            if (_control.Horizontal)
            {
                if ((eventData.moveDir == MoveDirection.Up) || (eventData.moveDir == MoveDirection.Down) ||
                    !(_isolationMode || ((_control.FindSelectableOnLeft() == null) && (_control.FindSelectableOnRight() == null))))
                {
                    Navigate(eventData);
                }
                else if (eventData.moveDir == MoveDirection.Left)
                {
                    if (_control.ReverseValue)
                    {
                        AxisIncrement();
                    }
                    else
                    {
                        AxisDecrement();
                    }
                }
                else
                {
                    if (_control.ReverseValue)
                    {
                        AxisDecrement();
                    }
                    else
                    {
                        AxisIncrement();
                    }
                }
            }
            else
            {
                if ((eventData.moveDir == MoveDirection.Left) || (eventData.moveDir == MoveDirection.Right) ||
                    !(_isolationMode || ((_control.FindSelectableOnUp() == null) && (_control.FindSelectableOnDown() == null))))
                {
                    Navigate(eventData);
                }
                else if (eventData.moveDir == MoveDirection.Up)
                {
                    if (_control.ReverseValue)
                    {
                        AxisDecrement();
                    }
                    else
                    {
                        AxisIncrement();
                    }
                }
                else
                {
                    if (_control.ReverseValue)
                    {
                        AxisIncrement();
                    }
                    else
                    {
                        AxisDecrement();
                    }
                }
            }
        }

        public void OnMoveEnd(BaseEventData eventData)
        {
            if (_axisDecrementing)
            {
                _axisDecrementing = false;
                _decrementButton.AnimateRelease();
            }

            if (_axisIncrementing)
            {
                _axisIncrementing = false;
                _incrementButton.AnimateRelease();
            }
        }


        public void OnSubmit(BaseEventData eventData)
        {
            if (!_control.IsInteractable()) return;
            
            if (_isolationMode)
            {
                SetIsolationMode(false);
            }
            else if (_alwaysRequireIsolation)
            {
                SetIsolationMode(true);
            }
            else if (_control.Horizontal)
            {
                if ((_control.FindSelectableOnLeft() != null) || (_control.FindSelectableOnRight() != null))
                {
                    SetIsolationMode(true);
                }
            }
            else
            {
                if ((_control.FindSelectableOnUp() != null) || (_control.FindSelectableOnDown() != null))
                {
                    SetIsolationMode(true);
                }
            }
        }
        
        public void OnCancel(BaseEventData eventData)
        {
            if (_isolationMode)
            {
                SetIsolationMode(false);
            }
        }
#endregion

        public void SetIsolationMode(bool on)
        {
            if (_isolationMode == on) return;
            
            if (on)
            {
                _isolationMode = true;
                _control.Transitioner.SetStateFlag(InteractState.Isolated, false);
                if (_incrementButton != null)
                {
                    _decrementButton.AnimateIsolated();
                    _incrementButton.AnimateIsolated();
                }
            }
            else
            {
                _isolationMode = false;
                _control.Transitioner.ClearStateFlag(InteractState.Isolated, false);
                if (_incrementButton != null)
                {
                    _decrementButton.AnimateUnisolated();
                    _incrementButton.AnimateUnisolated();
                }
            }
        }
        
        private void OnDecrementButtonDown()
        {
            StartCoroutine(HoldDecrement());
        }

        private void OnDecrementButtonUp()
        {
            StopAllCoroutines();
        }

        private void OnIncrementButtonDown()
        {
            StartCoroutine(HoldIncrement());
        }

        private void OnIncrementButtonUp()
        {
            StopAllCoroutines();
        }
        
        private void AxisDecrement()
        {
            if (!_axisDecrementing)
            {
                if (_decrementButton != null)
                { 
                    _decrementButton.AnimatePress(); 
                }

                _axisDecrementing = true;
            }

            _control.Decrement();
        }
        
        private void AxisIncrement()
        {
            if (!_axisIncrementing)
            {
                if (_incrementButton != null)
                {
                    _incrementButton.AnimatePress();
                }
                _axisIncrementing = true;
            }
           
            _control.Increment();
        }

        private IEnumerator HoldDecrement()
        {
            _control.Decrement();
            yield return _holdStart;
            while (!_control.IsAtMin)
            {
                _control.Decrement();
                yield return _holdInterval;
            }
        }

        private IEnumerator HoldIncrement()
        {
            _control.Increment();
            yield return _holdStart;
            while (!_control.IsAtMax)
            {
                _control.Increment();
                yield return _holdInterval;
            }
        }

        private void SetDecrementButton(CrossButton button)
        {
            if (button == _decrementButton) return;
            
            if (gameObject.activeInHierarchy)
            {
                if (_decrementButton != null)
                {
                    _decrementButton.OnButtonDown.RemoveListener(OnDecrementButtonDown);
                    _decrementButton.OnButtonUp.RemoveListener(OnDecrementButtonUp);
                }

                if (button != null)
                {
                    button.OnButtonDown.AddListener(OnDecrementButtonDown);
                    button.OnButtonUp.AddListener(OnDecrementButtonUp);
                    if (_control != null)
                    {
                        button.interactable = !_control.IsAtMin;                        
                    }
                }
            }

            _decrementButton = button;
        }

        private void SetIncrementButton(CrossButton button)
        {
            if (button == _incrementButton) return;
            
            if (gameObject.activeInHierarchy)
            {
                if (_incrementButton != null)
                {
                    _incrementButton.OnButtonDown.RemoveListener(OnIncrementButtonDown);
                    _incrementButton.OnButtonUp.RemoveListener(OnIncrementButtonUp);
                }

                if (button != null)
                {
                    button.OnButtonDown.AddListener(OnIncrementButtonDown);
                    button.OnButtonUp.AddListener(OnIncrementButtonUp);
                    if (_control != null)
                    {
                        button.interactable = !_control.IsAtMax;                       
                    }
                }
            }

            _incrementButton = button;            
        }
        
        public void UpdateButtons(float unused = 0)
        {
            if (_control == null) return;
            if (_control.WrapValues) return;

            
            if (_decrementButton != null)
            {
                _decrementButton.interactable = !_control.IsAtMin;
            }

            if (_incrementButton != null)
            {
                _incrementButton.interactable = !_control.IsAtMax;
            }
        }
    }
}
