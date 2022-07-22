using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ShapePainter : MonoBehaviour
{
    [SerializeField] CanvasGraphic m_canvas;

    void Update()
    {
        if (m_canvas == null) return;

        m_canvas.Begin();
        m_canvas.DrawCircle(Vector2.zero, 20f);
        m_canvas.DrawCircle(new Vector2(40f * Mathf.Sin(Time.time), 0), 20f);
        m_canvas.DrawLine(new Vector2(-50, 0), new Vector2(-50, 50));
        m_canvas.DrawLine(new Vector2(-50, 50), new Vector2(50, 50));
        m_canvas.DrawLine(new Vector2(50, 50), new Vector2(50, 0));
        m_canvas.End();
    }
}
