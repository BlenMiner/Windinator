using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Riten.Windinator.Audio;

namespace Riten.Windinator.Material
{
    public class ContextMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public SoundLibrary OnClickSound;

        public Graphic Graphic;

        public ColorPalette Color;

        public Colors ColorSelected = Colors.SurfaceVariant;

        bool m_selected = false;

        bool m_hovering = false;

        public bool IsSelected
        {
            get => m_selected;
            set
            {
                m_selected = value;
                UpdateSelectedColor();
            }
        }

        public UnityEvent onClick;

        public void UpdateSelectedColor()
        {
            Color c = m_hovering || m_selected ? ColorSelected.ToColor(this) : default;
            Graphic.color = c;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_hovering = true;
            UpdateSelectedColor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_hovering = false;
            UpdateSelectedColor();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
            OnClickSound?.PlayRandom();
        }
    }
}