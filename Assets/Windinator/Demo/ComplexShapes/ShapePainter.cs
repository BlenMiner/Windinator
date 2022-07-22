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

        m_canvas.Begin(Color.black);
        m_canvas.DrawCircle(Vector2.zero, 20f);
        m_canvas.DrawCircle(new Vector2(40f, 0), 20f);
        m_canvas.End();
    }
}
