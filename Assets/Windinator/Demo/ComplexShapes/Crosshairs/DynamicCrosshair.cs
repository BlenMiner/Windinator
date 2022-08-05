using Riten.Windinator.Shapes;
using UnityEngine;

public class DynamicCrosshair : CanvasDrawer
{
    [SerializeField] float m_thickness = 1f;

    [SerializeField, Range(0, 1)] float m_offset = 0f;

    [SerializeField] float m_expand = 0f;

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        float radius = Mathf.Min(size.x, size.y) * 0.5f;

        var offset = m_offset * radius;

        canvas.LineBrush.AddBatch(new Vector2(-radius - m_expand, 0), new Vector2(-offset - m_expand, 0));
        canvas.LineBrush.AddBatch(new Vector2(radius + m_expand, 0), new Vector2(offset + m_expand, 0));

        canvas.LineBrush.AddBatch(new Vector2(0, radius + m_expand), new Vector2(0, offset + m_expand));
        canvas.LineBrush.AddBatch(new Vector2(0, -radius - m_expand), new Vector2(0, -offset - m_expand));

        canvas.LineBrush.DrawBatch(m_thickness);
    }
}
