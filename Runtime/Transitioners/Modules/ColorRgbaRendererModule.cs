using System.Collections.Generic;
using LitMotion;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class ColorRgbaRendererModule : IStyleModule
    {
        private static readonly CrossInstancePool<ColorRgbaRendererModule> s_pool = new(() => new ColorRgbaRendererModule());
        private readonly Dictionary<Transform, CanvasRendererEntry> _componentCache = new();
        
        public static ColorRgbaRendererModule Create()
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
            CanvasRenderer renderer = component.GetComponent<CanvasRenderer>();
            if (renderer == null) return;
            renderer.SetColor(Color.white);
        }
        
        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not ColorRgbaRendererModuleRule colorRule) return;
            CanvasRenderer renderer = component.GetComponent<CanvasRenderer>();
            if (renderer == null) return;
            renderer.SetColor(colorRule.Color);
        }
        
        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not ColorRgbaRendererModuleRule colorRule) return;
            CanvasRendererEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Component.SetColor(colorRule.Color);
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not ColorRgbaRendererModuleRule colorRule) return;
            CanvasRendererEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Tween = LMotion.Create(entry.Component.GetColor(), colorRule.Color, duration)
                .WithEase(easing)
                .BindToColor(entry.Component);
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
        
        private ColorRgbaRendererModule()
        {}
    }
}