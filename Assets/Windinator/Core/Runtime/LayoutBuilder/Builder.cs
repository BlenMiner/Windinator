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

    public struct ShadowProperties
    {
        public float Size;

        public float Blur;

        public Color? Color;
    }

    public struct OutlineProperties
    {
        public float Size;

        public Color? Color;
    }

    public struct ShapeProperties
    {
        public Color? Color;

        public Vector4 Roundness;

        public OutlineProperties Outline;

        public ShadowProperties Shadow;
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
            return m_child.Build(parent);
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
                m_padding = Padding;
            }

            public virtual RectTransform Build(RectTransform parent)
            {
                return null;
            }

            public static RectTransform CreateMaximized(string name, RectTransform parent)
            {
                var go = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
                var layout = go.GetComponent<LayoutElement>();

                layout.ignoreLayout = true;

                var transform = go.transform as RectTransform;

                transform.SetParent(parent, false);

                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.anchoredPosition = Vector2.zero;

                transform.offsetMin = Vector2.zero;
                transform.offsetMax = Vector2.zero;

                return transform;
            }

            public VerticalLayoutGroup AddGenericGroup(Transform transform)
            {
                var group = transform.gameObject.AddComponent<VerticalLayoutGroup>();

                group.childForceExpandWidth = false;
                group.childForceExpandHeight = false;
                group.childControlWidth = true;
                group.childControlHeight = true;
                group.padding = new RectOffset((int)m_padding.x, (int)m_padding.y, (int)m_padding.z, (int)m_padding.w);

                return group;
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
            readonly Element m_child;

            readonly Vector2 m_size;

            readonly float m_flexibleHeight, m_flexibleWidth;

            public Container(Element child, Vector2 size, float flexibleHeight = 0, float flexibleWidth = 0) : base()
            {
                m_child = child;
                m_size = size;
                m_flexibleHeight = flexibleHeight;
                m_flexibleWidth = flexibleWidth;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("#Layout-Rectangle", parent);
                transform.sizeDelta = m_size;

                var layout = transform.gameObject.AddComponent<LayoutElement>();

                AddGenericGroup(transform);

                layout.preferredWidth = m_size.x;
                layout.preferredHeight = m_size.y;

                layout.flexibleHeight = m_flexibleHeight;
                layout.flexibleWidth = m_flexibleWidth;

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
                if (m_prefab == null) return null;
                return Object.Instantiate(m_prefab, parent, false).transform as RectTransform;
            }
        }

        [System.Serializable]
        public class ElementRef<T> : Element where T : Component
        {
            Reference<T> m_reference;

            public ElementRef(Vector4 padding = default) : base(padding)
            {
                m_reference = new Reference<T>();
            }

            public ElementRef<T> GetReference(out Reference<T> reference)
            {
                reference = m_reference;
                return this;
            }

            protected void SetReference(T value)
            {
                m_reference.Value = value;
            }

            public override RectTransform Build(RectTransform parent)
            {
                return base.Build(parent);
            }
        }

        [System.Serializable]
        public class PrefabRef<T> : Prefab where T : Component
        {
            Reference<T> m_reference;

            public PrefabRef(GameObject prefab = null) : base(prefab)
            {
                m_reference = new Reference<T>();
            }

            public PrefabRef<T> GetReference(out Reference<T> reference)
            {
                reference = m_reference;
                return this;
            }

            protected void SetReference(T value)
            {
                m_reference.Value = value;
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
                var transform = Create("#Layout-Horizontal", parent);
                var layoutGroup = transform.gameObject.AddComponent<HorizontalLayoutGroup>();
                var layoutElement = transform.gameObject.AddComponent<LayoutElement>();

                layoutElement.flexibleHeight = 1f;
                layoutElement.flexibleWidth = 1f;

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
        public class Expand : Element
        {
            Element m_child;

            public Expand(Element child) : base(default)
            {
                m_child = child;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = CreateMaximized("#Layout-Flexible-Space", parent);
                m_child?.Build(transform);
                AddGenericGroup(transform);
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

                var scrollRect = transform.GetComponent<ScrollRect>();

                SetReference(scrollRect);

                var nextParent = scrollRect.content;

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

                if (transform != null)
                {
                    transform.anchorMin = Vector2.zero;
                    transform.anchorMax = Vector2.one;
                    transform.anchoredPosition = Vector2.zero;
                    transform.sizeDelta = Vector2.zero;

                    SetReference(transform.GetComponent<ScrollViewDynamicRuntime>());
                }

                return transform;
            }
        }

        public class Stack : Element
        {
            Element[] m_children;

            TextAnchor m_aligmnet;

            public Stack(Element[] children = null, TextAnchor alignment = TextAnchor.UpperLeft, Vector4 Padding = default) : base(Padding)
            {
                m_aligmnet = alignment;
                m_children = children;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("#Layout-Stack", parent);

                foreach (var child in m_children)
                {
                    var vertical = new Vertical(new Element[] { child }, alignment: m_aligmnet, Padding: m_padding).Build(transform);

                    vertical.anchorMin = Vector2.zero;
                    vertical.anchorMax = Vector2.one;
                    vertical.anchoredPosition = Vector2.zero;
                    vertical.sizeDelta = Vector2.zero;

                    var fitter = vertical.gameObject.AddComponent<ContentSizeFitter>();
                    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                }

                transform.gameObject.AddComponent<LayoutSizeMax>();

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
                var transform = Create("#Layout-Vertical", parent);
                var layoutGroup = transform.gameObject.AddComponent<VerticalLayoutGroup>();
                var layoutElement = transform.gameObject.AddComponent<LayoutElement>();

                layoutElement.flexibleHeight = 1f;
                layoutElement.flexibleWidth = 1f;

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

        public class Rectangle : PrefabRef<RectangleGraphic>
        {
            private readonly Element m_child;

            private readonly Vector2? m_size;

            private readonly ShapeProperties m_shape;

            private float m_flexibleWidth = 0f;

            private float m_flexibleHeight = 0f;

            public Rectangle(Element child = null, Vector2? size = null,
                Vector4 padding = default,
                ShapeProperties shape = default,
                float flexibleWidth = 0, float flexibleHeight = 0) : base()
            {
                m_child = child;
                m_size = size;
                m_padding = padding;
                m_shape = shape;
                m_flexibleWidth = flexibleWidth;
                m_flexibleHeight = flexibleHeight;
            }

            public override RectTransform Build(RectTransform parent)
            {
                RectTransform transform = Create("#Layout-Rectangle-Graphic", parent);

                if (m_size.HasValue) transform.sizeDelta = m_size.Value;

                var cr = transform.gameObject.AddComponent<CanvasRenderer>();
                cr.cullTransparentMesh = false;

                var layout = transform.gameObject.AddComponent<LayoutElement>();
                var graphic = transform.gameObject.AddComponent<RectangleGraphic>();

                AddGenericGroup(transform);

                if (m_size.HasValue)
                {
                    var size = m_size.Value;

                    layout.minWidth = size.x;
                    layout.minHeight = size.y;
                }

                layout.flexibleHeight = m_flexibleHeight;
                layout.flexibleWidth = m_flexibleWidth;

                graphic.SetRoundness(m_shape.Roundness);
                graphic.SetOutline(m_shape.Outline.Color.GetValueOrDefault(Color.black), m_shape.Outline.Size);
                graphic.SetShadow(
                    m_shape.Shadow.Color.GetValueOrDefault(Color.black),
                    m_shape.Shadow.Size,
                    m_shape.Shadow.Blur
                );
                graphic.color = m_shape.Color.GetValueOrDefault(Color.white);

                SetReference(graphic);

                m_child?.Build(transform);

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
                var transform = Create("#Layout-Grid", parent);
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

        public class Graphic : ElementRef<Image>
        {
            readonly Sprite m_sprite;

            readonly float m_pixelScaler;

            readonly bool m_preserveAspect;

            readonly static Dictionary<Texture2D, Sprite> m_cache = new Dictionary<Texture2D, Sprite>();

            public Graphic(Sprite sprite = null, Texture2D texture = null, float pixelsPerUnitScaler = 1f, bool preserveAspect = true) : base(default)
            {
                m_preserveAspect = preserveAspect;
                m_pixelScaler = pixelsPerUnitScaler;
                m_sprite = sprite;

                if (texture != null && !m_cache.TryGetValue(texture, out m_sprite))
                {
                    m_sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        Vector2.one * 0.5f,
                        100f
                    );

                    m_cache.Add(texture, m_sprite);
                }
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("#Layout-Image", parent);

                var img = transform.gameObject.AddComponent<Image>();

                img.pixelsPerUnitMultiplier = m_pixelScaler;
                img.sprite = m_sprite;
                img.preserveAspect = m_preserveAspect;

                SetReference(img);

                return transform;
            }
        }
    }
}