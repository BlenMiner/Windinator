using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    float fps;
    float oldTime;

    void Update()
    {
        float currentFps = 1.0f / Time.deltaTime;

        if (Time.time > oldTime + 0.2f)
        {
            fps = currentFps;
            oldTime = Time.time;
        }
    }

    void OnGUI()
    {
        GUILayout.Label(fps.ToString("0.00"));
    }
}
