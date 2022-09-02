using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;

public class SettingsPanel : LayoutBaker
{
    public ColorAssigner m_theme;

    public Element SettingsCard(ColorAssigner theme)
    {
        const float LABEL_HEIGHT = 15f;
        const float CONTROL_HEIGHT = 32f;
        const float SEGMENTED_BUTTON_HEIGHT = 40f;
        const float SEPARATOR_HEIGHT = 1f;

        return new Theme(theme,
            new Rectangle(
                new ScrollView(
                    new Vertical(
                        new Element[]
                        {
                            new SizeVertical(
                                new SizedElement[] {
                                    new SizedElement(new MaterialUI.Label("General Settings", style: MaterialSize.Label), LABEL_HEIGHT),
                                    new SizedElement(new MaterialUI.LabeledSwitch(
                                        "Wi-Fi", false, MaterialIcons.wifi,
                                        MaterialIcons.wifi, MaterialIcons.wifi_off,
                                        "Public campus Wi-Fi", true
                                    ), CONTROL_HEIGHT),
                                    new SizedElement(new MaterialUI.LabeledSwitch("Bluetooth", true, MaterialIcons.bluetooth, MaterialIcons.check), CONTROL_HEIGHT),
                                    new SizedElement(new MaterialUI.LabeledSwitch("Airplane Mode", true, MaterialIcons.airplane), CONTROL_HEIGHT),
                                    new SizedElement(new MaterialUI.LabeledSwitch("Do not disturb", false, MaterialIcons.volume_mute), CONTROL_HEIGHT),
                                    new SizedElement(new MaterialUI.Separator(false), SEPARATOR_HEIGHT),

                                    new SizedElement(new MaterialUI.LabeledControl("Volume",
                                        child: new MaterialUI.Slider(0.5f).Flexible(2f, 0f),
                                        prepend: MaterialIcons.volume_source
                                    ), CONTROL_HEIGHT),

#if ENABLE_LEGACY_INPUT_MANAGER
                                    new SizedElement(new MaterialUI.LabeledControl("Move Forward",
                                        child: new MaterialUI.KeyButton(KeyCode.W).Small(),
                                        prepend: MaterialIcons.controller_classic
                                    ), CONTROL_HEIGHT),

                                    new SizedElement(new MaterialUI.LabeledControl("Sprint",
                                        child: new MaterialUI.KeyButton(KeyCode.LeftShift).Small(),
                                        prepend: MaterialIcons.controller_classic
                                    ), CONTROL_HEIGHT),

#else
                                    new SizedElement(new MaterialUI.LabeledControl("Move Forward",
                                        child: new MaterialUI.KeyButton(UnityEngine.InputSystem.Key.W).Small(),
                                        prepend: MaterialIcons.controller_classic
                                    ), CONTROL_HEIGHT),

                                    new SizedElement(new MaterialUI.LabeledControl("Sprint", 
                                        child: new MaterialUI.KeyButton(UnityEngine.InputSystem.Key.LeftShift).Small(),
                                        prepend: MaterialIcons.controller_classic
                                    ), CONTROL_HEIGHT),
#endif
                                    new SizedElement(new MaterialUI.Separator(false), SEPARATOR_HEIGHT),

                                    new SizedElement(new MaterialUI.SegmentedButton(new string[]
                                    {
                                        "Dark Mode", "Light Mode", "Invisible Mode"
                                    }, startSelectedIndex: 1), SEGMENTED_BUTTON_HEIGHT),

                                    new SizedElement(new MaterialUI.RadioGroup(
                                        new Vertical(
                                            new Element[] {
                                                new MaterialUI.LabeledRadio("Dark Mode", false),
                                                new MaterialUI.LabeledRadio("Light Mode", true),
                                                new MaterialUI.LabeledRadio("Invisible Mode", false),
                                            },
                                            spacing: 20f
                                        )
                                    ), CONTROL_HEIGHT * 3 + 20 * 2f /*Add spacing*/),

                                    new SizedElement(new MaterialUI.Separator(false), SEPARATOR_HEIGHT),

                                    new SizedElement(new MaterialUI.RadioGroup(
                                        new Vertical(
                                            new Element[] {
                                                new MaterialUI.LabeledRadio("Salsa", false),
                                                new MaterialUI.LabeledRadio("Tomato", true)
                                            },
                                            spacing: 20f
                                        )
                                    ), CONTROL_HEIGHT * 2 + 20 /*Add spacing*/),

                                    new SizedElement(new MaterialUI.Separator(false), SEPARATOR_HEIGHT),

                                    new SizedElement(new MaterialUI.Label("Volume Settings", style: MaterialSize.Label), LABEL_HEIGHT),
                                    new SizedElement(null, 40f) // Acts as spacer
                                },
                                spacing: 20f
                            ),
                        },
                        spacing: 20f,
                        padding: Vector4.one * 15f
                    ).Flexible()
                ),
                size: new Vector2(400f, -1f),
                padding: Vector4.one * 20f,
                shape: new ShapeProperties
                {
                    Color = Colors.Surface,
                    Shadow = new ShadowProperties
                    {
                        Blur = 80,
                        Size = 20
                    }
                }
            ).Flexible()
        );
    }

    public override Element Bake()
    {
        return new Horizontal(
            new Element[]
            {
                new FlexibleSpace(),
                SettingsCard(null),
                new FlexibleSpace(),
                SettingsCard(m_theme),
                new FlexibleSpace(),
            }
        ).Flexible();
    }

}
