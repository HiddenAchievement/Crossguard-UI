using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class ScaleModule : IStyleModule
    {
        private static readonly CrossInstancePool<ScaleModule> s_pool = new(() => new ScaleModule());

        private MotionHandle _motionHandle = MotionHandle.None;
        
        public static ScaleModule Create()
        {
            return s_pool.Fetch();
        }

        /// <inheritdoc />
        public void Free()
        {
            s_pool.Return(this);
        }

        /// <inheritdoc />
        public void CacheComponent(RectTransform component)
        {
            // We just need the transform, so there's nothing to cache.
        }

        /// <inheritdoc />
        public void ClearComponent(RectTransform component)
        {
            component.localScale = Vector3.one;
        }

        /// <inheritdoc />
        public void ForceComponentRule(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not ScaleModuleRule scaleRule) return;
            component.localScale = new Vector3(scaleRule.Scale.x, scaleRule.Scale.y, 1);
        }

        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not ScaleModuleRule scaleRule) return;
            component.localScale = new Vector3(scaleRule.Scale.x, scaleRule.Scale.y, 1);
        }

        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not ScaleModuleRule scaleRule) return;
            _motionHandle = LMotion.Create(
                new Vector3(component.localScale.x, component.localScale.y, 1f),
                new Vector3(scaleRule.Scale.x, scaleRule.Scale.y, 1f),
                duration)
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
            if (_motionHandle != MotionHandle.None && _motionHandle.IsPlaying())
            {
                _motionHandle.Cancel();
            }
            _motionHandle = MotionHandle.None;
        }
        
        private ScaleModule()
        {
        }
    }
}