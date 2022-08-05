using Riten.Windinator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialCardStyle
{
    Elevated,
    Filled,
    Outlined
}

[ExecuteAlways]
public class MaterialCard : MonoBehaviour
{
    [SerializeField] RectangleGraphic m_graphic;

    [Space]

    public MaterialCardStyle Style;

    [Space]

    public Colors ElevatedColor;

    public Colors FilledColor;

    public Colors OutlinedColor;

    private void Reset()
    {
        m_graphic = GetComponent<RectangleGraphic>();
    }

    private void OnValidate()
    {
        SetDirty();
    }

    public void SetDirty()
    {
        m_graphic.SetOutline(Colors.Outline.ToColor(this), Style == MaterialCardStyle.Outlined ? 1f : 0f);
        m_graphic.SetShadow(Color.black, Style == MaterialCardStyle.Elevated ? 15f : 0f, 80f);

        m_graphic.color = Style switch
        {
            MaterialCardStyle.Outlined => OutlinedColor.ToColor(this),
            MaterialCardStyle.Filled => FilledColor.ToColor(this),
            MaterialCardStyle.Elevated=> ElevatedColor.ToColor(this),
            _ => FilledColor.ToColor(this)
        } ;
    }
}
