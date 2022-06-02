using UnityEngine;
using System.Collections.Generic;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;
using static Riten.Windinator.LayoutBuilder.LayoutMaterial;

public struct SomeDataExample
{
    public string Content;
}

public class ScrollTest : LayoutBaker
{
    List<SomeDataExample> m_data = new List<SomeDataExample>(); 

    public override Element Bake()
    {
        return new Vertical(
            new Element[] {
                new Container(
                    new ScrollView(
                        new Vertical(
                            new Element[]
                            {
                                new Button("Hello"),
                                new Button("World"),
                                new Button("1"),
                                new Button("2"),
                                new Button("3"),
                                new Button("4"),
                                new Button("5"),
                                new Button("6"),
                            },
                            spacing: 10
                        )
                    ),
                    new Vector2(300, 200)
                )
            },
            alignment: TextAnchor.MiddleCenter
        );
    }

    private void Awake()
    {
        m_data.Add(new SomeDataExample
        {
            Content = "Some stuff"
        });
    }
}
