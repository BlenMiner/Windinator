using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Toon PopUp Profile", menuName = "UI Element Profiles/Toon PopUp Profile", order = 1)]
public class ToonPopUpProfile : ScriptableObject
{
    #region PUBLIC_VARIABLES
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
    #endregion

    #region UNITY_CALLBACKS
    #endregion

    #region PUBLIC_METHODS
    #endregion

    #region PRIVATE_METHODS
    #endregion

    #region EVENT_HANDLERS
    #endregion

    #region CO-ROUTINES
    #endregion
}
