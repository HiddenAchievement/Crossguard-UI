using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class LocalRotationModule : IStyleModule
    {
        private static readonly CrossInstancePool<LocalRotationModule> s_pool = new(() => new LocalRotationModule());

        private MotionHandle _motionHandle = MotionHandle.None;
        
        public static LocalRotationModule Create()
        {
            return s_pool.Fetch();
        }

        /// <inheritdoc />
        public void Free()
        {
            s_pool.Return(this);
        }

        /// <inheritdoc />
        public void CacheComponent(Transform component)
        {
            // We just need the transform, so there's nothing to cache.
        }

        /// <inheritdoc />
        public void ClearComponent(Transform component)
        {
            component.localEulerAngles = Vector3.zero;
        }

        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not LocalRotationModuleRule rotationRule) return;
            component.localEulerAngles = rotationRule.Rotation;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not LocalRotationModuleRule rotationRule) return;
            StopTween();
            component.localEulerAngles = rotationRule.Rotation;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not LocalRotationModuleRule rotationRule) return;
            StopTween();
            _motionHandle = LMotion.Create(component.localEulerAngles, rotationRule.Rotation, duration)
                .WithEase(easing)
                .BindToLocalEulerAngles(component);
        }

        /// <inheritdoc />
        public void Reset()
        {
            StopTween();
        }

        private void StopTween()
        {
            if (_motionHandle != MotionHandle.None && _motionHandle.IsPlaying())
            {
                _motionHandle.TryCancel();
            }
            _motionHandle = MotionHandle.None;
        }
        
        private LocalRotationModule()
        {
        }
    }
}