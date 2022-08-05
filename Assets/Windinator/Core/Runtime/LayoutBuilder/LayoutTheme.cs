using System;
using UnityEngine;

namespace Riten.Windinator.LayoutBuilder
{
    public class LayoutTheme : MonoBehaviour
    {
        [SerializeField] ColorAssigner m_theme;

        public ColorAssigner Theme => m_theme;

        public event Action OnThemeUpdated;

        public void UpdateTheme(ColorAssigner newTheme)
        {
            if (newTheme == null)
                newTheme = Windinator.WindinatorConfig.ColorPalette;

            m_theme = newTheme;
            OnThemeUpdated?.Invoke();
        }
    }
}