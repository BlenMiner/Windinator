using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class KnobOnRectangle : CanvasDrawer
{
    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        const float expand = 200f;
        canvas.DrawRect(new Vector2(0, -expand * 0.5f), new Vector2(size.x, size.y + expand));
        canvas.DrawCircle(Vector2.zero, 70f, 80f, DrawOperation.Substract);
    }
}
