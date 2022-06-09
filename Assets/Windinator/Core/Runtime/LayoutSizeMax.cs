using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Riten.Windinator.LayoutBuilder
{
    [ExecuteAlways]
    public class LayoutSizeMax : MonoBehaviour
    {
        List<ContentSizeFitter> m_children = new List<ContentSizeFitter>();

        LayoutElement m_element;

        void Awake()
        {
            if (!TryGetComponent<LayoutElement>(out m_element))
                m_element = gameObject.AddComponent<LayoutElement>();
        }

        private void LateUpdate()
        {
            RectTransform rectTransform = transform as RectTransform;

            m_children.Clear();

            GetComponentsInChildren<ContentSizeFitter>(false, m_children);

            Vector2 size = Vector2.zero;

            foreach (var c in m_children)
            {
                var rt = c.transform as RectTransform;

                size = Vector2.Max(size, rt.sizeDelta);
            }

            rectTransform.sizeDelta = size;

            m_element.preferredHeight = size.y;
            m_element.preferredWidth = size.x;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }
    }
}
