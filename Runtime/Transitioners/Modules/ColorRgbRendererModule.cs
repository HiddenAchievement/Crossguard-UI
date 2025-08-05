using System.Collections.Generic;
using LitMotion;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    /// <summary>
    /// Styles the color (but not the alpha) of a component.
    /// </summary>
    public class ColorRgbRendererModule : IStyleModule
    {
        private static readonly CrossInstancePool<ColorRgbRendererModule> s_pool = new(() => new ColorRgbRendererModule());
        private readonly Dictionary<Transform, CanvasRendererEntry> _componentCache = new();
        
        public static ColorRgbRendererModule Create()
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
            SetOnlyRgb(component.GetComponent<CanvasRenderer>(), Color.white);
        }

        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not ColorRgbRendererModuleRule colorRule) return;
            SetOnlyRgb(component.GetComponent<CanvasRenderer>(), colorRule.Color);
        }
        
        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not ColorRgbRendererModuleRule colorRule) return;
            CanvasRendererEntry entry = _componentCache[component];
            entry.StopTween();
            SetOnlyRgb(entry.Component, colorRule.Color);
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not ColorRgbRendererModuleRule colorRule) return;
            CanvasRendererEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Tween = LMotion.Create(entry.Component.GetColor(), colorRule.Color, duration)
                .WithEase(easing)
                .BindToColorNoAlpha(entry.Component);
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
        
        private static void SetOnlyRgb(CanvasRenderer renderer, Color color)
        {
            if (renderer == null) return;
            color.a = renderer.GetColor().a;
            renderer.SetColor(color);
        }
        
        private ColorRgbRendererModule()
        {}

    }
}