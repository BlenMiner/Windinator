using UnityEngine;

namespace Riten.Windinator.Shapes
{
    public enum CanvasRefreshMode
    {
        Once,
        OnDirty,
        Always
    }

    [ExecuteAlways]
    public abstract class CanvasDrawer : MonoBehaviour
    {
        [SerializeField] CanvasRefreshMode m_drawRefreshMode = CanvasRefreshMode.Always;

        Vector2 m_lastSize;

        CanvasGraphic m_canvas;

        public CanvasGraphic Canvas
        {
            get {
                if (m_canvas == null) m_canvas = GetComponent<CanvasGraphic>();
                return m_canvas;
            }
        }

        bool m_dirty = false;

        private void Awake()
        {
            m_canvas = GetComponent<CanvasGraphic>();
        }

        public void SetDirty()
        {
            m_dirty = true;
        }

        public void ForceRedraw()
        {
            SetDirty();
            Update();
        }

        private void OnValidate()
        {
            SetDirty();
        }

        protected abstract void Draw(CanvasGraphic canvas, Vector2 size);

        protected virtual void Update()
        {
            if (m_canvas == null)
            {
                m_canvas = GetComponent<CanvasGraphic>();
                if (m_canvas == null) return;
            }

            var size = m_canvas.Size;

            if (m_drawRefreshMode == CanvasRefreshMode.Always)
            {
                m_canvas.Clear();
                Draw(m_canvas, size);
                m_canvas.Apply();

                return;
            }
            else if (m_lastSize != size)
            {
                if (m_drawRefreshMode == CanvasRefreshMode.Once ||
                    m_drawRefreshMode == CanvasRefreshMode.OnDirty)
                {
                    m_canvas.Clear();
                    Draw(m_canvas, size);
                    m_canvas.Apply();
                    m_dirty = false;
                }
                
                m_lastSize = size;
            }
            else if (m_dirty)
            {
                m_canvas.Clear();
                Draw(m_canvas, size);
                m_canvas.Apply();
                m_dirty = false;
            }
        }
    }
}