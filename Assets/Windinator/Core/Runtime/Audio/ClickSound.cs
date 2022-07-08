using UnityEngine;
using UnityEngine.EventSystems;

namespace Riten.Windinator.Audio
{
    public class ClickSound : MonoBehaviour, IPointerUpHandler
    {
        public SoundLibrary Library;

        public void OnPointerUp(PointerEventData eventData)
        {
            Library?.PlayRandom();
        }
    }
}