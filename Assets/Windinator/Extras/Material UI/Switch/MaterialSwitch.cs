using Riten.Windinator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MaterialSwitch : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("General")]

    public bool Value = false;

    [SerializeField, Searchable] MaterialIcons m_IconTrue;
    [SerializeField, Searchable] MaterialIcons m_IconFalse;

    [SerializeField] float m_AnimSpeed = 10f;
    [SerializeField] AnimationCurve m_AnimThumb;
    [SerializeField] AnimationCurve m_AnimThumbStretch;

    [Space(20f), Header("Track")]

    [SerializeField] RectangleGraphic m_Track;
    [SerializeField] Colors m_TrackSelected = Colors.Primary;
    [SerializeField] Colors m_TrackUnselected = Colors.SurfaceVariant;
    [SerializeField] Colors m_OutlineSelected = Colors.Primary;
    [SerializeField] Colors m_OutlineUnselected = Colors.Outline;

    [Header("Thumb")]

    [SerializeField] RectangleGraphic m_Thumb;
    [SerializeField] Colors m_ThumbUnselected = Colors.OnPrimary;
    [SerializeField] Colors m_ThumbSelected = Colors.Outline;

    [Header("Icon")]

    [SerializeField] MaterialIcon m_Icon;

    [SerializeField] Colors m_IconUnselected = Colors.OnPrimaryContainer;
    [SerializeField] Colors m_IconSelected = Colors.OnSurfaceVariant;
    [SerializeField] MaterialIcons m_IconContent = MaterialIcons.none;

    const float Offset = 10f;
    const float ThumbSize = 24f;
    const float ThumbMinSize = 16f;
    const float ThumbMaxSize = 28f;

    struct AnimationState
    {
        public Color TrackColor;
        public Color TrackOutlineColor;

        public Color ThumbColor;
        public Vector2 ThumbPosition;
        public Vector2 ThumbSize;

        public MaterialIcons Icon;
        public Color IconColor;
    }

    private AnimationState StartState;
    private AnimationState TargetState;
    private float AnimationValue = 1f;

    public bool Selected { get; private set; } = false;

    private void Awake()
    {
        SnapState();
    }

    private void OnValidate()
    {
        SnapState();
    }

    public void SnapState()
    {
        TargetState = GetTargetState();
        Apply(TargetState);
        AnimationValue = 1f;
    }

    AnimationState GetTargetState()
    {
        return new AnimationState
        {
            TrackColor = (Value ? m_TrackSelected : m_TrackUnselected).ToColor(),
            TrackOutlineColor = (Value ? m_OutlineSelected : m_OutlineUnselected).ToColor(),

            ThumbColor = (Value ? m_ThumbSelected : m_ThumbUnselected).ToColor(),
            ThumbPosition = new Vector2(Offset * (Value ? 1 : -1), 0),
            ThumbSize = Vector2.one * (Value ? ThumbSize : ThumbMinSize),

            Icon = (Value ? m_IconTrue : m_IconFalse),
            IconColor = (Value ? m_IconSelected : m_IconUnselected).ToColor()
        };
    }

    AnimationState GetCurrentState()
    {
        return new AnimationState
        {
            TrackColor = m_Track.color,
            TrackOutlineColor = m_Track.OutlineColor,

            ThumbColor = m_Thumb.color,
            ThumbPosition = m_Thumb.rectTransform.anchoredPosition,
            ThumbSize = m_Thumb.rectTransform.sizeDelta,
            
            Icon = m_Icon.Icon,
            IconColor = m_Icon.IconColor
        };
    }

    void AnimateState()
    {
        TargetState = GetTargetState();
        StartState = GetCurrentState();
        AnimationValue = 0f;
    }

    void Apply(AnimationState state)
    {
        m_Track.color = state.TrackColor;
        m_Track.OutlineColor = state.TrackOutlineColor;

        m_Thumb.color = state.ThumbColor;
        m_Thumb.rectTransform.anchoredPosition = state.ThumbPosition;
        m_Thumb.rectTransform.sizeDelta = state.ThumbSize;

        m_Icon.UpdateIcon(state.Icon, state.IconColor);
    }

    float Berp(float start, float end, float value)
    {
        value = m_AnimThumb.Evaluate(Mathf.Clamp01(value));
        return start + (end - start) * value;
    }

    Vector2 Berp(Vector2 start, Vector2 end, float value)
    {
        return new Vector2(
            Berp(start.x, end.x, value),
            Berp(start.y, end.y, value)
        );
    }

    Color Berp(Color start, Color end, float value)
    {
        return new Color(
            Berp(start.r, end.r, value),
            Berp(start.g, end.g, value),
            Berp(start.b, end.b, value),
            Berp(start.a, end.a, value)
        );
    }

    private void Update()
    {
        if (AnimationValue < 1f)
        {
            float time = AnimationValue;

            m_Track.color = Berp(StartState.TrackColor, TargetState.TrackColor, time);
            m_Track.OutlineColor = Berp(StartState.TrackOutlineColor, TargetState.TrackOutlineColor, time);

            m_Thumb.color = Berp(StartState.ThumbColor, TargetState.ThumbColor, time);
            m_Thumb.rectTransform.anchoredPosition = Berp(StartState.ThumbPosition, TargetState.ThumbPosition, time);

            var size = Vector2.Lerp(StartState.ThumbSize, TargetState.ThumbSize, time);
            size.x += (size.y * (1f - m_AnimThumbStretch.Evaluate(time))) * 0.5f;
            size.y *= m_AnimThumbStretch.Evaluate(time);
            m_Thumb.rectTransform.sizeDelta = size;

            m_Icon.UpdateIcon(
                StartState.Icon,
                Color.Lerp(StartState.IconColor, TargetState.IconColor, time)
            );

            AnimationValue += Time.deltaTime * m_AnimSpeed;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Selected = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        Selected = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Value = !Value;
        AnimateState();
    }
}
