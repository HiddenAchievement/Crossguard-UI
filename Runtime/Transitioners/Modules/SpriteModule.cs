using System.Collections.Generic;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class SpriteModule : IStyleModule
    {
        private static readonly CrossInstancePool<SpriteModule> s_pool = new(() => new SpriteModule());
        private readonly Dictionary<Transform, ImageEntry> _componentCache = new();
        
        public static SpriteModule Create()
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
            image.overrideSprite = null;
        }

        /// <inheritdoc />
        public void ForceComponentRule(Transform component, IStyleModuleRule rule)
        {
            if (rule is not SpriteModuleRule spriteRule) return;
            Image image = component.GetComponent<Image>();
            if (image == null) return;
            image.overrideSprite = spriteRule.Sprite;
        }
        
        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule)
        {
            if (rule is not SpriteModuleRule spriteRule) return;
            ImageEntry entry = _componentCache[component];
            entry.Component.overrideSprite = spriteRule.Sprite;
        }

        /// <inheritdoc />
        public void Transition(Transform component, IStyleModuleRule rule, float duration, Ease _)
        {
            if (rule is not SpriteModuleRule spriteRule) return;
            ImageEntry entry = _componentCache[component];
            entry.Component.overrideSprite = spriteRule.Sprite;
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
        
        private SpriteModule()
        {}
    }
}