using System;
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

    [SerializeField] Color32 m_circleColor;

    [SerializeField] float m_circleRadius = 0f;

    [Header("Outline")]

    [SerializeField, Min(0f)] float m_outlineSize = 0f;

    [SerializeField] Color32 m_outlineColor = Color.black;

    [Header("Shadow")]

    [SerializeField, Min(0f)] float m_shadowSize = 0f;

    [SerializeField, Min(0f)] float m_shadowBlur = 0f;

    [SerializeField, Min(0f)] float m_shadowPower = 1f;

    [SerializeField] Color32 m_shadowColor = Color.black;

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
        set {
            m_texture = value; 
            SetMaterialDirty();
        }
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

    static Material RectangleRendererShader;

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateInstanciable();
    }

    private void LoadMaterial()
    {
        if (RectangleRendererShader == null)
        {
            RectangleRendererShader = new Material(Shader.Find("UI/Windinator/RectangleRenderer"));
            RectangleRendererShader.enableInstancing = true;
        }
        m_material = RectangleRendererShader;
    }

    public override Material defaultMaterial
    {
        get
        {
            if (m_material == null)
            {
                LoadMaterial();
            }
            return m_material;
        }
    }

    public Color CircleColor
    {
        get => m_circleColor;
        set
        {
            m_circleColor = value;
            SetVerticesDirty();
        }
    }

    public float CircleAlpha
    {
        get => m_circleAlpha;
        set
        {
            m_circleAlpha = value;
            SetVerticesDirty();
        }
    }

    public float CircleSize
    {
        get => m_circleRadius;
        set
        {
            m_circleRadius = value;
            SetVerticesDirty();
        }
    }

    public Color OutlineColor
    {
        get => m_outlineColor;
        set
        {
            m_outlineColor = value;
            SetVerticesDirty();
        }
    }

    public float OutlineSize
    {
        get => m_outlineSize;
        set
        {
            m_outlineSize = value;
            SetVerticesDirty();
        }
    }

    public Color ShadowColor
    {
        get => m_shadowColor;
        set
        {
            m_shadowColor = value;
            SetVerticesDirty();
        }
    }

    public float ShadowSize
    {
        get => m_shadowSize;
        set
        {
            m_shadowSize = value;
            SetVerticesDirty();
        }
    }

    public float ShadowBlur
    {
        get => m_shadowBlur;
        set
        {
            m_shadowBlur = value;
            SetVerticesDirty();
        }
    }

    public float ShadowPower
    {
        get => m_shadowPower;
        set
        {
            m_shadowPower = value;
            SetVerticesDirty();
        }
    }

    public Vector2 CirclePos { get => m_circlePos; set { m_circlePos = value; SetVerticesDirty();} }

    public Vector4 MaskRect { 
        get => m_maskRect; 
        set { 
            m_maskRect = value;
            SetMaterialDirty(); 
        } 
    }

    public Vector4 MaskOffset { get => m_maskOffset; set { 
            m_maskOffset = value;
            SetMaterialDirty(); 
        }  }

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

    private Vector2 m_sizeCached;

    private void UpdateShaderDimensions()
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        m_sizeCached.x = width;
        m_sizeCached.y = height;

        onMaterialUpdate?.Invoke(width, height);
    }

    public override void SetLayoutDirty()
    {
        base.SetLayoutDirty();
        UpdateShaderDimensions();
    }

    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();

        #if UNITY_EDITOR
        if (canvas != null && !Application.isPlaying)
        {
            canvas.additionalShaderChannels = 
                AdditionalCanvasShaderChannels.Normal | 
                AdditionalCanvasShaderChannels.Tangent | 
                AdditionalCanvasShaderChannels.TexCoord1 | 
                AdditionalCanvasShaderChannels.TexCoord2 | 
                AdditionalCanvasShaderChannels.TexCoord3;
        }
        #endif
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        UpdateShaderDimensions();
    }

    protected virtual Vector4 UpdateMeshData(float width, float height)
    {
        return default;
    }

    static readonly Vector2 ZERO = new Vector2(0, 0), UP = new Vector2(0, 1), ONE = new Vector2(1, 1), RIGHT = new Vector2(1, 0);

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        Vector3 pivot = new Vector3(
            rectTransform.pivot.x * width,
            rectTransform.pivot.y * height, 0);

        // Equal for all
        var uv1 = new Vector4(m_shadowSize, m_shadowBlur, 0, Margin);
        var uv2 = new Vector4(m_sizeCached.x, m_sizeCached.y, m_outlineSize, m_graphicAlphaMult);
        var uv3 = new Vector4(
            DecodeFloatRGBA(m_outlineColor),
            DecodeFloatRGBA(m_shadowColor),
            DecodeFloatRGBA(new Color32(m_circleColor.r, m_circleColor.g, m_circleColor.b, (byte)(m_circleAlpha * 255f))),
            m_circleRadius
        );

        var normal = new Vector3(
            CirclePos.x,
            CirclePos.y,
            0
        );

        float marginedWidth = width + Margin - pivot.x;
        float marginedHeight = height + Margin - pivot.y;
        float negativeMarginX = -Margin - pivot.x;
        float negativeMarginY = -Margin - pivot.y;

        var tangent = UpdateMeshData(width, height);

        vh.AddVert(
            new Vector3(negativeMarginX, negativeMarginY),
            color * LeftDownColor,
            ZERO,
            uv1, uv2, uv3, normal, tangent);

        vh.AddVert(
            new Vector3(negativeMarginX, marginedHeight),
            color * LeftUpColor,
            UP,
            uv1, uv2, uv3, normal, tangent);

        vh.AddVert(
            new Vector3(marginedWidth, marginedHeight),
            color * RightUpColor,
            ONE,
            uv1, uv2, uv3, normal, tangent);

        vh.AddVert(
            new Vector3(marginedWidth, negativeMarginY),
            color * RightDownColor,
            RIGHT,
            uv1, uv2, uv3, normal, tangent);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    float DecodeFloatRGBA(Color32 color)
    {
        int result = color.r;
        result |= color.g << 8;
        result |= color.b << 16;
        result |= Mathf.Max(0, (color.a / 2) - 1) << 24;

        return Int32BitsToSingle(result);
    }

    public static unsafe float Int32BitsToSingle(int value) {
        return *(float*)(&value);
    }

    public override void SetMaterialDirty()
    {
        if (m_material == null) LoadMaterial();

        base.SetMaterialDirty();

        UpdateInstanciable();
        UpdateShaderDimensions();

        m_material.SetTexture("_MainTex", mainTexture);

        m_material.SetVector("_MaskRect", MaskRect);
        m_material.SetVector("_MaskOffset", MaskOffset);
    }

    void UpdateInstanciable()
    {
        if (m_material == null) LoadMaterial();

        bool canInstance = mainTexture == null && MaskRect.z == 0 && MaskRect.w == 0 && MaskOffset == default;
        bool isInstance = m_material == RectangleRendererShader;

        if (canInstance != isInstance)
        {
            m_material = canInstance ? RectangleRendererShader : Instantiate(RectangleRendererShader);
            SetAllDirty();
        }
    }
}
