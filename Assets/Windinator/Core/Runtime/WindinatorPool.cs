using System;
using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator
{
    public class WindinatorPool
    {
        bool m_optimize = false;

        Dictionary<Type, Queue<WindinatorBehaviour>> m_instances;

        public WindinatorPool(bool optimize)
        {
            m_optimize = optimize;
            m_instances = new Dictionary<Type, Queue<WindinatorBehaviour>>();
        }

        /// <summary>
        /// Mark window free for reuse later.
        /// </summary>
        /// <param name="window">Window instance</param>
        public void Free(WindinatorBehaviour window)
        {
            var t = window.GetType();

            if (!m_instances.TryGetValue(t, out var queue))
            {
                queue = new Queue<WindinatorBehaviour>();
                m_instances.Add(t, queue);
            }

            queue.Enqueue(window);
            Deactivate(window);
        }

        /// <summary>
        /// Allocate a new window if available reuse existing one.
        /// </summary>
        /// <param name="prefab">Prefab to be used if instantiation is needed</param>
        /// <param name="parent">Parent for the instantiated prefab</param>
        /// <typeparam name="T">Returns a window instance of type T</typeparam>
        /// <returns></returns>
        public T Allocate<T>(GameObject prefab, Transform parent) where T : WindinatorBehaviour
        {
            return (T)Allocate(typeof(T), prefab, parent);
        }

        public WindinatorBehaviour Allocate(Type type, GameObject prefab, Transform parent)
        {
            if (m_instances.TryGetValue(type, out var queue) && queue.Count > 0)
            {
                var result = (WindinatorBehaviour)queue.Dequeue();
                Activate(result);
                return result;
            }
            else
            {
                return (WindinatorBehaviour)GameObject.Instantiate(prefab, parent).GetComponent(type);
            }
        }

        public WindinatorBehaviour PreAllocate(Type type, GameObject prefab, Transform parent)
        {
            var window = (WindinatorBehaviour)GameObject.Instantiate(prefab, parent).GetComponent(type);
            Deactivate(window);
            Free(window);
            return window;
        }

        public void Activate(WindinatorBehaviour window)
        {
            if (m_optimize)
            {
                window.enabled = true;
                window.Canvas.enabled = true;
                window.CanvasGroup.blocksRaycasts = true;
            }
            else
            {
                window.gameObject.SetActive(true);
            }
        }

        public void Deactivate(WindinatorBehaviour window)
        {
            if (m_optimize)
            {
                window.enabled = false;
                window.Canvas.enabled = false;
                window.CanvasGroup.blocksRaycasts = false;
            }
            else
            {
                window.gameObject.SetActive(false);
            }
        }
    }
}
