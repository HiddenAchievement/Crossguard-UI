using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class AnchorMaxModule : IStyleModule
    {
        private static readonly CrossInstancePool<AnchorMaxModule> s_pool = new(() => new AnchorMaxModule());

        private MotionHandle _motionHandle = MotionHandle.None;
        
        public static AnchorMaxModule Create()
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
            // rtComponent.anchorMax = Vector2.zero;
        }

        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AnchorMaxModuleRule anchorMaxRule) return;
            RectTransform rtComponent = (RectTransform)component;
            rtComponent.anchorMax = anchorMaxRule.AnchorMax;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AnchorMaxModuleRule anchorMaxRule) return;
            RectTransform rtComponent = (RectTransform)component;
            rtComponent.anchorMax = anchorMaxRule.AnchorMax;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not AnchorMaxModuleRule anchorMaxRule) return;
            RectTransform rtComponent = (RectTransform)component;
            _motionHandle = LMotion.Create(rtComponent.anchorMax, anchorMaxRule.AnchorMax, duration)
                .WithEase(easing)
                .BindToAnchorMax(rtComponent);
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
        
        private AnchorMaxModule()
        {
        }
    }
}