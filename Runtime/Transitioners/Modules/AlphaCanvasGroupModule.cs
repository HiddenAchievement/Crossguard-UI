using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class AlphaCanvasGroupModule  : IStyleModule
    {
        private static readonly CrossInstancePool<AlphaCanvasGroupModule> s_pool = new(() => new AlphaCanvasGroupModule());
        private readonly Dictionary<Transform, CanvasGroupEntry> _componentCache = new();
        
        public static AlphaCanvasGroupModule Create()
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
            _componentCache[component] = CanvasGroupEntry.Create(component);
        }

        /// <inheritdoc />
        public void ClearComponent(Transform component)
        {
            component.GetComponent<CanvasGroup>().alpha = 1f;
        }

        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AlphaCanvasGroupModuleRule alphaRule) return;
            CanvasGroup canvasGroup = component.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                component.GetComponent<CanvasGroup>().alpha = alphaRule.Alpha;
            }
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AlphaCanvasGroupModuleRule alphaRule) return;
            CanvasGroupEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Component.alpha = alphaRule.Alpha;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not AlphaCanvasGroupModuleRule alphaRule) return;
            CanvasGroupEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Tween = LMotion.Create(entry.Component.alpha, alphaRule.Alpha, duration)
                .WithEase(easing)
                .BindToAlpha(entry.Component);
        }

        /// <inheritdoc />
        public void Reset()
        {
            foreach (CanvasGroupEntry entry in _componentCache.Values)
            {
                entry.Free();
            }
            _componentCache.Clear();
        }
        
        private AlphaCanvasGroupModule()
        {
        }
    }
}