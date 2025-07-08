using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class PivotModule : IStyleModule
    {
        private static readonly CrossInstancePool<PivotModule> s_pool = new(() => new PivotModule());
        
        private MotionHandle _motionHandle = MotionHandle.None;
        
        public static PivotModule Create()
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
            // component.pivot = new Vector2(0.5f, 0.5f);
        }

        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not PivotModuleRule pivotRule) return;
            RectTransform rtComponent = (RectTransform)component;
            rtComponent.pivot = pivotRule.Pivot;
        }
        
        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not PivotModuleRule pivotRule) return;
            RectTransform rtComponent = (RectTransform)component;
            rtComponent.pivot = pivotRule.Pivot;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not PivotModuleRule pivotRule) return;
            RectTransform rtComponent = (RectTransform)component;
            _motionHandle = LMotion.Create(rtComponent.pivot, pivotRule.Pivot, duration)
                .WithEase(easing)
                .BindToPivot(rtComponent);
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
        
        private PivotModule()
        {
        }
    }
}