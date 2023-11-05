using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Crossguard's Selectable
    /// </summary>
    public class CrossSelectable : Selectable
	{
        protected ITransitioner _transitioner;

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
                }                
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
            }
        }
    }
}
