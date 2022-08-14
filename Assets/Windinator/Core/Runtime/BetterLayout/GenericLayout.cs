using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class GenericLayout : MonoBehaviour
{
    protected DefineLayout Layout;

    [SerializeField] Vector4 m_padding;

    [SerializeField] TextAnchor m_alignment;

    [SerializeField] bool m_fitContent;

    public bool FitContent
    {
        get => m_fitContent;
        set
        {
            m_fitContent = value;
            SetDirty();
        }
    }

    public Vector4 Padding
    {
        get => m_padding;
        set
        {
            m_padding = value;
            SetDirty();
        }
    }

    public TextAnchor Alignment
    {
        get => m_alignment;
        set
        {
            m_alignment = value;
            SetDirty();
        }
    }

    protected RectTransform RectTransform;

    public List<DefineLayout> Children {get; private set;}

    bool m_dirty = false;

    Vector3 m_pos;

    Rect m_rect;

    void OnEnable()
    {
        if (!gameObject.TryGetComponent(out Layout))
            Layout = gameObject.AddComponent<DefineLayout>();

        RectTransform = transform as RectTransform;
    }

    public void UpdateChildren(List<DefineLayout> children)
    {
        Children = children;
    }

    public void SetDirty()
    {
        m_dirty = true;
    }

    void OnValidate()
    {
        SetDirty();
    }

    void Update()
    {
        if (m_pos != RectTransform.position)
        {
            m_pos = RectTransform.position;
            m_rect = RectTransform.rect;
            SetDirty();
        }
        else if (m_rect != RectTransform.rect)
        {
            m_rect = RectTransform.rect;
            SetDirty();
        }
    }

    void LateUpdate()
    {
        if (m_dirty)
        {
            if (Children != null)
            {
                OnDirty(Children.Count);
                foreach(var c in Children) c.ApplyChanges();

                if (m_fitContent)
                {
                    RectTransform.sizeDelta = Layout.PrefferedSize;
                }
            }

            m_dirty = false;
        }
    }

    protected virtual void OnDirty(int childCount) {}

    protected Vector2 FitMinimum()
    {
        Vector2 totalSize = default;

        foreach (var layout in Children)
        {
            var calculatedSize = layout.MinSize;
            layout.CachedSize = calculatedSize;
            totalSize += calculatedSize;
        }
        return totalSize;
    }

    protected Vector2 FitPreffered(Vector2 totalPreffered, Vector2 remainingSpace)
    {
        Vector2 totalSize = default;

        foreach (var layout in Children)
        {
            var prefferedSize = Vector2.Max(default, layout.PrefferedSize - layout.MinSize);

            var percentageSize = prefferedSize / totalPreffered;
            var calculatedSize = percentageSize * remainingSpace;

            var size = layout.CachedSize;

            size.x += Mathf.Max(0, calculatedSize.x);
            size.y += Mathf.Max(0, calculatedSize.y);

            size = Vector2.Min(size, Vector2.Max(layout.MinSize, layout.PrefferedSize));

            layout.CachedSize = size;
            totalSize += size;
        }

        return totalSize;
    }

    protected Vector2 FitFlexible(Vector2 totalFlexible, Vector2 remainingSpace)
    {
        Vector2 totalSize = default;

        foreach (var layout in Children)
        {
            var flexible = layout.Flexible;

            var percentageSize = flexible / totalFlexible;
            var calculatedSize = percentageSize * remainingSpace;

            var size = layout.CachedSize;

            size.x += totalFlexible.x == 0 ? 0 : Mathf.Max(0, calculatedSize.x);
            size.y += totalFlexible.y == 0 ? 0 : Mathf.Max(0, calculatedSize.y);

            layout.CachedSize = size;
            totalSize += size;
        }

        return totalSize;
    }
}
