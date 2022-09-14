using Riten.Windinator.Shapes;
using UnityEngine;

public class ProfileDrawer : CanvasDrawer
{
    [Range(-2, 0)] public float YOffset = -5f;

    [Range(0, 1)] public float Blend = 1f;

    [Range(0, 1)] public float Hidden = 0f;

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        float radius = Mathf.Min(size.x, size.y) * 0.5f + 1f;

        canvas.CircleBrush.Draw(Vector2.zero, radius);
        canvas.CircleBrush.Draw(
            new Vector2(0, (YOffset * size.y - size.y) * (1 - Hidden)),
            radius, radius * Blend,
            operation: DrawOperation.Substract
        );
    }
}
