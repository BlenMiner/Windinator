using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;

public class GoogleResultEntry : LayoutBaker
{
    public Reference<MaterialLabel> Label;

    public Reference<MaterialButton> DeleteButton;

    public override Element Bake()
    {
        return new Horizontal(
            new Element[]
            {
                new MaterialUI.Label("Some Result").GetReference(out Label),
                new FlexibleSpace(),
                new MaterialUI.Button("A Button!"),
                new MaterialUI.Button(icon: MaterialIcons.pencil, type: MaterialButtonType.Text),
                new MaterialUI.Button(icon: MaterialIcons.trash_can, type: MaterialButtonType.Text).GetReference(out DeleteButton)
            }
        ).Flexible(1, 0);
    }
}
