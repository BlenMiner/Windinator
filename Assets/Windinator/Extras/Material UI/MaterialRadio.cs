using Riten.Windinator;
using Riten.Windinator.Animation;
using Riten.Windinator.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MaterialRadio : MonoBehaviour,
    ISelectHandler, IDeselectHandler, 
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IPointerClickHandler
{
    [Header("Value")]

    [SerializeField] bool m_value;

    [Header("References")]

    [SerializeField] RectangleGraphic m_selectionGraphic;

    [SerializeField] RectangleGraphic m_outlineGraphic;

    [SerializeField] RectangleGraphic m_knobGraphic;

    [Header("Style")]

    [SerializeField] Colors m_selectedColor = Colors.Primary;

    [SerializeField] Colors m_unselectedColor = Colors.Outline;

    [SerializeField] float m_animSpeed = 10f;

    [SerializeField] SoundLibrary m_clickSound = null;

    VarAnimator<Color> m_outlineColor;

    VarAnimator<float> m_knobScale;

    VarAnimator<float> m_selectionAlpha;

    bool m_oldValue;
    bool m_selected, m_hovering;
    bool m_pressing;

    float m_selectionValue = 0f;

    float m_pressingValue = 0f;

    public bool Value 
    {
        get => m_value;
        set 
        {
            m_value = value;
            UpdateTarget();
        }
    }

    public UnityEvent<bool> onValueChanged;

    public UnityEvent<MaterialRadio, bool> onValueChangedRef;

    void Awake()
    {
        if (m_outlineColor != null) return;

        m_oldValue = Value;
        m_outlineColor = new VarAnimator<Color>(VarAnimator<Color>.Lerp, 
            color => {
                if (m_outlineGraphic != null)
                    m_outlineGraphic.OutlineColor = color;

                if (m_knobGraphic != null)
                    m_knobGraphic.color = color;

                if (m_selectionGraphic != null)
                {
                    color.a = m_selectionGraphic.color.a;
                    m_selectionGraphic.color = color;
                }
            }, m_animSpeed);

        m_knobScale = new VarAnimator<float>(VarAnimator<float>.Lerp, 
            scale => {
                if (m_knobGraphic != null)
                    m_knobGraphic.transform.localScale = Vector3.one * scale;
            }, m_animSpeed);

        m_selectionAlpha = new VarAnimator<float>(VarAnimator<float>.Lerp, 
            alpha => {
                if (m_selectionGraphic != null)
                {
                    var c = m_selectionGraphic.color;
                    c.a = alpha;
                    m_selectionGraphic.color = c;
                }
            }, m_animSpeed);

        m_selectionAlpha.SetModifier(SelectionMod);

        SnapTarget();
    }

    float SelectionMod(float input, float delta)
    {
        return m_selectionValue * 0.4f + m_pressingValue * 0.15f;
    }
    
    void OnValidate()
    {
        if (m_outlineColor == null) Awake();

        SnapTarget();
    }

    void UpdateTarget()
    {
        if (m_outlineColor == null) Awake();
        
        m_outlineColor.AnimateToTarget(Value ? m_selectedColor.ToColor(this) : m_unselectedColor.ToColor(this));
        m_knobScale.AnimateToTarget(Value ? 1f : 0f);
    }

    void Update()
    {
        if (m_oldValue != Value)
        {
            onValueChanged?.Invoke(Value);
            onValueChangedRef?.Invoke(this, Value);
            m_oldValue = Value;
        }
        bool pressing = m_selected || m_hovering;
        m_selectionValue = Mathf.MoveTowards(m_selectionValue, pressing ? 1 : 0, Time.deltaTime * m_animSpeed);
        m_pressingValue = Mathf.MoveTowards(m_pressingValue, m_pressing ? 1 : 0, Time.deltaTime * m_animSpeed);

        m_outlineColor.Update(Time.deltaTime);
        m_knobScale.Update(Time.deltaTime);
        m_selectionAlpha.Update(Time.deltaTime);
    }

    public void SnapTarget()
    {
        UpdateTarget();

        m_outlineColor.Snap();
        m_knobScale.Snap();
        m_selectionAlpha.Snap();
    }

    public void OnSelect(BaseEventData eventData)
    {
        m_selected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        m_selected = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_hovering = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_hovering = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_pressing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_pressing = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_clickSound?.PlayRandom();
        Value = true;
    }
}
