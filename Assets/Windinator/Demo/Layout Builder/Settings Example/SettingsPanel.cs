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
        return new Theme(theme,
            new Rectangle(
                new ScrollView(
                    new Vertical( 
                        new Element[]
                        {
                            new MaterialUI.Label("General Settings", style: MaterialSize.Label),
                            new MaterialUI.LabeledSwitch(
                                "Wi-Fi", false, MaterialIcons.wifi,
                                MaterialIcons.wifi, MaterialIcons.wifi_off,
                                "Public campus Wi-Fi", true
                            ),
                            new MaterialUI.LabeledSwitch("Bluetooth", true, MaterialIcons.bluetooth, MaterialIcons.check),
                            new MaterialUI.LabeledSwitch("Airplane Mode", true, MaterialIcons.airplane),
                            new MaterialUI.LabeledSwitch("Do not disturb", false, MaterialIcons.volume_mute),

                            new MaterialUI.Separator(false), 

                            new MaterialUI.LabeledControl("Volume", 
                                child: new MaterialUI.Slider(0.5f).Flexible(2f, 0f),
                                prepend: MaterialIcons.volume_source
                            ), 

                            new MaterialUI.LabeledControl("Move Forward", 
                                child: new MaterialUI.KeyButton(KeyCode.W).Small(),
                                prepend: MaterialIcons.controller_classic 
                            ),

                            new MaterialUI.LabeledControl("Sprint", 
                                child: new MaterialUI.KeyButton(KeyCode.LeftShift).Small(),
                                prepend: MaterialIcons.controller_classic
                            ),

                            new MaterialUI.Separator(false),

                            new MaterialUI.SegmentedButton(new string[]
                            {
                                "Dark Mode", "Light Mode", "Invisible Mode"
                            }, startSelectedIndex: 1),

                            new MaterialUI.RadioGroup(
                                new Vertical(
                                    new Element[] {
                                        new MaterialUI.LabeledRadio("Dark Mode", false),
                                        new MaterialUI.LabeledRadio("Light Mode", true),
                                        new MaterialUI.LabeledRadio("Invisible Mode", false),
                                    },
                                    spacing: 20f
                                )
                            ),

                            new MaterialUI.Separator(false),

                            new MaterialUI.RadioGroup(
                                new Vertical(
                                    new Element[] {
                                        new MaterialUI.LabeledCheckbox("Salsa", true),
                                        new MaterialUI.LabeledCheckbox("Tomato", false),
                                    },
                                    spacing: 20f
                                )
                            ),

                            new MaterialUI.Separator(false),

                            new MaterialUI.Label("Volume Settings", style: MaterialSize.Label),
                            new Spacer(40f)
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
