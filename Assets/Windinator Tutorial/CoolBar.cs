using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(PolygonGraphic))]
public class CoolBar : MonoBehaviour
{
    [SerializeField, Range(4, 1023)] int m_resolution = 50;
    [SerializeField, Range(0, 1)] float m_circlePosition;
    [SerializeField, Range(0, 1)] float m_circleRadius;

    PolygonGraphic m_graphic;

    readonly Vector2[] Corners = new Vector2[4]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
    };

    readonly Vector2[] Direction = new Vector2[4]
{
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0),
};

    private void Awake()
    {
        m_graphic = GetComponent<PolygonGraphic>();
    }

    private void OnValidate()
    {
        if (m_graphic == null) Awake();

        UpdatePolygon();
    }

    private void PopulatePoint(StaticArray<Vector4> points, int index, int state, float progress)
    {
        Vector2 start = Corners[(int)state];
        Vector2 end = start + Direction[(int)state];
        Vector2 point = Vector2.Lerp(start, end, progress);

        var circleCenter = new Vector2(m_circlePosition, 1f);

        float direction = point.x - m_circlePosition;
        float distance = Vector2.Distance(point, circleCenter);

        if (distance < m_circleRadius)
        {
            float nd = ((direction / m_circleRadius) - 1f) * 0.5f;
            float angle = nd * Mathf.PI;

            float sin = Mathf.Sin(angle) * m_circleRadius;
            float cos = Mathf.Cos(angle) * m_circleRadius;

            point.y += sin;
            point.x = cos + m_circlePosition;

            if (index > 0 && (point.y < 0 || point.y > 1 || point.x < 0 || point.x > 1))
            {
                // Remove point by puting it in the same spot as the last
                points[index] = points[index - 1];
                return;
            }

            /*point.y = Mathf.Min(Mathf.Max(point.y, 0f), 1f);
            point.x = Mathf.Min(Mathf.Max(point.x, 0f), 1f);*/
        }

        points[index] = point;
    }

    private void UpdatePolygon()
    {
        var points = m_graphic.Points;

        m_resolution = (m_resolution / 4) * 4;

        points.Length = m_resolution;

        int pointsPerFace = m_resolution / 4;

        for (int i = 0; i < m_resolution; ++i)
        {
            int face = i / pointsPerFace;
            float progress = ((i % pointsPerFace) / (float)pointsPerFace);

            PopulatePoint(points, i, face, progress);
        }

        m_graphic.SetMaterialDirty();
    }
}
