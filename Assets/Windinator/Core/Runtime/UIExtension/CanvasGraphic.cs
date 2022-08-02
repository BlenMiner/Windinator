using UnityEngine;

namespace Riten.Windinator.Shapes
{
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

        Material m_canvas_material, m_clearCircle;

        public RenderTexture CurrentBuffer => m_useBackBuffer ? m_backBuffer : m_buffer;

        public RenderTexture BackBuffer => m_useBackBuffer ? m_buffer : m_backBuffer;

        [SerializeField] float m_margin;

        [SerializeField] float m_quality = 1f;

        public override float Margin => m_margin;

        public float Quality {
            get => m_quality;
            set {m_quality = value;}
        }

        public void SetMargin(float margin)
        {
            if (margin < 0) margin = 0;
            m_margin = margin;
        }

        public void SwitchBuffers()
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

        Material ClearCircleOp
        {
            get
            {
                if (m_clearCircle == null)
                    m_clearCircle = new Material(Shader.Find("UI/Windinator/ClearOp"));
                return m_clearCircle;
            }
        }

        public LineDrawer LineBrush { get; private set; }

        public CircleDrawer CircleBrush { get; private set; }

        public RectDrawer RectBrush { get; private set; }

        public Vector2 Size => m_size;

        Vector2 m_size;

        protected override void Awake()
        {
            base.Awake();

            LineBrush = new LineDrawer(this);
            CircleBrush = new CircleDrawer(this);
            RectBrush = new RectDrawer(this);
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (LineBrush == null) Awake();
        }
        #endif

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

        void UpdateShader(float width, float height)
        {
            m_size = new Vector2(width, height);

            int w = Mathf.CeilToInt(width * Quality + Margin);
            int h = Mathf.CeilToInt(height * Quality + Margin);

            if (w <= 0 || h <= 0) return;

            if (m_buffer == null || m_buffer.width != w || m_buffer.height != h)
            {
                if (m_buffer != null && m_buffer.IsCreated())
                    m_buffer.Release();

                if (m_backBuffer != null && m_backBuffer.IsCreated())
                    m_backBuffer.Release();

                m_buffer = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBFloat);
                m_backBuffer = new RenderTexture(m_buffer);
                m_finalBuffer = new RenderTexture(m_buffer);

                m_buffer.useMipMap = false;
                m_backBuffer.useMipMap = false;
                m_finalBuffer.useMipMap = false;

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
            if (Texture != m_finalBuffer)
                Texture = m_finalBuffer;

            Graphics.Blit(CurrentBuffer, BackBuffer, ClearCircleOp);
            SwitchBuffers();
        }

        public void End()
        {
            Graphics.Blit(CurrentBuffer, m_finalBuffer);
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
}