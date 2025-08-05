using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class LocalScaleModule : IStyleModule
    {
        private static readonly CrossInstancePool<LocalScaleModule> s_pool = new(() => new LocalScaleModule());
        private readonly Dictionary<Transform,TransformEntry> _componentCache = new();
        
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
            _componentCache[component] = TransformEntry.Create(component);
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
            TransformEntry entry = _componentCache[component];
            entry.StopTween();
            component.localScale = new Vector3(scaleRule.Scale.x, scaleRule.Scale.y, 1);
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not LocalScaleModuleRule scaleRule) return;
            TransformEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Tween = LMotion.Create(component.localScale, scaleRule.Scale, duration)
                .WithEase(easing)
                .BindToLocalScale(component);
        }

        /// <inheritdoc />
        public void Reset()
        {
            foreach (TransformEntry entry in _componentCache.Values)
            {
                entry.Free();
            }
            _componentCache.Clear();
        }
        
        private LocalScaleModule()
        {
        }
    }
}