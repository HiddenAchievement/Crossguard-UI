using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class PivotModule : IStyleModule
    {
        private static readonly CrossInstancePool<PivotModule> s_pool = new(() => new PivotModule());
        private readonly Dictionary<Transform, RectTransformEntry> _componentCache = new();

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
            _componentCache[component] = RectTransformEntry.Create(component);
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
            RectTransformEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Component.pivot = pivotRule.Pivot;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not PivotModuleRule pivotRule) return;
            RectTransformEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Tween = LMotion.Create(entry.Component.pivot, pivotRule.Pivot, duration)
                .WithEase(easing)
                .BindToPivot(entry.Component);
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
        
        private PivotModule()
        {
        }
    }
}