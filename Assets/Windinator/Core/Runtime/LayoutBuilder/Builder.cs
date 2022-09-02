using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.Events;

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
                {
                    _ScrollViewD = Resources.Load<GameObject>("Windinator.Presets/Scroll View Dynamic");
                }
                    
                return _ScrollViewD;
            }
        }
    }

    [Serializable]
    public struct ShadowProperties
    {
        public float Size;

        public float Blur;

        public Color? Color;
    }

    [Serializable]
    public struct OutlineProperties
    {
        public float Size;

        public Color? Color;
    }

    [Serializable]
    public struct GradientProperties
    {
        public Color? TopLeft;

        public Color? TopRight;

        public Color? BottomRight;

        public Color? BottomLeft;
    }

    [System.Serializable]
    public struct ShapeProperties
    {
        public Swatch? Color;

        public Vector4 Roundness;

        public Vector4 MaskOffset;

        public OutlineProperties Outline;

        public ShadowProperties Shadow;

        public GradientProperties Gradient;

        public float? AlphaMultiplier;
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
            return m_child?.Build(parent);
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

    public struct WeightedElement
    {
        public Layout.Element Element;

        public float Weight;

        public WeightedElement(Layout.Element element, float weight)
        {
            Element = element;
            Weight = weight;
        }

        public static implicit operator WeightedElement(Layout.Element d) => new WeightedElement { Element = d, Weight = 1 };
    }

    public struct SizedElement
    {
        public Layout.Element Element;

        public float Size;

        public SizedElement(Layout.Element element, float size)
        {
            Element = element;
            Size = size;
        }
    }


    public static class Layout
    {
        [System.Serializable]
        public abstract class Element
        {
            protected Vector4 m_padding;

            protected float m_flexibleWidth = -1f, m_flexibleHeight = -1f;

            protected float m_preferredWidth = -1f, m_preferredHeight = -1f;

            protected float m_minWidth = -1f, m_minHeight = -1f;

            protected Vector2? m_pivot = null;

            protected LayoutElement m_layout;

            public Element(Vector4 padding = default)
            {
                m_padding = padding;
            }

            public virtual RectTransform Build(RectTransform parent)
            {
                return null;
            }

            public T GetOrAdd<T>(RectTransform me) where T : Component
            {
                if (me == null) return null;

                if (me.TryGetComponent(out T val))
                    return val;
                return me.gameObject.AddComponent<T>();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="horizontal">-1 means undefined</param>
            /// <param name="vertical">-1 means undefined</param>
            /// <returns></returns>
            public Element Flexible(float horizontal = 1f, float vertical = 1f)
            {
                m_flexibleWidth = horizontal;
                m_flexibleHeight = vertical;
                return this;
            }

            public Element Pivot(Vector2 newPivot)
            {
                m_pivot = newPivot;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="width">-1 means undefined</param>
            /// <param name="height">-1 means undefined</param>
            public Element PreferredSize(float width, float height)
            {
                m_preferredHeight = height;
                m_preferredWidth = width;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="minWidth">-1 means undefined</param>
            /// <param name="minHeight">-1 means undefined</param>
            public Element MinSize(float minWidth, float minHeight)
            {
                m_minHeight = minHeight;
                m_minWidth = minWidth;
                return this;
            }

            public Element Small()
            {
                m_preferredHeight = 32f;
                return this;
            }

            public RectTransform CreateMaximized(string name, RectTransform parent)
            {
                var go = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));

                m_layout = go.GetComponent<LayoutElement>();
                m_layout.ignoreLayout = true;

                var transform = go.transform as RectTransform;

                transform.SetParent(parent, false);

                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.anchoredPosition = Vector2.zero;

                transform.offsetMin = Vector2.zero;
                transform.offsetMax = Vector2.zero;

                return transform;
            }

            public VerticalLayoutGroup AddGenericGroup(RectTransform transform, TextAnchor alignment = TextAnchor.UpperLeft)
            {
                var group = GetOrAdd<VerticalLayoutGroup>(transform);

                group.childForceExpandWidth = false;
                group.childForceExpandHeight = false;
                group.childControlWidth = true;
                group.childControlHeight = true;
                group.childAlignment = alignment;
                group.padding = new RectOffset((int)m_padding.x, (int)m_padding.y, (int)m_padding.z, (int)m_padding.w);

                return group;
            }

            public RectTransform Create(string name, RectTransform parent)
            {
                var go = new GameObject(name, typeof(RectTransform));

                var transform = go.transform as RectTransform;

                transform.SetParent(parent, false);

                var center = Vector2.one * 0.5f;

                transform.anchorMin = center;
                transform.anchorMax = center;

                m_layout = GetOrAdd<LayoutElement>(transform);

                return transform;
            }

            public void Setup(RectTransform transform)
            {
                var layout = GetOrAdd<LayoutElement>(transform);

                if (m_flexibleWidth >= 0) layout.flexibleWidth = m_flexibleWidth;
                if (m_flexibleHeight >= 0) layout.flexibleHeight = m_flexibleHeight;

                if (m_preferredWidth >= 0) layout.preferredWidth = m_preferredWidth;
                if (m_preferredHeight >= 0) layout.preferredHeight = m_preferredHeight;

                if (m_minWidth >= 0) layout.minWidth = m_minWidth;
                if (m_minHeight >= 0) layout.minHeight = m_minHeight;

                if (m_pivot.HasValue)
                    transform.pivot = m_pivot.Value;
            }
        }

        [Serializable]
        public class Theme : Element
        {
            ColorAssigner m_palette;

            Element m_child;

            public Theme(ColorAssigner palette, Element child)
            {
                m_palette = palette;
                m_child = child;
            }

            public override RectTransform Build(RectTransform parent)
            {
                if (m_child == null) return null;

                var container = new Container(null).Build(parent);
                var theme = container.gameObject.AddComponent<LayoutTheme>();
                theme.UpdateTheme(m_palette);

                return m_child.Build(container);
            }
        }

        [Serializable]
        public class AddComponent<T> : PrefabRef<T> where T : Component
        {
            readonly Element m_child;

            readonly Action<T> m_config;

            public AddComponent(Element child = null, Action<T> setup = null)
            {
                m_child = child;
                m_config = setup;
            }

            public override RectTransform Build(RectTransform parent)
            {
                if (m_child == null) return null;

                var child = m_child.Build(parent);

                var comp = GetOrAdd<T>(child);
                m_config?.Invoke(comp);

                SetReference(comp);

                return child;
            }
        }

        [Serializable]
        public class Container : Element
        {
            readonly Element m_child;

            readonly Vector2? m_size;

            private readonly TextAnchor m_alignment;

            public Container(Element child, Vector2? size = null, TextAnchor alignment = TextAnchor.MiddleCenter, Vector4 padding = default) : base(padding)
            {
                m_child = child;
                m_size = size;

                if (m_size.HasValue)
                {
                    m_preferredWidth = m_size.Value.x;
                    m_preferredHeight = m_size.Value.y;
                }

                m_alignment = alignment;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("#Layout-Container", parent);

                if (m_size.HasValue) transform.sizeDelta = m_size.Value;

                AddGenericGroup(transform, m_alignment);
                Setup(transform);

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
                try
                {
                    if (m_prefab == null) return null;
                    return GameObject.Instantiate(m_prefab, parent, false).transform as RectTransform;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message + "\n" + ex.StackTrace);
                    return null;
                }
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
                var b = base.Build(parent);
                Setup(b);
                return b;
            }
        }

        [System.Serializable]
        public class PrefabRef<T> : Prefab where T : Component
        {
            Reference<T> m_reference;

            public T ReferenceValue => m_reference.Value;

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
                var b = base.Build(parent);
                Setup(b);
                return b;
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
                var b = base.Build(parent);
                Setup(b);
                return b;
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

                m_flexibleWidth = 1;
                m_flexibleHeight = 0;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("#Layout-Horizontal", parent);
                var layoutGroup = GetOrAdd<HorizontalLayoutGroup>(transform);

                Setup(transform);

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
            public FlexibleSpace(float weight = 1f) : base(default)
            {
                m_flexibleHeight = weight;
                m_flexibleWidth = weight;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("#Layout-Flexible-Space", parent);
                Setup(transform);
                return transform;
            }
        }

        public class ScrollView : PrefabRef<ScrollRect>
        {
            Element m_child;

            bool m_vertical;

            public ScrollView(Element child, bool vertical = true) : base(LayoutPrefabs.ScrollView)
            {
                m_child = child;
                m_vertical = vertical;
                m_flexibleHeight = 1f;
                m_flexibleWidth = 1f;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = base.Build(parent);

                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.anchoredPosition = Vector2.zero;
                transform.sizeDelta = Vector2.zero;

                var scrollRect = transform.GetComponent<ScrollRect>();
                var contentSize = scrollRect.content.GetComponent<ContentSizeFitter>();

                if (m_vertical)
                {
                    var content = scrollRect.content;

                    contentSize.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    contentSize.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                    content.anchorMin = Vector2.up;
                    content.anchorMax = Vector2.one;
                    content.anchoredPosition = Vector2.zero;
                    content.sizeDelta = Vector2.zero;
                }
                else
                {
                    var content = scrollRect.content;

                    contentSize.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    contentSize.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

                    content.anchorMin = Vector2.zero;
                    content.anchorMax = Vector2.up;
                    content.anchoredPosition = Vector2.zero;
                    content.sizeDelta = Vector2.zero;
                }
                
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
                    var vertical = new Vertical(new Element[] { child }, alignment: m_aligmnet, padding: m_padding).Build(transform);

                    vertical.anchorMin = Vector2.zero;
                    vertical.anchorMax = Vector2.one;
                    vertical.anchoredPosition = Vector2.zero;
                    vertical.sizeDelta = Vector2.zero;

                    /*var fitter = vertical.gameObject.AddComponent<ContentSizeFitter>();
                    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;*/
                }

                GetOrAdd<LayoutSizeMax>(transform);

                return transform;
            }
        }

        public class WeightedHorizontal : Element
        {
            readonly WeightedElement[] m_children;

            readonly float m_spacing;

            public WeightedHorizontal(WeightedElement[] children, float height = 32f, float spacing = 0f)
            {
                m_children = children;
                m_flexibleWidth = 1f;
                m_flexibleHeight = 0f;
                m_spacing = spacing;
                m_preferredHeight = height;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("AnchoredHorizontal", parent);

                Setup(transform);

                if (m_children != null)
                {
                    int len = m_children.Length;

                    float totalWeight = 0f;

                    foreach (var c in m_children)
                        if (c.Element != null) totalWeight += c.Weight;

                    float currentWeight = 0f;

                    for (int i = 0; i < len; ++i)
                    {
                        if (m_children[i].Element == null)
                            continue;

                        float w = m_children[i].Weight;

                        var child = m_children[i].Element.Build(transform);
                        var currPos = currentWeight / totalWeight; currentWeight += w;
                        var nextPos = currentWeight / totalWeight;

                        child.anchorMin = new Vector2(currPos, 0f);
                        child.anchorMax = new Vector2(nextPos, 1f);

                        child.anchoredPosition = Vector2.zero;
                        child.sizeDelta = Vector2.zero;

                        if (i < len - 1) child.offsetMax = new Vector2(-m_spacing, 0);
                        if (i > 0      ) child.offsetMin = new Vector2(m_spacing, 0);
                    }
                }

                return transform;
            }
        }

        public class SizeVertical : Element
        {
            readonly SizedElement[] m_children;

            readonly float m_spacing;

            readonly float m_totalHeight;

            public SizeVertical(SizedElement[] children, float spacing = 0f, Vector4 padding = default) : base(padding)
            {
                foreach (var c in children)
                {
                    m_totalHeight += c.Size + spacing;
                }

                m_totalHeight += padding.z + padding.w;
                m_totalHeight -= spacing;

                m_children = children;
                m_flexibleWidth = 1f;
                m_flexibleHeight = 0f;
                m_preferredHeight = m_totalHeight;
                m_spacing = spacing;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("SizedHorizontal", parent);

                Setup(transform);

                if (m_children != null)
                {
                    int len = m_children.Length;

                    float currPos = -m_padding.z;

                    for (int i = 0; i < len; ++i)
                    {
                        if (m_children[i].Element == null)
                        {
                            currPos -= m_children[i].Size + m_spacing;
                            continue;
                        }

                        float height = m_children[i].Size;

                        var child = m_children[i].Element.Build(transform);

                        child.pivot = new Vector2(0f, 1f);

                        child.anchorMin = new Vector2(0f, 1f);
                        child.anchorMax = new Vector2(1f, 1f);

                        child.anchoredPosition = new Vector2(m_padding.x, currPos);
                        child.sizeDelta = new Vector2(-m_padding.y, height);

                        currPos -= height + m_spacing;
                    }
                }

                return transform;
            }
        }

        public class WeightedVertical : Element
        {
            readonly WeightedElement[] m_children;

            readonly float m_spacing;

            public WeightedVertical(WeightedElement[] children, float width = 32f, float spacing = 0f)
            {
                m_children = children;
                m_flexibleWidth = 1f;
                m_flexibleHeight = 0f;
                m_spacing = spacing;
                m_preferredWidth = width;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("AnchoredVertical", parent);

                Setup(transform);

                if (m_children != null)
                {
                    int len = m_children.Length;

                    float totalWeight = 0f;

                    foreach (var c in m_children)
                        if (c.Element != null) totalWeight += c.Weight;

                    float currentWeight = 0f;

                    for (int i = 0; i < len; ++i)
                    {
                        if (m_children[i].Element == null)
                            continue;

                        float w = m_children[i].Weight;

                        var child = m_children[i].Element.Build(transform);
                        var currPos = currentWeight / totalWeight; currentWeight += w;
                        var nextPos = currentWeight / totalWeight;

                        child.anchorMin = new Vector2(0f, currPos);
                        child.anchorMax = new Vector2(1f, nextPos);

                        child.anchoredPosition = Vector2.zero;
                        child.sizeDelta = Vector2.zero;

                        if (i < len - 1) child.offsetMax = new Vector2(0, -m_spacing);
                        if (i > 0) child.offsetMin = new Vector2(0, m_spacing);
                    }
                }

                return transform;
            }
        }

        public class Vertical : Element
        {
            Element[] m_children;

            TextAnchor m_aligmnet;

            float m_spacing;

            public Vertical(Element[] children = null, float spacing = 0f, TextAnchor alignment = TextAnchor.UpperLeft, Vector4 padding = default) : base(padding)
            {
                m_spacing = spacing;
                m_aligmnet = alignment;
                m_children = children;

                m_flexibleWidth = 0;
                m_flexibleHeight = 1;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var transform = Create("#Layout-Vertical", parent);
                var layoutGroup = GetOrAdd<VerticalLayoutGroup>(transform);

                Setup(transform);
                
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

            private readonly Texture m_texture;

            private readonly TextAnchor m_alignment;

            public Rectangle(Element child = null, Vector2? size = null,
                Vector4 padding = default,
                ShapeProperties shape = default, Texture texture = null,
                TextAnchor alignment = TextAnchor.UpperLeft) : base()
            {
                m_child = child;
                m_size = size;
                m_padding = padding;
                m_shape = shape;
                m_texture = texture;
                m_alignment = alignment;

                if (m_size.HasValue)
                {
                    m_preferredWidth = size.Value.x;
                    m_preferredHeight = size.Value.y;
                }
            }

            public override RectTransform Build(RectTransform parent)
            {
                RectTransform transform = Create("#Layout-Rectangle-Graphic", parent);

                if (m_size.HasValue) transform.sizeDelta = m_size.Value;

                var cr = GetOrAdd<CanvasRenderer>(transform);
                cr.cullTransparentMesh = false;

                var graphic = GetOrAdd<RectangleGraphic>(transform);

                AddGenericGroup(transform, m_alignment);


                Setup(transform);

                graphic.LeftDownColor = m_shape.Gradient.BottomLeft.GetValueOrDefault(Color.white);
                graphic.LeftUpColor = m_shape.Gradient.TopLeft.GetValueOrDefault(Color.white);
                graphic.RightDownColor = m_shape.Gradient.BottomRight.GetValueOrDefault(Color.white);
                graphic.RightUpColor = m_shape.Gradient.TopRight.GetValueOrDefault(Color.white);
                graphic.MaskOffset = m_shape.MaskOffset;
                graphic.Alpha = m_shape.AlphaMultiplier.GetValueOrDefault(1f);

                graphic.SetRoundness(m_shape.Roundness);
                graphic.SetOutline(m_shape.Outline.Color.GetValueOrDefault(Color.black), m_shape.Outline.Size);
                graphic.SetShadow(
                    m_shape.Shadow.Color.GetValueOrDefault(Color.black),
                    m_shape.Shadow.Size,
                    m_shape.Shadow.Blur
                );

                graphic.color = m_shape.Color.GetValueOrDefault(Colors.Surface).GetUnityColor(transform);
                graphic.Texture = m_texture;

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
                var layoutGroup = GetOrAdd<GridLayoutGroup>(transform);
                var layoutElement = GetOrAdd<LayoutElement>(transform);

                layoutElement.flexibleHeight = m_flexibleHeight;
                layoutElement.flexibleWidth = m_flexibleWidth;

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

        public class Spacer : Element
        {
            float m_space = 0f;

            public Spacer(float space) : base(default)
            {
                m_space = space;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var element = Create("#Layout-Space", parent);
                var layout = GetOrAdd<LayoutElement>(element);

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

            readonly Swatch m_color;

            readonly static Dictionary<Texture2D, Sprite> m_cache = new Dictionary<Texture2D, Sprite>();

            readonly Image.Type m_type;

            public Graphic(Sprite sprite = null, Texture2D texture = null, Swatch? color = null, float pixelsPerUnitScaler = 1f, bool preserveAspect = true,
                            Image.Type imageType = Image.Type.Simple) : base(default)
            {
                m_type = imageType;
                m_preserveAspect = preserveAspect;
                m_pixelScaler = pixelsPerUnitScaler;
                m_sprite = sprite;
                m_color = color.GetValueOrDefault(Swatch.FromColor(Color.white));

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

                var img = GetOrAdd<Image>(transform);
                var colorAssigner = GetOrAdd<ColorPalette>(transform);

                img.pixelsPerUnitMultiplier = m_pixelScaler;
                img.sprite = m_sprite;
                img.preserveAspect = m_preserveAspect;
                img.color = m_color.GetUnityColor(transform);
                img.type = m_type;

                colorAssigner.Color = m_color;

                SetReference(img);

                return transform;
            }
        }
    }
}