using UnityEngine;
using UnityEngine.UI;

namespace Riten.Windinator.LayoutBuilder
{
    public abstract class LayoutBaker : MonoBehaviour
    {
        [SerializeField] bool m_freeControl = false;

        [HideInInspector] public string ScriptHash;

        public void ClearContents()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject, true);
        }

        private void UpdateFullscreen()
        {
            RectTransform me = transform as RectTransform;
            var contentSize = GetComponent<ContentSizeFitter>();

            if (contentSize == null) return;

            if (m_freeControl)
            {
                contentSize.enabled = false;
            }
            else
            {
                contentSize.enabled = true;

                me.anchorMin = Vector3.one * 0.5f;
                me.anchorMax = Vector3.one * 0.5f;
            }
        }

        void OnValidate()
        {
            UpdateFullscreen();
        }
        
        public RectTransform Build()
        {
            UpdateFullscreen();

            return new Builder(transform as RectTransform, child: new Layout.Theme(null, Bake())).Build();
        }

        public abstract Layout.Element Bake();
    }
}