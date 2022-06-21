using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleGraphic : SignedDistanceFieldGraphic
{
    [SerializeField] float m_roudness;
    [SerializeField, Range(0, 1)] float m_position;

    Material m_tri_material;
    
    public override Material defaultMaterial
    {
        get
        {
            if (m_tri_material == null)
                m_tri_material = new Material(Shader.Find("UI/Windinator/TriangleRenderer"));
            return m_tri_material;
        }
    } 

    public void SetRoundness(float roundness)
    {
        m_roudness = roundness;
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
        defaultMaterial.SetVector("_Roundness", new Vector4(m_roudness, m_position * 2, 0, 0));
    }
}
