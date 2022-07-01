using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;

public class SettingsPanel : LayoutBaker
{
    public override Element Bake()
    {
        return new Rectangle(
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
                )
            ),
            size: new Vector2(400f, -1f),
            padding: Vector4.one * 20f,
            shape: new ShapeProperties
            {
                Color = Colors.Surface.ToColor(),
                Shadow = new ShadowProperties
                {
                    Blur = 80,
                    Size = 20
                }
            }
        ).Flexible(-1, 1f);
    }

}
