using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Riten.Windinator.LayoutBuilder
{
    public static class LayoutPrefabs
    {
        private static GameObject _ScrollView;

        public static GameObject ScrollView
        {
            get
            {
                if (_ScrollView == null)
                    _ScrollView = Resources.Load<GameObject>("Windinator.Presets/Scroll View");
                return _ScrollView;
            }
        }

        private static GameObject _ScrollViewD;

        public static GameObject ScrollViewDynamic
        {
            get
            {
                if (_ScrollViewD == null)
                    _ScrollViewD = Resources.Load<GameObject>("Windinator.Presets/Scroll View Dynamic");
                return _ScrollViewD;
            }
        }
    }

    public class Builder : Layout.Element
    {
        RectTransform m_root;

        Layout.Element m_child;

        public Builder(RectTransform root, Layout.Element child = null) : base(default)
        {
            m_root = root;
            m_child = child;
        }

        public RectTransform Build()
        {
            return Build(m_root);
        }

        public override RectTransform Build(RectTransform parent)
        {
            return new Layout.Horizontal(children: new Layout.Element[1] { m_child }).Build(parent);
        }
    }


    [System.Serializable]
    public class Reference<T> where T : Component
    {
        public T Value;

        public Reference(T Value)
        {
            this.Value = Value;
        }

        public Reference() { }
    }


    public static class Layout
    {
        [System.Serializable]
        public class Element
        {
            protected Vector4 m_padding;

            public Element(Vector4 Padding = default)
            {
                this.m_padding = Padding;
            }

            public virtual RectTransform Build(RectTransform parent)
            {
                return null;
            }

            public static RectTransform CreateMaximized(string name, RectTransform parent)
            {
                var go = new GameObject(name, typeof(RectTransform));

                var transform = go.transform as RectTransform;

                transform.SetParent(parent, false);

                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.anchoredPosition = Vector2.zero;

                transform.offsetMin = Vector2.zero;
                transform.offsetMax = Vector2.zero;

                return transform;
            }

            public static RectTransform Create(string name, RectTransform parent)
            {
                var go = new GameObject(name, typeof(RectTransform));

                var transform = go.transform as RectTransform;

                transform.SetParent(parent, false);

                var center = Vector2.one * 0.5f;

                transform.anchorMin = center;
                transform.anchorMax = center;

                return transform;
            }
        }

        [System.Serializable]
        public class Container : Element
        {
            Element m_child;

            Vector2 m_size;

            public Container(Element child, Vector2 size) : base()
            {
                m_child = child;
                m_size = size;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("#Layout-Rectangle", parent);
                transform.sizeDelta = m_size;

                var layout = transform.gameObject.AddComponent<LayoutElement>();

                layout.preferredWidth = m_size.x;
                layout.preferredHeight = m_size.y;

                m_child?.Build(transform);

                return transform;
            }
        }

        [System.Serializable]
        public class Prefab : Element
        {
            GameObject m_prefab;

            public Prefab(GameObject prefab = null) : base(default)
            {
                m_prefab = prefab;
            }

            public override RectTransform Build(RectTransform parent)
            {
                return Object.Instantiate(m_prefab, parent, false).transform as RectTransform;
            }
        }

        [System.Serializable]
        public class PrefabRef<T> : Prefab where T : Component
        {
            protected Reference<T> m_reference;

            public PrefabRef(GameObject prefab = null) : base(prefab)
            {
                m_reference = new Reference<T>();
            }

            public PrefabRef<T> GetReference(out Reference<T> reference)
            {
                reference = m_reference;
                return this;
            }

            public override RectTransform Build(RectTransform parent)
            {
                return base.Build(parent);
            }
        }

        [System.Serializable]
        public class PrefabRefs<T> : Prefab where T : Component
        {
            protected Reference<T>[] m_references;

            public PrefabRefs(int refCount, GameObject prefab = null) : base(prefab)
            {
                m_references = new Reference<T>[refCount];
            }

            public PrefabRefs<T> GetReference(out Reference<T>[] reference)
            {
                reference = m_references;
                return this;
            }

            public override RectTransform Build(RectTransform parent)
            {
                return base.Build(parent);
            }
        }

        [System.Serializable]
        public class Horizontal : Element
        {
            Element[] m_children;

            TextAnchor m_aligmnet;

            float m_spacing;

            public Horizontal(Element[] children = null, float spacing = 0f, TextAnchor alignment = TextAnchor.UpperLeft, Vector4 Padding = default) : base(Padding)
            {
                m_spacing = spacing;
                m_aligmnet = alignment;
                m_children = children;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = CreateMaximized("#Layout-Horizontal", parent);
                var layoutGroup = transform.gameObject.AddComponent<HorizontalLayoutGroup>();
                var layoutElement = transform.gameObject.AddComponent<LayoutElement>();

                layoutElement.flexibleHeight = 0f;

                if (m_children != null && m_children.Length > 0)
                {
                    foreach (var child in m_children)
                        child?.Build(transform);
                }

                layoutGroup.padding = new RectOffset((int)m_padding.x, (int)m_padding.y, (int)m_padding.z, (int)m_padding.w);
                layoutGroup.childForceExpandHeight = false;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childControlWidth = true;
                layoutGroup.childControlHeight = true;
                layoutGroup.childAlignment = m_aligmnet;
                layoutGroup.spacing = m_spacing;

                LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
                return transform;
            }
        }

        [System.Serializable]
        public class FlexibleSpace : Element
        {
            float m_weight;

            public FlexibleSpace(float weight = 1f) : base(default)
            {
                m_weight = weight;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("#Layout-Flexible-Space", parent);
                var layoutElement = transform.gameObject.AddComponent<LayoutElement>();
                layoutElement.flexibleWidth = m_weight;
                layoutElement.flexibleHeight = m_weight;
                return transform;
            }
        }

        public class ScrollView : PrefabRef<ScrollRect>
        {
            Element m_child;

            public ScrollView(Element child) : base(LayoutPrefabs.ScrollView)
            {
                m_child = child;
            }
             
            public override RectTransform Build(RectTransform parent)
            {
                var transform = base.Build(parent);

                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.anchoredPosition = Vector2.zero;
                transform.sizeDelta = Vector2.zero;

                m_reference.Value = transform.GetComponent<ScrollRect>();

                var nextParent = m_reference.Value.content;

                m_child?.Build(nextParent);

                return transform;
            }
        }

        public class ScrollViewDynamic : PrefabRef<ScrollViewDynamicRuntime>
        {
            public ScrollViewDynamic() : base(LayoutPrefabs.ScrollViewDynamic) { }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = base.Build(parent);

                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.anchoredPosition = Vector2.zero;
                transform.sizeDelta = Vector2.zero;

                m_reference.Value = transform.GetComponent<ScrollViewDynamicRuntime>();

                return transform;
            }
        }

        public class Vertical : Element
        {
            Element[] m_children;

            TextAnchor m_aligmnet;

            float m_spacing;

            public Vertical(Element[] children = null, float spacing = 0f, TextAnchor alignment = TextAnchor.UpperLeft, Vector4 Padding = default) : base(Padding)
            {
                m_spacing = spacing;
                m_aligmnet = alignment;
                m_children = children;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = CreateMaximized("#Layout-Vertical", parent);
                var layoutGroup = transform.gameObject.AddComponent<VerticalLayoutGroup>();
                var layoutElement = transform.gameObject.AddComponent<LayoutElement>();

                layoutElement.flexibleHeight = 0f;

                layoutGroup.padding = new RectOffset((int)m_padding.x, (int)m_padding.y, (int)m_padding.z, (int)m_padding.w);

                if (m_children != null && m_children.Length > 0)
                {
                    foreach (var child in m_children)
                        child?.Build(transform);
                }

                layoutGroup.childForceExpandHeight = false;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childControlWidth = true;
                layoutGroup.childControlHeight = true;
                layoutGroup.childAlignment = m_aligmnet;
                layoutGroup.spacing = m_spacing;

                LayoutRebuilder.ForceRebuildLayoutImmediate(transform);

                return transform;
            }
        }

        public class Grid : Element
        {
            Element[] m_children;

            TextAnchor m_aligmnet;

            Vector2 m_cellSize;

            Vector2 m_cellSpacing;

            public Grid(Vector2 cellSize, Vector2 cellSpacing = default, Element[] children = null, TextAnchor alignment = TextAnchor.UpperLeft, Vector4 Padding = default) : base(Padding)
            {
                m_cellSize = cellSize;
                m_cellSpacing = cellSpacing;
                m_aligmnet = alignment;
                m_children = children;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = CreateMaximized("#Layout-Grid", parent);
                var layoutGroup = transform.gameObject.AddComponent<GridLayoutGroup>();
                var layoutElement = transform.gameObject.AddComponent<LayoutElement>();

                layoutElement.flexibleHeight = 0f;
                layoutElement.flexibleWidth = 0f;

                if (m_children != null && m_children.Length > 0)
                {
                    foreach (var child in m_children)
                        child?.Build(transform);
                }

                layoutGroup.padding = new RectOffset((int)m_padding.x, (int)m_padding.y, (int)m_padding.z, (int)m_padding.w);
                layoutGroup.cellSize = m_cellSize;
                layoutGroup.spacing = m_cellSpacing;
                layoutGroup.childAlignment = m_aligmnet;

                LayoutRebuilder.ForceRebuildLayoutImmediate(transform);

                return transform;
            }
        }

        public class Space : Element
        {
            float m_space = 0f;

            public Space(float space) : base(default)
            {
                m_space = space;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var element = Create("#Layout-Space", parent);
                var layout = element.gameObject.AddComponent<LayoutElement>();

                layout.preferredWidth = m_space;
                layout.preferredHeight = m_space;

                return element;
            }
        }
    }
}