using UnityEngine;

namespace Riten.Windinator
{
    public static class WindinatorAnimations
    {
        public delegate void AnimationDelegade(WindinatorBehaviour window, float time);

        public static void FadeInLinear(WindinatorBehaviour window, float time)
        {
            window.CanvasGroup.alpha = Mathf.Lerp(0f, 1f, time);
        }

        public static void FadeOutLinear(WindinatorBehaviour window, float time)
        {
            window.CanvasGroup.alpha = Mathf.Lerp(1f, 0f, time);
        }

        public static void FadeInSin(WindinatorBehaviour window, float time)
        {
            window.CanvasGroup.alpha = Mathf.Lerp(0f, 1f, Mathf.Sin(time * Mathf.PI * 0.5f));
        }

        public static void FadeOutSin(WindinatorBehaviour window, float time)
        {
            window.CanvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.Sin(time * Mathf.PI * 0.5f));
        }

        // Feel free to expand with other functions
    }
}