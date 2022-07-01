using Riten.Windinator;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum MaterialLabelStyle
{
    Display = 57,
    Headline = 32,
    Title = 22,
    Body = 15,
    Label = 12
}

public class MaterialLabel : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] MaterialLabelStyle Style;

    [SerializeField] FontStyles FontStyle;

    [SerializeField] Swatch Color;

    [SerializeField] string Text;

    [SerializeField, Header("References")]

    TMP_Text m_text;

    public MaterialLabelStyle LabelStyle
    {
        get => Style;
        set { Style = value; SetDirty(); }
    }

    public string LabelText
    {
        get => Text;
        set { Text = value; SetDirty(); }
    }

    public Swatch LabelColor
    {
        get => Color;
        set { Color = value; SetDirty(); }
    }

    public FontStyles LabelFontStyle
    {
        get => FontStyle;
        set { FontStyle = value; SetDirty(); }
    }

    bool m_dirty = true;

    private void OnValidate()
    {
        if (m_text == null) return;

        SetDirty();
    }

    public void ForceUpdate()
    {
        m_text.text = Text;
        m_text.fontSize = (int)Style;
        m_text.color = Color.UnityColor;
        m_text.fontStyle = FontStyle;
        m_dirty = false;
    }

    public void SetDirty()
    {
        m_dirty = true;
    }

    private void Update()
    {
        if (m_dirty) ForceUpdate();
    }
}
