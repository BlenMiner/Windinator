using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class DefineLayout : MonoBehaviour, ILayoutElement
{
    [SerializeField] Vector2 m_minSize = new Vector2(0, 0);

    [SerializeField] Vector2 m_prefferedSize = new Vector2(-1, -1);

    [SerializeField] Vector2 m_flexible = new Vector2(0, 0);

    Vector2 m_cachedPrefferedSize;

    [System.NonSerialized] public RectTransform RectTransform;

    [System.NonSerialized] public Vector2 CachedSize;

    TMP_Text m_text;

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
            var result = m_prefferedSize;

            if (result.x < 0 || result.y < 0)
            {
                if (m_text != null)
                {
                    var size = m_text.GetPreferredValues();

                    if (result.x < 0) result.x = size.x;
                    if (result.y < 0) result.y = size.y;
                }
                else
                {
                    if (result.x < 0) result.x = 0;
                    if (result.y < 0) result.y = 0;
                }
            }

            return result;
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

    public float minWidth => m_minSize.x;

    public float preferredWidth => m_cachedPrefferedSize.x;

    public float flexibleWidth => m_flexible.x;

    public float minHeight => m_minSize.y;

    public float preferredHeight => m_cachedPrefferedSize.y;

    public float flexibleHeight => m_flexible.y;

    public int layoutPriority => 0;

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
        RectTransform = transform as RectTransform;
        Parent = transform.parent as RectTransform;

        m_minSize = Vector2.Max(default, m_minSize);
        m_flexible = Vector2.Max(default, m_flexible);

        NotifyParent(transform.parent);
    }
#endif

    void TextUpdated(TMP_TextInfo info)
    {
        NotifyParent(Parent);
    }

    void OnEnable()
    {
        if (m_text == null)
        {
            m_text = GetComponent<TMP_Text>();
            if (m_text != null) 
                m_text.OnPreRenderText += TextUpdated;
        }

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
        if (m_text != null) m_text.OnPreRenderText -= TextUpdated;

        if (Parent == null || !Layouts.ContainsKey(Parent)) return;

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
        if (parent == null || !Layouts.ContainsKey(parent)) return;

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
#if UNITY_EDITOR
        if (m_text == null)
        {
            m_text = GetComponent<TMP_Text>();
            if (m_text != null) m_text.OnPreRenderText += TextUpdated;
        }
#endif
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

    public void CalculateLayoutInputHorizontal()
    {
        m_cachedPrefferedSize = PrefferedSize;
    }

    public void CalculateLayoutInputVertical()
    {
        m_cachedPrefferedSize = PrefferedSize;
    }
}
