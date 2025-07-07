using System.Collections.Generic;
using LitMotion;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class AlphaRendererModule : IStyleModule
    {
        private static readonly CrossInstancePool<AlphaRendererModule> s_pool = new(() => new AlphaRendererModule());
        private readonly Dictionary<RectTransform, CanvasRenderer> _componentCache = new();

        private MotionHandle _motionHandle = MotionHandle.None;
        
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
        public void CacheComponent(RectTransform component)
        {
            CanvasRenderer renderer = component.GetComponent<CanvasRenderer>();
            Debug.Assert(renderer != null);
            _componentCache[component] = renderer;
        }

        /// <inheritdoc />
        public void ClearComponent(RectTransform component)
        {
            SetOnlyAlpha(component.GetComponent<CanvasRenderer>(), 1);
        }

        /// <inheritdoc />
        public void ForceComponentRule(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not AlphaRendererModuleRule alphaRule) return;
            SetOnlyAlpha(component.GetComponent<CanvasRenderer>(), alphaRule.Alpha);
        }

        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not AlphaRendererModuleRule alphaRule) return;
            SetOnlyAlpha(_componentCache[component], alphaRule.Alpha);
        }

        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not AlphaRendererModuleRule alphaRule) return;
            CanvasRenderer renderer = _componentCache[component];
            _motionHandle = LMotion.Create(renderer.GetColor().a, alphaRule.Alpha, duration)
                .WithEase(easing)
                .BindToColorA(renderer);
        }

        /// <inheritdoc />
        public void Reset()
        {
            StopTween();
            _componentCache.Clear();
        }

        private static void SetOnlyAlpha(CanvasRenderer renderer, float alpha)
        {
            if (renderer == null) return;
            Color color = renderer.GetColor();
            color.a = alpha;
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
        
        private AlphaRendererModule()
        {
        }
    }
}