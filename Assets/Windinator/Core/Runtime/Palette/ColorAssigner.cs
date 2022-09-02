using UnityEngine;

namespace Riten.Windinator
{
    [System.Serializable]
    public struct ColorPair
    {
        public Color Color, OnColor;

        public ColorPair(Color a, Color b)
        {
            Color = a;
            OnColor = b;
        }
    }

    [System.Serializable]
    public enum Colors
    {
        Primary,
        PrimaryContainer,
        Secondary,
        SecondaryContainer,
        Tertiary,
        TertiaryContainer,
        Error,
        ErrorContainer,
        Background,
        Surface,
        SurfaceVariant,
        Outline,
        Snackbar,

        OnPrimary,
        OnPrimaryContainer,
        OnSecondary,
        OnSecondaryContainer,
        OnTertiary,
        OnTertiaryContainer,
        OnError,
        OnErrorContainer,
        OnBackground,
        OnSurface,
        OnSurfaceVariant,
        OnOutline,
        OnSnackbar,
    }

    public static class AllColorTypeExtension
    {
        public static Color ToColor(this Colors color, GameObject caller)
        {
            var theme = caller.GetComponentInParent<LayoutBuilder.LayoutTheme>();

            if (theme == null)
            {
                var config = Windinator.WindinatorConfig;
                if (config == null || config.ColorPalette == null) 
                     return new Color(1, 0, 1);
                else return config.ColorPalette[color];
            }
            return theme.Theme[color];
        }

        public static Color ToColor(this Colors color, MonoBehaviour caller)
        {
            return ToColor(color, caller.gameObject);
        }

        public static Color ToColor(this Colors color, Transform caller)
        {
            return ToColor(color, caller.gameObject);
        }

        public static Color ToColorRaw(this Colors color)
        {
            var config = Windinator.WindinatorConfig;
            if (config == null || config.ColorPalette == null)
                return new Color(1, 0, 1);
            else return config.ColorPalette[color];
        }
    }

    [CreateAssetMenu(menuName = "Windinator/Material Palette")]
    public class ColorAssigner : ScriptableObject
    {
        public ColorPair Primary = new ColorPair(Color.white, Color.white);
        public ColorPair PrimaryContainer = new ColorPair(Color.white, Color.white);
        public ColorPair Secondary = new ColorPair(Color.white, Color.white);
        public ColorPair SecondaryContainer = new ColorPair(Color.white, Color.white);
        public ColorPair Tertiary = new ColorPair(Color.white, Color.white);
        public ColorPair TertiaryContainer = new ColorPair(Color.white, Color.white);
        public ColorPair Error = new ColorPair(Color.white, Color.white);
        public ColorPair ErrorContainer = new ColorPair(Color.white, Color.white);
        public ColorPair Background = new ColorPair(Color.white, Color.white);
        public ColorPair Surface = new ColorPair(Color.white, Color.white);
        public ColorPair SurfaceVariant = new ColorPair(Color.white, Color.white);
        public ColorPair Outline = new ColorPair(Color.white, Color.white);
        public ColorPair Snackbar = new ColorPair(Color.white, Color.white);

        public Color this[Colors idx]
        {
            get
            {
                switch (idx)
                {
                    case Colors.Primary: return Primary.Color;
                    case Colors.PrimaryContainer: return PrimaryContainer.Color;
                    case Colors.Secondary: return Secondary.Color;
                    case Colors.SecondaryContainer: return SecondaryContainer.Color;
                    case Colors.Tertiary: return Tertiary.Color;
                    case Colors.TertiaryContainer: return TertiaryContainer.Color;
                    case Colors.Error: return Error.Color;
                    case Colors.ErrorContainer: return ErrorContainer.Color;
                    case Colors.Background: return Background.Color;
                    case Colors.Surface: return Surface.Color;
                    case Colors.SurfaceVariant: return SurfaceVariant.Color;
                    case Colors.Outline: return Outline.Color;
                    case Colors.Snackbar: return Snackbar.Color;

                    case Colors.OnPrimary: return Primary.OnColor;
                    case Colors.OnPrimaryContainer: return PrimaryContainer.OnColor;
                    case Colors.OnSecondary: return Secondary.OnColor;
                    case Colors.OnSecondaryContainer: return SecondaryContainer.OnColor;
                    case Colors.OnTertiary: return Tertiary.OnColor;
                    case Colors.OnTertiaryContainer: return TertiaryContainer.OnColor;
                    case Colors.OnError: return Error.OnColor;
                    case Colors.OnErrorContainer: return ErrorContainer.OnColor;
                    case Colors.OnBackground: return Background.OnColor;
                    case Colors.OnSurface: return Surface.OnColor;
                    case Colors.OnSurfaceVariant: return SurfaceVariant.OnColor;
                    case Colors.OnOutline: return Outline.OnColor;
                    case Colors.OnSnackbar: return Snackbar.OnColor;
                    default: throw new System.Exception("Invalid Color");
                }
            }
        }
    }
}