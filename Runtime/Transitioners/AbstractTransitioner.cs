#if UNITY_IOS || UNITY_ANDROID
#define INTERACT_MOBILE
#elif UNITY_STANDALONE || UNITY_WEBGL
#define INTERACT_DESKTOP
#else
#define INTERACT_CONSOLE
#endif

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Provides a base class for all Crossguard Transitioners.
    /// </summary>
    public abstract class AbstractTransitioner : MonoBehaviour, ITransitioner, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, /*IMoveHandler,*/ INextFieldHandler, IPrevFieldHandler
    {
        [SerializeField]
        [Tooltip("(Optional) Previous item in a tab navigation sequence.")]
        private Selectable _tabBefore = null;
        
        [SerializeField]
        [Tooltip("(Optional) Next item in a tab navigation sequence.")]
        private Selectable _tabAfter = null;

        [SerializeField]
        [Tooltip("Select this item when it is enabled, if we're showing selections. (Usually used for the first navigation item.)")]
        private bool _selectOnEnable = false;

        [SerializeField]
        [Tooltip("(Optional) A scene element that can be moved to mark this item when it is selected.")]
        private SelectionIndicator _selectionIndicator = null;
        
        private Selectable _selectable;
        private bool _initialized = false;
        private static bool s_axisNavMode;
        public static bool AxisNavMode => s_axisNavMode;
        
        protected readonly BitArray _stateFlags = new BitArray((int)InteractState.Count);

        #region Unity Functions
        
        protected void Awake()
        {
            Initialize();
        }
        
        private void Start()
        {
            Initialize();
            if (_selectable != null)
            {
                // Selectable REALLY likes to acquire target graphics, and do unwanted things
                // to them, so we're going to clear it out, just in case there's still one attached.
                _selectable.targetGraphic = null;
            }
        }
        
        private void OnEnable()
        {
            Initialize();
            ResetState();
            // Make sure these match.
            if (_selectable != null)
            {
                IsInteractable = _selectable.interactable;
            }

            if (Application.isPlaying)
            {
                if (_selectOnEnable)
                {
                    StartCoroutine(SelectAtEndOfFrame());
                }              
            }
        }

        private void Initialize()
        {
            if (!Application.isPlaying) return;
            if (_initialized) return;
            
            InitializeStates();
            
            SetStateFlag(InteractState.Normal, true);
            // Turn on any flags that were enabled before initializing.
            for (int i = 0; i < _stateFlags.Length; i++)
            {
                if (_stateFlags[i])
                {
                    TransitionOn((InteractState)i, true);
                }
            }
            _selectable = GetComponent<Selectable>();
                
            _initialized = true;
        }
        
        
        
#if UNITY_EDITOR
        protected void OnValidate()
        {
            ManualInitializeAppearance();
        }

        protected void Reset()
        {
            ManualInitializeAppearance();
        }

        protected void ManualInitializeAppearance()
        {
            _initialized = false;
            
            ClearAllComponents();
            ForceAppearance(InteractState.Normal);

            Selectable selectable = GetComponent<Selectable>();
            if (selectable != null && !selectable.IsInteractable())
            {
                ForceAppearance(InteractState.Disabled);
            }
        }

#endif // if UNITY_EDITOR
        
        #endregion
        
        #region Event Handlers
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (_stateFlags[(int)InteractState.Disabled]) return;

            SetStateFlag(InteractState.Pressed, false);
            
            s_axisNavMode = !eventData.pointerPressRaycast.isValid;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            ClearStateFlag(InteractState.Pressed, false);
            
            s_axisNavMode = !eventData.pointerPressRaycast.isValid;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("<color=lime>OnPointerEnter</color>");
            SetStateFlag(InteractState.Highlighted, false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log("<color=lime>OnPointerExit</color>");
            ClearStateFlag(InteractState.Highlighted, false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (eventData is AxisEventData)
            {
                s_axisNavMode = true;
            }
            else if (eventData is PointerEventData)
            {
                s_axisNavMode = false;
            }
            // If this is neither of the above event types, then we want to leave our mode as it is.
            if (!_showSelection) return;
            SetStateFlag(InteractState.Selected, false);
            if (_selectionIndicator != null)
            {
                _selectionIndicator.Select((RectTransform)transform);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            ClearStateFlag(InteractState.Selected, false);
            if (_selectionIndicator != null)
            {
                _selectionIndicator.Deselect((RectTransform)transform);
            }
        }
        
        /*
        public void OnMove(AxisEventData eventData)
        {
            s_axisNavMode = true;
        }
        */

        public void OnNextField(BaseEventData eventData)
        {
            Selectable next = _tabAfter;
            while (next != null)
            {
                if (next.isActiveAndEnabled && next.interactable)
                {
                    next.Select();
                    break;
                }
                AbstractTransitioner transitioner = next.GetComponent<AbstractTransitioner>();
                if (transitioner == null) break;
                next = transitioner._tabAfter;
            }
        }

        public void OnPrevField(BaseEventData eventData)
        {
            if (_tabBefore == null) return;
            _tabBefore.Select();

            Selectable prev = _tabBefore;
            while (prev != null)
            {
                if (prev.isActiveAndEnabled && prev.interactable)
                {
                    prev.Select();
                    break;
                }

                AbstractTransitioner transitioner = prev.GetComponent<AbstractTransitioner>();
                if (transitioner == null) break;
                prev = transitioner._tabBefore;
            }
        }

        #endregion

        #region Public Interface

        public abstract float TransitionTime { get; }

        public bool IsInteractable
        {
            get => !GetStateFlag(InteractState.Disabled);
            set
            {
                if (value)
                {
                    ClearStateFlag(InteractState.Disabled, true);
                }
                else
                {
                    SetStateFlag(InteractState.Disabled, true);
                }
            }
        }

        public void SetStateFlag(InteractState flag, bool immediate, bool force = false)
        {
            //Debug.Log("<color=lime>SetStateFlag " + name + " flag: " + flag + "</color>");
            if (!force && _stateFlags[(int)flag]) return;
            _stateFlags[(int)flag] = true;

            if (!_initialized) return;

            TransitionOn(flag, immediate || !gameObject.activeInHierarchy);
        }

        public bool GetStateFlag(InteractState flag)
        {
            return _stateFlags[(int)flag];
        }

        public void ClearStateFlag(InteractState flag, bool immediate, bool force = false)
        {
            if (!force && !_stateFlags[(int)flag]) return;
            _stateFlags[(int)flag] = false;

            if (!_initialized) return;

            // Debug.Log("<color=orange>ClearStateFlag " + flag + "</color>");

            TransitionOff(flag, immediate || !gameObject.activeInHierarchy);
        }

        public void ResetState()
        {
            for (int i = 1; i < (int)_stateFlags.Length; i++)
            {
                _stateFlags[i] = false;
            }
            SetStateFlag(InteractState.Normal, true, true);
        }
        
        #endregion
        
        #region Interior Overridables
        protected virtual void InitializeStates() { }

        protected abstract void ClearAllComponents();

        protected abstract void ForceAppearance(InteractState state);

        protected abstract void TransitionOn(InteractState state, bool immediate);

        protected abstract void TransitionOff(InteractState state, bool immediate);
        
        #endregion

        protected IEnumerator SelectAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            
            // If there's still no event system, delay until there is one.
            while (EventSystem.current == null)
            {
                yield return null;
            }
            
            EventSystem.current.SetSelectedGameObject(_selectable.gameObject);
        }

        protected virtual bool _showSelection
        {
            get
            {
#if INTERACT_CONSOLE
                return true;
#elif INTERACT_MOBILE
                return s_axisNavMode;
#else
                return s_axisNavMode || _tabAfter || _tabBefore;
#endif
            }
        }
        
        
    }
}
