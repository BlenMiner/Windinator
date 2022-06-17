using UnityEngine;
using UnityEngine.UI;

namespace Riten.Windinator.Audio
{
    [RequireComponent(typeof(Button))]
    public class ButtonSound : MonoBehaviour
    {
        [SerializeField] Button m_button;

        public SoundLibrary Library;

        void Reset()
        {
            m_button = GetComponent<Button>();
        }

        void OnEnable()
        {
            m_button.onClick.AddListener(OnClick);
        }

        void OnDisable()
        {
            m_button.onClick.RemoveListener(OnClick);
        }

        void OnClick()
        {
            Library?.PlayRandom();
        }
    }
}