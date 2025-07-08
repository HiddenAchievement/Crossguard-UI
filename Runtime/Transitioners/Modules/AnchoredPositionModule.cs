using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class AnchoredPositionModule : IStyleModule
    {
        private static readonly CrossInstancePool<AnchoredPositionModule> s_pool = new(() => new AnchoredPositionModule());

        private MotionHandle _motionHandle = MotionHandle.None;
        
        public static AnchoredPositionModule Create()
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
            RectTransform rtComponent = (RectTransform)component;
            rtComponent.anchoredPosition = Vector2.one;
        }

        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AnchoredPositionModuleRule positionRule) return;
            RectTransform rtComponent = (RectTransform)component;
            rtComponent.anchoredPosition = positionRule.Position;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AnchoredPositionModuleRule positionRule) return;
            RectTransform rtComponent = (RectTransform)component;
            rtComponent.anchoredPosition = positionRule.Position;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not AnchoredPositionModuleRule positionRule) return;
            RectTransform rtComponent = (RectTransform)component;
            _motionHandle = LMotion.Create(rtComponent.anchoredPosition, positionRule.Position, duration)
                .WithEase(easing)
                .BindToAnchoredPosition(rtComponent);
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
        
        private AnchoredPositionModule()
        {
        }
    }
}