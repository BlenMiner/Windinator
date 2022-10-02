using Riten.Windinator;
using UnityEngine;
using UnityEngine.UI;

public class WindinatorCanvas : MonoBehaviour
{
    private void OnValidate()
    {
        UpdateCanvas();
    }

    private void Reset()
    {
        UpdateCanvas();
    }

    void UpdateCanvas()
    {
        var canvas = GetComponent<Canvas>();

        canvas.additionalShaderChannels =
                AdditionalCanvasShaderChannels.Normal |
                AdditionalCanvasShaderChannels.Tangent |
                AdditionalCanvasShaderChannels.TexCoord1 |
                AdditionalCanvasShaderChannels.TexCoord2 |
                AdditionalCanvasShaderChannels.TexCoord3;
    }
}
