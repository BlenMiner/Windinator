using System;
using System.Collections.Generic;

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
            m_instances.Add(new WindinatorAnimatorState {
                window = window,
                time = 0f,
                onDone = onDone,
                anim = anim
            });
        }

        public void Update(float delta)
        {
            for (int i = 0; i < m_instances.Count; ++i)
            {
                var state = m_instances[i];

                state.time += delta / state.window.AnimationDuration;

                if (state.time > 1f)
                {
                    state.time = 1f;
                    state.anim(state.window, state.time);
                    state.onDone?.Invoke();
                    m_instances.RemoveAt(i--);
                }
                else
                {
                    state.anim(state.window, state.time);
                    m_instances[i] = state;
                }
            }
        }
    }
}