using UnityEngine;

namespace Riten.Windinator
{
    public static class WindinatorAnimations
    {
        public delegate void AnimationDelegade(WindinatorBehaviour window, float time);

        static void SetCanvasPos(this WindinatorBehaviour window, Vector2 pos)
        {
            window.RectTransform.anchoredPosition = pos;
        }

        static Vector2 GetCanvasSize(this WindinatorBehaviour window)
        {
            return window.RectTransform.rect.size;
        }

        static void FadeInBackground(WindinatorBehaviour window, float time)
        {
            if (window.GeneratedBackgroundGroup != null)
                window.GeneratedBackgroundGroup.alpha = Mathf.Lerp(0f, 1f, time);
        }

        static void FadeOutBackground(WindinatorBehaviour window, float time)
        {
            if (window.GeneratedBackgroundGroup != null)
                window.GeneratedBackgroundGroup.alpha = Mathf.Lerp(1f, 0f, time);
        }

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

        public static void SlideFromTop(WindinatorBehaviour window, float time)
        {
            FadeInBackground(window, time);

            var screen = window.GetCanvasSize();
            window.SetCanvasPos(new Vector2(0, screen.y * (1f - time)));
        }

        public static void SlideToTop(WindinatorBehaviour window, float time)
        {
            FadeOutBackground(window, time);

            var screen = window.GetCanvasSize();
            window.SetCanvasPos(new Vector2(0, screen.y * time));
        }

        public static void SlideFromBottom(WindinatorBehaviour window, float time)
        {
            FadeInBackground(window, time);

            var screen = window.GetCanvasSize();
            window.SetCanvasPos(new Vector2(0, -screen.y * (1f - time)));
        }

        public static void SlideToBottom(WindinatorBehaviour window, float time)
        {
            FadeOutBackground(window, time);

            var screen = window.GetCanvasSize();
            window.SetCanvasPos(new Vector2(0, -screen.y * time));
        }

        public static void SlideFromLeft(WindinatorBehaviour window, float time)
        {
            FadeInBackground(window, time);

            var screen = window.GetCanvasSize();
            window.SetCanvasPos(new Vector2(-screen.x * (1f - time), 0));
        }

        public static void SlideToLeft(WindinatorBehaviour window, float time)
        {
            FadeOutBackground(window, time);
            
            var screen = window.GetCanvasSize();
            window.SetCanvasPos(new Vector2(-screen.x * time, 0));
        }

        public static void SlideFromRight(WindinatorBehaviour window, float time)
        {
            FadeInBackground(window, time);

            var screen = window.GetCanvasSize();
            window.SetCanvasPos(new Vector2(screen.x * (1f - time), 0));
        }

        public static void SlideToRight(WindinatorBehaviour window, float time)
        {
            FadeOutBackground(window, time);
            
            var screen = window.GetCanvasSize();
            window.SetCanvasPos(new Vector2(screen.x * time, 0));
        }
    }
}