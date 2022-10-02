using System;
using UnityEngine;

namespace Riten.Windinator.Shapes
{
    public class LayerGraphic : IDisposable
    {
        readonly RenderTexture m_buffer, m_backBuffer;
        readonly RenderTexture m_colorBuffer, m_backColorBuffer;

        bool m_useBackbuffer;

        bool m_useColorBackbuffer;

        public bool IsCreated { get; private set; }

        readonly bool HasColorSupport;

        public LayerGraphic(int width, int height, bool createColorBuffer = true)
        {
            HasColorSupport = createColorBuffer;
            m_useBackbuffer = false;

            m_buffer = new RenderTexture(width, height, 0, RenderTextureFormat.RG32);
            m_backBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.RG32);

            m_buffer.useMipMap = false;
            m_backBuffer.useMipMap = false;

            m_buffer.Create();
            m_backBuffer.Create();

            if (createColorBuffer)
            {
                m_colorBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                m_backColorBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);

                m_colorBuffer.useMipMap = false;
                m_backColorBuffer.useMipMap = false;

                m_colorBuffer.Create();
                m_backColorBuffer.Create();
            }


            IsCreated = true;
        }

        public RenderTexture Texture => m_useBackbuffer ? m_backBuffer : m_buffer;

        public RenderTexture BackTexture => m_useBackbuffer ? m_buffer : m_backBuffer;

        public RenderTexture ColorTexture => m_useColorBackbuffer ? m_backColorBuffer : m_colorBuffer;

        public RenderTexture ColorBackTexture => m_useColorBackbuffer ? m_colorBuffer : m_backColorBuffer;

        void SwitchBuffers()
        {
            m_useBackbuffer = !m_useBackbuffer;
        }

        void SwitchColorBuffers()
        {
            m_useColorBackbuffer = !m_useColorBackbuffer;
        }

        public void Dispose()
        {
            if (IsCreated)
            {
                m_buffer.Release();
                m_backBuffer.Release();
                IsCreated = false;

                if (HasColorSupport)
                {
                    m_colorBuffer.Release();
                    m_backColorBuffer.Release();
                }
            }
        }

        public void Blit(Material mat)
        {
            Graphics.Blit(Texture, BackTexture, mat);
            SwitchBuffers();
        }

        public void BlitColor(Material mat)
        {
#if UNITY_EDITOR
            if (!HasColorSupport)
            {
                Debug.LogError("You are using colors yet you specified no color on layer generation.");
                return;
            }
#endif
            Graphics.Blit(ColorTexture, ColorBackTexture, mat);
            SwitchColorBuffers();
        }

        public void Copy(RenderTexture destination)
        {
            Graphics.Blit(Texture, destination);
        }

        public void CopyColor(RenderTexture destination)
        {
#if UNITY_EDITOR
            if (!HasColorSupport)
            {
                Debug.LogError("You are using colors yet you specified no color on layer generation.");
                return;
            }
#endif

            Graphics.Blit(ColorTexture, destination);
        }
    }
}