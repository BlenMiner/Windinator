using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Riten.Windinator.LayoutBuilder;

namespace Riten.Windinator
{
    [System.Serializable]
    public struct CanvasPreset
    {
        public bool PixelPerfect;
    }

    [System.Serializable]
    public class CanvasScalerPreset
    {
        public UnityEngine.UI.CanvasScaler.ScaleMode UIScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ConstantPixelSize;

        public Vector2 ReferenceResolution = new Vector2(800, 600);

        public UnityEngine.UI.CanvasScaler.ScreenMatchMode ScreenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        [Range(0f, 1f)]
        public float Match = 0.5f;

        public UnityEngine.UI.CanvasScaler.Unit Physicalunit = UnityEngine.UI.CanvasScaler.Unit.Points;

        public float FallBackScreenDPI = 96f;

        public float DefaultSpriteDPI = 96f;

        public float ScaleFactor = 1f;

        public float ReferencePixelsPerUnit = 100f;
    }

    public class WindinatorConfig : ScriptableObject
    {
        [Header("Canvas Settings")]
        public CanvasPreset CanvasSettings;

        public CanvasScalerPreset ScalerSettings;

        [Header("General Settings")]
        [Tooltip("Where does sorting order start for new canvases")]
        public int StartingCanvasLayer = 1;

        public bool CloseWindowsWithEscape = true;

        public Color DefaultBackgroundColor = new Color(0, 0, 0, 0.5f);

        [Header("Style To Use")]
        public ColorAssigner ColorPalette;

        [Header("Optimization Settings")]
        [Tooltip("Instead of disabling the GameObject just cull it and disable the window component")]
        public bool OptimizePooling = true;

        [Header("Automatic References")]
        [Tooltip("A list of all your windows, this works as a lookup table"), Header("List of all your windows")]
        public List<WindinatorBehaviour> Windows;

        public List<LayoutBaker> Prefabs;

        private void Reset()
        {
            ColorPalette = Resources.Load<ColorAssigner>("Windinator.Material.Palettes/Light Baseline Palette");
        }
    }
}