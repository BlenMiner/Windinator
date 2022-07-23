using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WindinatorUtils
{

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector2 screenPosition)
    {
        return ScreenToCanvasPosition(canvas, canvas.transform as RectTransform, screenPosition);
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, RectTransform parent, Vector2 screenPosition)
    {
        var camera = canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPosition, camera, out var tempVector);

        return tempVector;
    }
}
