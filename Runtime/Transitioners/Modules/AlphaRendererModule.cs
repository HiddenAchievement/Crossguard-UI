using System.Collections.Generic;
using LitMotion;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class AlphaRendererModule : IStyleModule
    {
        private static readonly CrossInstancePool<AlphaRendererModule> s_pool = new(() => new AlphaRendererModule());
        private readonly Dictionary<Transform,CanvasRendererEntry> _componentCache = new();
        
        public static AlphaRendererModule Create()
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
            _componentCache[component] = CanvasRendererEntry.Create(component);
        }

        /// <inheritdoc />
        public void ClearComponent(Transform component)
        {
            SetOnlyAlpha(component.GetComponent<CanvasRenderer>(), 1);
        }

        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AlphaRendererModuleRule alphaRule) return;
            SetOnlyAlpha(component.GetComponent<CanvasRenderer>(), alphaRule.Alpha);
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not AlphaRendererModuleRule alphaRule) return;
            CanvasRendererEntry entry = _componentCache[component];
            entry.StopTween();
            SetOnlyAlpha(entry.Component, alphaRule.Alpha);
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not AlphaRendererModuleRule alphaRule) return;
            CanvasRendererEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Tween = LMotion.Create(entry.Component.GetColor().a, alphaRule.Alpha, duration)
                .WithEase(easing)
                .BindToColorA(entry.Component);
        }

        /// <inheritdoc />
        public void Reset()
        {
            foreach (CanvasRendererEntry entry in _componentCache.Values)
            {
                entry.Free();
            }
            _componentCache.Clear();
        }

        private static void SetOnlyAlpha(CanvasRenderer renderer, float alpha)
        {
            if (renderer == null) return;
            Color color = renderer.GetColor();
            color.a = alpha;
            renderer.SetColor(color);
        }

        private AlphaRendererModule()
        {
        }
    }
}