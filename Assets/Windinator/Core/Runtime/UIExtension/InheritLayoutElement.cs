using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class InheritLayoutElement : UIBehaviour, ILayoutElement
{
    [SerializeField] RectTransform m_inheritFrom;

    [SerializeField] Vector2 m_padding;
    [SerializeField] Vector2 m_minSize;

    public Vector2 Padding
    {
        get => m_padding;
        set
        {
            m_padding = value;
            SetDirty();
        }
    }

    public Vector2 MinSize
    {
        get => m_minSize;
        set
        {
            m_minSize = value;
            SetDirty();
        }
    }

    bool m_beingControlled = false;

    float m_prefferedWidth, m_prefferedHeight;

    public float minWidth => m_minSize.x;

    public float preferredWidth => Mathf.Max(m_prefferedWidth + m_padding.x, m_minSize.x);

    public float preferredHeight => Mathf.Max(m_prefferedHeight + m_padding.y, m_minSize.y);

    public float minHeight => m_minSize.y;

    public float flexibleHeight => -1;

    public float flexibleWidth => -1;

    public int layoutPriority => 0;

    RectTransform m_rectTransform;

    void UpdateBeingControlled()
    {
        m_beingControlled = transform.parent != null && transform.parent.GetComponent<HorizontalOrVerticalLayoutGroup>() != null;
    }

    protected override void Awake()
    {
        m_rectTransform = transform as RectTransform;
    }

    protected override void OnEnable()
    {
        if (m_rectTransform == null)
            m_rectTransform = transform as RectTransform;

        base.OnEnable();
        UpdateBeingControlled();
    }

    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        UpdateBeingControlled();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        CalculateLayoutInputHorizontal();
        CalculateLayoutInputVertical();
        SetDirty();
    }
#endif

    public void SetDirty()
    {
        LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
    }

    public void CalculateLayout()
    {
        if (m_inheritFrom == null) return;
        if (m_rectTransform == null) m_rectTransform = transform as RectTransform;

        m_prefferedWidth = LayoutUtility.GetPreferredSize(m_inheritFrom, 0);
        m_prefferedHeight = LayoutUtility.GetPreferredSize(m_inheritFrom, 1);

        if (!m_beingControlled)
            m_rectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);

        m_inheritFrom.sizeDelta = new Vector2(m_prefferedWidth, m_prefferedHeight);
    }

    public void CalculateLayoutInputHorizontal() { }

    public void CalculateLayoutInputVertical() 
    {
        CalculateLayout();
    }
}
