using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(PolygonGraphic))]
public class CoolBar : MonoBehaviour
{
    [SerializeField, Range(4, 1023)] int m_resolution = 50;
    [SerializeField, Range(0, 1)] float m_circlePosition;
    [SerializeField] float m_circleRadius;

    public float Position {
        get => m_circlePosition;
        set {
            m_circlePosition = value;
        }
    }

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

    readonly Vector2[] Normal = new Vector2[4]
    {
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
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

    void Update()
    {
        UpdatePolygon();
    }

    private void PopulatePoint(StaticArray<Vector4> points, int index, int faceId, float progress, Rect rect, float ratio)
    {
        Vector2 start = Corners[(int)faceId];
        Vector2 end = start + Direction[(int)faceId];
        Vector2 point = Vector2.Lerp(start, end, progress);

        Vector2 pointActual = Vector2.Scale(point, rect.size);
        var circleCenter = Vector2.Scale(new Vector2(m_circlePosition, 1f), rect.size);

        float direction = pointActual.x - circleCenter.x;
        float distance = Vector2.Distance(pointActual, circleCenter);
        float normlizedDirection = ((direction / m_circleRadius) - 1f) * 0.5f;

        if (distance < m_circleRadius)
        {
            float angle = normlizedDirection * Mathf.PI;

            float sin = Mathf.Sin(angle) * (m_circleRadius / rect.height);
            float cos = Mathf.Cos(angle) * (m_circleRadius / rect.width);

            point.y += sin;
            point.x = cos + m_circlePosition;
        }

        var normal = Normal[faceId];

        if (index > 0 && faceId < 3)
        {
            pointActual = Vector2.Scale(point, rect.size);
            var offset = Vector2.down * (Mathf.PerlinNoise(Time.time + pointActual.x * 0.02f, -Time.time + pointActual.y * 0.02f) + 1) * 0.35f * ratio * 0.1f;
            
            if (distance < 250f)
                offset += Vector2.down * (Mathf.PerlinNoise(Time.time + pointActual.x * 0.01f, -Time.time + pointActual.y * 0.01f) + 1) * 0.35f * ratio * 0.5f
                        * (1f - (distance / 250f));

            point += offset;
        }

        if (index > 0 && (point.y < 0 || point.y > 1 || point.x < 0 || point.x > 1))
        {
            // Remove point by puting it in the same spot as the last
            points[index] = points[index - 1];
        }
        else points[index] = point;
    }

    private void UpdatePolygon()
    {
        RectTransform rect = transform as RectTransform;

        var points = m_graphic.Points;

        m_resolution = (m_resolution / 4) * 4;

        points.Length = m_resolution;

        float ratio = rect.rect.height / (rect.rect.height + rect.rect.width);
        float ratioW = rect.rect.width / (rect.rect.height + rect.rect.width);

        if (ratio <= 0f) return;

        int pointsForSides, pointsForTops;

        pointsForSides = (int)(m_resolution * ratio);
        pointsForTops = m_resolution - pointsForSides;

        pointsForSides /= 2;
        pointsForTops /= 2;

        int j = 0;

        for (int i = 0; i < pointsForSides; ++i)
        {
            float progress = i / (float)(pointsForSides - 1);
            PopulatePoint(points, i, 0, progress, rect.rect, ratioW);
        }

        j += pointsForSides;
        
        for (int i = 0; i < pointsForTops; ++i)
        {
            float progress = i / (float)(pointsForTops - 1);
            PopulatePoint(points, j + i, 1, progress, rect.rect, ratioW);
        }

        j += pointsForTops;

        for (int i = 0; i < pointsForSides; ++i)
        {
            float progress = i / (float)(pointsForSides - 1);
            PopulatePoint(points, j + i, 2, progress, rect.rect, ratioW);
        }

        j += pointsForSides;

        for (int i = 0; i < pointsForTops; ++i)
        {
            float progress = i / (float)(pointsForTops - 1);
            PopulatePoint(points, j + i, 3, progress, rect.rect, ratioW);
        }

        j += pointsForTops;

        points.Length = j;

        m_graphic.SetMaterialDirty();
    }
}
