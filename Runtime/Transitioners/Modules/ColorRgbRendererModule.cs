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
        private readonly Dictionary<RectTransform, CanvasRenderer> _componentCache = new();
        
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
        public void CacheComponent(RectTransform component)
        {
            CanvasRenderer renderer = component.GetComponent<CanvasRenderer>();
            Debug.Assert(renderer != null);
            _componentCache[component] = renderer;
        }
        
        /// <inheritdoc />
        public void ClearComponent(RectTransform component)
        {
            SetOnlyRgb(component.GetComponent<CanvasRenderer>(), Color.white);
        }

        public void ForceComponentRule(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not ColorRgbRendererModuleRule colorRule) return;
            SetOnlyRgb(component.GetComponent<CanvasRenderer>(), colorRule.Color);
        }
        
        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not ColorRgbRendererModuleRule colorRule) return;
            SetOnlyRgb(_componentCache[component], colorRule.Color);
        }

        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule, float duration)
        {
            if (rule is not ColorRgbRendererModuleRule colorRule) return;
            CanvasRenderer renderer = _componentCache[component];
            _motionHandle = LMotion.Create(renderer.GetColor(), colorRule.Color, duration).BindToColorNoAlpha(renderer);
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
            if (_motionHandle != MotionHandle.None && _motionHandle.IsPlaying())
            {
                _motionHandle.Cancel();
            }
            _motionHandle = MotionHandle.None;
        }
        
        private ColorRgbRendererModule()
        {}

    }
}