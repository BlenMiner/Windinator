using UnityEngine;

namespace Riten.Windinator.LayoutBuilder
{
    public abstract class LayoutBaker : MonoBehaviour
    {
        public void ClearContents()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject, true);
        }

        public RectTransform Build()
        {
            return new Builder(transform as RectTransform, child: Bake()).Build();
        }

        public abstract Layout.Element Bake();
    }
}