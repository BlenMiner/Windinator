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

    public MaterialLabelStyle Style;

    public FontStyles FontStyle;

    public ColorAssigner.AllColorType Color;

    public string Text;

    [SerializeField, Header("References")]

    TMP_Text m_text;
    ColorAssigner m_palette => Windinator.WindinatorConfig.ColorPalette;

    private void OnValidate()
    {
        if (m_text == null) return;

        SetDirty();
    }

    public void SetDirty()
    {
        m_text.text = Text;
        m_text.fontSize = (int)Style;
        m_text.color = m_palette[Color];
        m_text.fontStyle = FontStyle;
    }
}
