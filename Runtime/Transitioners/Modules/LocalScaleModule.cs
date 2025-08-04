using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class LocalScaleModule : IStyleModule
    {
        private static readonly CrossInstancePool<LocalScaleModule> s_pool = new(() => new LocalScaleModule());

        private MotionHandle _motionHandle = MotionHandle.None;
        
        public static LocalScaleModule Create()
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
            component.localScale = Vector3.one;
        }

        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not LocalScaleModuleRule scaleRule) return;
            component.localScale = new Vector3(scaleRule.Scale.x, scaleRule.Scale.y, 1);
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not LocalScaleModuleRule scaleRule) return;
            StopTween();
            component.localScale = new Vector3(scaleRule.Scale.x, scaleRule.Scale.y, 1);
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not LocalScaleModuleRule scaleRule) return;
            StopTween();
            _motionHandle = LMotion.Create(component.localScale, scaleRule.Scale, duration)
                .WithEase(easing)
                .BindToLocalScale(component);
        }

        /// <inheritdoc />
        public void Reset()
        {
            StopTween();
        }

        private void StopTween()
        {
            if (_motionHandle != MotionHandle.None)
            {
                _motionHandle.TryCancel();
            }
            _motionHandle = MotionHandle.None;
        }
        
        private LocalScaleModule()
        {
        }
    }
}