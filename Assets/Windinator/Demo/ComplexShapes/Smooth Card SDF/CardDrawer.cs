using Riten.Windinator.Shapes;
using UnityEngine;

public class CardDrawer : CanvasDrawer
{
    [SerializeField] RectTransform m_ball;

    [Range(0, 1)] public float BallScale = 1;

    [Header("Settings")]

    [SerializeField] float m_blend = 0f;

    [SerializeField] float m_roundness = 10f;

    [SerializeField] float m_sphereRadius = 50f;

    [Range(0, 1)] public float SphereToRect;


    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        if (m_ball == null) return;
        
        Vector2 rectSize = Vector2.Lerp(Vector2.one * m_sphereRadius * 0.5f, size * 0.5f, SphereToRect);
        float roundness = Mathf.Lerp(m_sphereRadius * 0.5f, m_roundness, SphereToRect);

        canvas.RectBrush.Draw(Vector2.zero, rectSize, Vector4.one * roundness);

        ProfileBall(canvas);
    }

    void ProfileBall(CanvasGraphic canvas)
    {
        var rect = canvas.GetRect(m_ball);
        var size = rect.size;
        float radius = (Mathf.Min(size.x, size.y) * 0.5f + 1f) * BallScale;

        canvas.CircleBrush.Draw(rect.position + size * 0.5f, radius, m_blend, operation: DrawOperation.Union);
    }
}
