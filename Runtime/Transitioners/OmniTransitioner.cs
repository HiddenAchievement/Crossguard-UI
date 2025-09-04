using System;
using System.Collections;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    [ExecuteAlways]
    public class OmniTransitioner : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Time in seconds the transitions will occur over.")]
        private float _transitionTime = 0.25f;
        public float TransitionTime => _transitionTime;

        [SerializeField]
        [Tooltip("Whether states should be treated as mutually exclusive.")]
        private bool _mutuallyExclusive = true;

        [SerializeField]
        [Tooltip("Turn this on if you want the transitioner to reset to state 0 when enabled.")]
        private bool _resetOnEnable;
        
        private BitArray _stateFlags;
        private OmniTransitionerState[] _states;
        private ModularStyleEntry[][] _stateStyles;
        private bool _initialized;
        
        private readonly ModularTransitionManager _manager = new();
        private int _lastState; // For mutually-exclusive;
        
        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            Initialize();
            if (_resetOnEnable)
            {
                ResetState();
            }
        }
        
        private void OnDestroy()
        {
            _manager.Cleanup();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            ManualInitializeAppearance();
        }

        private void Reset()
        {
            ManualInitializeAppearance();
        }

        private void ManualInitializeAppearance()
        {
            _initialized = false;
            
            ClearAllComponents();
            ForceAppearance(0);
        }

#endif // if UNITY_EDITOR

        #region Public Interface

        public void SetState(int state)
        {
            SetStateFlag(state, false);
        }
        
        public void SetState(int state, bool immediate)
        {
            SetStateFlag(state, immediate);
        }
        
        public void ClearState(int state)
        {
            ClearStateFlag(state, false);
        }

        public void ClearState(int state, bool immediate)
        {
            ClearStateFlag(state, immediate);
        }
        
        public void SetState(bool state)
        {
            SetStateFlag(state ? 1 : 0, false);
        }
        
        public void SetState(bool state, bool immediate)
        {
            SetStateFlag(state ? 1 : 0, immediate);
        }
        
        public void SetOn()
        {
            SetStateFlag(1, false);
        }

        public void SetOn(bool immediate)
        {
            SetStateFlag(1, immediate);
        }

        public void SetOff()
        {
            SetStateFlag(0, false);
        }
        
        public void SetOff(bool immediate)
        {
            SetStateFlag(0, immediate);
        }

        public void Open()
        {
            SetStateFlag(1, false);
        }
        
        public void Open(bool immediate)
        {
            SetStateFlag(1, immediate);
        }
        
        public void Close()
        {
            SetStateFlag(0, false);
        }

        public void Close(bool immediate)
        {
            SetStateFlag(0, immediate);
        }
        
        public void ToggleState()
        {
            SetStateFlag(_stateFlags[1] ? 0 : 1, false);
        }
        
        public void ToggleState(bool immediate)
        {
            SetStateFlag(_stateFlags[1] ? 0 : 1, immediate);
        }
        
        public void IncrementState()
        {
            SetStateFlag(Mathf.Min(_lastState + 1, _states.Length - 1), false);
        }

        public void IncrementState(bool immediate)
        {
            SetStateFlag(Mathf.Min(_lastState + 1, _states.Length - 1), immediate);
        }
        
        public void DecrementState()
        {
            SetStateFlag(Math.Max(_lastState - 1, 0), false);
        }

        public void DecrementState(bool immediate)
        {
            SetStateFlag(Math.Max(_lastState - 1, 0), immediate);
        }
        
        public void CycleState()
        {
            SetStateFlag(_lastState + 1 % _states.Length, false);
        }

        public void CycleState(bool immediate)
        {
            SetStateFlag(_lastState + 1 % _states.Length, immediate);
        }
        #endregion
        
        private void Initialize()
        {
            if (!Application.isPlaying) return;
            if (_initialized) return;
            
            _states = GetComponentsInChildren<OmniTransitionerState>();
            _stateFlags  = new BitArray(_states.Length);
            _stateStyles = new ModularStyleEntry[_states.Length][];
            for (int i = 0; i < _states.Length; i++)
            {
                _stateStyles[i] = _states[i].Style;
            }
            
            InitializeStates();
            
            _lastState = 0;
            SetStateFlag(0, true);

            // Turn on any flags that were enabled before initializing.
            for (int i = 0; i < _stateFlags.Length; i++)
            {
                if (_stateFlags[i])
                {
                    _lastState = i;
                    TransitionOn(i, true);
                }
            }
            _initialized = true;
        }
        
        private void SetStateFlag(int flag, bool immediate, bool force = false)
        {
            // Debug.Log("<color=lime>SetStateFlag " + name + " flag: " + flag + "</color>");
            if (!force && _stateFlags[flag]) return;

            if (_mutuallyExclusive)
            {
                _stateFlags[_lastState] = false;
                _lastState = flag;
            }
            
            _stateFlags[flag] = true;
            
            if (!_initialized) return;

            TransitionOn(flag, immediate || !gameObject.activeInHierarchy);
        }

        private bool GetStateFlag(int flag)
        {
            return _stateFlags[flag];
        }

        private void ClearStateFlag(int flag, bool immediate, bool force = false)
        {
            if (flag == 0) return; // Normal is always present as a fallback.

            if (_lastState == flag)
            {
                _lastState = 0;
            }
            
            if (!force && !_stateFlags[flag]) return;
            _stateFlags[flag] = false;

            if (!_initialized) return;

            // Debug.Log("<color=orange>ClearStateFlag " + flag + "</color>");
            TransitionOff(flag, immediate || !gameObject.activeInHierarchy);
        }
        
        private void ResetState()
        {
            _lastState = 0;
            if (_stateFlags == null) return;
            for (int i = 1; i < _stateFlags.Length; i++)
            {
                _stateFlags[i] = false;
            }
            SetStateFlag(0, true, true);
        }
        
        private void InitializeStates()
        {
            if (_states == null)
            {
                Debug.LogWarning($"OmniTransitioner {name} is missing a Style.", gameObject);
                return;
            }
            _manager.Initialize(transform, _transitionTime, _mutuallyExclusive, _stateStyles);
            _manager.InitializeStates();
        }
        
        private void ClearAllComponents()
        {
            OmniTransitionerState stateDesc = GetComponent<OmniTransitionerState>();
            if (stateDesc == null) return;
            ModularTransitionManager.ClearAllComponents(transform, stateDesc.Style);
        }
        
        private void ForceAppearance(int state)
        {
            OmniTransitionerState[] stateDesc = GetComponents<OmniTransitionerState>();
            if (stateDesc == null || stateDesc.Length == 0) return;
            ModularTransitionManager.ForceAppearance(transform, stateDesc[state].Style);
            _lastState = state;
        }
        
        private void TransitionOn(int state, bool immediate)
        {
            _manager.TransitionOn(state, immediate, _states[state].Easing);
        }
        
        private void TransitionOff(int state, bool immediate)
        {
            _manager.TransitionOff(state, immediate, _stateFlags);
        }

    }
}