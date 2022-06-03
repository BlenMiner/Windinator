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

    public ColorAssigner.AllColorType ElevatedColor;

    public ColorAssigner.AllColorType FilledColor;

    public ColorAssigner.AllColorType OutlinedColor;

    public ColorAssigner Pallete => Windinator.WindinatorConfig.ColorPalette;

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
        m_graphic.SetOutline(Pallete[ColorAssigner.ColorType.Outline].Color, Style == MaterialCardStyle.Outlined ? 1f : 0f);
        m_graphic.SetShadow(Color.black, Style == MaterialCardStyle.Elevated ? 5f : 0f, 9f);

        m_graphic.color = Style switch
        {
            MaterialCardStyle.Outlined => Pallete[OutlinedColor],
            MaterialCardStyle.Filled => Pallete[FilledColor],
            MaterialCardStyle.Elevated=> Pallete[ElevatedColor],
            _ => Pallete[FilledColor]
        } ;
    }
}
