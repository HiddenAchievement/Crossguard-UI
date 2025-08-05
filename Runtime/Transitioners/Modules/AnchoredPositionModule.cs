using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class AnchoredPositionModule : IStyleModule
    {
        private static readonly CrossInstancePool<AnchoredPositionModule> s_pool = new(() => new AnchoredPositionModule());
        private readonly Dictionary<Transform, RectTransformEntry> _componentCache = new();
        
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
            _componentCache[component] = RectTransformEntry.Create(component);
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
            RectTransformEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Component.anchoredPosition = positionRule.Position;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not AnchoredPositionModuleRule positionRule) return;
            RectTransformEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Tween = LMotion.Create(entry.Component.anchoredPosition, positionRule.Position, duration)
                .WithEase(easing)
                .BindToAnchoredPosition(entry.Component);
        }

        /// <inheritdoc />
        public void Reset()
        {
            foreach (RectTransformEntry entry in _componentCache.Values)
            {
                entry.Free();
            }
            _componentCache.Clear();
        }
        
        private AnchoredPositionModule()
        {
        }
    }
}