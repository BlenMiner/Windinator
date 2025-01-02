using System;
using UnityEngine;

namespace Riten.Windinator.Shapes
{
    public class LayerGraphic : IDisposable
    {
        RenderTexture m_buffer, m_backBuffer;
        RenderTexture m_colorBuffer, m_backColorBuffer;

        bool m_useBackbuffer;

        bool m_useColorBackbuffer;

        public bool IsCreated { get; private set; }

        readonly bool HasColorSupport;

        public LayerGraphic(int width, int height, bool createColorBuffer = true)
        {
            HasColorSupport = createColorBuffer;
            m_useBackbuffer = false;

            WindinatorUtils.Create(ref m_buffer, width, height, RenderTextureFormat.RG32);
            WindinatorUtils.Create(ref m_backBuffer, width, height, RenderTextureFormat.RG32);

            if (createColorBuffer)
            {
                WindinatorUtils.Create(ref m_colorBuffer, width, height, RenderTextureFormat.ARGB32);
                WindinatorUtils.Create(ref m_backColorBuffer, width, height, RenderTextureFormat.ARGB32);
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
                WindinatorUtils.Destroy(ref m_buffer);
                WindinatorUtils.Destroy(ref m_backBuffer);
                IsCreated = false;

                if (HasColorSupport)
                {
                    WindinatorUtils.Destroy(ref m_colorBuffer);
                    WindinatorUtils.Destroy(ref m_backColorBuffer);
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