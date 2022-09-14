using Riten.Windinator;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Riten.Windinator.Material.MaterialButton;
using static Riten.Windinator.LayoutBuilder.MaterialUI;
using UnityEngine.Events;
using Button = UnityEngine.UI.Button;

namespace Riten.Windinator.Material
{
    [System.Serializable]
    public enum MaterialButtonStylePresets
    {
        Elevated,
        Filled,
        Tonal,
        Outlined,
        Text,
        Manual,
        WaitingSelection
    }

    [System.Serializable]
    public class MaterialButtonStyle
    {
        [Header("Graphic")]
        public Swatch Color = Colors.Surface;
        public Swatch TextColor = Colors.OnSurface;
        public Swatch CircleColor = Colors.Primary;

        [Header("Outline")]
        public float OutlineSize = 0f;
        public Swatch OutlineColor = Colors.Outline;

        [Header("Shadow")]
        public float ShadowSize = 15f;
        public float ShadowBlur = 80f;
        public Swatch ShadowColor = UnityEngine.Color.black;
    }

    [ExecuteInEditMode]
    public class MaterialButton : MonoBehaviour, ILayoutElement
    {
        [SerializeField, Searchable] MaterialIcons MaterialIcon = MaterialIcons.none;

        [SerializeField] string m_text = "Button";

        [SerializeField] MaterialButtonStyle m_buttonStyle;

        [SerializeField] MaterialButtonStylePresets m_loadPreset = MaterialButtonStylePresets.WaitingSelection;

        [Header("References")]

        [SerializeField] Button m_button;

        [SerializeField] RectangleGraphic m_graphic;

        [SerializeField] MaterialIcon m_materialIcon;

        [SerializeField] TMP_Text m_textComponent;

        Vector2 m_preferredSize;

        Vector2 m_padding;

        private DrivenRectTransformTracker m_Tracker;

        public float minWidth => 40f;

        public float preferredWidth => Mathf.Max(m_preferredSize.x + m_padding.x, minWidth);

        public float preferredHeight => Mathf.Max(m_preferredSize.y + m_padding.y, minHeight);

        public float flexibleWidth => 0;

        public float minHeight => 40f;

        public float flexibleHeight => 0;

        public int layoutPriority => 0;

        bool m_beingControlled = false;

        RectTransform m_rectTransform;

        void UpdateBeingControlled()
        {
            m_beingControlled = transform.parent != null && transform.parent.GetComponent<HorizontalOrVerticalLayoutGroup>() != null;
        }

        public void CalculateLayoutInputHorizontal() { }

        public void CalculateLayoutInputVertical() { }

        void OnClicked()
        {
            onClick?.Invoke();
        }

        private void OnEnable()
        {
            if (m_button != null)
                m_button.onClick.AddListener(OnClicked);

            if (m_textComponent != null)
                m_textComponent.autoSizeTextContainer = false;

            m_rectTransform = transform as RectTransform;
            m_Tracker.Add(this, m_rectTransform, DrivenTransformProperties.SizeDelta);

            UpdateBeingControlled();
            UpdateButton();
        }

        private void OnDisable()
        {
            if (m_button != null)
                m_button.onClick.RemoveListener(OnClicked);
            m_Tracker.Clear();
        }

        private void OnValidate()
        {
            m_rectTransform = transform as RectTransform;

            if (m_loadPreset != MaterialButtonStylePresets.WaitingSelection)
            {
                ApplyButtonStyle(m_loadPreset);
                m_loadPreset = MaterialButtonStylePresets.WaitingSelection;
            }
            else UpdateButton();
        }

        public void ApplyButtonStyle(MaterialButtonStylePresets style)
        {
            switch (style)
            {
                case MaterialButtonStylePresets.Elevated:
                    {
                        m_buttonStyle.Color = Colors.Surface;
                        m_buttonStyle.TextColor = Colors.OnSurface;

                        m_buttonStyle.CircleColor = Colors.SecondaryContainer;

                        m_buttonStyle.OutlineSize = 0f;

                        m_buttonStyle.ShadowColor = Color.black;
                        m_buttonStyle.ShadowSize = 15f;
                        m_buttonStyle.ShadowBlur = 80f;
                        break;
                    }
                case MaterialButtonStylePresets.Filled:
                    {
                        m_buttonStyle.Color = Colors.Primary;
                        m_buttonStyle.TextColor = Colors.OnPrimary;

                        m_buttonStyle.CircleColor = Colors.OnPrimary;

                        m_buttonStyle.OutlineSize = 0f;
                        m_buttonStyle.ShadowSize = 0f;
                        break;
                    }
                case MaterialButtonStylePresets.Tonal:
                    {
                        m_buttonStyle.Color = Colors.SecondaryContainer;
                        m_buttonStyle.TextColor = Colors.OnSecondaryContainer;

                        m_buttonStyle.CircleColor = Colors.Primary;

                        m_buttonStyle.OutlineSize = 0f;
                        m_buttonStyle.ShadowSize = 0f;
                        break;
                    }
                case MaterialButtonStylePresets.Outlined:
                    {
                        m_buttonStyle.Color = Color.clear;
                        m_buttonStyle.TextColor = Colors.OnSecondaryContainer;

                        m_buttonStyle.CircleColor = Colors.Primary;

                        m_buttonStyle.OutlineColor = Colors.Outline;
                        m_buttonStyle.OutlineSize = 1.5f;

                        m_buttonStyle.ShadowSize = 0f;
                        break;
                    }
                case MaterialButtonStylePresets.Text:
                    {
                        m_buttonStyle.Color = Color.clear;
                        m_buttonStyle.TextColor = Colors.Primary;
                        m_buttonStyle.CircleColor = Colors.SecondaryContainer;

                        m_buttonStyle.OutlineSize = 0f;
                        m_buttonStyle.ShadowSize = 0f;
                        break;
                    }
            }

            UpdateButton();
        }

        public void SetText(string content)
        {
            m_text = content;
            UpdateButton();
        }

        public string GetText() => m_text;

        public UnityEvent onClick;

        bool m_dirty = false;

        public void SetIcon(MaterialIcons icon)
        {
            MaterialIcon = icon;
            UpdateButton();
        }

        public MaterialButtonStyle ButtonStyle
        {
            get => m_buttonStyle;
            set => m_buttonStyle = value;
        }

        private void OnTransformChildrenChanged()
        {
            UpdateBeingControlled();
        }

        void UpdateButton()
        {
            if (m_textComponent == null || m_materialIcon == null || m_graphic == null) return;

#if UNITY_EDITOR
            if (m_rectTransform == null)
                m_rectTransform = transform as RectTransform;
#endif

            m_dirty = true;
        }

        void UpdateColors()
        {
            m_graphic.color = m_buttonStyle.Color.GetUnityColor(this);
            m_graphic.CircleColor = m_buttonStyle.CircleColor.GetUnityColor(this);
            m_graphic.SetOutline(m_buttonStyle.OutlineColor.GetUnityColor(this), m_buttonStyle.OutlineSize);
            m_graphic.SetShadow(m_buttonStyle.ShadowColor.GetUnityColor(this), m_buttonStyle.ShadowSize, m_buttonStyle.ShadowBlur);

            var textColor = m_buttonStyle.TextColor.GetUnityColor(this);

            m_materialIcon.UpdateColor(m_buttonStyle.TextColor);
            m_textComponent.color = textColor;
        }

        private void LateUpdate()
        {
            if (m_dirty)
            {
                bool hasIcon = MaterialIcon != MaterialIcons.none;

                m_preferredSize = m_textComponent.GetPreferredValues(m_text);
                m_padding = new Vector2(hasIcon ? (string.IsNullOrEmpty(m_text) ? 40f : 48f) : 20f, 20f);

                if (!m_beingControlled)
                    m_rectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);

                m_textComponent.rectTransform.sizeDelta = m_preferredSize;
                m_textComponent.SetText(m_text);
                m_materialIcon.Icon = MaterialIcon;

                UpdateColors();
                m_dirty = false;
            }
        }
    }
}
