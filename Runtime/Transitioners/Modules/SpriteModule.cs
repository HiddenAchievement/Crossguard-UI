using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class SpriteModule : IStyleModule
    {
        private static readonly CrossInstancePool<SpriteModule> s_pool = new(() => new SpriteModule());
        private readonly Dictionary<RectTransform, Image> _componentCache = new();
        
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
        public void CacheComponent(RectTransform component)
        {
            Image image =  component.GetComponent<Image>();
            Debug.Assert(image != null);
            _componentCache[component] = image;
        }

        /// <inheritdoc />
        public void ClearComponent(RectTransform component)
        {
            Image image = component.GetComponent<Image>();
            if (image == null) return;
            image.overrideSprite = null;
        }

        /// <inheritdoc />
        public void ForceComponentRule(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not SpriteModuleRule spriteRule) return;
            Image image = component.GetComponent<Image>();
            if (image == null) return;
            image.overrideSprite = spriteRule.Sprite;
        }
        
        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule)
        {
            if (rule is not SpriteModuleRule spriteRule) return;
            Image image = component.GetComponent<Image>();
            if (image == null) return;
            image.overrideSprite = spriteRule.Sprite;
        }

        /// <inheritdoc />
        public void Transition(RectTransform component, IStyleModuleRule rule, float duration)
        {
            if (rule is not SpriteModuleRule spriteRule) return;
            Image image = _componentCache[component];
            if (image == null) return;
            image.overrideSprite = spriteRule.Sprite;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _componentCache.Clear();
        }
        
        private SpriteModule()
        {}
    }
}