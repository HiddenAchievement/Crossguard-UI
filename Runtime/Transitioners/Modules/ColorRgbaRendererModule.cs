using System.Collections.Generic;
using LitMotion;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class ColorRgbaRendererModule : IStyleModule
    {
        private static readonly CrossInstancePool<ColorRgbaRendererModule> s_pool = new(() => new ColorRgbaRendererModule());
        private readonly Dictionary<RectTransform, CanvasRenderer> _componentCache = new();
        
        private MotionHandle _motionHandle = MotionHandle.None;
        
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
        public void CacheComponent(RectTransform component)
        {
            CanvasRenderer renderer = component.GetComponent<CanvasRenderer>();
            Debug.Assert(renderer != null);
            _componentCache[component] = renderer;
        }
        
        /// <inheritdoc />
        public void ClearComponent(RectTransform component)
        {
            CanvasRenderer renderer = component.GetComponent<CanvasRenderer>();
            if (renderer == null) return;
            renderer.SetColor(Color.white);
        }
        
        /// <inheritdoc />
        public void ForceComponentRule(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not ColorRgbaRendererModuleRule colorRule) return;
            CanvasRenderer renderer = component.GetComponent<CanvasRenderer>();
            if (renderer == null) return;
            renderer.SetColor(colorRule.Color);
        }
        
        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not ColorRgbaRendererModuleRule colorRule) return;
            CanvasRenderer renderer = _componentCache[component];
            if (renderer == null) return;
            renderer.SetColor(colorRule.Color);
        }

        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not ColorRgbaRendererModuleRule colorRule) return;
            CanvasRenderer renderer = _componentCache[component];
            _motionHandle = LMotion.Create(renderer.GetColor(), colorRule.Color, duration)
                .WithEase(easing)
                .BindToColor(renderer);
        }
        
        /// <inheritdoc />
        public void Reset()
        {
            StopTween();
            _componentCache.Clear();
        }
        
        private void StopTween()
        {
            if (_motionHandle != MotionHandle.None && _motionHandle.IsPlaying())
            {
                _motionHandle.Cancel();
            }
            _motionHandle = MotionHandle.None;
        }
        
        private ColorRgbaRendererModule()
        {}
    }
}