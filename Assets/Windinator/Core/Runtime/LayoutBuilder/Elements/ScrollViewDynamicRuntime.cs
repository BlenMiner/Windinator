using Riten.Windinator.LayoutBuilder;
using System;
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

        bool m_dirty = false;

        public ScrollViewController<T, D> Setup<T, D>(
            ObservableCollection<D> data,
            float elementSize,
            Action<T, D> updateCell,
            UIDirection direction = UIDirection.Vertical,
            float spacing = 0f
        ) where T : LayoutBaker
        {
            var scrollView = new ScrollViewController<T, D>(m_scrollView, data, elementSize, updateCell, direction, spacing);

            m_update = () =>
            {
                scrollView.Update();
            };

            m_dispose = () =>
            {
                scrollView.Dispose();
            };

            return scrollView;
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
                m_dirty = false;
            }
        }

        public void SetDirty()
        {
            m_dirty = true;
        }
    }
}
