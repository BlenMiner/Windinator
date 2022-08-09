using System;
using UnityEngine;

namespace Riten.Windinator.Shapes
{
    public class LayerGraphic : IDisposable
    {
        readonly RenderTexture m_buffer, m_backBuffer;

        bool m_useBackbuffer;

        public bool IsCreated { get; private set; }

        public LayerGraphic(int width, int height)
        {
            m_useBackbuffer = false;

            m_buffer = new RenderTexture(width, height, 0, RenderTextureFormat.R16);
            m_backBuffer = new RenderTexture(m_buffer);

            m_buffer.useMipMap = false;
            m_backBuffer.useMipMap = false;

            m_buffer.Create();
            m_backBuffer.Create();
            IsCreated = true;
        }

        public RenderTexture Texture => m_useBackbuffer ? m_backBuffer : m_buffer;

        public RenderTexture BackTexture => m_useBackbuffer ? m_buffer : m_backBuffer;

        void SwitchBuffers()
        {
            m_useBackbuffer = !m_useBackbuffer;
        }

        public void Dispose()
        {
            if (IsCreated)
            {
                m_buffer.Release();
                m_backBuffer.Release();
                IsCreated = false;
            }
        }

        public void Blit(Material mat)
        {
            Graphics.Blit(Texture, BackTexture, mat);
            SwitchBuffers();
        }

        public void Copy(RenderTexture destination)
        {
            Graphics.Blit(Texture, destination);
        }
    }
}