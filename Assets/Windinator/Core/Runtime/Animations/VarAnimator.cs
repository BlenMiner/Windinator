using System;
using UnityEngine;

namespace Riten.Windinator.Animation
{
    public class VarAnimator<T> where T : struct
    {
        public static float Lerp(float start, float end, float value)
        {
            return start + (end - start) * value;
        }

        public static Vector2 Lerp(Vector2 start, Vector2 end, float value)
        {
            return new Vector2(
                Lerp(start.x, end.x, value),
                Lerp(start.y, end.y, value)
            );
        }

        public static Vector3 Lerp(Vector3 start, Vector3 end, float value)
        {
            return new Vector3(
                Lerp(start.x, end.x, value),
                Lerp(start.y, end.y, value),
                Lerp(start.z, end.z, value)
            );
        }

        public static Color Lerp(Color start, Color end, float value)
        {
            return new Color(
                Lerp(start.r, end.r, value),
                Lerp(start.g, end.g, value),
                Lerp(start.b, end.b, value),
                Lerp(start.a, end.a, value)
            );
        }

        Func<T, T, float, T> m_lerp;

        Action<T> m_updateValue;

        Func<T, float, T> m_modifier;

        float m_animSpeed;

        float m_time;

        T m_start;

        T m_target;

        T m_current;

        T m_previous;

        AnimationCurve m_curve;

        public VarAnimator(Func<T, T, float, T> lerp, Action<T> updateValue, float animSpeed, AnimationCurve curve = null)
        {
            m_lerp = lerp;
            m_animSpeed = animSpeed;
            m_updateValue = updateValue;
            m_curve = curve;
        }

        public void SetModifier(Func<T, float, T> mod)
        {
            m_modifier = mod;
        }

        public void AnimateToTarget(T target)
        {
            m_start = m_current;
            m_target = target;
            m_time = 0f;
        }

        public void Snap()
        {
            m_time = 1f;
            
            m_current = m_target;

            if (m_modifier != null)
                m_current = m_modifier.Invoke(m_current, m_time);

            m_updateValue?.Invoke(m_current);
        }

        public void SnapToTarget(T target)
        {
            m_target = target;
            m_current = target;
            m_start = target;
            m_previous = m_current;

            m_updateValue?.Invoke(m_current);

            m_time = 1f;
        }

        public void Update(float deltaTime)
        {
            m_current = m_lerp(m_start, m_target, m_curve == null ? m_time : m_curve.Evaluate(m_time));

            if (m_modifier != null)
                m_current = m_modifier.Invoke(m_current, m_time);

            if (!m_current.Equals(m_previous))
            {
                m_previous = m_current;
                m_updateValue?.Invoke(m_current);
            }

            if (m_time >= 1f) m_time = 1f;
            else m_time += deltaTime * m_animSpeed;
        }
    }
}