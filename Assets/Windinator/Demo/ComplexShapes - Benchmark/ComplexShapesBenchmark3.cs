using Riten.Windinator.Shapes;
using UnityEngine;

public class ComplexShapesBenchmark3 : CanvasDrawer
{
    public int XCount = 100;
    public int YCount = 100;

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        float w = (size.x / XCount) * 0.5f;
        float h = (size.y / YCount) * 0.5f;

        var upperCorner = new Vector2(size.x * -0.5f + w, size.y * 0.5f - h);

        for (int x = 0; x < XCount; ++x)
        {
            for (int y = 0; y < YCount; ++y)
            {
                Vector2 pos = upperCorner + new Vector2(x * w, -y * h) * 2;
                canvas.RectBrush.Draw(pos, new Vector2(w, h), Vector4.one * Mathf.Min(w, h), blend:
                    Mathf.PerlinNoise(Time.time + x * 1.01515f, y * 1.8452f) * Mathf.Max(w, h) * (Mathf.Sin(Time.time) + 1) * 0.5f);
            }
        }
        // canvas.RectBrush.DrawBatch();
    }
}
