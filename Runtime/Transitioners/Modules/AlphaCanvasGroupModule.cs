using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class AlphaCanvasGroupModule  : IStyleModule
    {
        private static readonly CrossInstancePool<AlphaCanvasGroupModule> s_pool = new(() => new AlphaCanvasGroupModule());
        private readonly Dictionary<Transform, CanvasGroup> _componentCache = new();

        private MotionHandle _motionHandle = MotionHandle.None;
        
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
            CanvasGroup canvasGroup = component.GetComponent<CanvasGroup>();
            Debug.Assert(canvasGroup != null);
            _componentCache[component] = canvasGroup;
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
            StopTween();
            CanvasGroup canvasGroup = component.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                component.GetComponent<CanvasGroup>().alpha = alphaRule.Alpha;
            }
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not AlphaCanvasGroupModuleRule alphaRule) return;
            StopTween();
            CanvasGroup canvasGroup = _componentCache[component];
            _motionHandle = LMotion.Create(canvasGroup.alpha, alphaRule.Alpha, duration)
                .WithEase(easing)
                .BindToAlpha(canvasGroup);
        }

        /// <inheritdoc />
        public void Reset()
        {
            StopTween();
            _componentCache.Clear();
        }
        
        private void StopTween()
        {
            if (_motionHandle != MotionHandle.None)
            {
                _motionHandle.TryCancel();
            }
            _motionHandle = MotionHandle.None;
        }
        
        private AlphaCanvasGroupModule()
        {
        }
    }
}