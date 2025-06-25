using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class PositionModule : IStyleModule
    {
        private static readonly CrossInstancePool<PositionModule> s_pool = new(() => new PositionModule());

        private MotionHandle _motionHandle = MotionHandle.None;
        
        public static PositionModule Create()
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
            component.anchoredPosition = Vector2.one;
        }

        /// <inheritdoc />
        public void ForceComponentRule(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not PositionModuleRule positionRule) return;
            component.anchoredPosition = positionRule.Position;
        }

        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not PositionModuleRule positionRule) return;
            component.anchoredPosition = positionRule.Position;
        }

        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule, float duration)
        {
            if (rule is not PositionModuleRule positionRule) return;
            _motionHandle = LMotion.Create(component.anchoredPosition, positionRule.Position, duration)
                .BindToAnchoredPosition(component);
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
        
        private PositionModule()
        {
        }
    }
}