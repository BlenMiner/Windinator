using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator.Shapes
{
    [System.Serializable]
    public class RectDrawer : ShapeDrawer
    {
        List<StaticArray<Vector4>> m_batchedData;

        List<StaticArray<Vector4>> m_transformData;

        List<StaticArray<Vector4>> m_batchedDataExtra;

        List<StaticArray<Vector4>> m_batchedDataExtra2;

        public RectDrawer(CanvasGraphic canvas) : base(canvas)
        {
            m_transformData = new List<StaticArray<Vector4>>();
            m_batchedData = new List<StaticArray<Vector4>>();
            m_batchedDataExtra = new List<StaticArray<Vector4>>();
            m_batchedDataExtra2 = new List<StaticArray<Vector4>>();
        }

        public override string MaterialName => "UI/Windinator/DrawRect";

        protected override void DrawBatches(LayerGraphic layer = null)
        {
            if (m_batchedData.Count == 0) return;

            for (int i = 0; i < m_batchedData.Count; ++i)
            {
                var batch = m_batchedData[i];
                var extra = m_batchedDataExtra[i];
                var extra2 = m_batchedDataExtra2[i];
                var transform = m_transformData[i];

                Material.SetVectorArray("_Transform", transform.Array);
                Material.SetVectorArray("_Points", batch.Array);
                Material.SetVectorArray("_PointsExtra", extra2.Array);
                Material.SetVectorArray("_PointsExtra2", extra.Array);
                Material.SetInt("_PointsCount", batch.Length);

                Dispatch(layer);

                transform.Length = 0;
                batch.Length = 0;
                extra.Length = 0;
                extra2.Length = 0;

                ArrayPool.Free(transform);
                ArrayPool.Free(batch);
                ArrayPool.Free(extra);
                ArrayPool.Free(extra2);
            }

            m_batchedData.Clear();
            m_batchedDataExtra.Clear();
            m_batchedDataExtra2.Clear();
        }

        public void Draw(Vector2 center, Vector2 size, Vector4 roundness = default, float blend = default, float rotationInRadian = 0f, DrawOperation operation = DrawOperation.Union, LayerGraphic layer = null)
        {
            m_tmp.x = center.x;
            m_tmp.y = center.y;
            m_tmp.z = size.x;
            m_tmp.w = size.y;

            var array = ArrayPool.Allocate();
            var extra = ArrayPool.Allocate();
            var extra2 = ArrayPool.Allocate();
            var transform = ArrayPool.Allocate();

            transform.Add(new Vector4(rotationInRadian, 0, 0, 0f));

            array.Add(m_tmp);
            extra.Add(roundness);

            m_tmp.x = blend;

            extra2.Add(m_tmp);

            SetupMaterial(blend, operation, layer);

            Material.SetVectorArray("_Transform", transform.Array);
            Material.SetVectorArray("_Points", array.Array);
            Material.SetVectorArray("_PointsExtra", extra.Array);
            Material.SetVectorArray("_PointsExtra2", extra2.Array);
            Material.SetInt("_PointsCount", array.Length);

            transform.Length = 0;
            array.Length = 0;
            extra.Length = 0;
            extra2.Length = 0;

            ArrayPool.Free(transform);
            ArrayPool.Free(array);
            ArrayPool.Free(extra);
            ArrayPool.Free(extra2);

            Dispatch(layer);
        }

        Vector4 m_tmp;

        public void AddBatch(Vector2 center, Vector2 size, Vector4 roundness = default, float blend = 0f, float rotationInRadian = 0f)
        {
            #if UNITY_EDITOR
            if (m_batchedData == null)
                m_batchedData = new List<StaticArray<Vector4>>();
            if (m_batchedDataExtra == null)
                m_batchedDataExtra = new List<StaticArray<Vector4>>();
            if (m_batchedDataExtra2 == null)
                m_batchedDataExtra2 = new List<StaticArray<Vector4>>();
            if (m_transformData == null)
                m_transformData = new List<StaticArray<Vector4>>();
            #endif

            if (m_batchedData.Count == 0 || m_batchedData[m_batchedData.Count - 1].Length >= m_batchedData.Capacity)
            {
                m_batchedData.Add(ArrayPool.Allocate());
                m_batchedDataExtra.Add(ArrayPool.Allocate());
                m_batchedDataExtra2.Add(ArrayPool.Allocate());
                m_transformData.Add(ArrayPool.Allocate());
            }

            m_tmp.x = center.x;
            m_tmp.y = center.y;
            m_tmp.z = size.x;
            m_tmp.w = size.y;

            m_batchedData[m_batchedData.Count - 1].Add(m_tmp);
            m_batchedDataExtra[m_batchedDataExtra.Count - 1].Add(roundness);

            m_transformData[m_transformData.Count - 1].Add(new Vector4(rotationInRadian, 0, 0, 0f));

            m_tmp.x = blend;

            m_batchedDataExtra2[m_batchedDataExtra2.Count - 1].Add(m_tmp);
        }

        public void DrawBatch(DrawOperation operation = DrawOperation.Union, LayerGraphic layer = null)
        {
            DrawBatchInternal(0, operation, layer);
        }
    }
}