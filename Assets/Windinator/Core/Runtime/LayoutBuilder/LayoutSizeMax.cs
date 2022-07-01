using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Riten.Windinator.LayoutBuilder
{
    [ExecuteAlways]
    public class LayoutSizeMax : MonoBehaviour
    {
        LayoutElement m_element;

        void Awake()
        {
            if (!TryGetComponent<LayoutElement>(out m_element))
                m_element = gameObject.AddComponent<LayoutElement>();
        }

        private void LateUpdate()
        {
            RectTransform rectTransform = transform as RectTransform;

            Vector2 size = Vector2.zero;

            for (int i = 0; i < transform.childCount; ++i)
            {
                var rt = transform.GetChild(i) as RectTransform;

                float x = LayoutUtility.GetPreferredSize(rt, 0);
                float y = LayoutUtility.GetPreferredSize(rt, 1);

                var psize = new Vector2(x, y);

                size = Vector2.Max(size, psize);
            }

            rectTransform.sizeDelta = size;

            m_element.preferredHeight = size.y;
            m_element.preferredWidth = size.x;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }
    }
}
