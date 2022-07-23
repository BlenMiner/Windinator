using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ShapePainter : MonoBehaviour
{
    [SerializeField] CanvasGraphic m_canvas;

    [SerializeField] RectTransform m_knob;

    void Update()
    {
        if (m_canvas == null) return;

        m_canvas.Begin();
        
        m_canvas.DrawRect(Vector2.zero, m_canvas.Size);

        var knob = m_canvas.GetRect(m_knob);

        float rad = Mathf.Min(knob.size.x, knob.size.y) * 0.5f;

        m_canvas.DrawCircle(knob.center, rad, rad * 0.5f, DrawOperation.Substract);

        m_canvas.End();
    }
}
