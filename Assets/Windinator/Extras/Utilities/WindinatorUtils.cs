using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WindinatorUtils
{
    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition)
    {
        Vector3 Return = Vector3.zero;
        var _Cam = canvas.worldCamera;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Return = _Cam.WorldToScreenPoint(worldPosition);
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, _Cam.WorldToScreenPoint(worldPosition), _Cam, out var tempVector);
            Return = canvas.transform.TransformPoint(tempVector);
        }

        return Return;
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector2 screenPosition)
    {
        return ScreenToCanvasPosition(canvas, canvas.transform as RectTransform, screenPosition);
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, RectTransform parent, Vector2 screenPosition)
    {
        Vector3 Return = Vector3.zero;
        var camera = canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPosition, camera, out var tempVector);

        return tempVector;
    }
}
