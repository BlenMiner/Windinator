using Riten.Windinator.Shapes;
using UnityEngine;
using UnityEngine.EventSystems;

public class PolygonSidebarDemo : CanvasDrawer, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] float m_noise = 10f;
    [SerializeField] float m_roundness = 0;
    [SerializeField] float m_mouseRange = 100;

    [SerializeField, Range(0, 1)] float m_side = 0f;
    [SerializeField, Range(0, 1)] float m_noisePower = 0f;
    [SerializeField, Range(0, 1)] float m_mousePower = 0f;

    bool m_dragging = false;
    bool m_open = false;

    protected override void Update()
    {
        base.Update();

        m_side = Mathf.Lerp(m_side, m_open ? 1f : 0f, Time.deltaTime * 5f);

        if (m_dragging)
        {
            m_mousePower = Mathf.Lerp(m_mousePower, 1f, Time.deltaTime * 10f);
            m_noisePower = Mathf.Lerp(m_noisePower, 1f, Time.deltaTime * 2f);
        }
        else
        {
            m_mousePower = Mathf.Lerp(m_mousePower, 0f, Time.deltaTime * 5f);
            m_noisePower = Mathf.Lerp(m_noisePower, 0f, Time.deltaTime * 2f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!m_dragging) return;

        var mp = Canvas.GetMousePosition();
        var halfSize = Canvas.Size * 0.5f;

        float border = Mathf.Lerp(halfSize.x, -halfSize.x, m_side);

        if (Mathf.Abs(mp.x - border) > Canvas.Size.x * 0.8f)
        {
            m_open = !m_open;
            m_dragging = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_dragging = false;
    }

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        var halfSize = size * 0.5f;
        var brush = canvas.PolyBrush;

        var up = new Vector2(Mathf.Lerp(halfSize.x, -halfSize.x, m_side), halfSize.y);
        var down = new Vector2(up.x, -up.y);

        var mouse = canvas.GetMousePosition();
        mouse.x = Mathf.Clamp(mouse.x, -halfSize.x, halfSize.x);

        const int POINTS = 200;

        brush.AddPoint(new Vector2(halfSize.x, halfSize.y));
        for (int i = 0; i <= POINTS; ++i)
        {
            float l = i / (float)POINTS;

            var point = Vector2.Lerp(up, down, l);

            float distanceToMouse = Mathf.Abs(point.y - mouse.y);
            float distanceToMouseX = up.x - mouse.x;
            float mousePower = 1f - Mathf.Min(distanceToMouse / m_mouseRange, 1);

            mousePower = mousePower * mousePower * m_mousePower;

            float d = Mathf.PerlinNoise(l * 5f, Time.time) * m_noise * (1f - mousePower * mousePower);

            brush.AddPoint(point + Vector2.left * (distanceToMouseX * mousePower - d * m_noisePower));
        }
        brush.AddPoint(new Vector2(halfSize.x, -halfSize.y));

        brush.Draw(roundness: m_roundness);
    }
}
