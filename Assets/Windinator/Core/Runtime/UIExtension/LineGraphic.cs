using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGraphic : SignedDistanceFieldGraphic
{
    [SerializeField] float m_roudness;
    [SerializeField] StaticArray<Vector4> points = new StaticArray<Vector4>(1023);

    [SerializeField] float m_size = 1;

    Material m_poly_material;

    public StaticArray<Vector4> Points
    {
        get => points;
        set {
            points = value;
        }
    }

    public override Material defaultMaterial
    {
        get
        {
            if (m_poly_material == null)
                m_poly_material = new Material(Shader.Find("UI/Windinator/LineRenderer"));
            return m_poly_material;
        }
    }

    public override float ExtraMargin => m_size;

    public float Roundness {get => m_roudness; set {
        m_roudness = value;
        SetMaterialDirty();
    }}

    public float Size {get => m_size; set {
        m_size = value;
        SetMaterialDirty();
    }}

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

        defaultMaterial.SetVectorArray("_Points", points.Array);
        defaultMaterial.SetInt("_PointsCount", points.Length);
        defaultMaterial.SetFloat("_LineThickness", m_size);
    }
}
