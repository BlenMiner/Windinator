using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexShapesBenchmark1 : CanvasDrawer
{
    public int m_ballCount = 500;

    public float m_speed = 1f;

    public float m_blend = 0f;

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        for (int i = 0; i < m_ballCount; ++i)
        {
            float t = (Time.time * m_speed) + i * 100.59f;

            Vector2 noise = new Vector2(
                Mathf.PerlinNoise(t + 695, i * 100.12f) - 0.5f,
                Mathf.PerlinNoise(t + 571, i * 100.32f + 5000) - 0.5f
            );

            float rad = Mathf.PerlinNoise(-i * 1000.12f, i * 1000.63f) * 50f;

            canvas.DrawCircle(noise * size, rad, rad * m_blend);
        }
    }
}
