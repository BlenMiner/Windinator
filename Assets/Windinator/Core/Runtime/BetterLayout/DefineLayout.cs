using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DefineLayout : MonoBehaviour
{
    [SerializeField] Vector2 m_minSize = new Vector2(0, 0);

    [SerializeField] Vector2 m_prefferedSize = new Vector2(-1, -1);

    [SerializeField] Vector2 m_flexible = new Vector2(0, 0);

    [System.NonSerialized] public RectTransform RectTransform;

    [System.NonSerialized] public Vector2 CachedSize;

    static Dictionary<RectTransform, List<DefineLayout>> Layouts = 
       new Dictionary<RectTransform, List<DefineLayout>>();

    GenericLayout Layout;

    RectTransform Parent;

    Rect LastRect;

    Vector3 LastPosition;

    int ChildIndex;

    public Vector2 PrefferedSize
    {
        get 
        {
            return Vector2.Max(default, m_prefferedSize);
        }
        set
        {
            m_prefferedSize = value;
            NotifyParent(transform.parent);
        }
    }

    public Vector2 Flexible
    {
        get 
        {
            return m_flexible;
        }
        set
        {
            m_flexible = Vector2.Max(default, value);
            NotifyParent(transform.parent);
        }
    }

    public Vector2 MinSize
    {
        get 
        {
            return m_minSize;
        }
        set
        {
            m_minSize = Vector2.Max(default, value);
            NotifyParent(transform.parent);
        }
    }

    public static List<DefineLayout> GetLayouts(RectTransform parent)
    {
        if (!Layouts.TryGetValue(parent, out var list))
            return null;
        return list;
    }

    void NotifyParent(Transform parent, List<DefineLayout> newParents)
    {
        Layout = parent.GetComponent<GenericLayout>();

        if (Layout != null)
        {
            Layout.UpdateChildren(newParents);
            Layout.SetDirty();
        }
    }

    void NotifyParent(Transform parent)
    {
        parent.GetComponent<GenericLayout>()?.SetDirty();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        NotifyParent(transform.parent);
    }
#endif

    void OnEnable()
    {
        RectTransform = transform as RectTransform;
        Parent = transform.parent as RectTransform;

        if (Parent == null) return;

        if (!Layouts.TryGetValue(Parent, out var list))
        {
            list = new List<DefineLayout>();
            Layouts.Add(Parent, list);
        }

        if (list.Count != transform.GetSiblingIndex())
        {
            ChildOrderChanged(Parent);
        }
        else
        {
            list.Add(this);
            NotifyParent(Parent, list);
        }
    }
 
    void OnDisable()
    {
        if (Parent == null) return;

        var list = Layouts[Parent];

        list.Remove(this);
        NotifyParent(Parent, list);
    }

    void OnTransformParentChanged()
    {
        if (transform.parent != Parent)
        {
            if (Parent != null)  ChildOrderChanged(Parent);

            Parent = transform.parent as RectTransform;

            if (Parent != null) ChildOrderChanged(Parent);
        }
    }

    void ChildOrderChanged(RectTransform parent)
    {
        if (parent == null) return;

        var list = Layouts[parent];
        int cCount = parent.childCount;

        list.Clear();

        for(int i = 0; i < cCount; ++i)
        {
            var layout = parent.GetChild(i).GetComponent<DefineLayout>();
            if (layout != null && layout.isActiveAndEnabled)
            {
                list.Add(layout);
                layout.ChildIndex = layout.transform.GetSiblingIndex();
            }
        }

        NotifyParent(parent, list);
    }

    void Update()
    {
        var sibling = RectTransform.GetSiblingIndex();

        if (ChildIndex != sibling)
        {
            ChildOrderChanged(Parent);
        }
        else if (RectTransform.rect != LastRect)
        {
            NotifyParent(Parent);
        }
        else if (RectTransform.position != LastPosition)
        {
            NotifyParent(Parent);
        }
    }

    public void ApplyChanges()
    {
        LastRect = RectTransform.rect;
        LastPosition = RectTransform.position;
    }
}
