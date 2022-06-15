using Riten.Windinator.LayoutBuilder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace Riten.Windinator
{
    public class ScrollViewDynamicRuntime : MonoBehaviour
    {
        [SerializeField] ScrollRect m_scrollView;

        Action m_dispose;

        Action m_update;

        Func<Type, bool> m_is;

        bool m_dirty = false;

        bool m_first = true;

        public void Setup<T, D>(
            IList<D> data,
            float elementSize,
            Action<int, T, D> updateCell,
            UIDirection direction = UIDirection.Vertical,
            float spacing = 0f
        ) where T : LayoutBaker
        {
            Setup(Windinator.GetElementPrefab<T>(), data, elementSize, updateCell, direction, spacing);
        }

        public void Setup<T, D>(
            GameObject prefab,
            IList<D> data,
            float elementSize,
            Action<int, T, D> updateCell,
            UIDirection direction = UIDirection.Vertical,
            float spacing = 0f
        ) where T : MonoBehaviour
        {
            if (m_is != null && m_is(typeof(T)))
            {
                m_update?.Invoke();
                return;
            }

            m_dispose?.Invoke();

            var scrollView = new ScrollViewController<T, D>(m_scrollView, prefab, data, elementSize, updateCell, direction, spacing);

            m_update = () =>
            {
                scrollView.Update();
            };

            m_dispose = () =>
            {
                scrollView.Dispose();
            };

            m_is = (a) =>
            {
                return scrollView.Is(a);
            };

            return;
        }

        private void Start()
        {
            SetDirty();
        }

        private void OnDestroy()
        {
            m_dispose?.Invoke();
        }

        private void LateUpdate()
        {
            if (m_dirty)
            {
                m_update?.Invoke();

                // This just ensures unity had time to calculate the layout sizes
                if (m_first)
                {
                    m_first = false;
                    return;
                }

                m_dirty = false;
            }
        }

        public void SetDirty()
        {
            m_dirty = true;
        }
    }
}
