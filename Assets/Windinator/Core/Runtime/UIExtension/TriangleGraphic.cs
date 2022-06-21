using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleGraphic : SignedDistanceFieldGraphic
{
    [SerializeField] float m_roudness;
    [SerializeField] Vector2 point0 = new Vector2(0.5f, 1f);
    [SerializeField] Vector2 point1 = new Vector2(0f, 0f);
    [SerializeField] Vector2 point2 = new Vector2(1f, 0f);

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
        defaultMaterial.SetVector("_Roundness", new Vector4(m_roudness, 0, 0, 0));
        defaultMaterial.SetVector("_Point0", point0);
        defaultMaterial.SetVector("_Point1", point1);
        defaultMaterial.SetVector("_Point2", point2);
    }
}