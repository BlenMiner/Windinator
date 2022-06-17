using System;
using UnityEngine;

namespace Riten.Windinator.Audio.Internal
{
    public class AudioSourceHelper : MonoBehaviour
    {
        AudioSource m_source;

        public AudioSource Source => m_source;

        bool m_waitEnd = false;

        Action m_callback;

        void Awake()
        {
            m_source = GetComponent<AudioSource>();
        }

        void OnDisable()
        {
            m_waitEnd = false;
            m_callback = null;
        }

        public void OnClipFinished(Action onDone)
        {
            m_waitEnd = true;
            m_callback = onDone;
        }

        void Update()
        {
            if (m_waitEnd)
            {
                if (!m_source.isPlaying)
                {
                    m_waitEnd = false;
                    m_callback?.Invoke();
                }
            }
        }
    }
}