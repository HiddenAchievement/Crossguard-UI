using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// An improved Animator Transitioner compatible with Crossguard UI.
    /// </summary>
    public class AnimationTransitioner : AbstractTransitioner
    {
        [SerializeField]
        [Tooltip("The animator that describes state appearances and transitions.")]
        private Animator _animator = null;

        [SerializeField]
        [Tooltip("This should be about how long it takes to transition into a press, used for Driven click animations.")]
        private float _transitionTime = 0.15f;
        public override float TransitionTime => _transitionTime;
        
        protected override void ClearAllComponents()
        {
            if (!Application.isPlaying) return;
            if (!gameObject.activeInHierarchy) return;
            float speed = _animator.speed;
            _animator.speed = float.MaxValue;
            for (int i = 0; i < (int) InteractState.Count; i++)
            {
                _animator.SetBool(((InteractState) i).ToString(), false);
            }
            _animator.speed = speed;
        }

        protected override void ForceAppearance(InteractState state)
        {
            TransitionOn(state, true);
        }

        protected override void TransitionOn(InteractState state, bool immediate)
        {
            if (!Application.isPlaying) return;
            if (!gameObject.activeInHierarchy) return;
            if (immediate)
            {
                float speed = _animator.speed;
                _animator.speed = float.MaxValue;
                _animator.SetBool(state.ToString(), true);
                _animator.speed = speed;
            }
            else
            {
                _animator.SetBool(state.ToString(), true);
            }
        }

        protected override void TransitionOff(InteractState state, bool immediate)
        {
            if (!Application.isPlaying) return;
            if (!gameObject.activeInHierarchy) return;
            if (immediate)
            {
                float speed = _animator.speed;
                _animator.speed = float.MaxValue;
                _animator.SetBool(state.ToString(), false);
                _animator.speed = speed;
            }
            else
            {
                _animator.SetBool(state.ToString(), false);
            }
        }
    }
}
