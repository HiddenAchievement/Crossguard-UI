using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public class ComponentCacheEntry<T, C> : IResettable where C : ComponentCacheEntry<T, C>, new()
    {
        protected static readonly CrossInstancePool<C> s_pool = new(() => new C());
        public T Component;
        public MotionHandle Tween = MotionHandle.None;

        protected void Initialize(Transform transform)
        {
            Component = transform.GetComponent<T>();
            if (Component == null)
            {
                Debug.LogError($"Crossguard: Could not find component of type {typeof(T)} on {transform.name}.");
            }
        }

        public void StopTween()
        {
            if (Tween != MotionHandle.None)
            {
                Tween.TryCancel();
            }
            Tween = MotionHandle.None;
        }

        public void Reset()
        {
            StopTween();
            Component = default;
            Tween = MotionHandle.None;
        }
    }

    public class CanvasGroupEntry : ComponentCacheEntry<CanvasGroup, CanvasGroupEntry>
    {
        public static CanvasGroupEntry Create(Transform transform)
        {
            CanvasGroupEntry entry = s_pool.Fetch();
            entry.Initialize(transform);
            return entry;
        }

        public void Free()
        {
            s_pool.Return(this);
        }
    }

    public class CanvasRendererEntry : ComponentCacheEntry<CanvasRenderer, CanvasRendererEntry>
    {
        public static CanvasRendererEntry Create(Transform transform)
        {
            CanvasRendererEntry entry = s_pool.Fetch();
            entry.Initialize(transform);
            return entry;
        }
        public void Free()
        {
            s_pool.Return(this);
        }
    }

    public class ImageEntry :  ComponentCacheEntry<Image, ImageEntry>
    {
        public static ImageEntry Create(Transform transform)
        {
            ImageEntry entry = s_pool.Fetch();
            entry.Initialize(transform);
            return entry;
        }
        public void Free()
        {
            s_pool.Return(this);
        }
    }

    public class TransformEntry :  ComponentCacheEntry<Transform, TransformEntry>
    {
        public static TransformEntry Create(Transform transform)
        {
            TransformEntry entry = s_pool.Fetch();
            entry.Initialize(transform);
            return entry;
        }
        public void Free()
        {
            s_pool.Return(this);
        }
    }

    public class RectTransformEntry : ComponentCacheEntry<RectTransform, RectTransformEntry>
    {
        public static RectTransformEntry Create(Transform transform)
        {
            RectTransformEntry entry = s_pool.Fetch();
            entry.Initialize(transform);
            return entry;
        }
        public void Free()
        {
            s_pool.Return(this);
        }
    }
}