using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Riten.Windinator
{
    using static WindinatorAnimations;

    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public abstract class WindinatorBehaviour : MonoBehaviour
    {
        [Serializable]
        public struct OptimizationSettings
        {
            public int WarmupCount;

            [Tooltip("If this window should stop your player from receiving input")]
            public bool CullBackgroundWindows;
        }

        [Serializable]
        public struct BackgroundSettings
        {
            public bool AutoAssignBackground;

            public bool BackgroundClosesWindow;

            public Color BackgroundColor;
        }

        /// <summary>
        /// Resets alpha to 0 & the rect transform's position to take all the screen
        /// </summary>
        internal void ResetAnimationState()
        {
            RectTransform.anchorMin = Vector2.zero;
            RectTransform.anchorMax = Vector2.one;
            RectTransform.sizeDelta = Vector2.zero;
            RectTransform.anchoredPosition = Vector2.zero;
            RectTransform.localScale = Vector3.one;
            CanvasGroup.alpha = 1f;

            if (GeneratedBackground != null)
            {
                var back = GeneratedBackground.transform as RectTransform;

                back.anchorMin = Vector2.zero;
                back.anchorMax = Vector2.one;
                back.sizeDelta = Vector2.zero;
                back.anchoredPosition = Vector2.zero;
                back.localScale = Vector3.one;
                GeneratedBackgroundGroup.alpha = m_windowSettings.BackgroundSettings.BackgroundColor.a;
            }
        }

        [Serializable]
        public struct AnimationSettings
        {
            public bool AnimatedByDefault;

            public float TransitionAnimDuration;
        }

        [Serializable]
        public struct WindowFlowSettings
        {
            [Tooltip("If this window should stop your player from receiving input")]
            public bool ShoudBlockGameFlow;

            public bool CanCloseWindow;
        }

        [Serializable]
        private class WindowSettings
        {
            public WindowFlowSettings WindowFlowSettings;

            public BackgroundSettings BackgroundSettings;

            public AnimationSettings AnimationSettings;

            public OptimizationSettings OptimizationSettings;
        }

        [SerializeField] WindowSettings m_windowSettings;

        private Canvas _canvas;

        public int AnimationActors { get; set; }

        public bool IsAnimating => AnimationActors > 0;

        public Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                    _canvas = GetComponent<Canvas>();
                return _canvas;
            }
        }

        private CanvasScaler _canvasScaler;

        public CanvasScaler CanvasScaler
        {
            get
            {
                if (_canvasScaler == null)
                    _canvasScaler = GetComponent<CanvasScaler>();
                return _canvasScaler;
            }
        }

        private CanvasGroup _canvasGroup;

        public CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }

        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = transform as RectTransform;
                return _rectTransform;
            }
        }

        protected bool CanExitWindow { get; private set; } = true;

        public AnimationDelegade FadeIn { get; private set; }

        public AnimationDelegade FadeOut { get; private set; }

        public event Action onBackgroundClicked;

        public event Action onWindowClosed;

        public float AnimationDuration => m_windowSettings.AnimationSettings.TransitionAnimDuration;

        public bool ShoudBlockGameFlow => m_windowSettings.WindowFlowSettings.ShoudBlockGameFlow;

        public bool AnimatedByDefault => m_windowSettings.AnimationSettings.AnimatedByDefault;

        public bool CullBackgroundWindows => m_windowSettings.OptimizationSettings.CullBackgroundWindows;

        public OptimizationSettings GetOptimizationSettings() => m_windowSettings.OptimizationSettings;

        private GameObject m_generatedBackground;

        private CanvasGroup m_generatedBackgroundcanvasGroup;

        public GameObject GeneratedBackground => m_generatedBackground;

        public CanvasGroup GeneratedBackgroundGroup => m_generatedBackgroundcanvasGroup;

        internal void OnWindowClosedEvent() => onWindowClosed?.Invoke();

        public virtual void OnSafeEnable() { }

        public virtual void OnSafeDisable() { }
        
        private void OnValidate()
        {
            Windinator.SetupCanvas(Canvas, CanvasScaler);
        }

        private void Reset()
        {
            if (Windinator.WindinatorConfig != null)
            {
                m_windowSettings = new WindowSettings();
                m_windowSettings.BackgroundSettings.BackgroundColor = Windinator.WindinatorConfig.DefaultBackgroundColor;
                m_windowSettings.AnimationSettings.AnimatedByDefault = true;
                m_windowSettings.AnimationSettings.TransitionAnimDuration = 0.1f;
                m_windowSettings.WindowFlowSettings.CanCloseWindow = true;
                m_windowSettings.OptimizationSettings.CullBackgroundWindows = false;
            }
        }

        /// <summary>
        /// This is used to initialize the window by the Windinator system
        /// </summary>
        public void PreAwake()
        {
            if (EventSystem.current == null)
            {
                new GameObject("EventSystem",
                    typeof(EventSystem),
                    typeof(StandaloneInputModule)
                );
            }

            Canvas.overrideSorting = true;

            CanExitWindow = m_windowSettings.WindowFlowSettings.CanCloseWindow;

            if (AnimatedByDefault)
                CanvasGroup.alpha = 0f;

            if (AnimatedByDefault && FadeIn == null && FadeOut == null)
                SetBasicAnimation();

            EnableInteraction(true);
            AssignBackground();
            OnSafeEnable();
        }

        private void AssignBackground()
        {
            if (!m_windowSettings.BackgroundSettings.AutoAssignBackground || m_generatedBackground != null) return;

            GameObject background = new GameObject("Background",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement)
            );

            background.transform.SetParent(transform, false);
            background.transform.SetAsFirstSibling();

            var rect = background.GetComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            var img = background.GetComponent<Image>();

            img.color = m_windowSettings.BackgroundSettings.BackgroundColor;

            var btn = background.GetComponent<Button>();

            if (m_windowSettings.BackgroundSettings.BackgroundClosesWindow)
                btn.onClick.AddListener(() => ForcePopWindow());

            btn.transition = Selectable.Transition.None;
            btn.onClick.AddListener(() => onBackgroundClicked?.Invoke());

            var layout = background.GetComponent<LayoutElement>();
            layout.ignoreLayout = true;

            m_generatedBackgroundcanvasGroup = background.AddComponent<CanvasGroup>();
            m_generatedBackground = background;
        }

        /// <summary>
        /// If CanExit is false, the window can only be closed manually with 'ForcePopWindow'
        /// Otherwise it can be closed by multiple things like the escape key, pressing on the background, etc
        /// </summary>
        /// <param name="canExit">CanExit new value</param>
        public void SetCanExit(bool canExit)
        {
            CanExitWindow = canExit;
        }

        /// <summary>
        /// Pops this window
        /// </summary>
        /// <returns>Returns true if successful, this depends on 'CanExit' state</returns>
        public bool PopWindow(AnimationDelegade animation = null)
        {
            if (CanExitWindow)
            {
                ForcePopWindow(animation);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Pops this window, always succeeds
        /// </summary>
        public void ForcePopWindow(AnimationDelegade animation = null)
        {
            Windinator.Pop(this, animation);
        }

        /// <summary>
        /// Enable interaction with the user and this window
        /// </summary>
        /// <param name="enable">Can interact if true</param>
        public void EnableInteraction(bool enable)
        {
            CanvasGroup.blocksRaycasts = enable;
            CanvasGroup.interactable = enable;
        }

        /// <summary>
        /// Is the window culled? Aka. Invisible.
        /// </summary>
        /// <value>Culled</value>
        public bool IsCulled { get; private set; }

        /// <summary>
        /// Change the window culling mode.
        /// </summary>
        /// <param name="cull">Cull it?</param>
        public void CullWindow(bool cull)
        {
            IsCulled = cull;
            bool show = !cull;

            EnableInteraction(show);
            Canvas.enabled = show;
        }

        /// <summary>
        /// Changes the in & out transition duration
        /// </summary>
        /// <param name="duration">New duration</param>
        public void SetAnimationDuration(float duration)
        {
            m_windowSettings.AnimationSettings.TransitionAnimDuration = duration;
        }

        /// <summary>
        /// Sets the fade in & fade out to a basic linear fade animation
        /// If you want more control check out 'SetOpenCloseAnimation' and 'WindinatorAnimations'
        /// </summary>
        public void SetBasicAnimation()
        {
            FadeIn = FadeInSin;
            FadeOut = FadeOutSin;
        }

        /// <summary>
        /// Sets the fade in & fade out to a basic linear fade animation
        /// If you want more control check out 'SetOpenCloseAnimation' and 'WindinatorAnimations'
        /// </summary>
        public void ClearAnimations()
        {
            FadeIn = null;
            FadeOut = null;
        }

        /// <summary>
        /// Example usage:
        /// SetOpenCloseAnimation(WindinatorAnimations.FadeInLinear, WindinatorAnimations.FadeOutLinear);
        /// </summary>
        /// <param name="fadeIn">Fade in animation/transition</param>
        /// <param name="fadeOut">Fade out animation/transition</param>
        public void SetOpenCloseAnimation(AnimationDelegade fadeIn, AnimationDelegade fadeOut)
        {
            FadeIn = fadeIn;
            FadeOut = fadeOut;
        }
    }
}