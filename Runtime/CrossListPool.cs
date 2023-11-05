using System.Collections.Generic;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public static class CrossListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly CrossObjectPool<List<T>> s_ListPool = new CrossObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }
}
