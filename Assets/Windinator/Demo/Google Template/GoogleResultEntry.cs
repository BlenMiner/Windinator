using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;

public class GoogleResultEntry : LayoutBaker
{
    public Reference<MaterialLabel> Label;

    public override Element Bake()
    {
        return new Horizontal(
            new Element[]
            {
                new MaterialUI.Label("Some Result").GetReference(out Label),
                new FlexibleSpace(),
                new MaterialUI.Button("A Button"),
                new MaterialUI.Button(icon: MaterialIcons.pencil),
                new MaterialUI.Button(icon: MaterialIcons.trash_can)
            }
        );
    }
}
