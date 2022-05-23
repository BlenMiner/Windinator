using Riten.Windinator.Material;
using TMPro;
using UnityEngine;
using static Riten.Windinator.LayoutBuilder.Layout;

namespace Riten.Windinator.LayoutBuilder
{
    public static class LayoutMaterialPrefabs
    {
        private static GameObject _MaterialLabel;

        public static GameObject MaterialLabel
        {
            get
            {
                if (_MaterialLabel == null)
                    _MaterialLabel = Resources.Load<GameObject>("Windinator.Material.UI/Material Label");
                return _MaterialLabel;
            }
        }

        private static GameObject _MaterialButton;

        public static GameObject MaterialButton
        {
            get
            {
                if (_MaterialButton == null)
                    _MaterialButton = Resources.Load<GameObject>("Windinator.Material.UI/Material Button");
                return _MaterialButton;
            }
        }

        private static GameObject _ActionButton;

        public static GameObject ActionButton
        {
            get
            {
                if (_ActionButton == null)
                    _ActionButton = Resources.Load<GameObject>("Windinator.Material.UI/Action Button");
                return _ActionButton;
            }
        }

        private static GameObject _InputField;

        public static GameObject InputField
        {
            get
            {
                if (_InputField == null)
                    _InputField = Resources.Load<GameObject>("Windinator.Material.UI/Material Text Field");
                return _InputField;
            }
        }
    }
    public static class LayoutMaterial
    {
        public class SegmentedButton : PrefabRef<MaterialButtonSegment>
        {
            string[] m_text;

            MaterialIcons[] m_icon;

            MaterialButtonType[] m_type;

            int m_startIndex;

            int m_count;

            public SegmentedButton(
                string[] text = null,
                MaterialIcons[] icon = null,
                MaterialButtonType[] type = null,
                int startSelectedIndex = -1,
                Vector4 padding = default
            )
                : base()
            {
                m_count = Mathf.Max(
                    text == null ? 0 : text.Length,
                    icon == null ? 0 : icon.Length,
                    type == null ? 0 : type.Length);

                m_startIndex = startSelectedIndex;
                m_text = text;
                m_icon = icon;
                m_padding = padding;
                m_type = type;
            }

            public override RectTransform Build(RectTransform parent)
            {
                Button[] buttonList = new Button[m_count];
                Reference<MaterialButton>[] buttonUI = new Reference<MaterialButton>[m_count];

                for (int i = 0; i < buttonList.Length; i++)
                {
                    buttonList[i] = new Button(
                        text: (m_text == null ? null : m_text[i]),
                        icon: (m_icon == null ? MaterialIcons.none : m_icon[i]),
                        type: (m_type == null ? MaterialButtonType.Outlined : m_type[i])
                    );

                    var btn = buttonList[i];

                    btn.GetReference(out buttonUI[i]);
                }

                var prefab = new Horizontal(
                    buttonList,
                    alignment: TextAnchor.MiddleCenter,
                    spacing: 1f
                ).Build(parent);

                prefab.name = "#Layout-Segmented-Button";

                var segments = prefab.gameObject.AddComponent<MaterialButtonSegment>();

                segments.Initialize(m_startIndex, buttonUI);

                for (int i = 0; i < buttonList.Length; i++)
                {
                    bool start = i == 0;
                    bool end = i == buttonList.Length - 1;

                    var btn = buttonUI[i].Value;

                    btn.SetButtonType(MaterialButtonType.Manual);

                    var graphic = btn.GetComponent<RectangleGraphic>();
                    var color = graphic.OutlineColor;
                    color.a = 0f;

                    graphic.color = color;
                    graphic.SetMaxRoundness(false);
                    graphic.SetUniformRoundness(false);

                    if (start)
                    {
                        graphic.SetRoundness(new Vector4(-10, -10, float.PositiveInfinity, float.PositiveInfinity));
                    }
                    else if (end)
                    {
                        graphic.SetRoundness(new Vector4(float.PositiveInfinity, float.PositiveInfinity, -10, -10));
                    }
                    else
                    {
                        graphic.SetRoundness(new Vector4(-10, -10, -10, -10));
                    }
                }
                return prefab;
            }
        }

        public class Button : PrefabRef<MaterialButton>
        {
            string m_text;

            MaterialIcons m_icon;

            MaterialButtonType m_type;

            public Button(
                string text = null,
                MaterialIcons icon = MaterialIcons.none,
                MaterialButtonType type = MaterialButtonType.Filled,
                Vector4 padding = default
            )
                : base(prefab: LayoutMaterialPrefabs.MaterialButton)
            {
                m_text = text;
                m_icon = icon;
                m_padding = padding;
                m_type = type;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var prefab = base.Build(parent);
                var btn = prefab.GetComponentInChildren<MaterialButton>();

                btn.SetText(m_text);
                btn.SetIcon(m_icon);
                btn.SetPadding(new RectOffset((int)m_padding.x, (int)m_padding.y, (int)m_padding.z, (int)m_padding.w));
                btn.SetButtonType(m_type);

                m_reference.Value = btn;

                return prefab;
            }
        }

        public class ActionButton : PrefabRef<MaterialButton>
        {
            MaterialIcons m_icon;

            MaterialButtonType m_type;

            string m_text;

            public ActionButton(
                string text = null,
                MaterialIcons icon = MaterialIcons.plus,
                MaterialButtonType type = MaterialButtonType.Filled,
                Vector4 padding = default
            )
                : base(prefab: LayoutMaterialPrefabs.ActionButton)
            {
                m_text = text;
                m_icon = icon;
                m_padding = padding;
                m_type = type;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var prefab = base.Build(parent);
                var btn = prefab.GetComponentInChildren<MaterialButton>();

                btn.SetText(m_text);
                btn.SetIcon(m_icon);
                btn.SetPadding(new RectOffset((int)m_padding.x, (int)m_padding.y, (int)m_padding.z, (int)m_padding.w));
                btn.SetButtonType(m_type);

                m_reference.Value = btn;

                return prefab;
            }
        }

        public class InputField : PrefabRef<MaterialInputField>
        {
            string m_text;
            string m_labelText;
            string m_helperText;
            string m_errorText;
            string m_regexExpression;
            MaterialTextFieldType m_style;
            MaterialIcons m_icon;
            TMP_InputField.ContentType m_contentType;


            public InputField(
                    string text = "",
                    string labelText = "",
                    string helperText = "",
                    string errorText = "",
                    string regexExpression = "",
                    MaterialTextFieldType style = MaterialTextFieldType.Outlined,
                    MaterialIcons icon = MaterialIcons.none,
                    TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard
            ) : base(LayoutMaterialPrefabs.InputField)
            {
                m_text = text;
                m_labelText = labelText;
                m_helperText = helperText;
                m_errorText = errorText;
                m_regexExpression = regexExpression;
                m_style = style;
                m_icon = icon;
                m_contentType = contentType;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var prefab = base.Build(parent);
                var field = prefab.GetComponentInChildren<MaterialInputField>();

                field.Text = m_text;
                field.LabelText = m_labelText;
                field.HelperText = m_helperText;
                field.ErrorText = m_errorText;
                field.RegexExpression = m_regexExpression;
                field.Style = m_style;
                field.Icon = m_icon;
                field.ContentType = m_contentType;

                field.SetDirty();

                m_reference.Value = field;

                return prefab;
            }
        }

        public class Label : PrefabRef<MaterialLabel>
        {
            MaterialLabelStyle m_style;
            FontStyles m_fontStyle;
            ColorAssigner.AllColorType m_color;
            string m_text;

            public Label(
                string text = "",
                MaterialLabelStyle style = MaterialLabelStyle.Body,
                FontStyles fontStyle = FontStyles.Normal,
                ColorAssigner.AllColorType color = ColorAssigner.AllColorType.Primary
            ) : base(LayoutMaterialPrefabs.MaterialLabel)
            {
                m_text = text;
                m_style = style;
                m_fontStyle = fontStyle;
                m_color = color;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var prefab = base.Build(parent);
                var field = prefab.GetComponentInChildren<MaterialLabel>();

                field.Text = m_text;
                field.Style = m_style;
                field.Color = m_color;
                field.FontStyle = m_fontStyle;

                field.SetDirty();

                m_reference.Value = field;

                return prefab;
            }
        }
    }
}