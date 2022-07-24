using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignedDistanceFieldGraphic : MaskableGraphic
{
    [Header("Graphic")]

    [SerializeField] Texture m_texture;

    [SerializeField, Range(0f, 1f)] float m_graphicAlphaMult = 1f;

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

    [SerializeField] Vector4 m_maskOffset;

    Material m_material;

    [Header("Gradient")]

    [SerializeField] Color m_leftUpColor = Color.white;
    [SerializeField] Color m_rightUpColor = Color.white;
    [SerializeField] Color m_rightDownColor = Color.white;
    [SerializeField] Color m_leftDownColor = Color.white;

    public Texture Texture {
        get => m_texture;
        set {m_texture = value; SetMaterialDirty();}
    }
     
    public virtual float Margin => Mathf.Max(m_outlineSize, m_shadowSize) + 2 + ExtraMargin;

    public virtual float ExtraMargin => 0;

    protected event Action<float, float> onMaterialUpdate;

    public override Texture mainTexture => m_texture;

    public float Alpha {get => m_graphicAlphaMult; set
    {
        m_graphicAlphaMult = value;
        SetAllDirty();

        if (value == 0) canvasRenderer.cull = true;
        else            canvasRenderer.cull = false;
    }}

    public override Material defaultMaterial
    {
        get
        {
            if (m_material == null)
                m_material = new Material(Shader.Find("UI/Windinator/RectangleRenderer"));
            return m_material;
        }
    }

    public Color CircleColor
    {
        get => m_circleColor;
        set
        {
            m_circleColor = value;
            SetMaterialDirty();
        }
    }

    public float CircleAlpha
    {
        get => m_circleAlpha;
        set
        {
            m_circleAlpha = value;
            SetMaterialDirty();
        }
    }

    public float CircleSize
    {
        get => m_circleRadius;
        set
        {
            m_circleRadius = value;
            SetMaterialDirty();
        }
    }

    public Color OutlineColor
    {
        get => m_outlineColor;
        set
        {
            m_outlineColor = value;
            SetMaterialDirty();
        }
    }

    public float OutlineSize
    {
        get => m_outlineSize;
        set
        {
            m_outlineSize = value;
            SetAllDirty();
        }
    }

    public Color ShadowColor
    {
        get => m_shadowColor;
        set
        {
            m_shadowColor = value;
            SetMaterialDirty();
        }
    }

    public float ShadowSize
    {
        get => m_shadowSize;
        set
        {
            m_shadowSize = value;
            SetAllDirty();
        }
    }

    public float ShadowBlur
    {
        get => m_shadowBlur;
        set
        {
            m_shadowBlur = value;
            SetMaterialDirty();
        }
    }

    public float ShadowPower
    {
        get => m_shadowPower;
        set
        {
            m_shadowPower = value;
            SetMaterialDirty();
        }
    }

    public Vector2 CirclePos { get => m_circlePos; set { m_circlePos = value; SetMaterialDirty();} }

    public Vector4 MaskRect { get => m_maskRect; set { m_maskRect = value; SetMaterialDirty(); } }

    public Vector4 MaskOffset { get => m_maskOffset; set { m_maskOffset = value; SetMaterialDirty(); } }

    public Color LeftUpColor { get => m_leftUpColor; set => m_leftUpColor = value; }
    
    public Color RightUpColor { get => m_rightUpColor; set => m_rightUpColor = value; }

    public Color RightDownColor { get => m_rightDownColor; set => m_rightDownColor = value; }

    public Color LeftDownColor { get => m_leftDownColor; set => m_leftDownColor = value; }

    public void SetCircle(Vector2 pos, Color color, float size, float alpha)
    {
        m_circleAlpha = alpha;
        m_circleRadius = size;
        CirclePos = pos;
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
        MaskRect = mask;
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

        vertex.position = new Vector3(-Margin, -Margin) - pivot;
        vertex.uv0 = new Vector2(0, 0);
        vh.AddVert(vertex);

        vertex.position = new Vector3(-Margin, height + Margin) - pivot;
        vertex.uv0 = new Vector2(0, 1);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width + Margin, height + Margin) - pivot;
        vertex.uv0 = new Vector2(1, 1);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width + Margin, -Margin) - pivot;
        vertex.uv0 = new Vector2(1, 0);
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    public override void SetMaterialDirty()
    {
        base.SetMaterialDirty();

        UpdateShaderDimensions();

        defaultMaterial.SetTexture("_MainTex", mainTexture);
        defaultMaterial.SetFloat("_Alpha", m_graphicAlphaMult);

        defaultMaterial.SetFloat("_GraphicBlur", 0f);

        var outlineCol = m_outlineColor;

        defaultMaterial.SetFloat("_OutlineSize", m_outlineSize);
        defaultMaterial.SetColor("_OutlineColor", outlineCol);

        var circleCol = m_circleColor;

        defaultMaterial.SetVector("_MaskRect", MaskRect);
        defaultMaterial.SetVector("_MaskOffset", MaskOffset);
        defaultMaterial.SetVector("_CirclePos", CirclePos);
        defaultMaterial.SetVector("_CircleColor", circleCol);
        defaultMaterial.SetFloat("_CircleRadius", m_circleRadius);
        defaultMaterial.SetFloat("_CircleAlpha", m_circleAlpha);

        var shadowCol = m_shadowColor;

        defaultMaterial.SetFloat("_ShadowSize", m_shadowSize);
        defaultMaterial.SetFloat("_ShadowBlur", m_shadowBlur);
        defaultMaterial.SetFloat("_ShadowPow", m_shadowPower);
        defaultMaterial.SetColor("_ShadowColor", shadowCol);

        defaultMaterial.SetFloat("_Padding", Margin);

        defaultMaterial.SetColor("_LU", LeftUpColor);
        defaultMaterial.SetColor("_RU", RightUpColor);
        defaultMaterial.SetColor("_LD", LeftDownColor);
        defaultMaterial.SetColor("_RD", RightDownColor);
    }
}
