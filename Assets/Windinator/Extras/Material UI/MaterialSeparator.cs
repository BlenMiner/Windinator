using Riten.Windinator;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSeparator : Image
{
    [SerializeField] LayoutElement m_layout;

    [SerializeField] bool m_vertical;

    [SerializeField] Swatch m_color;

    public Swatch Color
    {
        get => m_color;
        set
        {
            m_color = value;
            ColorChanged();
        }
    }

    void ColorChanged()
    {
        color = m_color.GetUnityColor(this);
    }

    public bool Vertical
    {
        get => m_vertical;
        set
        {
            m_vertical = value;
            UpdateVisuals();
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateVisuals();
    }
#endif

    public void UpdateVisuals()
    {
        ColorChanged();
        
        if (m_layout != null)
        {
            m_layout.flexibleHeight = m_vertical ? 1 : -1;
            m_layout.flexibleWidth = !m_vertical ? 1 : -1;
        }
    }
}
