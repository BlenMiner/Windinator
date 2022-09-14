using Riten.Windinator.LayoutBuilder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using WindinatorTools;
using static Riten.Windinator.LayoutBuilder.Layout;

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

        UIPool m_pool;

        float m_itemSize;

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

            m_pool = new UIPool(Windinator.GetElementPrefab<T>(), m_scrollView.content);
            m_pool.OnInstantiated = go =>
            {
                var fitter = go.GetComponent<ContentSizeFitter>();
                if (fitter != null) GameObject.Destroy(fitter);
            };
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

            m_pool = new UIPool(prefab, m_scrollView.content);
            m_pool.OnInstantiated = go =>
            {
                var fitter = go.GetComponent<ContentSizeFitter>();
                if (fitter != null) GameObject.Destroy(fitter);
            };
            Update();
        }

        private void ScrollChanged(Vector2 newValue)
        {
            Update();
        }

        public void Dispose()
        {
            m_scrollView.onValueChanged.RemoveListener(ScrollChanged);
            m_pool.Dispose();
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
                itemsCountThatFit = Mathf.CeilToInt(view.rect.height / itemSize) + 2;
            }
            else
            {
                minAnchor = new Vector2(0, 0);
                maxAnchor = new Vector2(0, 1);
                dirVector = new Vector2(1, 0);
                scaleVector = new Vector2(1, 0);
                scrollPos = Mathf.Abs(m_scrollView.content.anchoredPosition.x);
                itemsCountThatFit = Mathf.CeilToInt(view.rect.width / itemSize) + 2;
            }

            int indexOffset = Mathf.FloorToInt(scrollPos / itemSize);

            ResizeContent(totalSize, contentSize, minAnchor, maxAnchor);

            m_pool.ResetCounter();

            for (int i = 0; i < itemsCountThatFit; ++i)
            {
                int idx = indexOffset + i;

                if (idx >= m_data.Count) break;

                T element = m_pool.GetInstance<T>();

                RectTransform tr = element.transform as RectTransform;

                if (tr.anchorMin != minAnchor) tr.anchorMin = minAnchor;
                if (tr.anchorMax != maxAnchor) tr.anchorMax = maxAnchor;
                if (tr.pivot != Vector2.up) tr.pivot = Vector2.up;

                var newPos = dirVector * idx * itemSize;
                var newSize = scaleVector * m_itemSize;

                if (tr.anchoredPosition != newPos)
                    tr.anchoredPosition = newPos;

                if (tr.sizeDelta != newSize)
                    tr.sizeDelta = newSize;

                m_requestElement(idx, element, m_data[idx]);
            }

            m_pool.DiscardRest();
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