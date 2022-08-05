using Riten.Windinator;
using Riten.Windinator.Shapes;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class ShapePainter : CanvasDrawer, IPointerEnterHandler, IPointerExitHandler
{
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

    [System.Serializable]
    struct Particle
    {
        [HideInInspector] public float LastTime;

        [HideInInspector] public float Wobble;

        [HideInInspector] public float LerpedWobble;

        public ColorPalette Color;

        public RectTransform Transform;
    }

    [SerializeField] Particle[] particles = new Particle[3];


    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        Vector2 bottom = new Vector2(0, -canvas.Size.y * 0.5f + m_offset);

        // Draw main circle
        canvas.CircleBrush.Draw(bottom, m_radius);

        // Draw smaller circles
        for (int i = 0; i < 3; ++i)
        {
            float normalized = i / 2f;

            float angle = -((normalized - 0.5f) * Mathf.PI * m_angle) + Mathf.PI * 0.5f;
            Vector2 nPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            float anim = m_expandMovement.Evaluate(Mathf.Max(0f, m_expandTiming.Evaluate(m_anim) - normalized * 0.5f) * 2f);
            float space = Mathf.Lerp(-m_radius, m_spacing, anim);

            var circlePos = bottom + nPos * (m_radius + m_others_radius + space);

            // Set icon position
            canvas.SetRect(particles[i].Transform, circlePos, new Vector2(m_others_radius, m_others_radius));

            // Draw the actual circle
            canvas.CircleBrush.Draw(
                circlePos,
                m_others_radius,
                m_blending * m_others_radius
            );

            // Wobble when the ball is moving up passing by the center or down passing by the center
            if (particles[i].LastTime < 0.5f && anim >= 0.5f ||
                particles[i].LastTime > 0.5f && anim <= 0.5f)
            {
                particles[i].Wobble = 1f;
            }
            

            var c = particles[i].Color.Color;
            
            c.Alpha = anim > 0.5f ? 1f : 0f;

            particles[i].Color.Color = c;
            particles[i].Color.UpdateColor();

            float displacementStrength = particles[i].LerpedWobble;

            float disortionRad = m_radius * 0.3f;
            float noiseY = Mathf.PerlinNoise(Time.time * 4f, i * 1000) * displacementStrength * disortionRad * m_noiseMult;
            float noiseX = (Mathf.PerlinNoise(i * 1000, Time.time * 5f) - 0.5f) * 2 * displacementStrength * disortionRad * m_noiseMult;
            
            // Draw shackiness
            canvas.CircleBrush.Draw(
                bottom + nPos * (disortionRad + noiseY) + new Vector2(noiseX, 0),
                disortionRad,
                disortionRad * 3
            );

            particles[i].LerpedWobble = Mathf.Lerp(particles[i].LerpedWobble, particles[i].Wobble, Time.deltaTime * 10f);
            particles[i].Wobble = Mathf.MoveTowards(particles[i].Wobble, 0f, Time.deltaTime * 1.5f);
            particles[i].LastTime = anim;
        }
    }

    protected override void Update()
    {
        base.Update();

        float anim = m_anim;

        if (m_hovering)
             m_anim = Mathf.Lerp(m_anim, 1f, Time.deltaTime * m_animSpeed);
        else m_anim = Mathf.Lerp(m_anim, 0f, Time.deltaTime * m_animSpeed);

        if (m_anim != anim)
        {
            if      (m_anim < 0.01f) m_anim = 0f;
            else if (m_anim > 0.99f) m_anim = 1f;

            SetDirty();
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
