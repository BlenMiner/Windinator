using System;
using System.Collections.Generic;
using UnityEngine;

namespace WindinatorTools
{
    public class UIPool
    {
        GameObject m_gameobject;

        RectTransform m_parent;

        List<GameObject> m_instances;

        private int m_index = 0;

        public UIPool(GameObject gameObject, RectTransform parent)
        {
            m_instances = new List<GameObject>();
            m_gameobject = gameObject;
            m_parent = parent;
        }

        public Action<GameObject> OnInstantiated;

        public void ResetCounter()
        {
            m_index = 0;
        }

        public GameObject GetInstance()
        {
            if (m_instances.Count <= m_index)
            {
                var gameo = GameObject.Instantiate(m_gameobject, m_parent, false);
                OnInstantiated?.Invoke(gameo);
                m_instances.Add(gameo);
            }

            var go = m_instances[m_index++];

            if (!go.activeSelf) go.SetActive(true);

            return go;
        }

        public T GetInstance<T>()
        {
            var i = GetInstance();
            return i.GetComponentInChildren<T>();
        }

        public void DiscardRest()
        {
            for (int i = m_index; i < m_instances.Count; i++)
                m_instances[i].SetActive(false);
            ResetCounter();
        }

        internal void Dispose()
        {
            ResetCounter();

            for (int i = 0; i < m_instances.Count; i++)
                GameObject.Destroy(m_instances[i]);

            m_instances.Clear();
            m_instances.TrimExcess();
        }
    }
}