using UnityEngine;
using UnityEngine.UI;

public enum DrawOperation
{
    Union = 0,
    Substract = 1,
    Intersect = 2
}

public class CanvasGraphic : SignedDistanceFieldGraphic
{
    RenderTexture m_buffer, m_backBuffer;

    RenderTexture m_finalBuffer;

    Material m_canvas_material, m_drawLine, m_drawRect, m_drawCircle, m_clearCircle;

    RenderTexture CurrentBuffer => m_useBackBuffer ? m_backBuffer : m_buffer;

    RenderTexture BackBuffer => m_useBackBuffer ? m_buffer : m_backBuffer;

    [SerializeField] float m_margin;

    public override float Margin => m_margin;

    public void SetMargin(float margin)
    {
        if (margin < 0) margin = 0;
        m_margin = margin;
    }

    private void SwitchBuffers()
    {
        m_useBackBuffer = !m_useBackBuffer;
    }

    bool m_useBackBuffer = false;

    public override Material defaultMaterial
    {
        get
        {
            if (m_canvas_material == null)
                m_canvas_material = new Material(Shader.Find("UI/Windinator/CanvasProceduralRenderer"));
            return m_canvas_material;
        }
    }

    Material DrawCircleOp
    {
        get
        {
            if (m_drawCircle == null)
                m_drawCircle = new Material(Shader.Find("UI/Windinator/DrawCircle"));
            return m_drawCircle;
        }
    }

    Material ClearCircleOp
    {
        get
        {
            if (m_clearCircle == null)
                m_clearCircle = new Material(Shader.Find("UI/Windinator/ClearOp"));
            return m_clearCircle;
        }
    }

    Material DrawLineOp
    {
        get
        {
            if (m_drawLine == null)
                m_drawLine = new Material(Shader.Find("UI/Windinator/DrawLine"));
            return m_drawLine;
        }
    }

    Material DrawRectOp
    {
        get
        {
            if (m_drawRect == null)
                m_drawRect = new Material(Shader.Find("UI/Windinator/DrawRect"));
            return m_drawRect;
        }
    }
    
    override protected void OnEnable()
    {
        onMaterialUpdate += UpdateShader;
        base.OnEnable();
    }

    override protected void OnDisable()
    {
        onMaterialUpdate -= UpdateShader;
        base.OnDisable();
    }

    Vector2 m_size;

    public Vector2 Size => m_size;

    void UpdateShader(float width, float height)
    {
        m_size = new Vector2(width, height);

        int w = Mathf.CeilToInt(width);
        int h = Mathf.CeilToInt(height);

        if (w <= 0 || h <= 0) return;

        if (m_buffer == null || m_buffer.width != w || m_buffer.height != h)
        {
            if (m_buffer != null && m_buffer.IsCreated())
                m_buffer.Release();

            if (m_backBuffer != null && m_backBuffer.IsCreated())
                m_backBuffer.Release();

            m_buffer = new RenderTexture(w, h, 1, RenderTextureFormat.ARGBFloat);
            m_backBuffer = new RenderTexture(m_buffer);
            m_finalBuffer = new RenderTexture(m_buffer);

            m_buffer.filterMode = FilterMode.Bilinear;
            m_backBuffer.filterMode = FilterMode.Bilinear;
            m_finalBuffer.filterMode = FilterMode.Bilinear;

            m_buffer.Create();
            m_backBuffer.Create();
            m_finalBuffer.Create();

            Texture = m_finalBuffer;

            SwitchBuffers();
        }
    }

    public void Begin()
    {
        Graphics.Blit(CurrentBuffer, BackBuffer, ClearCircleOp);
        SwitchBuffers();
    }

    void FeedMaterialBasics(Material mat, DrawOperation operation, float blend)
    {
        mat.SetTexture("_MainTexture", CurrentBuffer);
        mat.SetFloat("_Union", blend);
        mat.SetVector("_Size", m_size);
        mat.SetInt("_Operation", (int)operation);
        mat.SetFloat("_Padding", Margin);
    }

    public void DrawCircle(Vector2 position, float size, float blend = 0f, DrawOperation op = DrawOperation.Union)
    {
        FeedMaterialBasics(DrawCircleOp, op, blend);

        DrawCircleOp.SetVector("_OpPosition", position);
        DrawCircleOp.SetFloat("_OpSize", size);

        Graphics.Blit(CurrentBuffer, BackBuffer, DrawCircleOp);
        SwitchBuffers();
    }

    public void DrawLine(Vector2 a, Vector2 b, float thickness = 2f, float blend = 0f, DrawOperation op = DrawOperation.Union)
    {
        FeedMaterialBasics(DrawLineOp, op, blend);

        DrawLineOp.SetVector("_Point0", a);
        DrawLineOp.SetVector("_Point1", b);
        DrawLineOp.SetFloat("_LineThickness", thickness);

        Graphics.Blit(CurrentBuffer, BackBuffer, DrawLineOp);
        SwitchBuffers();
    }

    public void DrawRect(Vector2 position, Vector2 size, Vector4 roundness = default, float blend = 0f, DrawOperation op = DrawOperation.Union)
    {
        FeedMaterialBasics(DrawRectOp, op, blend);

        DrawRectOp.SetVector("_OpPosition", position);
        DrawRectOp.SetVector("_OpSize", size * 0.5f);
        DrawRectOp.SetVector("_OpRoundness", roundness);

        Graphics.Blit(CurrentBuffer, BackBuffer, DrawRectOp);
        SwitchBuffers();
    }

    public void End()
    {
        Graphics.CopyTexture(CurrentBuffer, m_finalBuffer);
    }

    public Rect GetRect(RectTransform transform)
    {
        Vector2 size = transform.rect.size;

        Vector2 center = (Vector2)rectTransform.InverseTransformPoint((Vector2)transform.position - (size * transform.pivot))
                + (rectTransform.pivot - Vector2.one * 0.5f) * rectTransform.rect.size;

        return new Rect(center, size);
    }

    public void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.position = (Vector2)rectTransform.position + position - (rectTransform.pivot - Vector2.one * 0.5f) * rectTransform.rect.size;
        rect.sizeDelta = size;
    }
}
