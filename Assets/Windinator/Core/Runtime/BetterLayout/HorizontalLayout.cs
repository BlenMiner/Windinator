using UnityEngine;

public class HorizontalLayout : GenericLayout
{
    protected override void OnDirty(int childCount)
    {
        Rect parent = RectTransform.rect;

        Vector2 totalPrefferedSize = default;
        Vector2 totalFlexibleSpace = default;

        foreach (var child in Children)
        {
            totalFlexibleSpace += child.Flexible;
            totalPrefferedSize += Vector2.Max(default, child.PrefferedSize - child.MinSize);
        }

        Vector2 containerSize = parent.size;
        Vector2 usedSize = FitMinimum();

        usedSize = FitPreffered(totalPrefferedSize, containerSize - usedSize);
        
        FitFlexible(totalFlexibleSpace, containerSize - usedSize);

        Arrange(containerSize);
    }

    void Arrange(Vector2 container)
    {
        float advance = 0f;
        foreach(var layout in Children)
        {
            var child = layout.RectTransform;
            var size = layout.CachedSize;

            float height = layout.Flexible.y != 0 ? 
                Mathf.Max(layout.MinSize.y, container.y) : 
                Mathf.Max(layout.MinSize.y, Mathf.Min(layout.PrefferedSize.y, container.y));

            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, height);
            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, advance, size.x);
            advance += size.x;
        }
    }

    Vector2 FitMinimum()
    {
        Vector2 totalSize = default;

        foreach (var layout in Children)
        {
            var calculatedSize = layout.MinSize;
            layout.CachedSize = calculatedSize;
            totalSize += calculatedSize;
        }
        return totalSize;
    }

    Vector2 FitPreffered(Vector2 totalPreffered, Vector2 remainingSpace)
    {
        Vector2 totalSize = default;

        foreach (var layout in Children)
        {
            var prefferedSize = Vector2.Max(default, layout.PrefferedSize - layout.MinSize);

            var percentageSize = prefferedSize / totalPreffered;
            var calculatedSize = percentageSize * remainingSpace;

            var size = layout.CachedSize;

            size.x += Mathf.Max(0, calculatedSize.x);
            size.y += Mathf.Max(0, calculatedSize.y);

            size = Vector2.Min(size, Vector2.Max(layout.MinSize, layout.PrefferedSize));

            layout.CachedSize = size;
            totalSize += size;
        }

        return totalSize;
    }

    Vector2 FitFlexible(Vector2 totalFlexible, Vector2 remainingSpace)
    {
        Vector2 totalSize = default;

        foreach (var layout in Children)
        {
            var flexible = layout.Flexible;

            var percentageSize = flexible / totalFlexible;
            var calculatedSize = percentageSize * remainingSpace;

            var size = layout.CachedSize;

            size.x += totalFlexible.x == 0 ? 0 : Mathf.Max(0, calculatedSize.x);
            size.y += totalFlexible.y == 0 ? 0 : Mathf.Max(0, calculatedSize.y);

            layout.CachedSize = size;
            totalSize += size;
        }

        return totalSize;
    }
}
