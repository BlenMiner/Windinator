using UnityEngine;
using System.Collections.Generic;
using Riten.Windinator.LayoutBuilder;

namespace Riten.Windinator
{
    public class WindinatorConfig : ScriptableObject
    {
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