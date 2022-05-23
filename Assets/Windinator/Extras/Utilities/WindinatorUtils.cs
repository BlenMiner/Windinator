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

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
    {
        Vector3 Return = Vector3.zero;
        var _Cam = canvas.worldCamera;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Return = screenPosition;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, _Cam, out var tempVector);
            Return = canvas.transform.TransformPoint(tempVector);
        }

        return Return;
    }
}
