using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignedDistanceFieldGraphic : MaskableGraphic
{
    [Header("Graphic")]

    [SerializeField] Texture m_texture;

    [SerializeField] float m_graphicBlur = 0f;

    [Header("Circle")]

    [SerializeField, Range(0f, 1f)] float m_circleAlpha = 0.2f;

    [SerializeField] Vector2 m_circlePos;

    [SerializeField] Color m_circleColor;

    [SerializeField] float m_circleRadius = 0f;

    [Header("Outline")]

    [SerializeField, Min(0f)] float m_outlineSize = 0f;

    [SerializeField] Color m_outlineColor = Color.black;

    [Header("Shadow")]

    [SerializeField, Min(0f)] float m_shadowSize = 0f;

    [SerializeField, Min(0f)] float m_shadowBlur = 0f;

    [SerializeField, Min(0f)] float m_shadowPower = 1f;

    [SerializeField] Color m_shadowColor = Color.black;

    [Header("Mask")]

    [SerializeField] Vector4 m_maskRect;

    private Material m_material;

    private float m_extraMargin => Mathf.Max(m_outlineSize, m_shadowSize) + 1;

    protected event Action<float, float> onMaterialUpdate;

    public override Texture mainTexture => m_texture;

    public override Material defaultMaterial
    {
        get
        {
            if (m_material == null)
                m_material = new Material(Shader.Find("Unlit/RectangleRenderer"));
            return m_material;
        }
    }

    public Color CircleColor;

    public Color OutlineColor
    {
        get => m_outlineColor;
        set
        {
            m_outlineColor = value;
            SetAllDirty();
        }
    }

    public void SetCircle(Vector2 pos, Color color, float size, float alpha)
    {
        m_circleAlpha = alpha;
        m_circleRadius = size;
        m_circlePos = pos;
        m_circleColor = color;

        SetAllDirty();
    }

    public void SetOutline(Color color, float size)
    {
        m_outlineSize = size;
        m_outlineColor = color;

        SetAllDirty();
    }

    public void SetMask(Vector4 mask)
    {
        m_maskRect = mask;
        SetAllDirty();
    }

    public void SetShadow(Color color, float size, float blur, float power = 1f)
    {
        m_shadowColor = color;
        m_shadowSize = size;
        m_shadowBlur = blur;
        m_shadowPower = power;

        SetAllDirty();
    }

    private void UpdateShaderDimensions()
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        defaultMaterial.SetVector("_Size", new Vector2(
            width,
            height)
        );

        onMaterialUpdate?.Invoke(width, height);
    }

    public override void SetLayoutDirty()
    {
        base.SetLayoutDirty();
        UpdateShaderDimensions();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        UpdateShaderDimensions();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        Vector3 pivot = new Vector3(
            rectTransform.pivot.x * width,
            rectTransform.pivot.y * height, 0);

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(-m_extraMargin, -m_extraMargin) - pivot;
        vertex.uv0 = new Vector2(0, 0);
        vh.AddVert(vertex);

        vertex.position = new Vector3(-m_extraMargin, height + m_extraMargin) - pivot;
        vertex.uv0 = new Vector2(0, 1);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width + m_extraMargin, height + m_extraMargin) - pivot;
        vertex.uv0 = new Vector2(1, 1);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width + m_extraMargin, -m_extraMargin) - pivot;
        vertex.uv0 = new Vector2(1, 0);
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    public override void SetMaterialDirty()
    {
        base.SetMaterialDirty();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        UpdateShaderDimensions();

        defaultMaterial.SetTexture("_MainTex", mainTexture);

        defaultMaterial.SetFloat("_GraphicBlur", m_graphicBlur);

        defaultMaterial.SetFloat("_OutlineSize", m_outlineSize);
        defaultMaterial.SetColor("_OutlineColor", m_outlineColor);

        defaultMaterial.SetVector("_MaskRect", m_maskRect);
        defaultMaterial.SetVector("_CirclePos", m_circlePos);
        defaultMaterial.SetVector("_CircleColor", m_circleColor);
        defaultMaterial.SetFloat("_CircleRadius", m_circleRadius);
        defaultMaterial.SetFloat("_CircleAlpha", m_circleAlpha);

        defaultMaterial.SetFloat("_ShadowSize", m_shadowSize);
        defaultMaterial.SetFloat("_ShadowBlur", m_shadowBlur);
        defaultMaterial.SetFloat("_ShadowPow", m_shadowPower);
        defaultMaterial.SetColor("_ShadowColor", m_shadowColor);

        defaultMaterial.SetFloat("_Padding", m_extraMargin);
    }
}
