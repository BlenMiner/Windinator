using UnityEngine;

namespace Riten.Windinator.Shapes
{
    [System.Serializable]
    public abstract class ShapeDrawer
    {
        public static GenericPool<StaticArray<Vector4>> ArrayPool = new GenericPool<StaticArray<Vector4>>(
            () => { return new StaticArray<Vector4>(512); }
        );

        public ShapeDrawer(CanvasGraphic canvas)
        {
            Canvas = canvas;
            Material = new Material(Shader.Find(MaterialName));
        }

        protected CanvasGraphic Canvas;

        public abstract string MaterialName { get; }

        public Material Material;

        protected void SetupMaterial(float blend = 0f, DrawOperation operation = DrawOperation.Union)
        {
            Material.SetFloat("_Union", blend);
            Material.SetFloat("_Operation", (int)operation);

            Material.SetTexture("_MainTexture", Canvas.CurrentBuffer);
            Material.SetVector("_Size", Canvas.Size);
            Material.SetFloat("_Padding", Canvas.Margin);
        }

        protected void DrawBatchInternal(float blend = 0f, DrawOperation operation = DrawOperation.Union)
        {
            SetupMaterial(blend, operation);
            DrawBatches();
        }

        protected void Dispatch()
        {
            Graphics.Blit(Canvas.CurrentBuffer, Canvas.BackBuffer, Material);
            Canvas.SwitchBuffers();
        }

        protected abstract void DrawBatches();
    }
}