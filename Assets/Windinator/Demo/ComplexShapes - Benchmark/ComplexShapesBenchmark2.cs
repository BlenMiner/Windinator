using Riten.Windinator.Shapes;
using UnityEngine;

public class ComplexShapesBenchmark2 : CanvasDrawer
{
    public int m_lineCount = 100;

    public float m_lineStep = 0.1f;

    [Header("Graphics")]

    public float m_lineWeight = 2f;

    public float m_blend = 0f;

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        Vector2 lastPoint = new Vector2(
            Mathf.PerlinNoise(-m_lineStep, 56.151f) - 0.5f,
            Mathf.PerlinNoise(694.5101f, -m_lineStep) - 0.5f
        ) * size;

        float time = Time.time;

        for (int i = 0; i < m_lineCount; ++i)
        {
            Vector2 point = new Vector2(
                (Mathf.PerlinNoise(i * m_lineStep, 56.151f) - 0.5f) * size.x,
                (Mathf.PerlinNoise(694.5101f, i * m_lineStep) - 0.5f) * size.y
            );

            float len = i * m_lineStep;

            var pointNoise = point + new Vector2(
                (Mathf.PerlinNoise(len * 1.6868f + time,len * 1.2515f) - 0.5f) * 10f,
                (Mathf.PerlinNoise(len * 1.8778f, len * 1.8686f + time) - 0.5f) * 10f
            );

            // Add/batch lines
            canvas.LineBrush.AddBatch(lastPoint, pointNoise);

            lastPoint = pointNoise;
        }

        // Flush what we batched
        canvas.LineBrush.DrawBatch(m_lineWeight, m_lineWeight * m_blend);
    }
}
