using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonGraphic : SignedDistanceFieldGraphic
{
    [SerializeField] float m_roudness;
    [SerializeField] Vector2 point0 = new Vector2(0.5f, 1f);
    [SerializeField] Vector2 point1 = new Vector2(0f, 0f);
    [SerializeField] Vector2 point2 = new Vector2(1f, 0f);

    Material m_poly_material;
    
    public override Material defaultMaterial
    {
        get
        {
            if (m_poly_material == null)
                m_poly_material = new Material(Shader.Find("UI/Windinator/PolygonRenderer"));
            return m_poly_material;
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

        defaultMaterial.SetVectorArray("_Points", new List<Vector4>
        {
            point0,
            point1,
            point2
        });

        defaultMaterial.SetInt("_PointsCount", 3);
    }
}
