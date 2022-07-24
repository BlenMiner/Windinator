using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public abstract class CanvasDrawer : MonoBehaviour
{
    [SerializeField] bool m_drawOnlyOnce;

    Vector2 m_lastSize;

    CanvasGraphic m_canvas;

    protected abstract void Draw(CanvasGraphic canvas, Vector2 size);

    protected virtual void Update()
    {
        if (m_canvas == null)
        {
            m_canvas = GetComponent<CanvasGraphic>();
            if (m_canvas == null) return;
        }

        var size = m_canvas.Size;

        if (!m_drawOnlyOnce)
        {
            m_canvas.Begin();
            Draw(m_canvas, size);
            m_canvas.End();

            return;
        }
        else if (m_lastSize != size)
        {
            m_canvas.Begin();
            Draw(m_canvas, size);
            m_canvas.End();
            
            m_lastSize = size;
        }
    }
}
