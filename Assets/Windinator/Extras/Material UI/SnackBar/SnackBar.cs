using Riten.Windinator;
using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator.Material
{
    [System.Serializable]
    public enum SnackbarPos
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }

    public class SnackBar : WindinatorBehaviour
    {
        [SerializeField] RectTransform m_parent;

        [SerializeField] TMPro.TMP_Text m_message;

        [SerializeField] TMPro.TMP_Text m_buttonMsg;

        [SerializeField] GameObject m_button;

        SnackbarPos m_pos;

        System.Action m_callback;

        float m_padding;

        float m_timer;

        static Queue<SnackBar> Queue = new Queue<SnackBar>();

        static SnackBar CurrentSnackbar = null;

        public static Vector2 AnchorFromPos(SnackbarPos pos)
        {
            switch (pos)
            {
                case SnackbarPos.TopLeft: return new Vector2(0, 1);
                case SnackbarPos.TopCenter: return new Vector2(.5f, 1);
                case SnackbarPos.TopRight: return new Vector2(1, 1);

                case SnackbarPos.MiddleLeft: return new Vector2(0, .5f);
                case SnackbarPos.MiddleCenter: return new Vector2(.5f, .5f);
                case SnackbarPos.MiddleRight: return new Vector2(1, .5f);

                case SnackbarPos.BottomLeft: return new Vector2(0, 0);
                case SnackbarPos.BottomCenter: return new Vector2(.5f, 0);
                case SnackbarPos.BottomRight: return new Vector2(1, 0);
            }

            return Vector2.zero;
        }

        void OnEnable()
        {
            this.onWindowClosed += OnPopped;
        }

        void OnDisable()
        {
            this.onWindowClosed -= OnPopped;
        }
        
        void OnPopped()
        {
            m_blockUpdate = true;
            if (CurrentSnackbar == this)
                CurrentSnackbar = null;
        }

        public void Setup(string message, string action = null, System.Action actionCallback = null, SnackbarPos position = SnackbarPos.BottomCenter, float padding = 10f, float aliveTime = 5f)
        {
            if (action != null)
            {
                m_buttonMsg.SetText(action);
                m_button.SetActive(true);
            }
            else m_button.SetActive(false);

            m_message.text = message;
            m_callback = actionCallback;
            m_pos = position;
            m_padding = padding;
            m_timer = aliveTime;

            Vector2 anchor = AnchorFromPos(m_pos);
            Vector2 offset = new Vector2(
                (anchor.x - 0.5f) * 2f,
                (anchor.y - 0.5f) * 2f
            );

            m_parent.anchorMax = anchor;
            m_parent.anchorMin = m_parent.anchorMax;
            m_parent.pivot = anchor;

            m_parent.anchoredPosition = -offset * m_padding;

            CanvasGroup.alpha = 0f;
            CanvasGroup.blocksRaycasts = false;

            Queue.Enqueue(this);
            m_blockUpdate = false;
        }

        bool m_blockUpdate = false;

        private void Update()
        {
            if (m_blockUpdate) return;

            if (CurrentSnackbar == null && Queue.Count > 0)
            {
                CurrentSnackbar = Queue.Dequeue();
                CurrentSnackbar.CanvasGroup.blocksRaycasts = true;

                Windinator.ClearAnimations(this);
                Windinator.Animate(this, WindinatorAnimations.FadeInSin);
            }

            if (CurrentSnackbar != this) return;

            m_timer -= Time.deltaTime;

            if (m_timer < 0f)
                CuratedPop();
        }

        private void CuratedPop()
        {
            m_blockUpdate = true;
            CanvasGroup.blocksRaycasts = false;

            Windinator.ClearAnimations(this);
            Windinator.Animate(this, WindinatorAnimations.FadeOutSin, () =>
            {
                PopWindow();

                if (CurrentSnackbar == this)
                    CurrentSnackbar = null;
            });
        }

        public void ButtonCB()
        {
            if (CurrentSnackbar == this)
                CurrentSnackbar = null;

            CuratedPop();
            m_callback?.Invoke();
        }
    }
}