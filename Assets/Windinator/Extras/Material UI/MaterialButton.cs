using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Riten.Windinator.Material
{
    [System.Serializable]
    public enum MaterialButtonType
    {
        Elevated,
        Filled,
        Tonal,
        Outlined,
        Text,
        Manual
    }

    [ExecuteInEditMode]
    public class MaterialButton : MonoBehaviour
    {
        [System.Serializable]
        private struct PresetReferences
        {
            public TMPro.TMP_Text m_label;
            public TMPro.TMP_Text m_iconRenderer;
            public MaterialIcon m_icon;
        }

        [System.Serializable]
        public struct MaterialButtonStyle
        {
            public RectOffset Padding;
            public float OutlineSize;
            public float ShadowSize;
            public float ShadowBlur;
            public bool FitToContent;
        }

        public bool InvertColors = false;

        internal string GetText() => m_text;

        [SerializeField] PresetReferences m_presets;

        [Header("Content")]

        [SerializeField] MaterialButtonType m_buttonType = MaterialButtonType.Filled;

        [SerializeField, Searchable] MaterialIcons m_icon = MaterialIcons.none;

        [SerializeField] string m_text = "Button";

        [SerializeField] MaterialButtonStyle m_style;

        RectangleGraphic m_graphic;

        ContentSizeFitter m_contentFitter;

        HorizontalLayoutGroup m_horizontalLayoutGroup;

        CanvasGroup m_canvasGroup;

        CanvasRenderer m_renderer;

        public UnityEvent onClick = new UnityEvent();

        private T GetOrAdd<T>() where T : Component
        {
            var c = GetComponent<T>();

            if (c == null)
                c = gameObject.AddComponent<T>();

            c.hideFlags = HideFlags.HideInInspector;

            return c;
        }

        public bool FitToContent
        {
            set { m_style.FitToContent = false; m_contentFitter.enabled = m_style.FitToContent & enabled; }
        }

        private void Reset()
        {
            m_style.OutlineSize = 1.5f;
            m_style.ShadowSize = 15f;
            m_style.ShadowBlur = 80f;
            m_style.FitToContent = true;

            OnValidate();
        }

        public void SimulateClick()
        {
            onClick?.Invoke();
        }

        private void OnEnable()
        {
            if (m_presets.m_label == null)
                m_presets.m_label = GetComponentInChildren<TMPro.TMP_Text>();

            if (m_presets.m_icon == null)
                m_presets.m_icon = GetComponentInChildren<MaterialIcon>(true);

            m_canvasGroup = GetOrAdd<CanvasGroup>();
            m_graphic = GetOrAdd<RectangleGraphic>();
            m_contentFitter = GetOrAdd<ContentSizeFitter>();
            m_renderer = GetComponent<CanvasRenderer>();
            m_renderer.cullTransparentMesh = false;
            m_horizontalLayoutGroup = GetOrAdd<HorizontalLayoutGroup>();

            m_contentFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            m_contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            m_canvasGroup.alpha = 1f;
            m_canvasGroup.interactable = true;
            m_canvasGroup.blocksRaycasts = true;

            SetButtonType(m_buttonType);
        }

        private void Start()
        {
            m_graphic.SetAllDirty();
        }

        public void SetPadding(RectOffset padding)
        {
            m_style.Padding = padding;
            m_horizontalLayoutGroup.padding = new RectOffset(padding.left + 10, padding.right + 10, padding.top + 10, padding.bottom + 10);
        }

        public void SetText(string content)
        {
            m_text = content;

            if (m_presets.m_label != null)
            {
                m_presets.m_label.text = m_text;
                m_presets.m_label.gameObject.SetActive(!string.IsNullOrEmpty(content));
            }
        }

        public void SetIcon(MaterialIcons icon)
        {
            m_icon = icon;
            if (m_presets.m_icon != null)
            {
                m_presets.m_icon.UpdateIcon(icon);
                m_presets.m_icon.gameObject.SetActive(icon != MaterialIcons.none);
            }
        }

        public void SetButtonType(MaterialButtonType type)
        {
            m_buttonType = type;

            if (type != MaterialButtonType.Manual)
                ApplyButtonStyle(type);
        }

        public void ApplyButtonStyle(MaterialButtonType type)
        {
            var primaryColor = (!InvertColors ? Colors.Primary : Colors.OnPrimary).ToColor(this);
            var secondaryColor = (!InvertColors ? Colors.SecondaryContainer : Colors.OnSecondaryContainer).ToColor(this);
            var outlineColor = (!InvertColors ? Colors.Outline : Colors.OnOutline).ToColor(this);
            var surfaceColor = (!InvertColors ? Colors.Surface : Colors.OnSurface).ToColor(this);

            var primaryOnColor = (InvertColors ? Colors.Primary : Colors.OnPrimary).ToColor(this);
            var secondaryOnColor = (InvertColors ? Colors.SecondaryContainer : Colors.OnSecondaryContainer).ToColor(this);
            var surfaceOnColor = (InvertColors ? Colors.Surface : Colors.OnSurface).ToColor(this);

            if (m_renderer.cull)
                m_renderer.cull = false;

            switch (type)
            {
                case MaterialButtonType.Elevated:
                    {
                        m_graphic.color = surfaceColor;
                        m_graphic.CircleColor = secondaryColor;
                        m_graphic.SetOutline(default, 0f);
                        m_graphic.SetShadow(Color.black, m_style.ShadowSize, m_style.ShadowBlur);

                        m_presets.m_iconRenderer.color = surfaceOnColor;
                        m_presets.m_label.color = surfaceOnColor;
                        break;
                    }
                case MaterialButtonType.Filled:
                    {
                        m_graphic.color = primaryColor;
                        m_graphic.CircleColor = primaryColor;
                        m_graphic.SetOutline(Color.clear, 0f);
                        m_graphic.SetShadow(default, 0f, 0f);

                        m_presets.m_iconRenderer.color = primaryOnColor;
                        m_presets.m_label.color = primaryOnColor;
                        break;
                    }
                case MaterialButtonType.Tonal:
                    {
                        m_graphic.color = secondaryColor;
                        m_graphic.CircleColor = primaryColor;
                        m_graphic.SetOutline(Color.clear, 0f);
                        m_graphic.SetShadow(default, 0f, 0f);

                        m_presets.m_iconRenderer.color = secondaryOnColor;
                        m_presets.m_label.color = secondaryOnColor;
                        break;
                    }
                case MaterialButtonType.Outlined:
                    {
                        m_graphic.CircleColor = secondaryColor;
                        m_graphic.color = default;
                        m_graphic.SetOutline(outlineColor, m_style.OutlineSize);
                        m_graphic.SetShadow(default, 0f, 0f);

                        m_presets.m_iconRenderer.color = primaryColor;
                        m_presets.m_label.color = primaryColor;
                        break;
                    }
                case MaterialButtonType.Text:
                    {
                        m_graphic.CircleColor = secondaryColor;
                        m_graphic.color = default;
                        m_graphic.SetOutline(default, 0f);
                        m_graphic.SetShadow(default, 0f, 0f);

                        m_presets.m_iconRenderer.color = primaryColor;
                        m_presets.m_label.color = primaryColor;
                        break;
                    }
            }

            m_graphic.CircleColor *= 1.25f;

            if (m_buttonType != MaterialButtonType.Manual)
                m_graphic.SetMaxRoundness(true);
            m_graphic.SetAllDirty();
        }

        private void OnValidate()
        {
            if (m_contentFitter == null) return;

            m_contentFitter.enabled = m_style.FitToContent && enabled;

            SetButtonType(m_buttonType);
            SetText(m_text);
            SetIcon(m_icon);
            SetPadding(m_style.Padding);
        }

        private void OnDisable()
        {
            m_contentFitter.enabled = false;

            m_canvasGroup.alpha = 1f;
            m_canvasGroup.interactable = true;
            m_canvasGroup.blocksRaycasts = true;
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            m_graphic.hideFlags = HideFlags.None;
            m_contentFitter.hideFlags = HideFlags.None;
            m_horizontalLayoutGroup.hideFlags = HideFlags.None;
            m_canvasGroup.hideFlags = HideFlags.None;
#endif
        }
    }
}