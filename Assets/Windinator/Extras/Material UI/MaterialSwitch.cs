using Riten.Windinator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Riten.Windinator.Audio;
using UnityEngine.Events;
using Riten.Windinator.Animation;

public class MaterialSwitch : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("General")]

    public bool Value = false;

    [SerializeField, Searchable] MaterialIcons m_IconTrue;
    [SerializeField, Searchable] MaterialIcons m_IconFalse;

    public MaterialIcons IconOn
    {
        get => m_IconTrue;
        set
        {
            m_IconTrue = value;
            SnapState();
        }
    }

    public MaterialIcons IconOff
    {
        get => m_IconFalse;
        set
        {
            m_IconFalse = value;
            SnapState();
        }
    }

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

    [Header("Sound")]

    [SerializeField] SoundLibrary m_TapSound;

    const float Offset = 10f;
    const float ThumbSize = 24f;
    const float ThumbMinSize = 16f;
    const float ThumbMaxSize = 28f;

    public UnityEvent<bool> onValueChanged;

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

    private AnimationState TargetState;
    private float PressingValue = 0f;

    public bool Selected { get; private set; } = false;

    public bool Pressing { get; private set; } = false;

    bool OldValue;

    public static MaterialIcons Lerp(MaterialIcons start, MaterialIcons end, float value)
    {
        return value > 0.5f ? end : start;
    }

    VarAnimator<Color> TrackColorVar;
    VarAnimator<Color> TrackOutlineColorVar;
    VarAnimator<Color> ThumbColorVar;
    VarAnimator<Color> IconColorVar;

    VarAnimator<Vector2> ThumbSizeVar;
    VarAnimator<Vector2> ThumbPosVar;

    VarAnimator<MaterialIcons> IconVar;

    public void Setup()
    {
        if (TrackColorVar != null) return;

        TrackColorVar = new VarAnimator<Color>(VarAnimator<Color>.Lerp, v => m_Track.color = v, m_AnimSpeed, m_AnimThumb);
        TrackOutlineColorVar = new VarAnimator<Color>(VarAnimator<Color>.Lerp, v => m_Track.OutlineColor = v, m_AnimSpeed, m_AnimThumb);
        ThumbColorVar = new VarAnimator<Color>(VarAnimator<Color>.Lerp, v => m_Thumb.color = v, m_AnimSpeed, m_AnimThumb);
        IconColorVar = new VarAnimator<Color>(VarAnimator<Color>.Lerp, v => m_Icon.IconColor = v, m_AnimSpeed, m_AnimThumb);
        ThumbSizeVar = new VarAnimator<Vector2>(VarAnimator<Vector2>.Lerp, v => m_Thumb.rectTransform.sizeDelta = v, m_AnimSpeed, m_AnimThumb);
        ThumbPosVar = new VarAnimator<Vector2>(VarAnimator<Vector2>.Lerp, v => m_Thumb.rectTransform.anchoredPosition = v, m_AnimSpeed, m_AnimThumb);
        IconVar = new VarAnimator<MaterialIcons>(Lerp, v => m_Icon.Icon = v, m_AnimSpeed, m_AnimThumb);

        ThumbSizeVar.SetModifier((value, time) =>
        {
            value.y *= m_AnimThumbStretch.Evaluate(time);
            value = Vector2.Lerp(value, Vector2.one * ThumbMaxSize, PressingValue);
            return value;
        });
    }

    private void Awake()
    {
        OldValue = Value;
        SnapState();
    }

    private void OnValidate()
    {
        SnapState();
    }

    AnimationState GetTargetState()
    {
        return new AnimationState
        {
            TrackColor = (Value ? m_TrackSelected : m_TrackUnselected).ToColor(this),
            TrackOutlineColor = (Value ? m_OutlineSelected : m_OutlineUnselected).ToColor(this),

            ThumbColor = (Value ? m_ThumbSelected : m_ThumbUnselected).ToColor(this),
            ThumbPosition = new Vector2(Offset * (Value ? 1 : -1), 0),
            ThumbSize = Vector2.one * (Value ? ThumbSize : ThumbMinSize),

            Icon = (Value ? m_IconTrue : m_IconFalse),
            IconColor = (Value ? m_IconSelected : m_IconUnselected).ToColor(this)
        };
    }

    public void SnapState()
    {
        Setup();

        TargetState = GetTargetState();

        TrackColorVar.SnapToTarget(TargetState.TrackColor);
        TrackOutlineColorVar.SnapToTarget(TargetState.TrackOutlineColor);
        ThumbColorVar.SnapToTarget(TargetState.ThumbColor);
        IconColorVar.SnapToTarget(TargetState.IconColor);
        ThumbSizeVar.SnapToTarget(TargetState.ThumbSize);
        ThumbPosVar.SnapToTarget(TargetState.ThumbPosition);
        IconVar.SnapToTarget(TargetState.Icon);
    }

    void AnimateState()
    {
        TargetState = GetTargetState();

        TrackColorVar.AnimateToTarget(TargetState.TrackColor);
        TrackOutlineColorVar.AnimateToTarget(TargetState.TrackOutlineColor);
        ThumbColorVar.AnimateToTarget(TargetState.ThumbColor);
        IconColorVar.AnimateToTarget(TargetState.IconColor);
        ThumbSizeVar.AnimateToTarget(TargetState.ThumbSize);
        ThumbPosVar.AnimateToTarget(TargetState.ThumbPosition);
        IconVar.AnimateToTarget(TargetState.Icon);
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

    private void Update()
    {
        PressingValue = Mathf.MoveTowards(PressingValue, Pressing ? 1 : 0, Time.deltaTime * 10f);
        float delta = Time.deltaTime;

        TrackColorVar.Update(delta);
        TrackOutlineColorVar.Update(delta);
        ThumbColorVar.Update(delta);
        IconColorVar.Update(delta);
        ThumbSizeVar.Update(delta);
        ThumbPosVar.Update(delta);
        IconVar.Update(delta);

        if (OldValue != Value)
        {
            OldValue = Value;
            onValueChanged?.Invoke(Value);
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
