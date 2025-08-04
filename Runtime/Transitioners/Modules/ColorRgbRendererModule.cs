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
        private readonly Dictionary<Transform, CanvasRenderer> _componentCache = new();
        
        private MotionHandle _motionHandle = MotionHandle.None;
        
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
            CanvasRenderer renderer = component.GetComponent<CanvasRenderer>();
            Debug.Assert(renderer != null);
            _componentCache[component] = renderer;
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
            StopTween();
            SetOnlyRgb(_componentCache[component], colorRule.Color);
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not ColorRgbRendererModuleRule colorRule) return;
            StopTween();
            CanvasRenderer renderer = _componentCache[component];
            _motionHandle = LMotion.Create(renderer.GetColor(), colorRule.Color, duration)
                .WithEase(easing)
                .BindToColorNoAlpha(renderer);
        }

        /// <inheritdoc />
        public void Reset()
        {
            StopTween();
            _componentCache.Clear();
        }
        
        private static void SetOnlyRgb(CanvasRenderer renderer, Color color)
        {
            if (renderer == null) return;
            color.a = renderer.GetColor().a;
            renderer.SetColor(color);
        }
        
        private void StopTween()
        {
            if (_motionHandle != MotionHandle.None)
            {
                _motionHandle.TryCancel();
            }
            _motionHandle = MotionHandle.None;
        }
        
        private ColorRgbRendererModule()
        {}

    }
}