using Riten.Windinator.Shapes;
using UnityEngine;

public class ComplexShapesBenchmark1 : CanvasDrawer
{
    public int BallCount = 500;

    public float Speed = 1f;

    public float Blend = 0f;

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        for (int i = 0; i < BallCount; ++i)
        {
            float t = (Time.time * Speed) + i * 100.59f;

            Vector2 noise = new Vector2(
                Mathf.PerlinNoise(t + 695, i * 100.12f) - 0.5f,
                Mathf.PerlinNoise(t + 571, i * 100.32f + 5000) - 0.5f
            );

            float rad = Mathf.PerlinNoise(-i * 1000.12f, i * 1000.63f) * 50f;

            canvas.CircleBrush.AddBatch(noise * size, rad, rad * Blend);
        }

        canvas.CircleBrush.DrawBatch();
    }
}
