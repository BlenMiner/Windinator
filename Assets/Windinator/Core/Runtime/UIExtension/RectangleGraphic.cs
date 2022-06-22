using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class RectangleGraphic : SignedDistanceFieldGraphic
{
    [Header("Shape")]

    [SerializeField, Min(0f)] bool m_useMaxRoundness = false;

    [SerializeField, Min(0f)] bool m_uniformRoundness = false;

    [SerializeField] Vector4 m_roudnessInPixels;

    public Vector4 Roundness {get => m_roudnessInPixels; set {
        m_roudnessInPixels = value;
        SetMaterialDirty();
    }}

    public bool UniformRoundness {get => m_uniformRoundness; set {
        m_uniformRoundness = value;
        SetMaterialDirty();
    }}

    public bool MaxRoundess {get => m_useMaxRoundness; set {
        m_useMaxRoundness = value;
        SetMaterialDirty();
    }}

    public void SetRoundness(Vector4 roundness)
    {
        m_roudnessInPixels = roundness;
        SetMaterialDirty();
    }

    public void SetUniformRoundness(bool value)
    {
        m_uniformRoundness = value;
        SetMaterialDirty();
    }

    public void SetMaxRoundness(bool value)
    {
        m_useMaxRoundness = value;
        SetMaterialDirty();
    }

    override protected void OnEnable()
    {
        onMaterialUpdate += UpdateShaderRoundness;
        base.OnEnable();
    }

    override protected void OnDisable()
    {
        onMaterialUpdate -= UpdateShaderRoundness;
        base.OnDisable();
    }

    void UpdateShaderRoundness(float width, float height)
    {
        float maxRoundedValue = Mathf.Min(width, height) * 0.5f;

        Vector4 maxRounded = new Vector4(maxRoundedValue, maxRoundedValue, maxRoundedValue, maxRoundedValue);

        Vector4 uniformRoundness = new Vector4(
            m_roudnessInPixels.x, 
            m_roudnessInPixels.x,
            m_roudnessInPixels.x,
            m_roudnessInPixels.x
        );

        Vector4 roudness = m_uniformRoundness ? uniformRoundness : m_roudnessInPixels * 0.5f;

        roudness.x = Mathf.Min(roudness.x, maxRoundedValue);
        roudness.y = Mathf.Min(roudness.y, maxRoundedValue);
        roudness.z = Mathf.Min(roudness.z, maxRoundedValue);
        roudness.w = Mathf.Min(roudness.w, maxRoundedValue);

        defaultMaterial.SetVector("_Roundness", m_useMaxRoundness ? maxRounded : roudness);
    }
}
