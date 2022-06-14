using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Riten.Windinator
{
    public class ColorPalette : MonoBehaviour
    {
        [SerializeField] Graphic m_targetGraphic;

        [SerializeField] Colors m_color;

        [SerializeField] bool m_useCustomColor;

        [SerializeField] Color m_customColor;

        [SerializeField, Range(0, 1)] float m_alpha = 1f;

        public Colors Value
        {
            get { return m_color; }
            set { m_color = value; m_useCustomColor = false; UpdateColor(); }
        }

        public Color RawValue
        {
            get { return m_useCustomColor ? m_customColor : m_color.ToColor(); }
            set { m_customColor = value; m_useCustomColor = true; UpdateColor(); }
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

            var c = m_useCustomColor ? m_customColor : m_color.ToColor();
            c.a = m_alpha;
            m_targetGraphic.color = c;
        }
    }
}