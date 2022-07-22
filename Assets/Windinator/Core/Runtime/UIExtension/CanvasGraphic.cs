using UnityEngine;
using UnityEngine.UI;

public class CanvasGraphic : SignedDistanceFieldGraphic
{
    RenderTexture m_buffer, m_backBuffer;

    RenderTexture m_finalBuffer;

    Material m_canvas_material;

    Material m_drawCircle;
    
    RenderTexture CurrentBuffer => m_useBackBuffer ? m_backBuffer : m_buffer;

    RenderTexture BackBuffer => m_useBackBuffer ? m_buffer : m_backBuffer;

    protected override float Margin => 0f;

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

    Material drawCircle
    {
        get
        {
            if (m_drawCircle == null)
                m_drawCircle = new Material(Shader.Find("UI/Windinator/DrawCircle"));
            return m_drawCircle;
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

            m_buffer = new RenderTexture(w, h, 1, RenderTextureFormat.ARGB32);
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

    public void Begin(Color color = default)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = CurrentBuffer;
        GL.Clear(false, true, color);
        RenderTexture.active = rt;
    }

    public void DrawCircle(Vector2 position, float size)
    {
        drawCircle.SetTexture("_MainTexture", CurrentBuffer);
        drawCircle.SetVector("_OpPosition", position);
        drawCircle.SetFloat("_OpSize", size);

        drawCircle.SetVector("_Size", m_size);
        drawCircle.SetFloat("_Padding", Margin);

        Graphics.Blit(CurrentBuffer, BackBuffer, drawCircle);
        SwitchBuffers();
    }

    public void End()
    {
        Graphics.CopyTexture(CurrentBuffer, m_finalBuffer);
    }
}
