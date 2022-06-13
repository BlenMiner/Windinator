using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;

public class GoogleTemplate : LayoutBaker
{
    [SerializeField] Sprite m_logo;

    public override Element Bake()
    {
        return new Vertical(
            children: new Element[]
            {
                new Graphic(sprite: m_logo),
            }
        );
    }
}
