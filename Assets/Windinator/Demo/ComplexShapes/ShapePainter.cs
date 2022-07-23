using System.Collections;
using System.Collections.Generic;
using Riten.Windinator;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class ShapePainter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] CanvasGraphic m_canvas;

    [Header("Main Blob")]
    [SerializeField] float m_offset = 0f;

    [SerializeField] float m_radius = 40f;

    [Header("Side Blobs")]

    [SerializeField] float m_others_radius = 20f;

    [SerializeField] float m_spacing = 0f;

    [SerializeField, Range(0, 1)] float m_blending = 0f;

    [SerializeField, Range(0f, 1f)] float m_angle = 0.25f;

    [Header("Animation")]

    [SerializeField] float m_animSpeed = 3f;

    [SerializeField, Range(0, 1)] float m_anim = 0f;

    [SerializeField] float m_noiseMult = 2f;

    [SerializeField] AnimationCurve m_expandMovement;

    [SerializeField] AnimationCurve m_expandTiming;

    bool m_hovering = false;

    void Update()
    {
        if (m_canvas == null) return;

        m_canvas.Begin();
        Draw();
        m_canvas.End();

        if (m_hovering)
             m_anim = Mathf.Lerp(m_anim, 1f, Time.deltaTime * m_animSpeed);
        else m_anim = Mathf.Lerp(m_anim, 0f, Time.deltaTime * m_animSpeed);
    }

    [System.Serializable]
    struct Particle
    {
        [HideInInspector] public float LastTime;

        [HideInInspector] public float Distortion;

        [HideInInspector] public float LerpedDistortion;

        public ColorPalette Color;

        public RectTransform Transform;
    }

    [SerializeField] Particle[] particles = new Particle[3];

    void Draw()
    {
        Vector2 bottom = new Vector2(0, -m_canvas.Size.y * 0.5f + m_offset);

        m_canvas.DrawCircle(bottom, m_radius);

        for (int i = 0; i < 3; ++i)
        {
            float normalized = i / 2f;

            float angle = -((normalized - 0.5f) * Mathf.PI * m_angle) + Mathf.PI * 0.5f;
            Vector2 nPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            float anim = m_expandMovement.Evaluate(Mathf.Max(0f, m_expandTiming.Evaluate(m_anim) - normalized * 0.5f) * 2f);
            float space = Mathf.Lerp(-m_radius, m_spacing, anim);

            // Draw actual circle

            var circlePos = bottom + nPos * (m_radius + m_others_radius + space);

            m_canvas.SetRect(particles[i].Transform, circlePos, new Vector2(m_others_radius, m_others_radius));

            m_canvas.DrawCircle(
                circlePos,
                m_others_radius,
                m_blending * m_others_radius
            );

            if (particles[i].LastTime < 0.5f && anim >= 0.5f ||
                particles[i].LastTime > 0.5f && anim <= 0.5f)
            {
                // Passed circle
                particles[i].Distortion = 1f;
            }
            

            var c = particles[i].Color.Color;
            
            c.Alpha = anim > 0.5f ? 1f : 0f;

            particles[i].Color.Color = c;
            particles[i].Color.UpdateColor();

            float displacementStrength = particles[i].LerpedDistortion;

            float disortionRad = m_radius * 0.3f;
            float noiseY = Mathf.PerlinNoise(Time.time * 4f, i * 1000) * displacementStrength * disortionRad * m_noiseMult;
            float noiseX = (Mathf.PerlinNoise(i * 1000, Time.time * 5f) - 0.5f) * 2 * displacementStrength * disortionRad * m_noiseMult;
            
            // Draw shackiness
            m_canvas.DrawCircle(
                bottom + nPos * (disortionRad + noiseY) + new Vector2(noiseX, 0),
                disortionRad,
                disortionRad * 3
            );

            particles[i].LerpedDistortion = Mathf.Lerp(particles[i].LerpedDistortion, particles[i].Distortion, Time.deltaTime * 10f);
            particles[i].Distortion = Mathf.MoveTowards(particles[i].Distortion, 0f, Time.deltaTime * 1.5f);
            particles[i].LastTime = anim;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_hovering = false;
    }
}
