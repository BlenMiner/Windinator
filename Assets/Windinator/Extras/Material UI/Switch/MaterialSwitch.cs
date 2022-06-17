using Riten.Windinator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Riten.Windinator.Audio;

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

    [Header("Sound")]

    [SerializeField] SoundLibrary m_TapSound;

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
    private float PressingValue = 0f;

    public bool Selected { get; private set; } = false;

    public bool Pressing { get; private set; } = false;

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
        float time = AnimationValue;

        PressingValue = Mathf.MoveTowards(PressingValue, Pressing ? 1 : 0, Time.deltaTime * 10f);

        var trackColor = Berp(StartState.TrackColor, TargetState.TrackColor, time);
        var trackOutline = Berp(StartState.TrackOutlineColor, TargetState.TrackOutlineColor, time);

        var thumbColor = Berp(StartState.ThumbColor, TargetState.ThumbColor, time);
        var position = Berp(StartState.ThumbPosition, TargetState.ThumbPosition, time);

        var size = Vector2.Lerp(StartState.ThumbSize, TargetState.ThumbSize, time);
        var scale = Vector3.one;

        if (Pressing) size = Vector2.Lerp(size, Vector2.one * ThumbMaxSize, PressingValue);

        scale.y *= m_AnimThumbStretch.Evaluate(time);

        var iconColor = Color.Lerp(StartState.IconColor, TargetState.IconColor, time);
        var icon = time > 0.5f ? TargetState.Icon : StartState.Icon;

        if (trackColor != m_Track.color ||
            trackOutline != m_Track.OutlineColor ||
            m_Thumb.color != thumbColor ||
            m_Thumb.rectTransform.anchoredPosition != position ||
            m_Thumb.rectTransform.sizeDelta != size ||
            m_Icon.Icon != icon ||
            m_Icon.IconColor != iconColor ||
            scale != m_Thumb.transform.localScale)
        {
            m_Track.color = trackColor;
            m_Track.OutlineColor = trackOutline;
            m_Thumb.color = thumbColor;
            m_Thumb.rectTransform.anchoredPosition = position;
            m_Thumb.rectTransform.sizeDelta = size;
            m_Thumb.rectTransform.localScale = scale;

            m_Icon.UpdateIcon(
                icon,
                iconColor
            );
        }

        if (AnimationValue != 1f)
        {
            AnimationValue += Time.deltaTime * m_AnimSpeed;

            if (AnimationValue > 1f)
                AnimationValue = 1f;
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
        Pressing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_TapSound?.PlayRandom();
        Value = !Value;
        AnimateState();
        Pressing = false;
    }
}
