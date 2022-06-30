using Riten.Windinator.Internal;
using UnityEngine;

namespace Riten.Windinator.Internal
{
    public struct SwatchInternal
    {
        public Color CustomColor;

        public Colors PaletteColor;

        public bool UseCustomColor;

        public float Alpha;

        public float Saturation;
    }
}

namespace Riten.Windinator
{
    [System.Serializable]
    public struct Swatch
    {
        [SerializeField] Color CustomColor;

        [SerializeField] Colors PaletteColor;

        [SerializeField] bool UseCustomColor;

        [SerializeField, Range(0, 1)] float Alpha;

        [SerializeField, Range(0, 1)] float Saturation;

        public Color TransformColor(Color c)
        {
            c = Color.Lerp(Color.white, c, Saturation);
            c.a = Alpha;
            return c;
        }

        public Color UnityColor => TransformColor(UseCustomColor ? CustomColor : PaletteColor.ToColor());

        public Swatch(Color color, float saturation = 1f)
        {
            this = FromColor(color, saturation);
        }

        public Swatch(Colors color, float alpha = 1, float saturation = 1f)
        {
            this = FromTheme(color, alpha, saturation);
        }

        public static Swatch FromColor(Color color, float saturation = 1f)
        {
            return new Swatch{
                CustomColor = color,
                Alpha = color.a,
                Saturation = saturation,
                UseCustomColor = true,
                PaletteColor = Colors.Primary
            };
        }

        public static Swatch FromTheme(Colors color, float alpha = 1, float saturation = 1f)
        {
            return new Swatch{
                Alpha = alpha,
                Saturation = saturation,
                UseCustomColor = false,
                PaletteColor = color,
                CustomColor = default
            };
        }

        public static Swatch FromInternal(SwatchInternal val)
        {
            return new Swatch{
                Alpha = val.Alpha,
                CustomColor = val.CustomColor,
                PaletteColor = val.PaletteColor,
                Saturation = val.Saturation,
                UseCustomColor = val.UseCustomColor
            };
        }

        public SwatchInternal ToInternal()
        {
            return new SwatchInternal{
                Alpha = Alpha,
                CustomColor = CustomColor,
                PaletteColor = PaletteColor,
                Saturation = Saturation,
                UseCustomColor = UseCustomColor
            };
        }
    }
}
