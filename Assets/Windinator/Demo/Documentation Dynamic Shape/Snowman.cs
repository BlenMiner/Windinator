using Riten.Windinator.Shapes;
using UnityEngine;

public class Snowman : CanvasDrawer
{
    [SerializeField] float m_blend = 0f;

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        float radius = Mathf.Min(size.x, size.y) * 0.5f;

        float firstRadius = radius * 0.5f;
        float secondRadius = radius * 0.3f;
        float thirdRadius = radius * 0.2f;

        Vector2 basePosition = new Vector2(0, -size.y * 0.5f + firstRadius);
        Vector2 bodyPosition = basePosition + Vector2.up * (firstRadius + secondRadius);
        Vector2 headPosition = bodyPosition + Vector2.up * (secondRadius + thirdRadius);

        canvas.CircleBrush.AddBatch(headPosition, thirdRadius, m_blend);
        canvas.CircleBrush.AddBatch(basePosition, firstRadius, m_blend);
        canvas.CircleBrush.AddBatch(bodyPosition, secondRadius, m_blend);

        canvas.CircleBrush.DrawBatch(DrawOperation.Union);
    }
}
