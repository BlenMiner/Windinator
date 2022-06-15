using Riten.Windinator.LayoutBuilder;
using UnityEngine;

namespace Riten.Windinator.Material
{
    public class ModalDialog : WindinatorBehaviour
    {
        public RectTransform m_parent;

        public void Clear()
        {
            int children = m_parent.childCount;

            for (int i = 0; i < children; i++)
                Destroy(m_parent.GetChild(i).gameObject);
        }

        public void Setup(Builder layout)
        {
            Clear();
            layout.Build(m_parent);
        }

        public void Setup(GameObject prefab)
        {
            Clear();

            if (prefab != null)
                Instantiate(prefab, m_parent, false);
        }

        public T Setup<T>(GameObject prefab) where T : Component
        {
            Setup(prefab);
            return m_parent.GetComponentInChildren<T>();
        }
    }
}