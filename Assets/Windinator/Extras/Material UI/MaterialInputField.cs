using TMPro;
using Riten.Windinator;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

[System.Serializable]
public enum MaterialTextFieldType
{
    Filled,
    Outlined
}

[ExecuteInEditMode]
public class MaterialInputField : MonoBehaviour
{
    public static class Rules
    {
        public const string Email = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        public const string Url = @"^(http(s)?://)?([\w-]+\.)+[\w-]+[.com]+(/[/?%&=]*)?$";
        public const string StrongPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}";
    }

    [Header("Settings")]

    [SerializeField] TMP_InputField.ContentType m_contentType;

    [SerializeField] MaterialTextFieldType m_type;

    [SerializeField, Searchable] MaterialIcons m_labelIcon;

    [SerializeField] string m_labelText;

    [SerializeField] string m_text;

    [SerializeField] string m_helperText;

    [SerializeField] string m_regex;

    [SerializeField] string m_errorTxt;

    [Header("Color")]

    [SerializeField] Colors m_fieldColor = Colors.SecondaryContainer;
    [SerializeField] Colors m_normalColor = Colors.OnSecondaryContainer;
    [SerializeField] Colors m_selectedColor = Colors.Primary;
    [SerializeField] Colors m_errorColor = Colors.Error;

    [Header("Component References")]

    [SerializeField] GameObject m_closeBtnContainer;

    [SerializeField] GameObject m_iconContainer;

    [SerializeField] MaterialIcon m_icon;

    [SerializeField] RectangleGraphic m_background;

    [SerializeField] TMP_InputField m_textField;

    [SerializeField] TMP_Text m_helper;

    [SerializeField] TMP_Text m_label;

    [SerializeField] Graphic m_bottomLine;

    [SerializeField] Graphic m_preIcon, m_postIcon;

    [SerializeField] RectTransform m_inputArea;

    ColorAssigner m_palette => Windinator.WindinatorConfig == null ? null : Windinator.WindinatorConfig.ColorPalette;

    public TMP_InputField.ContentType ContentType { get => m_contentType; set { m_contentType = value; } }

    public MaterialTextFieldType Style { get => m_type; set { m_type = value; } }

    public MaterialIcons Icon { get => m_labelIcon; set { m_labelIcon = value; } }

    public string LabelText { get => m_labelText; set { m_labelText = value; } }

    public string Text { get => m_textField.text; set { m_textField.text = value; } }

    public string HelperText { get => m_helperText; set { m_helperText = value; } }

    public string ErrorText { get => m_errorTxt; set { m_errorTxt = value; } }

    public string RegexExpression { get => m_regex; set { m_regex = value; } }

    bool m_selected;

    float m_labelPaddingTarget;
    float m_labelPaddingValue;

    Color m_bottomBarColorTarget;
    Color m_bottomBarColorValue;

    Color m_labelColorTarget;
    Color m_labelColorValue;

    bool m_error;
    bool m_dirty;

    float m_shakeTime = 0f;
    bool m_shake = false;

    float m_helperTextStartX;

    public void SetDirty()
    {
        OnUpdated();
    }

    public void ShakeHelperText()
    {
        if (!m_shake)
        {
            m_helperTextStartX = m_helper.transform.localPosition.x;
        }

        m_shake = true;
        m_shakeTime = 0f;
    }

    public void ResetField()
    {
        m_error = false;
        m_dirty = false;
    }

    public bool ValidateField()
    {
        UpdateError();
        OnUpdated();

        if (m_error) ShakeHelperText();
        return !m_error;
    }

    private void OnEnable()
    {
        OnUpdated(true);
    }

    private void OnValidate()
    {
        m_textField.text = m_text;

        OnUpdated(!Application.isPlaying);
    }

    private void Reset()
    {
        OnValidate();
    }

    public void OnSelectedChanged(bool selected)
    {
        m_selected = selected;
        OnUpdated();
    }

    void UpdateError()
    {
        var newError = !IsValid();

        if (m_error != newError)
        {
            m_error = newError;
            if (m_error) ShakeHelperText();
        }
    }

    public bool IsValid()
    {
        return Regex.IsMatch(m_textField.text, m_regex);
    }

    private void OnUpdated(bool snap = false)
    {
        if (m_palette == null) return;

        m_textField.contentType = m_contentType;
        bool selected = m_selected;
        bool emptyText = string.IsNullOrEmpty(m_textField.text);
        bool displayIcon = m_labelIcon != MaterialIcons.none;

        m_labelPaddingTarget = selected || !emptyText ? (m_type == MaterialTextFieldType.Outlined ? -40f : -24f) : 0;

        float bottomBarHeight = selected ? 3f : 2f;
        m_bottomBarColorTarget = selected ? m_selectedColor.ToColor(this) : m_normalColor.ToColor(this);

        if (m_error) m_bottomBarColorTarget = m_errorColor.ToColor(this);

        m_labelColorTarget = m_bottomBarColorTarget;

        if (!selected) m_labelColorTarget.a = 0.5f;

        var newSize = new Vector2(m_bottomLine.rectTransform.sizeDelta.x, bottomBarHeight);

        var color = m_fieldColor.ToColor(this);
        var empty = color;
        empty.a = 0;

        m_background.color = m_type == MaterialTextFieldType.Outlined ? empty : color;
        m_background.SetOutline(color, m_type == MaterialTextFieldType.Outlined ? 2f : 0f);
        m_background.SetRoundness(m_type == MaterialTextFieldType.Outlined ? new Vector4(5, 5, 5, 5) : new Vector4(5, 0, 5, 0));
        m_bottomLine.enabled = m_type == MaterialTextFieldType.Filled;

        if (m_type == MaterialTextFieldType.Filled)
            m_background.SetMask(default);

        if (snap)
        {
            m_labelPaddingValue = m_labelPaddingTarget;
            m_bottomBarColorValue = m_bottomBarColorTarget;
            m_labelColorValue = m_labelColorTarget;
        }

        m_icon.UpdateIcon(m_labelIcon);
        m_iconContainer.SetActive(displayIcon);
        m_closeBtnContainer.SetActive(!emptyText);

        m_helper.text = m_error ? m_errorTxt : m_helperText;
        m_label.text = m_labelText;

        m_bottomLine.rectTransform.sizeDelta = newSize;
    }

    public void OnInputChanged(string newVal)
    {
        if (m_error) UpdateError();
        OnUpdated();
    }

    public void ClearText()
    {
        m_textField.text = string.Empty;
        ResetField();
        OnUpdated();
    }

    private void Update()
    {
        if (m_shake && m_shakeTime < 1f)
        {
            m_shakeTime += Time.deltaTime * 2.5f;
            if (m_shakeTime > 1f)
            {
                m_shakeTime = 1f;
                m_shake = false;
            }

            var pos = m_helper.transform.localPosition;
            pos.x = m_helperTextStartX + Mathf.Sin(m_shakeTime * Mathf.PI * 3) * 10f;
            m_helper.transform.localPosition = pos;

        }

        if (m_textField.isFocused != m_selected)
        {
            m_selected = m_textField.isFocused;

            if (!m_dirty)
            {
                m_dirty = !string.IsNullOrEmpty(m_textField.text);
            }

            if (m_dirty) UpdateError();

            OnUpdated();
        }

        m_labelPaddingValue = Mathf.Lerp(m_labelPaddingValue, m_labelPaddingTarget, Time.deltaTime * 10f);
        if (m_labelPaddingValue != m_label.margin.y)
        {
            m_label.margin = new Vector4(0, m_labelPaddingValue, 0, 0);

            if (m_type == MaterialTextFieldType.Outlined)
            {
                var rectTransform = transform as RectTransform;
                var rect = rectTransform.rect;
                const float padding = 10f;

                m_background.SetMask(new Vector4(
                    -rect.width * 0.5f + Mathf.Abs(m_inputArea.localPosition.x) - padding,
                    -m_labelPaddingValue * 0.5f - rect.height * 0.4f,
                    m_label.rectTransform.rect.width + padding * 2,
                    m_label.rectTransform.rect.height
                ));
            }
        }

        m_bottomBarColorValue = Color.Lerp(m_bottomBarColorValue, m_bottomBarColorTarget, Time.deltaTime * 10f);
        if (m_bottomBarColorValue != m_bottomLine.color) m_bottomLine.color = m_bottomBarColorValue;
        if (m_bottomBarColorValue != m_helper.color) m_helper.color = m_bottomBarColorValue;
        if (m_bottomBarColorValue != m_postIcon.color) m_postIcon.color = m_bottomBarColorValue;
        if (m_bottomBarColorValue != m_preIcon.color) m_preIcon.color = m_bottomBarColorValue;

        m_labelColorValue = Color.Lerp(m_labelColorValue, m_labelColorTarget, Time.deltaTime * 10f);
        if (m_labelColorValue != m_label.color) m_label.color = m_labelColorValue;
    }
}
