using System;
using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator
{
    public class WindinatorAnimator
    {
        private struct WindinatorAnimatorState
        {
            public float time;

            public WindinatorBehaviour window;
            
            public WindinatorAnimations.AnimationDelegade anim;

            public Action onDone;
        }

        List<WindinatorAnimatorState> m_instances = new List<WindinatorAnimatorState>();

        public void Animate(WindinatorBehaviour window, WindinatorAnimations.AnimationDelegade anim, Action onDone)
        {
            window.AnimationActors += 1;
            m_instances.Add(new WindinatorAnimatorState
            {
                window = window,
                time = 0f,
                onDone = onDone,
                anim = anim
            });
        }

        void ResetBackgroundPos(WindinatorBehaviour window)
        {
            if (window.GeneratedBackground == null) return;

            var backRect = window.GeneratedBackground.transform as RectTransform;
            var rect = window.RectTransform;
            backRect.anchoredPosition = -rect.anchoredPosition;
        }

        public void Update(float delta)
        {
            for (int i = 0; i < m_instances.Count; ++i)
            {
                var state = m_instances[i];

                if (state.time == 0f)
                    state.window.ResetAnimationState();

                state.time += delta / state.window.AnimationDuration;

                if (state.time > 1f)
                {
                    state.time = 1f;
                    state.window.AnimationActors -= 1;
                    state.window.ResetAnimationState();
                    state.onDone?.Invoke();
                    m_instances.RemoveAt(i--);
                }
                else
                {
                    state.anim(state.window, state.time);
                    ResetBackgroundPos(state.window);
                    m_instances[i] = state;
                }
            }
        }

        internal void Clear(WindinatorBehaviour window)
        {
            for (int i = 0; i < m_instances.Count; ++i)
            {
                var w = m_instances[i];
                if (w.window == window)
                {
                    w.window.AnimationActors -= 1;
                    m_instances.RemoveAt(i--);
                }
            }
        }
    }
}