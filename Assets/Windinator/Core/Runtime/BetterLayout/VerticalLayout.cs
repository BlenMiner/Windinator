using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalLayout : GenericLayout
{
    protected override void OnDirty(int childCount)
    {
        Rect parent = RectTransform.rect;

        Vector2 prefferedSize = default;

        Vector2 totalPrefferedSize = default;
        Vector2 totalFlexibleSpace = default;

        foreach (var child in Children)
        {
            totalFlexibleSpace += child.Flexible;
            totalPrefferedSize += Vector2.Max(default, child.PrefferedSize - child.MinSize);
            prefferedSize += child.PrefferedSize;
        }

        Vector2 paddingSize = new Vector2(Padding.x + Padding.z, Padding.y + Padding.w);
        Vector2 containerSize = parent.size - paddingSize;
        Vector2 usedSize = FitMinimum();

        usedSize = FitPreffered(totalPrefferedSize, containerSize - usedSize);

        Layout.PrefferedSize = prefferedSize + new Vector2(Padding.x + Padding.z, Padding.y + Padding.w);

        usedSize = FitFlexible(totalFlexibleSpace, containerSize - usedSize);

        Arrange(containerSize, usedSize);
    }

    void Arrange(Vector2 container, Vector2 usedSize)
    {
        float advance = Padding.y;

        switch (Alignment)
        {
            case TextAnchor.LowerLeft:
            case TextAnchor.LowerCenter:
            case TextAnchor.LowerRight:

            advance += container.y - usedSize.y;

            break;

            case TextAnchor.MiddleLeft:
            case TextAnchor.MiddleCenter:
            case TextAnchor.MiddleRight:

            advance += (container.y - usedSize.y) * 0.5f;

            break;
        }

        foreach(var layout in Children)
        {
            var child = layout.RectTransform;
            var size = layout.CachedSize;

            float width = layout.Flexible.x != 0 ? 
                Mathf.Max(layout.MinSize.x, container.x) : 
                Mathf.Max(layout.MinSize.x, Mathf.Min(layout.PrefferedSize.x, container.x));

            float xOffset = Padding.x;

            switch (Alignment)
            {
                case TextAnchor.UpperCenter:
                case TextAnchor.LowerCenter:
                case TextAnchor.MiddleCenter:

                xOffset += (container.x - width) * 0.5f;

                break;

                case TextAnchor.UpperRight:
                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:

                xOffset += container.x - width;

                break;
            }

            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, advance, size.y);
            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, xOffset, width);

            advance += size.y;
        }
    }
}
