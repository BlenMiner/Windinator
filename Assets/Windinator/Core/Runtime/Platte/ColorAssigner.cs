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

    [CreateAssetMenu(menuName = "Windinator/Material Palette")]
    public class ColorAssigner : ScriptableObject
    {
        [System.Serializable]
        public enum ColorType
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
        }

        [System.Serializable]
        public enum AllColorType
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

        public ColorPair this[ColorType idx]
        {
            get
            {
                switch (idx)
                {
                    case ColorType.Primary: return Primary;
                    case ColorType.PrimaryContainer: return PrimaryContainer;
                    case ColorType.Secondary: return Secondary;
                    case ColorType.SecondaryContainer: return SecondaryContainer;
                    case ColorType.Tertiary: return Tertiary;
                    case ColorType.TertiaryContainer: return TertiaryContainer;
                    case ColorType.Error: return Error;
                    case ColorType.ErrorContainer: return ErrorContainer;
                    case ColorType.Background: return Background;
                    case ColorType.Surface: return Surface;
                    case ColorType.SurfaceVariant: return SurfaceVariant;
                    case ColorType.Outline: return Outline;
                    case ColorType.Snackbar: return Snackbar;
                    default: throw new System.Exception("Invalid Color");
                }
            }
        }

        public Color this[AllColorType idx]
        {
            get
            {
                switch (idx)
                {
                    case AllColorType.Primary: return Primary.Color;
                    case AllColorType.PrimaryContainer: return PrimaryContainer.Color;
                    case AllColorType.Secondary: return Secondary.Color;
                    case AllColorType.SecondaryContainer: return SecondaryContainer.Color;
                    case AllColorType.Tertiary: return Tertiary.Color;
                    case AllColorType.TertiaryContainer: return TertiaryContainer.Color;
                    case AllColorType.Error: return Error.Color;
                    case AllColorType.ErrorContainer: return ErrorContainer.Color;
                    case AllColorType.Background: return Background.Color;
                    case AllColorType.Surface: return Surface.Color;
                    case AllColorType.SurfaceVariant: return SurfaceVariant.Color;
                    case AllColorType.Outline: return Outline.Color;
                    case AllColorType.Snackbar: return Snackbar.Color;
                         
                    case AllColorType.OnPrimary: return Primary.OnColor;
                    case AllColorType.OnPrimaryContainer: return PrimaryContainer.OnColor;
                    case AllColorType.OnSecondary: return Secondary.OnColor;
                    case AllColorType.OnSecondaryContainer: return SecondaryContainer.OnColor;
                    case AllColorType.OnTertiary: return Tertiary.OnColor;
                    case AllColorType.OnTertiaryContainer: return TertiaryContainer.OnColor;
                    case AllColorType.OnError: return Error.OnColor;
                    case AllColorType.OnErrorContainer: return ErrorContainer.OnColor;
                    case AllColorType.OnBackground: return Background.OnColor;
                    case AllColorType.OnSurface: return Surface.OnColor;
                    case AllColorType.OnSurfaceVariant: return SurfaceVariant.OnColor;
                    case AllColorType.OnOutline: return Outline.OnColor;
                    case AllColorType.OnSnackbar: return Snackbar.OnColor;
                    default: throw new System.Exception("Invalid Color");
                }
            }
        }
    }
}