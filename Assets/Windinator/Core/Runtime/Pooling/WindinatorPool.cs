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

    public class GameObjectPool<T> where T : Component
    {
        Queue<T> m_instances;

        List<T> m_active;

        GameObject m_prefab;

        public GameObjectPool(GameObject prefab)
        {
            m_instances = new Queue<T>();
            m_active = new List<T>();
            m_prefab = prefab;
        }

        /// <summary>
        /// Mark window free for reuse later.
        /// </summary>
        /// <param name="instance">Window instance</param>
        public void Free(T instance)
        {
            m_active.Remove(instance);
            m_instances.Enqueue(instance);
            Deactivate(instance);
        }

        public T Allocate(Transform parent)
        {
            if (m_instances.Count > 0)
            {
                var result = m_instances.Dequeue();

                if (result.transform.parent != parent)
                    result.transform.SetParent(parent, false);

                Activate(result);
                m_active.Add(result);
                return result;
            }
            else
            {
                var i = GameObject.Instantiate(m_prefab, parent).GetComponent<T>();
                m_active.Add(i);
                return i;
            }
        }

        public C Allocate<C>(Transform parent) where C : Component
        {
            return Allocate(parent).GetComponentInChildren<C>();
        }

        public T PreAllocate(Transform parent)
        {
            var instance = GameObject.Instantiate(m_prefab, parent).GetComponent<T>();
            Free(instance);
            return instance;
        }

        public void Activate(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        public void Deactivate(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        internal void DestroyAllFree()
        {
            foreach (var go in m_instances)
                GameObject.Destroy(go.gameObject);

            m_instances.Clear();
        }

        internal void DestroyAll()
        {
            DestroyAllFree();

            foreach (var go in m_active)
                GameObject.Destroy(go.gameObject);

            m_active.Clear();
        }
    }

    public class GenericPool<T>
    {
        Queue<T> m_instances;

        List<T> m_active;

        Func<T> m_contructor;

        public GenericPool(Func<T> contructor)
        {
            m_contructor = contructor;
            m_instances = new Queue<T>();
            m_active = new List<T>();
        }

        /// <summary>
        /// Mark window free for reuse later.
        /// </summary>
        /// <param name="instance">Window instance</param>
        public void Free(T instance)
        {
            m_active.Remove(instance);
            m_instances.Enqueue(instance);
        }

        public T Allocate()
        {
            if (m_instances.Count > 0)
            {
                var result = m_instances.Dequeue();
                Activate(result);
                m_active.Add(result);
                return result;
            }
            else
            {
                var i = m_contructor();
                m_active.Add(i);
                return i;
            }
        }

        public T PreAllocate(Transform parent)
        {
            var instance = m_contructor();
            Free(instance);
            return instance;
        }

        public void Activate(T instance)
        { }

        public void Deactivate(T instance)
        { }

        internal void DestroyAllFree()
        {
            m_instances.Clear();
        }

        internal void DestroyAll()
        {
            DestroyAllFree();
            m_active.Clear();
        }
    }
}
