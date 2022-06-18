using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;

public class SettingsPanel : LayoutBaker
{
    public override Element Bake()
    {
        return new Rectangle(
            new Vertical(
                new Element[]
                {
                    new Horizontal(
                        new Element[] { new MaterialUI.Label("Fullscreen?", style: MaterialLabelStyle.Title) , new MaterialUI.Switch(false) },
                        alignment: TextAnchor.MiddleCenter,
                        spacing: 20f
                    ),
                    new MaterialUI.Switch(false),
                    new MaterialUI.Switch(true),
                },
                spacing: 15f
            ),
            size: new Vector2(200f, -1f),
            padding: Vector4.one * 20f,
            shape: new ShapeProperties
            {
                Roundness = Vector4.one * 40,
                Shadow = new ShadowProperties
                {
                    Blur = 20f,
                    Size = 10f
                }
            }
        );
    }
}
