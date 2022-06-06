using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Riten.Windinator
{
    public class ColorPalette : MonoBehaviour
    {
        [SerializeField] Graphic m_targetGraphic;

        [SerializeField] ColorType m_color;

        [SerializeField] bool m_onColor;

        [SerializeField, Range(0, 1)] float m_alpha = 1f;

        public bool OnColor
        {
            get { return m_onColor; }
            set { m_onColor = value; UpdateColor(); }
        }

        public ColorType ColorType
        {
            get { return m_color; }
            set { m_color = value; UpdateColor(); }
        }

        public ColorPair Value => Windinator.WindinatorConfig == null ? default : Windinator.WindinatorConfig.ColorPalette[ColorType];

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

            var pair = Value;
            var c = m_onColor ? pair.OnColor : pair.Color;
            c.a = m_alpha;
            m_targetGraphic.color = c;
        }
    }
}