using Riten.Windinator.Shapes;
using UnityEngine;

[ExecuteAlways]
public class KnobOnRectangle : CanvasDrawer
{
    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        const float expand = 200f;
        canvas.RectBrush.Draw(new Vector2(0, -expand - size.y * 0.5f), new Vector2(size.x, size.y + expand));
        canvas.CircleBrush.Draw(Vector2.zero, 70f, 80f, operation: DrawOperation.Substract);
    }
}
