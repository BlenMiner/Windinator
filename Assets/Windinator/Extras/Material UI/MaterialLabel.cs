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

    [SerializeField] Colors Color;

    [SerializeField] string Text;

    [SerializeField, Header("References")]

    TMP_Text m_text;

    ColorAssigner m_palette => Windinator.WindinatorConfig == null ? null : Windinator.WindinatorConfig.ColorPalette;

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

    public Colors LabelColor
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

        if (m_palette != null) m_text.color = m_palette[Color];

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