#if CROSS_REVAMP

using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A Toggle slightly customized to work with TweenTransitioner.
    /// </summary>
    public class CrossRevampToggle : CrossToggleBase
	{
        /// <summary>
        /// Display settings for when a toggle is activated or deactivated.
        /// </summary>
        public enum ToggleTransition
        {
            /// <summary>
            /// Show / hide the toggle instantly
            /// </summary>
            None,

            /// <summary>
            /// Fade the toggle in / out smoothly.
            /// </summary>
            Fade
        }
        
        /// <summary>
        /// Transition mode for the toggle.
        /// </summary>
        [SerializeField]
        private ToggleTransition _toggleTransition = ToggleTransition.Fade;
        public ToggleTransition CheckTransition
        {
            get => _toggleTransition;
            set => _toggleTransition = value;
        }

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        [SerializeField]
        private Graphic _checkGraphic;
        public Graphic CheckGraphic
        {
            get => _checkGraphic;
            set
            {
                _checkGraphic = value;
                PlayEffect(true);
            }
        }
        
        protected override void PlayEffect(bool instant)
        {
            if (_checkGraphic == null)
                return;

            // TODO: We need to get this to where the transition system can't interfere with it, and vice-versa.
#if UNITY_EDITOR
            if (!Application.isPlaying)
                _checkGraphic.canvasRenderer.SetAlpha(_isOn ? 1f : 0f);
            else
#endif
            {
                if (_toggleTransition == ToggleTransition.None) instant = true;
                _checkGraphic.CrossFadeAlpha(_isOn ? 1f : 0f, instant ? 0f : 0.1f, true);
            }
        }
    }
}

#endif // CROSS_REVAMP
