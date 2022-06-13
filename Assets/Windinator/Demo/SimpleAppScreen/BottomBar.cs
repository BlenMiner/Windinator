using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;

public class BottomBar : LayoutBaker
{
    public override Layout.Element Bake()
    {
        return new Layout.Rectangle(
            shape: new ShapeProperties()
            {
                Color = AllColorType.Primary.ToColor(),
                Shadow = new ShadowProperties()
                {
                    Size = 10f,
                    Blur = 20f
                }
            },
            child: new Layout.Horizontal(
                new Layout.Element[] {
                    new Material.Icon(MaterialIcons.home, color: AllColorType.OnPrimary),
                    new Material.Icon(MaterialIcons.apple, color: AllColorType.OnPrimary),
                    new Material.Icon(MaterialIcons.ornament, color: AllColorType.OnPrimary),
                    new Material.Icon(MaterialIcons.settings_helper, color: AllColorType.OnPrimary),
                },
                Padding: new UnityEngine.Vector4(10, 10, 10, 10),
                spacing: 20f,
                alignment: UnityEngine.TextAnchor.MiddleCenter
            ),
            flexibleWidth: 1f
        );
    }
}
