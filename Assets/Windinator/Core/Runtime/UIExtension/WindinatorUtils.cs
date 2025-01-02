using System;
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


    public static bool Create(ref RenderTexture texture, int width, int height, RenderTextureFormat format) {
        texture = new RenderTexture(width, height, 0, format) { useMipMap = false };
        return texture.Create();
    }

    public static void Destroy(ref RenderTexture texture) {
        if (texture != null) {
            texture.Release();
            if (Application.isPlaying) {
                RenderTexture.Destroy(texture);
            }
            else {
                RenderTexture.DestroyImmediate(texture);
            }
        }
        texture = null;
    }
}
