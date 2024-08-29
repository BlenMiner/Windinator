using UnityEngine;
using UnityEngine.UI;

namespace Riten.Windinator.Shapes
{
    public struct ColorProperties
    {
        public Texture2D Texture;
        public Color Color;
        public Image.Type Type;

        public ColorProperties(Texture2D texture)
        {
            Texture = texture;
            Color = Color.white;
            Type = Image.Type.Filled;
        }
    }

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

        protected void SetupMaterial(float blend = 0f, DrawOperation operation = DrawOperation.Union, LayerGraphic layer = null)
        {
            Material.SetFloat("_Union", blend);
            Material.SetFloat("_Operation", (int)operation);

            Material.SetTexture("_MainTexture", Canvas.GetLayer(layer).Texture);
            Material.SetVector("_Size", Canvas.Size);
            Material.SetFloat("_Padding", Canvas.Margin);
        }

        protected void DrawBatchInternal(float blend = 0f, DrawOperation operation = DrawOperation.Union, LayerGraphic layer = null)
        {
            SetupMaterial(blend, operation, layer);
            DrawBatches(layer);
        }

        protected void Dispatch(LayerGraphic layer = null)
        {
            Canvas.GetLayer(layer).Blit(Material);
        }

        protected void DispatchColor(LayerGraphic layer = null)
        {
            Canvas.GetLayer(layer).BlitColor(Material);
        }

        protected abstract void DrawBatches(LayerGraphic layer = null);
    }
}