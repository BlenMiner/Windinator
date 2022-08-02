using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator.Shapes
{
    [System.Serializable]
    public class RectDrawer : ShapeDrawer
    {
        List<StaticArray<Vector4>> m_batchedData;

        List<StaticArray<Vector4>> m_batchedDataExtra;

        List<StaticArray<Vector4>> m_batchedDataExtra2;

        public RectDrawer(CanvasGraphic canvas) : base(canvas)
        {
            m_batchedData = new List<StaticArray<Vector4>>();
            m_batchedDataExtra = new List<StaticArray<Vector4>>();
            m_batchedDataExtra2 = new List<StaticArray<Vector4>>();
        }

        public override string MaterialName => "UI/Windinator/DrawRect";

        protected override void DrawBatches()
        {
            if (m_batchedData.Count == 0) return;

            for (int i = 0; i < m_batchedData.Count; ++i)
            {
                var batch = m_batchedData[i];
                var extra = m_batchedDataExtra[i];
                var extra2 = m_batchedDataExtra2[i];

                Material.SetVectorArray("_Points", batch.Array);
                Material.SetVectorArray("_PointsExtra", extra2.Array);
                Material.SetVectorArray("_PointsExtra2", extra.Array);
                Material.SetInt("_PointsCount", batch.Length);

                Dispatch();

                batch.Length = 0;
                extra.Length = 0;
                extra2.Length = 0;

                ArrayPool.Free(batch);
                ArrayPool.Free(extra);
                ArrayPool.Free(extra2);
            }

            m_batchedData.Clear();
            m_batchedDataExtra.Clear();
            m_batchedDataExtra2.Clear();
        }

        public void Draw(Vector2 center, Vector2 size, Vector4 roundness = default, float blend = default, DrawOperation operation = DrawOperation.Union)
        {
            m_tmp.x = center.x;
            m_tmp.y = center.y;
            m_tmp.z = size.x;
            m_tmp.w = size.y;

            var array = ArrayPool.Allocate();
            var extra = ArrayPool.Allocate();
            var extra2 = ArrayPool.Allocate();

            array.Add(m_tmp);
            extra.Add(roundness);

            m_tmp.x = blend;

            extra2.Add(m_tmp);

            SetupMaterial(blend, operation);

            Material.SetVectorArray("_Points", array.Array);
            Material.SetVectorArray("_PointsExtra", extra.Array);
            Material.SetVectorArray("_PointsExtra2", extra2.Array);
            Material.SetInt("_PointsCount", array.Length);

            array.Length = 0;
            extra.Length = 0;
            extra2.Length = 0;

            ArrayPool.Free(array);
            ArrayPool.Free(extra);
            ArrayPool.Free(extra2);

            Dispatch();
        }

        Vector4 m_tmp;

        public void AddBatch(Vector2 center, Vector2 size, Vector4 roundness = default, float blend = 0f)
        {
            #if UNITY_EDITOR
            if (m_batchedData == null)
                m_batchedData = new List<StaticArray<Vector4>>();
            if (m_batchedDataExtra == null)
                m_batchedDataExtra = new List<StaticArray<Vector4>>();
            if (m_batchedDataExtra2 == null)
                m_batchedDataExtra2 = new List<StaticArray<Vector4>>();
            #endif

            if (m_batchedData.Count == 0 || m_batchedData[^1].Length >= m_batchedData.Capacity)
            {
                m_batchedData.Add(ArrayPool.Allocate());
                m_batchedDataExtra.Add(ArrayPool.Allocate());
                m_batchedDataExtra2.Add(ArrayPool.Allocate());
            }

            m_tmp.x = center.x;
            m_tmp.y = center.y;
            m_tmp.z = size.x;
            m_tmp.w = size.y;

            m_batchedData[^1].Add(m_tmp);
            m_batchedDataExtra[^1].Add(roundness);

            m_tmp.x = blend;

            m_batchedDataExtra2[^1].Add(m_tmp);
        }

        public void DrawBatch(DrawOperation operation = DrawOperation.Union)
        {
            DrawBatchInternal(0, operation);
        }
    }
}