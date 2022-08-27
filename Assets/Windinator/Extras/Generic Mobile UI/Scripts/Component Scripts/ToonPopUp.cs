using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ToonPopUp : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    public bool autoUpdateWithProfile;
    public ToonPopUpProfile profile;
    //
    [Space(20)]
    public Color upperColor = Color.white;
    public Color lowerColor = Color.white;
    [Range(-180f, 180f)]
    public float gradientDirection;
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
    public Color innerGlowColor = Color.white;
    public Color innerCardGradientUpperColor = Color.white;
    public Color innerCardGradientLowerColor = Color.white;
    [Range(-180f, 180f)]
    public float innerCardGradientDirection;
    //
    [Space(20)]
    public Color innerCardOutlineColor = Color.white;
    public float innerCardOutlineWidth;
    //
    [Space(20)]
    public Color closeIconColor = Color.white;
    #endregion

    #region PRIVATE_VARIABLES
    private UIGradient uIGradient;

    private Image innerGlow;
    private Image closeIcon;
    private Image closeInnerGlow;
    private Image closeOuter;

    private DropShadow shadow;
    private Outline outline;

    private UIGradient innerCardGradient;
    private Outline innerCardOutline;
    private Outline closeOutline;
    private Outline closeIconOutline;

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
        //
        shadowColor = profile.shadowColor;
        shadowDistance = profile.shadowDistance;
        shadowSpread = profile.shadowSpread;
        //
        outlineColor = profile.outlineColor;
        outlineWidth = profile.outlineWidth;
        //
        innerGlowColor = profile.innerGlowColor;
        innerCardGradientUpperColor = profile.innerCardGradientUpperColor;
        innerCardGradientLowerColor = profile.innerCardGradientLowerColor;
        innerCardGradientDirection = profile.innerCardGradientDirection;
        //
        innerCardOutlineColor = profile.innerCardOutlineColor;
        innerCardOutlineWidth = profile.innerCardOutlineWidth;
        //
        closeIconColor = profile.closeIconColor;
    }

    public void UpdateValues()
    {
        innerGlow.color = innerGlowColor;

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
        closeOutline.effectDistance = tempVec2;

        innerCardGradient.m_color1 = innerCardGradientUpperColor;
        innerCardGradient.m_color2 = innerCardGradientLowerColor;
        innerCardGradient.m_angle = innerCardGradientDirection;

        innerCardOutline.effectColor = innerCardOutlineColor;
        tempVec2.x = innerCardOutlineWidth;
        tempVec2.y = -innerCardOutlineWidth;
        innerCardOutline.effectDistance = tempVec2;

        closeIcon.color = closeIconColor;
        closeInnerGlow.color = Color.Lerp(upperColor,outlineColor,0.5f);
        closeOuter.color = upperColor;
        closeOutline.effectColor = outlineColor;
        closeIconOutline.effectColor = outlineColor;
        tempVec2.x = outlineWidth / 2f;
        tempVec2.y = -outlineWidth / 2f;
        closeIconOutline.effectDistance = tempVec2;
    }

    [ContextMenu("Auto Assign")]
    public void AutoAssign()
    {
        innerGlow = FindChild("Image - Card Inner IG", transform).GetComponent<Image>();
        closeIcon = FindChild("Image - Close", transform).GetComponent<Image>();
        closeInnerGlow = FindChild("Image - Close Inner", transform).GetComponent<Image>();
        innerCardGradient = FindChild("Image - Card Inner", transform).GetComponent<UIGradient>();
        uIGradient = GetComponent<UIGradient>();
        shadow = GetComponent<DropShadow>();
        outline = GetComponent<Outline>();
        innerCardOutline = innerCardGradient.GetComponent<Outline>();
        closeOuter = FindChild("Image - Close Outer", transform).GetComponent<Image>();
        closeOutline = closeOuter.GetComponent<Outline>();
        closeIconOutline = closeIcon.GetComponent<Outline>();
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
