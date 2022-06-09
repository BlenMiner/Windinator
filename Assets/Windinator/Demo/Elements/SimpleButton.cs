using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;
using static Riten.Windinator.LayoutBuilder.Material;

public class SimpleButton : LayoutBaker
{
    public Reference<MaterialButton> ButtonReference;

    public override Element Bake()
    {
        return new Button("Test").GetReference(out ButtonReference);
    }
}
