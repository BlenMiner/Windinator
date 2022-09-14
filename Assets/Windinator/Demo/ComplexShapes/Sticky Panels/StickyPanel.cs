using Riten.Windinator.Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPanel : CanvasDrawer
{
    static List<StickyPanel> m_panels = new List<StickyPanel>();

    [SerializeField] float m_roundness = 15f;

    [SerializeField] float m_blend = 15f;

    [Header("Debug")]
    [SerializeField] bool m_renderOnlyStickyPoints;

    [System.NonSerialized] List<Vector2> m_stickyPoints;

    private void OnEnable()
    {
        if (m_stickyPoints == null)
        {
            m_stickyPoints = new List<Vector2>();

            int count = Random.Range(10, 30);

            for (int i = 0; i < count; ++i)
            {
                bool top = Random.Range(0, 2) == 1;

                Vector2 point = new Vector2();

                float sideVal = Random.Range(0, 2);
                float offsetVal = Random.Range(0f, 1f);

                if (top)
                {
                    point.y = sideVal;
                    point.x = offsetVal;
                }
                else
                {
                    point.y = offsetVal;
                    point.x = sideVal;
                }

                m_stickyPoints.Add(point);
            }
        }
        m_panels.Add(this);
    }

    private void OnDisable()
    {
        m_panels.Remove(this);
    }

    float RectDist(Rect ra, Rect rb)
    {
        float yDist;
        float xDist;

        if (rb.center.y > ra.center.y)
             yDist = rb.yMin - ra.yMax;
        else yDist = ra.yMin - rb.yMax;

        if (rb.center.x > ra.center.x)
             xDist = rb.xMin - ra.xMax;
        else xDist = ra.xMin - rb.xMax;

        return Mathf.Max(xDist, yDist);
    }

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        var stickyLayer = Canvas.GetNewLayer();

        canvas.Clear(stickyLayer);

        canvas.RectBrush.Draw(Vector2.zero, size * 0.5f, Vector4.one * m_roundness);

        var innerSize = size - new Vector2(20f, 20f);

        foreach(var p in m_stickyPoints)
        {
            canvas.CircleBrush.Draw(p * innerSize - innerSize * 0.5f, 5f, layer : stickyLayer);
        }

        var myRect = canvas.rectTransform.rect;

        foreach (var panel in m_panels)
        {
            if (panel == this) continue;

            var otherRect = canvas.GetRect(panel.Canvas.rectTransform);

            float d = RectDist(myRect, otherRect);
            float sticky = 1f;

            if (d < 0) sticky = 1f - Mathf.Clamp01(-d / (m_roundness * 2f));

            canvas.RectBrush.Draw(otherRect.center, otherRect.size * 0.5f, Vector4.one * panel.m_roundness, m_blend * sticky, layer: stickyLayer);
        }

        if (m_renderOnlyStickyPoints)
        {
            canvas.Copy(stickyLayer, canvas.MainLayer);
        }
        else
        {
            canvas.Add(stickyLayer, canvas.MainLayer);
        }

        stickyLayer.Dispose();
    }
}
