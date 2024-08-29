using Riten.Windinator.Shapes;
using UnityEngine;

public class TestTexturing : CanvasDrawer
{
    [SerializeField] Texture2D m_texture;

    [SerializeField, Space] RectTransform m_texture2Rect;
    [SerializeField] Texture2D m_texture2;
    [SerializeField] float m_texture2Blend;

    [SerializeField, Space] RectTransform m_texture3Rect;
    [SerializeField] Texture2D m_texture3;
    [SerializeField] float m_texture3Blend;

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        canvas.RectBrush.Draw(
            Vector2.zero,
            size * 0.5f,
            Vector4.one * 10f,
            color: new ColorProperties(m_texture)
        );
         
        var rect = canvas.GetRect(m_texture2Rect);

        canvas.RectBrush.Draw(
            rect.position + rect.size * 0.5f,
            rect.size * 0.5f,
            Vector4.one * 10f,
            blend: m_texture2Blend,
            color: new ColorProperties(m_texture2)
        );

        var rect3 = canvas.GetRect(m_texture3Rect);

        canvas.RectBrush.Draw(
            rect3.position + rect3.size * 0.5f,
            rect3.size * 0.5f,
            Vector4.one * 20f,
            blend: m_texture3Blend,
            color: new ColorProperties(m_texture3)
        );
    }
}
