using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class ImageFillModule : IStyleModule
    {
        private static readonly CrossInstancePool<ImageFillModule> s_pool = new(() => new ImageFillModule());
        private readonly Dictionary<Transform, Image> _componentCache = new();

        private MotionHandle _motionHandle = MotionHandle.None;
        
        public static ImageFillModule Create()
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
            Image image = component.GetComponent<Image>();
            Debug.Assert(image != null);
            _componentCache[component] = image;
        }
        
        /// <inheritdoc />
        public void ClearComponent(Transform component)
        {
            Image image = component.GetComponent<Image>();
            if (image == null) return;
            image.fillAmount = 1;
        }
        
        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not ImageFillModuleRule fillRule) return;
            Image image = component.GetComponent<Image>();
            if (image == null) return;
            image.fillAmount = fillRule.Fill;
        }
        
        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not ImageFillModuleRule fillRule) return;
            Image image = _componentCache[component];
            if (image == null) return;
            image.fillAmount = fillRule.Fill;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not ImageFillModuleRule fillRule) return;
            Image image = _componentCache[component];
            _motionHandle = LMotion.Create(image.fillAmount, fillRule.Fill, duration)
                .WithEase(easing)
                .BindToFillAmount(image);
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
        
        private ImageFillModule()
        {}
    }
}