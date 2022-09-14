using Riten.Windinator.Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator.Shapes
{
    public class PolyDrawer : ShapeDrawer
    {
        public override string MaterialName => "UI/Windinator/DrawPoly";

        StaticArray<Vector4> m_points;

        public PolyDrawer(CanvasGraphic canvas) : base(canvas)
        {
            m_points = new StaticArray<Vector4>(1023);
        }

        public void AddPoint(Vector2 point)
        {
            if (m_points.Length >= 1023) return;
            m_points.Add(new Vector4(point.x, point.y, 0, 0));
        }

        public void Draw(float roundness = 0f, float blend = 0f, DrawOperation operation = DrawOperation.Union, LayerGraphic layer = null)
        {
            if (m_points.Length == 0) return;

            SetupMaterial(blend, operation, layer);

            Material.SetVector("_Roundness", new Vector4(roundness, 0, 0, 0));
            Material.SetVectorArray("_Points", m_points.Array);
            Material.SetInt("_PointsCount", m_points.Length);

            m_points.Length = 0;

            Dispatch(layer);
        }

        protected override void DrawBatches(LayerGraphic layer = null)
        {
            Debug.LogError("[Windinator.Shapes] Poly Brush doesn't support batching");
        }
    }

}