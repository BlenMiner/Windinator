using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class KnobOnRectangle : MonoBehaviour
{
    [SerializeField] CanvasGraphic m_canvas;

    void Update()
    {
        if (m_canvas == null) return;

        m_canvas.Begin();

        var size = m_canvas.Size;
        const float expand = 200f;

        m_canvas.DrawRect(new Vector2(0, -expand * 0.5f), new Vector2(size.x, size.y + expand));
        m_canvas.DrawCircle(Vector2.zero, 70f, 80f, DrawOperation.Substract);

        m_canvas.End();
    }
}
