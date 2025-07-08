using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class AnchorMinModule : IStyleModule
    {
        private static readonly CrossInstancePool<AnchorMinModule> s_pool = new(() => new AnchorMinModule());

        private MotionHandle _motionHandle = MotionHandle.None;
        
        public static AnchorMinModule Create()
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
            // We can't do this because it triggers RectTransformDimensionsChange, which is not allowed in Awake.
            // RectTransform rtComponent = (RectTransform)component;
            // rtComponent.anchorMin = Vector2.zero;
        }

        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AnchorMinModuleRule anchorMinRule) return;
            RectTransform rtComponent = (RectTransform)component;
            rtComponent.anchorMin = anchorMinRule.AnchorMin;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AnchorMinModuleRule anchorMinRule) return;
            RectTransform rtComponent = (RectTransform)component;
            rtComponent.anchorMin = anchorMinRule.AnchorMin;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not AnchorMinModuleRule anchorMinRule) return;
            RectTransform rtComponent = (RectTransform)component;
            _motionHandle = LMotion.Create(rtComponent.anchorMin, anchorMinRule.AnchorMin, duration)
                .WithEase(easing)
                .BindToAnchorMin(rtComponent);
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
        
        private AnchorMinModule()
        {
        }
    }
}