using Riten.Windinator.Shapes;
using UnityEngine;

public class ComplexShapesBenchmark3 : CanvasDrawer
{

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        canvas.RectBrush.Draw(Vector2.zero, Vector2.one * 50f, Vector4.one * 10f);
        // canvas.RectBrush.DrawBatch();
    }
}
