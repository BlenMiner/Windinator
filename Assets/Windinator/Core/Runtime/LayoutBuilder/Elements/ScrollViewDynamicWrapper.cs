using Riten.Windinator.LayoutBuilder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

namespace Riten.Windinator
{
    public enum UIDirection
    {
        Horizontal,
        Vertical
    }

    public class ScrollViewController<T, D> : IDisposable where T : MonoBehaviour
    {
        UIDirection m_direction;

        Action<int, T, D> m_requestElement;

        ScrollRect m_scrollView;

        IList<D> m_data;

        GameObjectPool<T> m_pool;

        float m_itemSize;

        List<T> m_instances = new List<T>();

        float m_spacing;

        public float Spacing
        {
            get => m_spacing;
            set
            {
                m_spacing = value;
                Update();
            }
        }

        public bool Is(Type a)
        {
            return a.IsEquivalentTo(typeof(T));
        }

        public ScrollViewController(ScrollRect rect, IList<D> data, float elementSize, Action<int, T, D> updateCell, UIDirection direction = UIDirection.Vertical, float spacing = 0f)
        {
            m_spacing = spacing;
            m_requestElement = updateCell;
            m_scrollView = rect;
            m_data = data;
            m_itemSize = elementSize;
            m_direction = direction;

            m_scrollView.onValueChanged.AddListener(ScrollChanged);

            m_pool = new GameObjectPool<T>(Windinator.GetElementPrefab<T>());
            Update();
        }

        public ScrollViewController(ScrollRect rect, GameObject prefab, IList<D> data, float elementSize, Action<int, T, D> updateCell, UIDirection direction = UIDirection.Vertical, float spacing = 0f)
        {
            m_spacing = spacing;
            m_requestElement = updateCell;
            m_scrollView = rect;
            m_data = data;
            m_itemSize = elementSize;
            m_direction = direction;

            m_scrollView.onValueChanged.AddListener(ScrollChanged);

            m_pool = new GameObjectPool<T>(prefab);
            Update();
        }

        private void ScrollChanged(Vector2 newValue)
        {
            Update();
        }

        public void Dispose()
        {
            m_scrollView.onValueChanged.RemoveListener(ScrollChanged);
            m_pool.DestroyAll();
        }

        public void Update()
        {
            RectTransform view = m_scrollView.transform as RectTransform;

            float itemSize = (m_itemSize + m_spacing);

            float totalSize = itemSize * m_data.Count;
            var contentSize = m_scrollView.content.sizeDelta;

            Vector2 minAnchor, maxAnchor, dirVector, scaleVector;
            int itemsCountThatFit;
            float scrollPos;

            if (m_direction == UIDirection.Vertical)
            {
                minAnchor = new Vector2(0, 1);
                maxAnchor = new Vector2(1, 1);
                dirVector = new Vector2(0, -1);
                scaleVector = new Vector2(0, 1);
                scrollPos = Mathf.Abs(m_scrollView.content.anchoredPosition.y);
                itemsCountThatFit = Mathf.RoundToInt(view.rect.height / itemSize) + 2;
            }
            else
            {
                minAnchor = new Vector2(0, 0);
                maxAnchor = new Vector2(0, 1);
                dirVector = new Vector2(1, 0);
                scaleVector = new Vector2(1, 0);
                scrollPos = Mathf.Abs(m_scrollView.content.anchoredPosition.x);
                itemsCountThatFit = Mathf.RoundToInt(view.rect.width / itemSize) + 2;
            }

            int indexOffset = Mathf.FloorToInt(scrollPos / itemSize);

            int count = indexOffset + itemsCountThatFit;
            int diff = count - m_data.Count;

            if (diff > 0) itemsCountThatFit -= diff;

            ResizeContent(totalSize, contentSize, minAnchor, maxAnchor);
            AllocateNecessaryInstances(itemsCountThatFit, minAnchor, maxAnchor);

            for (int i = 0; i < m_instances.Count; ++i)
            {
                T element = m_instances[i];

                RectTransform tr = element.transform as RectTransform;

                var newPos = dirVector * (i + indexOffset) * itemSize;
                var newSize = scaleVector * m_itemSize;

                if (tr.anchoredPosition != newPos)
                    tr.anchoredPosition = newPos;

                if (tr.sizeDelta != newSize)
                    tr.sizeDelta = newSize;

                int idx = indexOffset + i;
                if (idx < m_data.Count) m_requestElement(idx, element, m_data[idx]);
            }
        }

        private void AllocateNecessaryInstances(int itemsCountThatFit, Vector2 minAnchor, Vector2 maxAnchor)
        {
            while (m_instances.Count < itemsCountThatFit)
            {
                var prefab = m_pool.Allocate(m_scrollView.content);
                var tr = prefab.transform as RectTransform;

                var sizeFitter = prefab.gameObject.GetComponent<ContentSizeFitter>();
                if (sizeFitter != null) GameObject.Destroy(sizeFitter);

                if (tr.anchorMin != minAnchor) tr.anchorMin = minAnchor;
                if (tr.anchorMax != maxAnchor) tr.anchorMax = maxAnchor;
                tr.pivot = Vector2.up;

                m_instances.Add(prefab);
            }

            while (m_instances.Count > itemsCountThatFit)
            {
                int last = m_instances.Count - 1;
                var instance = m_instances[last];
                m_instances.RemoveAt(last);
                m_pool.Free(instance);
            }
        }

        private void ResizeContent(float totalSize, Vector2 contentSize, Vector2 minAnchor, Vector2 maxAnchor)
        {
            if (m_scrollView.content.anchorMin != minAnchor)
                m_scrollView.content.anchorMin = minAnchor;

            if (m_scrollView.content.anchorMax != maxAnchor)
                m_scrollView.content.anchorMax = maxAnchor;

            if (m_direction == UIDirection.Vertical)
            {
                if (contentSize.y != totalSize)
                {
                    contentSize.y = totalSize;
                    m_scrollView.content.sizeDelta = contentSize;
                }
            }
            else
            {
                if (contentSize.x != totalSize)
                {
                    contentSize.x = totalSize;
                    m_scrollView.content.sizeDelta = contentSize;
                }
            }
        }
    }
}