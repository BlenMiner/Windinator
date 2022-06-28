using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolBarController : MonoBehaviour
{
    [SerializeField] CoolBar[] m_bar;

    [SerializeField] RectTransform m_circle;
    
    void Update()
    {
        var rect = (m_bar[0].transform as RectTransform).rect;

        float p = (Mathf.Sin(Time.time * 0.5f) + 1f) * 0.5f;

        UpdateBarPos(p);

        var pos = m_circle.anchoredPosition;
        pos.x = p * rect.width;
        
        UpdateCirclePos(pos);
    }
    
    void UpdateBarPos(float pos)
    {
        foreach(var bar in m_bar)
            bar.Position = Mathf.Lerp(bar.Position, pos, Time.deltaTime * 10f);
    }

    void UpdateCirclePos(Vector2 newPos)
    {
        m_circle.anchoredPosition = Vector2.Lerp(m_circle.anchoredPosition, newPos, Time.deltaTime * 8f);
    }
}
