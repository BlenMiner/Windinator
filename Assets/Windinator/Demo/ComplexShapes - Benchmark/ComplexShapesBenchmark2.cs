using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexShapesBenchmark2 : CanvasDrawer
{
    [SerializeField] int m_lineCount = 100;

    [SerializeField] float m_lineStep = 0.1f;

    [Header("Graphics")]

    [SerializeField] float m_lineWeight = 2f;

    [SerializeField] float m_blend = 0f;

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        Vector2 lastPoint = new Vector2(
            Mathf.PerlinNoise(-m_lineStep, 56.151f) - 0.5f,
            Mathf.PerlinNoise(694.5101f, -m_lineStep) - 0.5f
        ) * size;

        for (int i = 0; i < m_lineCount; ++i)
        {
            Vector2 normalized_point = new Vector2(
                Mathf.PerlinNoise(i * m_lineStep, 56.151f) - 0.5f,
                Mathf.PerlinNoise(694.5101f, i * m_lineStep) - 0.5f
            );

            Vector2 point = normalized_point * size;

            var pointNoise = point + new Vector2(
                Mathf.PerlinNoise(point.x * 0.01f + Time.time, point.y * 0.01f) - 0.5f,
                Mathf.PerlinNoise(point.y * 0.01f, point.x * 0.01f + Time.time) - 0.5f
            ) * 10f;

            canvas.DrawLine(lastPoint, pointNoise, m_lineWeight, m_lineWeight * m_blend);

            lastPoint = pointNoise;
        }
    }
}
