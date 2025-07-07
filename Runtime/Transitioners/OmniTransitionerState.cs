using LitMotion;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    [RequireComponent(typeof(OmniTransitioner))]
    public class OmniTransitionerState : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The name of the state (should match an enum, if you're using one).")]
        private string _name;
        public string Name => _name;

        [SerializeField]
        [Tooltip("The easing to use when transitioning to this state.")]
        private Ease _easing;
        public Ease Easing => _easing;

        [SerializeField]
        private ModularStyleEntry[] _style;
        public ModularStyleEntry[] Style => _style;
    }
}