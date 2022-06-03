using UnityEngine;
using UnityEngine.UI;
using System.Collections.ObjectModel;

namespace Riten.Windinator
{
    public class ScrollViewDynamicStarter : MonoBehaviour
    {
        [SerializeField] ScrollRect m_scrollView;

        ObservableCollection<int> someNumbers = new ObservableCollection<int>();

        ScrollViewController<SimpleButton, int> m_scroll;

        void Start()
        {
            m_scroll = new ScrollViewController<SimpleButton, int>(
                m_scrollView, someNumbers, 40f, UpdateItem, spacing: 10f
            );
        }

        private void Update()
        {
            if (Time.time > 6f && Time.frameCount % 60 == 0)
                someNumbers.Add(Time.frameCount / 60);
        }

        void UpdateItem(SimpleButton item, int data)
        {
            item.ButtonReference.Value.SetText($"Number {data}");
        }

        private void OnDestroy()
        {
            m_scroll.Dispose();
        }
    }
}
