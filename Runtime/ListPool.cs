using System.Collections.Generic;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A very simple list pool.
    /// </summary>
    internal static class ListPool<T>
    {
        private static readonly Stack<List<T>> s_container = new();
        
        public static List<T> Fetch()
        {
            return s_container.Count > 0 ? s_container.Pop() : new List<T>();
        }

        public static void Return(List<T> instance)
        {
            instance.Clear();
            s_container.Push(instance);
        }

        /// <summary>
        /// Use this to empty the pool.
        /// </summary>
        public static void Drain()
        {
            s_container.Clear();
        }
    }
}
