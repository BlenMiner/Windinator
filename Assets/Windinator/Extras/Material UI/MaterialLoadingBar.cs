using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MaterialLoadingBar : MonoBehaviour
{
    [SerializeField] RectTransform m_parent;

    [SerializeField] RectTransform m_content;

    [Header("Anim Settings")]

    [SerializeField] float m_sizeSpeed = 2f;

    [SerializeField] float m_moveSpeed = 1.5f;

    void OnValidate()
    {
        Awake();
    }

    void Awake()
    {
        if (m_content == null ) return;

        m_content.anchorMin = Vector2.up * 0.5f;
        m_content.anchorMax = m_content.anchorMin;
        m_content.pivot = new Vector2(0, 0.5f);
    }

    void Update()
    {
        if (m_content == null || m_parent == null) return;

        var rect = m_parent.rect;

        float lerp = Time.time * m_moveSpeed % 1.0f;
        float size = (Mathf.Sin(Time.time * m_sizeSpeed) + 1) * 0.5f * rect.width;

        size = Mathf.Max(rect.width * 0.1f, size);

        float startPos = -rect.width;
        float endPos = rect.width + rect.width;

        float pos = Mathf.Lerp(startPos, endPos, lerp);

        m_content.anchoredPosition = new Vector2(pos, 0);
        m_content.sizeDelta = new Vector2(size, rect.height);
    }
}
