using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Toon Title Profile", menuName = "UI Element Profiles/Toon Title Profile", order = 1)]
public class ToonTitleProfile : ScriptableObject
{
    #region PUBLIC_VARIABLES
    public Color upperColor = Color.white;
    public Color lowerColor = Color.white;
    [Range(-180f, 180f)]
    public float gradientDirection;
    //
    [Space(20)]
    public Color textureColor = Color.white;
    //
    [Space(20)]
    public Color thicknessColor = Color.white;
    public float thickness;
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
    //
    [Space(20)]
    public Color textOutlineColor = Color.white;
    public float textOutlineWidth;
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
