using Riten;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Riten.Windinator.WindinatorAnimations;

namespace Riten.Windinator
{
    public class Windinator : MonoBehaviour
    {
        static Windinator singleton;

        static WindinatorConfig _WindinatorConfig;

        public static WindinatorConfig WindinatorConfig
        {
            get
            {
                if (_WindinatorConfig == null)
                    _WindinatorConfig = Resources.Load<WindinatorConfig>("WindinatorConfig");

                return _WindinatorConfig;
            }
        }

        static Windinator Instance
        {
            get
            {
                if (singleton == null)
                {
                    GameObject go = new GameObject("[Bootstrap] Windinator");
                    singleton = go.AddComponent<Windinator>();
                    singleton.m_windinatorConfig =
                        Resources.Load<WindinatorConfig>("WindinatorConfig");
                    singleton.Bootstrap();
                }

                return singleton;
            }
        }

        /// <summary>
        /// Returns whether there is any screens pushed.
        /// </summary>
        public static bool IsEmpty => Instance.m_windows.Count == 0;

        /// <summary>
        /// Returns the ammount of pushed windows
        /// </summary>
        public static int WindowCount => Instance.m_windows.Count;

        WindinatorConfig m_windinatorConfig;

        WindinatorPool m_windowPool;

        WindinatorAnimator m_animator = new WindinatorAnimator();

        List<WindinatorBehaviour> m_windows = new List<WindinatorBehaviour>();

        Queue<Action> m_nextFrame = new Queue<Action>();

        private int m_shouldBlockGameFlow = 0;

        public static Color GetColor(Colors color)
        {
            if (WindinatorConfig == null) return Color.white;
            return WindinatorConfig.ColorPalette[color];
        }

        public static bool Warmup()
        {
            return Instance != null;
        }

        public static void RunNextFrame(Action action)
        {
            Instance.m_nextFrame.Enqueue(action);
        }

        public static void ClearAnimations(WindinatorBehaviour window)
        {
            Instance.m_animator.Clear(window);
        }

        public static void SetupCanvas(Canvas canvas, CanvasScaler scaler)
        {
            if (!Application.isPlaying && WindinatorConfig == null) return;

            RectTransform transform = canvas.transform as RectTransform;

            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;
            transform.localScale = Vector3.one;

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = WindinatorConfig.StartingCanvasLayer;
            canvas.pixelPerfect = WindinatorConfig.CanvasSettings.PixelPerfect;
            canvas.additionalShaderChannels = 
                AdditionalCanvasShaderChannels.Normal |
                AdditionalCanvasShaderChannels.Tangent |
                AdditionalCanvasShaderChannels.TexCoord1 |
                AdditionalCanvasShaderChannels.TexCoord2 |
                AdditionalCanvasShaderChannels.TexCoord3;

            var scalerSettings = WindinatorConfig.ScalerSettings;
            scaler.uiScaleMode = scalerSettings.UIScaleMode;
            scaler.referenceResolution = scalerSettings.ReferenceResolution;
            scaler.screenMatchMode = scalerSettings.ScreenMatchMode;
            scaler.matchWidthOrHeight = scalerSettings.Match;
            scaler.physicalUnit = scalerSettings.Physicalunit;
            scaler.fallbackScreenDPI = scalerSettings.FallBackScreenDPI;
            scaler.defaultSpriteDPI = scalerSettings.DefaultSpriteDPI;
            scaler.scaleFactor = scalerSettings.ScaleFactor;
            scaler.referencePixelsPerUnit = scalerSettings.ReferencePixelsPerUnit;
        }

        public void Bootstrap()
        {
            m_windowPool = new WindinatorPool(m_windinatorConfig.OptimizePooling);

            Canvas = gameObject.AddComponent<Canvas>();
            var scaler = gameObject.AddComponent<CanvasScaler>();

            SetupCanvas(Canvas, scaler);

            foreach(var window in m_windinatorConfig.Windows)
            {
                var settings = window.GetOptimizationSettings();
                if (settings.WarmupCount > 0)
                {
                    for (int i = 0; i < settings.WarmupCount; i++)
                    {
                        var w = m_windowPool.PreAllocate(window.GetType(), window.gameObject, transform);
                        w.enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Used for manual animation handling
        /// </summary>
        /// <param name="window">Window to animate</param>
        /// <param name="anim">Animation</param>
        /// <param name="onDone">Callback when animation finishes</param>
        public static void Animate(WindinatorBehaviour window, WindinatorAnimations.AnimationDelegade anim, Action onDone = null)
        {
            Instance.m_animator.Animate(window, anim, onDone);
        }

        /// <summary>
        /// This indicated if you should stop input for things happening in the background.
        /// For example if your player should stop moving when a window is opened.
        /// </summary>
        public static bool ShoudBlockGameFlow => Instance.m_shouldBlockGameFlow > 0;

        public static Canvas Canvas { get; private set; }

        /// <summary>
        /// Pushes a new window to the stack, it will be on top.
        /// </summary>
        public static WindinatorBehaviour PushPrefab(WindinatorBehaviour prefab, AnimationDelegade animation = null)
        {
            var instance = Instance;
            var config = WindinatorConfig;

            var window = instance.m_windowPool.Allocate(prefab.GetType(), prefab.gameObject, instance.transform);
            window.PreAwake();
            window.Canvas.sortingOrder = config.StartingCanvasLayer + instance.m_windows.Count;
            instance.m_windows.Add(window);

            if (window.ShoudBlockGameFlow)
                instance.m_shouldBlockGameFlow += 1;

            if (window.AnimatedByDefault || animation != null)
            {
                instance.m_nextFrame.Enqueue(() =>
                {
                    if (window.gameObject.activeSelf && (window.FadeIn != null || animation != null))
                    {
                        instance.m_animator.Animate(window, animation == null ? window.FadeIn : animation, () =>
                        {
                            window.CanvasGroup.alpha = 1f;
                            UpdateVisibility();
                        });
                    }
                    else
                    {
                        window.CanvasGroup.alpha = 1f;
                        UpdateVisibility();
                    }
                });
            }
            else UpdateVisibility();

            return window;
        }

        /// <summary>
        /// Pushes a new window to the stack, it will be on top.
        /// </summary>
        public static T Push<T>(AnimationDelegade animation = null) where T : WindinatorBehaviour
        {
            var instance = Instance;
            var config = WindinatorConfig;

            foreach (var w in config.Windows)
            {
                if (w == null) continue;

                if (w is T)
                {
                    return (T)PushPrefab(w, animation);
                }
            }

            Debug.LogError($"[<b>Windinator</b>] Failed to find {typeof(T).Name}, try to Link the prefab again. (@Windinator/Link Selected Prefabs)");

            return null;
        }

        /// <summary>
        /// Returns the baked prefab of an element.
        /// </summary>
        /// <typeparam name="T">Element's type</typeparam>
        /// <returns>The element or NULL if not found.</returns>
        internal static GameObject GetElementPrefab<T>()
        {
            var instance = Instance;
            var config = WindinatorConfig;

            foreach (var p in config.Prefabs)
            {
                if (p == null) continue;

                if (p is T)
                {
                    return p.gameObject;
                }
            }

            return null;
        }

        /// <summary>
        /// Replace the top most window with new window
        /// </summary>
        /// <typeparam name="T">New window</typeparam>
        /// <returns></returns>
        public static T Replace<T>() where T : WindinatorBehaviour
        {
            Pop(true);
            return Push<T>();
        }

        /// <summary>
        /// Pops top most window.
        /// </summary>
        public static void Pop(bool force = false, AnimationDelegade animation = null)
        {
            var instance = Instance;
            var windows = instance.m_windows;

            if (windows.Count == 0) return;

            var top = windows[windows.Count - 1];

            if (force) top.ForcePopWindow(animation);
            else       top.PopWindow(animation);
        }

        /// <summary>
        /// Pops a specific instance of an window.
        /// </summary>
        /// <param name="target">The window instance</param>
        public static void Pop(WindinatorBehaviour target, AnimationDelegade animation = null)
        {
            var instance = Instance;
            if (instance.m_windows.Contains(target))
            {
                int id = instance.m_windows.IndexOf(target);
                var window = instance.m_windows[id];
                
                window.EnableInteraction(false);
                target.OnWindowClosedEvent();
                window.OnSafeDisable();

                if (window.ShoudBlockGameFlow)
                    instance.m_shouldBlockGameFlow = Mathf.Max(0, instance.m_shouldBlockGameFlow - 1);

                if (window.FadeOut != null || animation != null)
                {
                    instance.m_animator.Animate(window, animation == null ? window.FadeOut : animation,
                        () => {
                            instance.m_windowPool.Free(window);
                            UpdateVisibility();
                        }
                    );
                }
                else
                {
                    instance.m_windowPool.Free(window);
                }

                instance.m_windows.RemoveAt(id);
                instance.UpdateSorting(id);

                UpdateVisibility();
            }
        }

        /// <summary>
        /// Moves window to the top of the stack. Leaving all other windows behind.
        /// </summary>
        /// <param name="target">Reference to the window.</param>
        public static void MoveToTop(WindinatorBehaviour target)
        {
            var instance = Instance;
            int index = instance.m_windows.IndexOf(target);

            if (index >= 0)
            {
                instance.m_windows.RemoveAt(index);
                instance.m_windows.Add(target);
                instance.UpdateSorting(index);
            }
        }

        public static void UpdateVisibility()
        {
            var m_windows = Instance.m_windows;
            bool cull = false;

            for (int i = m_windows.Count - 1; i >= 0; --i)
            {
                var window = m_windows[i];

                if (cull != window.IsCulled)
                    window.CullWindow(cull);

                if (!cull && window.CullBackgroundWindows && !window.IsAnimating)
                    cull = true;
            }
        }

        void UpdateSorting(int startId)
        {
            for (int i = startId; i < m_windows.Count; ++i)
                m_windows[i].Canvas.sortingOrder = m_windinatorConfig.StartingCanvasLayer + i++;
        }

        void Update()
        {
            bool escapePressed = false;

#if ENABLE_LEGACY_INPUT_MANAGER
            escapePressed = Input.GetKeyDown(KeyCode.Escape);
#elif ENABLE_INPUT_SYSTEM

            var currentKeyboard = UnityEngine.InputSystem.Keyboard.current;
            if (currentKeyboard != null)
                escapePressed = currentKeyboard.escapeKey.wasPressedThisFrame;
#endif
            // This code handles closing windows with the escape key
            if (m_windinatorConfig.CloseWindowsWithEscape && escapePressed && m_windows.Count > 0)
            {
                Pop();
            }

            m_animator.Update(Time.deltaTime);

            while (m_nextFrame.Count > 0) m_nextFrame.Dequeue()?.Invoke();
        }
    }
}