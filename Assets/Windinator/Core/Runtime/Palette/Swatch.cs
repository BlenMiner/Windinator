using UnityEngine;

namespace Riten.Windinator
{
    [System.Serializable]
    public struct Swatch
    {
        [SerializeField] Color CustomColor;

        [SerializeField] Colors PaletteColor;

        [SerializeField] bool UseCustomColor;

        [SerializeField, Range(0, 1)] public float Alpha;

        [SerializeField, Range(0, 1)] public float Saturation;

        public Color TransformColor(Color c)
        {
            Color.RGBToHSV(c, out var h, out var s, out var v);
            c = Color.HSVToRGB(h, s * Saturation, v);
            c.a = Alpha;
            return c;
        }

        public Color GetUnityColor(GameObject caller)
        {
            return TransformColor(UseCustomColor ? CustomColor : PaletteColor.ToColor(caller));
        }

        public Color GetUnityColor(Transform caller)
        {
            return TransformColor(UseCustomColor ? CustomColor : PaletteColor.ToColor(caller.gameObject));
        }

        public Color GetUnityColor(MonoBehaviour caller)
        {
            return TransformColor(UseCustomColor ? CustomColor : PaletteColor.ToColor(caller.gameObject));
        }

        public static implicit operator Swatch(Color d) => new Swatch(d);

        public static implicit operator Swatch(Colors d) => new Swatch(d);

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
                CustomColor = new Color(color.r, color.g, color.b),
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
    }
}
