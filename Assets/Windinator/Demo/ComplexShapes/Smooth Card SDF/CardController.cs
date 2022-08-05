using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] Animator m_animator;
    [SerializeField] float m_speed = 1f;

    void Update()
    {
        m_animator.SetFloat("time", (Time.time * m_speed) % 1.0f);
    }
}
