using Riten.Windinator.Material;
using TMPro;
using UnityEngine;
using static Riten.Windinator.LayoutBuilder.Layout;

namespace Riten.Windinator.LayoutBuilder
{
    public static class LayoutMaterialPrefabs
    {
        private static GameObject _MaterialSeparator;

        public static GameObject MaterialSeparator
        {
            get
            {
                if (_MaterialSeparator == null)
                    _MaterialSeparator = Resources.Load<GameObject>("Windinator.Material.UI/Material Separator");
                return _MaterialSeparator;
            }
        }

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

        private static GameObject _MaterialIcon;

        public static GameObject MaterialIcon
        {
            get
            {
                if (_MaterialIcon == null)
                    _MaterialIcon = Resources.Load<GameObject>("Windinator.Material.UI/Material Icon");
                return _MaterialIcon;
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

        private static GameObject _SwitchField;

        public static GameObject SwitchField
        {
            get
            {
                if (_SwitchField == null)
                    _SwitchField = Resources.Load<GameObject>("Windinator.Material.UI/Material Switch");
                return _SwitchField;
            }
        }
    }
    public static class MaterialUI
    {
        public class LabeledSwitch : Element
        {
            public string Text { get; }
            public string SubTitle { get; }
            public bool Separator { get; }
            public bool Value { get; }
            public MaterialIcons Prepend { get; }
            public MaterialIcons On { get; }
            public MaterialIcons Off { get; }

            public LabeledSwitch(string text, bool value = false,
                MaterialIcons prepend = MaterialIcons.none,
                MaterialIcons on = MaterialIcons.none,
                MaterialIcons off = MaterialIcons.none,
                string subTitle = null,
                bool separator = false) : base(default)
            {
                Text = text;
                Value = value;
                Prepend = prepend;
                On = on;
                Off = off;
                SubTitle = subTitle;
                Separator = separator;
            }

            Element Bake()
            {
                return new Horizontal(
                    new Element[] {
                        new Icon(Prepend, color: Colors.OnSurface.ToColor()),
                        new Vertical(
                            new Element[]
                            {
                                new Label(Text, color: Colors.OnSurface),
                                new Label(SubTitle, color: Colors.OnSurfaceVariant, style: MaterialLabelStyle.Label),
                            },
                            alignment: TextAnchor.MiddleLeft
                        ),
                        new FlexibleSpace(),
                        Separator ? new Separator(true) : null,
                        new Switch(Value, On, Off)
                    },
                    alignment: TextAnchor.MiddleCenter,
                    spacing: 20f
                );
            }

            public override RectTransform Build(RectTransform parent)
            {
                return Bake().Build(parent);
            }
        }

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
                    spacing: -1f
                ).Build(parent);

                for (int i = 0; i < buttonList.Length; i++)
                {
                    if (buttonUI[i].Value == null) return null;
                }

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
                        graphic.SetRoundness(new Vector4(0, 0, float.PositiveInfinity, float.PositiveInfinity));
                    }
                    else if (end)
                    {
                        graphic.SetRoundness(new Vector4(float.PositiveInfinity, float.PositiveInfinity, 0, 0));
                    }
                    else
                    {
                        graphic.SetRoundness(Vector4.zero);
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

                if (prefab == null) return null;

                var btn = prefab.GetComponentInChildren<MaterialButton>();

                btn.SetText(m_text);
                btn.SetIcon(m_icon);
                btn.SetPadding(new RectOffset((int)m_padding.x, (int)m_padding.y, (int)m_padding.z, (int)m_padding.w));
                btn.SetButtonType(m_type);

                SetReference(btn);

                return prefab;
            }
        }

        public class Separator : PrefabRef<MaterialSeparator>
        {
            private readonly bool vertical;
            private readonly Color? color;

            public Separator(
                bool Vertical,
                Color? Color = null
            )
                : base(prefab: LayoutMaterialPrefabs.MaterialSeparator)
            {
                vertical = Vertical;
                color = Color;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var prefab = base.Build(parent);

                if (prefab == null) return null;

                var separator = prefab.GetComponentInChildren<MaterialSeparator>();

                separator.Color = color.GetValueOrDefault(Colors.SurfaceVariant.ToColor());
                separator.Vertical = vertical;

                SetReference(separator);

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

                if (prefab == null) return null;

                var btn = prefab.GetComponentInChildren<MaterialButton>();

                btn.SetText(m_text);
                btn.SetIcon(m_icon);
                btn.SetPadding(new RectOffset((int)m_padding.x, (int)m_padding.y, (int)m_padding.z, (int)m_padding.w));
                btn.SetButtonType(m_type);

                SetReference(btn);

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

                if (prefab == null) return null;

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

                SetReference(field);

                return prefab;
            }
        }

        public class Label : PrefabRef<MaterialLabel>
        {
            MaterialLabelStyle m_style;
            FontStyles m_fontStyle;
            Colors m_color;
            string m_text;

            public Label(
                string text = "",
                MaterialLabelStyle style = MaterialLabelStyle.Body,
                FontStyles fontStyle = FontStyles.Normal,
                Colors color = Colors.Primary
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

                if (prefab == null) return null;

                var field = prefab.GetComponentInChildren<MaterialLabel>();

                field.LabelText = m_text;
                field.LabelStyle = m_style;
                field.LabelColor = m_color;
                field.LabelFontStyle = m_fontStyle;
                field.ForceUpdate();

                SetReference(field);

                return prefab;
            }
        }

        public class Switch : PrefabRef<MaterialSwitch>
        {
            readonly bool m_value;

            readonly MaterialIcons m_offIcon;

            readonly MaterialIcons m_onIcon;

            public Switch(
                bool value, 
                MaterialIcons onIcon = MaterialIcons.none,
                MaterialIcons offIcon = MaterialIcons.none
            ) : base(LayoutMaterialPrefabs.SwitchField)
            {
                m_value = value;
                m_onIcon = onIcon;
                m_offIcon = offIcon;
            }

            public override RectTransform Build(RectTransform parent)
            {
                var prefab = base.Build(parent);

                if (prefab == null) return null;

                var field = prefab.GetComponentInChildren<MaterialSwitch>();

                field.Value = m_value;
                field.IconOn = m_onIcon;
                field.IconOff = m_offIcon;
                field.SnapState();

                SetReference(field);

                return prefab;
            }
        }

        public class Icon : PrefabRef<MaterialIcon>
        {
            MaterialIcons m_icon;
            Color m_color;

            public Icon(
                MaterialIcons icon = MaterialIcons.plus,
                Color? color = null
            ) : base(LayoutMaterialPrefabs.MaterialIcon)
            {
                m_icon = icon;
                m_color = color.GetValueOrDefault(Color.black);
            }

            public override RectTransform Build(RectTransform parent)
            {
                var prefab = base.Build(parent);

                if (prefab == null) return null;

                var field = prefab.GetComponentInChildren<MaterialIcon>();

                field.UpdateIcon(m_icon, m_color);

                SetReference(field);

                return prefab;
            }
        }
    }
}