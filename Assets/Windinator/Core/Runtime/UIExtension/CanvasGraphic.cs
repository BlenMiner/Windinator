using System;
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
        RenderTexture m_finalBuffer;

        Material m_canvas_material, m_clearCircle, m_blend;

        [SerializeField] float m_margin;

        [SerializeField] float m_quality = 1f;

        LayerGraphic m_mainLayer;

        public LayerGraphic MainLayer => m_mainLayer;

        public LayerGraphic GetLayer(LayerGraphic layer = null)
        {
            return layer == null ? m_mainLayer : layer;
        }

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

        Material BlendOp
        {
            get
            {
                if (m_blend == null)
                    m_blend = new Material(Shader.Find("UI/Windinator/BlendOp"));
                return m_blend;
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

            if (m_mainLayer == null || !m_mainLayer.IsCreated || m_finalBuffer == null || m_finalBuffer.width != w || m_finalBuffer.height != h)
            {
                if (m_finalBuffer != null && m_finalBuffer.IsCreated())
                    m_finalBuffer.Release();
                
                m_mainLayer?.Dispose();

                m_finalBuffer = new RenderTexture(w, h, 0, RenderTextureFormat.R16);
                m_finalBuffer.useMipMap = false;
                m_finalBuffer.Create();

                m_mainLayer = new LayerGraphic(w, h);
                Texture = m_finalBuffer;
            }
        }

        /// <summary>
        /// You have to dispose it manually after you use it this frame.
        /// </summary>
        /// <returns></returns>
        public LayerGraphic GetNewLayer()
        {
            int w = Mathf.CeilToInt(m_size.x * Quality + Margin);
            int h = Mathf.CeilToInt(m_size.y * Quality + Margin);

            var layer = new LayerGraphic(w, h);

            Clear(layer);

            return layer;
        }

        public void Clear(LayerGraphic layer = null)
        {
            var selectedLayer = GetLayer(layer);

            selectedLayer.Blit(ClearCircleOp);

            if (Texture != m_finalBuffer)
                Texture = m_finalBuffer;
        }

        public void Apply(LayerGraphic layer = null)
        {
            var selectedLayer = GetLayer(layer);

            selectedLayer.Copy(m_finalBuffer);

            if (Texture != m_finalBuffer)
                Texture = m_finalBuffer;
        }

        public void Copy(LayerGraphic source, LayerGraphic dest)
        {
            var a = GetLayer(source);
            var b = GetLayer(dest);

            a.Copy(b.Texture);
        }

        public void Blend(LayerGraphic a, LayerGraphic b, float v, LayerGraphic target)
        {
            BlendOp.SetFloat("_Lerp", v);
            BlendOp.SetTexture("_TextureA", a.Texture);
            BlendOp.SetTexture("_TextureB", b.Texture);
            Graphics.Blit(null, target.Texture, BlendOp);
        }

        public void Blend(RenderTexture a, RenderTexture b, float v, RenderTexture target)
        {
            BlendOp.SetFloat("_Lerp", v);
            BlendOp.SetTexture("_TextureA", a);
            BlendOp.SetTexture("_TextureB", b);
            Graphics.Blit(null, target, BlendOp);
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