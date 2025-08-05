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
        private readonly Dictionary<Transform, ImageEntry> _componentCache = new();
        
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
            _componentCache[component] = ImageEntry.Create(component);
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
            ImageEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Component.fillAmount = fillRule.Fill;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease easing)
        {
            if (rule is not ImageFillModuleRule fillRule) return;
            ImageEntry entry = _componentCache[component];
            entry.StopTween();
            entry.Tween = LMotion.Create(entry.Component.fillAmount, fillRule.Fill, duration)
                .WithEase(easing)
                .BindToFillAmount(entry.Component);
        }
        
        /// <inheritdoc />
        public void Reset()
        {
            foreach (ImageEntry entry in _componentCache.Values)
            {
                entry.Free();
            }
            _componentCache.Clear();
        }
        
        private ImageFillModule()
        {}
    }
}