using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ToonButton : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    public bool autoUpdateWithProfile;
    public ToonButtonProfile profile;
    //
    [Space(20)]
    public Color upperColor = Color.white;
    public Color lowerColor = Color.white;
    [Range(-180f, 180f)]
    public float gradientDirection;
    //
    [Space(20)]
    [Range(0f, 1f)]
    public float innerShineOpacity = 0.25f;
    [Range(0f, 1f)]
    public float cornerShineOpacity = 1f;
    //
    [Space(20)]
    public Color shadowColor = Color.white;
    public Vector2 shadowDistance;
    public Vector2 shadowSpread;
    //
    [Space(20)]
    public Color outlineColor = Color.white;
    public float outlineWidth;
    //
    [Space(20)]
    public string textString = "Button";
    public Color textGradientUpperColor = Color.white;
    public Color textGradientLowerColor = Color.white;
    [Range(-180f, 180f)]
    public float textGradientDirection;
    //
    [Space(20)]
    public Color textShadowColor = Color.white;
    public Vector2 textShadowDistance;
    public Vector2 textShadowSpread;
    //
    [Space(20)]
    public Color textOutlineColor = Color.white;
    public float textOutlineWidth;
    #endregion

    #region PRIVATE_VARIABLES
    private Text text;
    private UIGradient uIGradient;

    private Image innerShine;
    private Image cornerShineLeft;
    private Image cornerShineRight;

    private DropShadow shadow;
    private Outline outline;

    private UIGradient textGradient;
    private DropShadow textShadow;
    private Outline textOutline;

    private Color tempColor;
    private Vector2 tempVec2;

    private bool inited;

    #endregion

    #region UNITY_CALLBACKS
    private void OnEnable()
    {
        if (profile != null)
        {
            FetchFromProfile();
        }
        AutoAssign();
        UpdateValues();
    }

    private void OnGUI()
    {
        if (autoUpdateWithProfile)
        {
            if (profile != null)
            {
                FetchFromProfile();
                UpdateValues();
            }
        }
    }

    private void OnValidate()
    {
        if (!inited)
        {
            AutoAssign();
            inited = true;
        }
        if (autoUpdateWithProfile)
        {
            if (profile != null)
            {
                FetchFromProfile();
            }
        }
        UpdateValues();
    }
    #endregion

    #region PUBLIC_METHODS
    public void FetchFromProfile()
    {
        upperColor = profile.upperColor;
        lowerColor = profile.lowerColor;
        gradientDirection = profile.gradientDirection;
        innerShineOpacity = profile.innerShineOpacity;
        cornerShineOpacity = profile.cornerShineOpacity;
        //
        shadowColor = profile.shadowColor;
        shadowDistance = profile.shadowDistance;
        shadowSpread = profile.shadowSpread;
        //
        outlineColor = profile.outlineColor;
        outlineWidth = profile.outlineWidth;
        //
        textString = profile.textString;
        textGradientUpperColor = profile.textGradientUpperColor;
        textGradientLowerColor = profile.textGradientLowerColor;
        textGradientDirection = profile.textGradientDirection;
        //
        textShadowColor = profile.textShadowColor;
        textShadowDistance = profile.textShadowDistance;
        textShadowSpread = profile.textShadowSpread;
        //
        textOutlineColor = profile.textOutlineColor;
        textOutlineWidth = profile.textOutlineWidth;
    }

    public void UpdateValues()
    {
        tempColor = innerShine.color;
        tempColor.a = innerShineOpacity;
        innerShine.color = tempColor;

        tempColor = cornerShineLeft.color;
        tempColor.a = cornerShineOpacity;
        cornerShineLeft.color = tempColor;

        tempColor = cornerShineRight.color;
        tempColor.a = cornerShineOpacity;
        cornerShineRight.color = tempColor;

        uIGradient.m_color1 = upperColor;
        uIGradient.m_color2 = lowerColor;
        uIGradient.m_angle = gradientDirection;

        shadow.effectColor = shadowColor;
        shadow.shadowSpread = shadowSpread;
        shadow.EffectDistance = shadowDistance;

        outline.effectColor = outlineColor;
        tempVec2.x = outlineWidth;
        tempVec2.y = -outlineWidth;
        outline.effectDistance = tempVec2;

        text.text = textString;
        textGradient.m_color1 = textGradientUpperColor;
        textGradient.m_color2 = textGradientLowerColor;
        textGradient.m_angle = textGradientDirection;

        textShadow.effectColor = textShadowColor;
        textShadow.shadowSpread = textShadowSpread;
        textShadow.EffectDistance = textShadowDistance;

        textOutline.effectColor = textOutlineColor;
        tempVec2.x = textOutlineWidth;
        tempVec2.y = -textOutlineWidth;
        textOutline.effectDistance = tempVec2;
    }

    [ContextMenu("Auto Assign")]
    public void AutoAssign()
    {
        innerShine = FindChild("Image - Button Inner", transform).GetComponent<Image>();
        cornerShineLeft = FindChild("Image - Button Shine Small", transform).GetComponent<Image>();
        cornerShineRight = FindChild("Image - Button Shine Big", transform).GetComponent<Image>();
        text = FindChild("Text - Button Green Text", transform).GetComponent<Text>();
        uIGradient = GetComponent<UIGradient>();
        shadow = GetComponent<DropShadow>();
        outline = GetComponent<Outline>();
        textGradient = text.GetComponent<UIGradient>();
        textOutline = text.GetComponent<Outline>();
        textShadow = text.GetComponent<DropShadow>();
    }
    #endregion

    #region PRIVATE_METHODS
    private Transform FindChild(string objectToFind, Transform parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == objectToFind)
            {
                return children[i];
            }
        }
        return null;
    }
    #endregion

    #region EVENT_HANDLERS
    #endregion

    #region CO-ROUTINES
    #endregion
}
