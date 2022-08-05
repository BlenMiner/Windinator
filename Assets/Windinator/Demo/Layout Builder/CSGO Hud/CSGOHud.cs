using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;

public class CSGOHud : LayoutBaker
{
    public Color Primary;

    public Color Secondary;

    public Color Tertiary;

    public Color Background;

    public Element FadedRectangle(float fadeLength, Element[] children, TextAnchor alignment = TextAnchor.MiddleLeft)
    {
        return new Rectangle(
            new Horizontal(children, Padding: Vector4.one * 10f, alignment: TextAnchor.MiddleCenter, spacing: 5f),
            shape: new ShapeProperties {
                Color = Background,
                AlphaMultiplier = 0.25f,

                // This is where the magic happens, trim the upper & lower shadows
                MaskOffset = new Vector4 (
                    0, fadeLength,       // Pos
                    0, -fadeLength       // Size
                ),

                Shadow = new ShadowProperties {
                    Size = fadeLength,
                    Blur = fadeLength,
                    Color = Background
                }
            },
            alignment: alignment
        ).Flexible(0, 0);
    }

    public Element ProgressBar(float fill, Vector2 size, float outline, Color color)
    {
        return new Rectangle(size: new Vector2(size.x * fill, size.y), shape: new ShapeProperties{
            Color = color,
            Outline = new OutlineProperties {
                Color = Secondary,
                Size = outline
            }
        });
    }

    public Element Radar()
    {
        return new Container(
            new Rectangle(shape: new ShapeProperties{
                    Roundness = Vector4.positiveInfinity,
                    Color = Background,
                    Outline = new OutlineProperties {
                        Color = Secondary,
                        Size = 4
                    },
                },
                size: new Vector2(200, 200)
            ),
            padding: Vector4.one * 10f
        );
    }

    public Element HPAndShield()
    {
        var halfWayProgressBar = new Stack(
            new Element[] {
                ProgressBar(1f, new Vector2(50f, 10f), 1f, Secondary),
                ProgressBar(0.5f, new Vector2(50f, 10f), 0f, Primary)
            },
            alignment: TextAnchor.MiddleLeft
        );

        return FadedRectangle(50f, new Element[] {
            new MaterialUI.Icon(MaterialIcons.hospital, color: Primary, style: MaterialSize.Title),
            new MaterialUI.Label(" 50", color: Primary, style: MaterialSize.Title),
            halfWayProgressBar,

            new Spacer(10f),

            new MaterialUI.Icon(MaterialIcons.shield, color: Primary, style: MaterialSize.Title),
            new MaterialUI.Label(" 50", color: Primary, style: MaterialSize.Title),
            halfWayProgressBar
        });
    }

    public Element GunAmmo()
    {
        return FadedRectangle(50f, new Element[] {
            new MaterialUI.Label("30", color: Primary, style: MaterialSize.Headline),
            new MaterialUI.Label("/90", color: Primary, style: MaterialSize.Title),

            new Horizontal(
                new Element[] {
                    new MaterialUI.Icon(MaterialIcons.bullet, color: Primary, style: MaterialSize.Title),
                    new MaterialUI.Icon(MaterialIcons.bullet, color: Primary, style: MaterialSize.Title),
                    new MaterialUI.Icon(MaterialIcons.bullet, color: Primary, style: MaterialSize.Title),
                    new MaterialUI.Icon(MaterialIcons.bullet, color: Primary, style: MaterialSize.Title),
                    new MaterialUI.Icon(MaterialIcons.bullet, color: Primary, style: MaterialSize.Title),
                },
                spacing: -10f
            )
        });
    }

    public Element GunElement(MaterialIcons icon, string name, string id, bool selected)
    {
        var content = new Stack(new Element[] {
            new MaterialUI.Icon(
                icon: icon, 
                style: MaterialSize.Display, 
                color: Primary, 
                padding: new Vector4(50f, 0, 50f, 0)
            ),
            new Container(
                child: new MaterialUI.Label(name, color: Color.white),
                alignment: TextAnchor.LowerRight
            ).Flexible(),
            new Container(
                child: new MaterialUI.Label(id, color: Color.white),
                alignment: TextAnchor.UpperRight
            ).Flexible(),
        });

        return selected ? FadedRectangle(50f, new Element[] {content}) : 
                          new Container(content, padding: Vector4.one * 10f);
    }

    public override Element Bake()
    {
        return new Vertical(
            new Element[]
            {
                new Horizontal(
                    children: new Element[]
                    {
                        new Vertical(
                            new Element[] {
                                FadedRectangle(50f, new Element[]
                                {
                                    new MaterialUI.Label("Pit", style: MaterialSize.Title, color: Tertiary, fontStyle: TMPro.FontStyles.Bold)
                                }).Flexible(1, -1),

                                Radar(),

                                FadedRectangle(50f, new Element[]
                                {
                                    new MaterialUI.Label("$1250", style: MaterialSize.Title, color: Primary)
                                }, alignment: TextAnchor.MiddleCenter).Flexible(1, -1), // Make it flexible only on the horizontal axis, it will expand to occupy all available space
                            },
                            spacing: 20f
                        )
                    }
                ),
                new FlexibleSpace(),
                new Horizontal(
                    children: new Element[]
                    {
                        HPAndShield(),
                        new FlexibleSpace(),

                        new Vertical(
                            new Element[] {
                                GunElement(MaterialIcons.pistol, "M4A4", "1", true),
                                GunElement(MaterialIcons.knife, "Knife", "2", false),
                                GunElement(MaterialIcons.bomb, "Grenade", "3", false),
                                GunAmmo()
                            },
                            alignment: TextAnchor.MiddleRight,
                            spacing: 20f
                        )
                    },
                    alignment: TextAnchor.LowerLeft
                ).Flexible(1, 0)
            }
        ).Flexible();
    }
}
