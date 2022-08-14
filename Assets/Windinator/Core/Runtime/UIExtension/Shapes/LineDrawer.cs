using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator.Shapes
{
    [System.Serializable]
    public class LineDrawer : ShapeDrawer
    {
        public override string MaterialName => "UI/Windinator/DrawLine";

        List<StaticArray<Vector4>> m_batchedData;

        public LineDrawer(CanvasGraphic canvas) : base(canvas)
        {
            m_batchedData = new List<StaticArray<Vector4>>();
        }

        protected override void DrawBatches(LayerGraphic layer = null)
        {
            if (m_batchedData.Count == 0) return;

            foreach(var batch in m_batchedData)
            {
                Material.SetVectorArray("_Points", batch.Array);
                Material.SetInt("_PointsCount", batch.Length);

                Dispatch(layer);

                batch.Length = 0;
                ArrayPool.Free(batch);
            }

            m_batchedData.Clear();
        }

        public void Draw(Vector2 a, Vector2 b, float thickness, float blend = 0f, DrawOperation operation = DrawOperation.Union, LayerGraphic layer = null)
        {
            m_tmp.x = a.x;
            m_tmp.y = a.y;
            m_tmp.z = b.x;
            m_tmp.w = b.y;

            var array = ArrayPool.Allocate();
            array.Add(m_tmp);

            SetupMaterial(blend, operation, layer);

            Material.SetFloat("_LineThickness", thickness);
            Material.SetVectorArray("_Points", array.Array);
            Material.SetInt("_PointsCount", array.Length);

            array.Length = 0;
            ArrayPool.Free(array);

            Dispatch(layer);
        }

        Vector4 m_tmp;

        public void AddBatch(Vector2 a, Vector2 b)
        {
            #if UNITY_EDITOR
            if (m_batchedData == null)
                m_batchedData = new List<StaticArray<Vector4>>();
            #endif
            if (m_batchedData.Count == 0 || m_batchedData[m_batchedData.Count - 1].Length >= m_batchedData.Capacity)
                m_batchedData.Add(ArrayPool.Allocate());

            m_tmp.x = a.x;
            m_tmp.y = a.y;
            m_tmp.z = b.x;
            m_tmp.w = b.y;

            m_batchedData[m_batchedData.Count - 1].Add(m_tmp);
        }

        public void DrawBatch(float thickness, float blend = 0f, DrawOperation operation = DrawOperation.Union, LayerGraphic layer = null)
        {
            Material.SetFloat("_LineThickness", thickness);
            DrawBatchInternal(blend, operation, layer);
        }
    }
}