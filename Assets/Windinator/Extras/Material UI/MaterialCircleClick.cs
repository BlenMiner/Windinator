using UnityEngine;
using UnityEngine.EventSystems;

public class MaterialCircleClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float m_sizeSpeed = 5f;
    [SerializeField] float m_alphaSpeed = 10f;

    SignedDistanceFieldGraphic _graphic;

    SignedDistanceFieldGraphic m_graphic
    {
        get
        {
            if (_graphic == null) 
                _graphic = GetComponent<SignedDistanceFieldGraphic>();
            return _graphic;
        }
    }

    float m_alpha = 0f;
    float m_size = 0f;
    Vector2 m_pos = Vector2.zero;
    bool m_mouseDown = false;
    bool m_dirty = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_forceClick) return;

        RectTransform rectTransform = transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.pressPosition, null, out m_pos);

        var pivot = Vector2.Scale(rectTransform.pivot, rectTransform.sizeDelta);
        var middle = rectTransform.sizeDelta * 0.5f;
        m_pos = (m_pos + pivot) - middle;

        m_alpha = 0f;
        m_size = 0f;

        m_dirty = true;
        m_mouseDown = true;
    }

    bool m_forceClick = false;

    public void ForceClick(bool force)
    {
        m_forceClick = force;
        m_mouseDown = force;
        m_dirty = true;
    }

    public void SnapAnimation()
    {
        float newSize = m_mouseDown ? Mathf.Max(Screen.width, Screen.height) : 0f;
        float newAlpha = m_mouseDown ? 1 : 0;

        m_alpha = newAlpha;
        m_size = newSize;

        var g = m_graphic;

        g.SetCircle(m_pos, g.CircleColor, m_size, m_alpha * 0.5f);

        m_dirty = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_forceClick) return;

        m_dirty = true;
        m_mouseDown = false;
    }

    private void Update()
    {
        var g = m_graphic;

        if (g == null) return;

        RectTransform rectTransform = transform as RectTransform;
        var size = rectTransform.sizeDelta;
        float speedMult = Mathf.Max(size.x, size.y);

        if (m_mouseDown)
        {
            float newSize = m_size + Time.deltaTime * m_sizeSpeed * speedMult;
            float newAlpha = Mathf.MoveTowards(m_alpha, 1f, Time.deltaTime * m_alphaSpeed);

            if (newAlpha != m_alpha || newSize != m_size)
            {
                m_alpha = newAlpha;
                m_size = newSize;
                m_dirty = true;
            }
        }
        else
        {
            float newAlpha = Mathf.MoveTowards(m_alpha, 0f, Time.deltaTime * m_alphaSpeed);

            if (newAlpha != m_alpha)
            {
                m_alpha = newAlpha;
                m_size = Mathf.Max(0f, m_size + Time.deltaTime * m_sizeSpeed * speedMult);
                m_dirty = true;
            }
            else
            {
                m_size = 0f;
            }
        }

        if (m_dirty)
        {
            g.SetCircle(m_pos, g.CircleColor, m_size, m_alpha * 0.5f);
            m_dirty = false;
        }
    }
}
