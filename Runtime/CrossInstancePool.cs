using System;
using System.Collections.Generic;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A simple non-threadsafe instance pool.
    /// </summary>
    public class CrossInstancePool<T> where T : IResettable
    {
        private readonly Stack<T> _container = new();
        private readonly Func<T> _generator;
        
        public CrossInstancePool(Func<T> generator)
        {
            _generator = generator;
        }
        
        public T Fetch()
        {
            if (_container.Count > 0)
            {
                return _container.Pop();
            }
            Debug.Assert(_generator != null);
            return _generator.Invoke();
        }

        public void Return(T instance)
        {
            instance.Reset();
            _container.Push(instance);
        }

        /// <summary>
        /// Use this to empty the pool.
        /// </summary>
        public void Drain()
        {
            _container.Clear();
        }
    }
}