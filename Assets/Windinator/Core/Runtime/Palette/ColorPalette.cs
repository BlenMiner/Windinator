using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Riten.Windinator
{
    public class ColorPalette : MonoBehaviour
    {

        [SerializeField] Graphic m_targetGraphic;

        [SerializeField] Swatch m_color = Swatch.FromTheme(Colors.Primary);

        public Swatch Color
        {
            get => m_color;
            set => m_color = value;
        }

        private void Reset()
        {
            m_targetGraphic = GetComponentInChildren<Graphic>();
            OnValidate();
        }

        private void OnValidate()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            UpdateColor();
        }

        public void UpdateColor()
        {
            if (m_targetGraphic == null) return;
            m_targetGraphic.color = m_color.GetUnityColor(this);
        }
    }
}