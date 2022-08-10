using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class GenericLayout : MonoBehaviour
{
    protected RectTransform RectTransform;

    public List<DefineLayout> Children {get; private set;}

    bool m_dirty = false;

    Vector3 m_pos;

    Rect m_rect;

    void OnEnable()
    {
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
            OnDirty(Children.Count);

            foreach(var c in Children) c.ApplyChanges();

            m_dirty = false;
        }
    }

    protected virtual void OnDirty(int childCount) {}
}
