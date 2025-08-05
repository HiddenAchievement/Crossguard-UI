using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class AnchorMaxModule : IStyleModule
    {
        private static readonly CrossInstancePool<AnchorMaxModule> s_pool = new(() => new AnchorMaxModule());
        private readonly Dictionary<Transform, RectTransformEntry> _componentCache = new();
        
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
            _componentCache[component] = RectTransformEntry.Create(component);
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
            RectTransformEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Component.anchorMax = anchorMaxRule.AnchorMax;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not AnchorMaxModuleRule anchorMaxRule) return;
            RectTransformEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Tween = LMotion.Create(entry.Component.anchorMax, anchorMaxRule.AnchorMax, duration)
                .WithEase(easing)
                .BindToAnchorMax(entry.Component);
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
        
        private AnchorMaxModule()
        {
        }
    }
}