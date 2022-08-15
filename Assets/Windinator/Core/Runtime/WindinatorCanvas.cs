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
        Windinator.SetupCanvas(GetComponent<Canvas>(), GetComponent<CanvasScaler>());
    }
}
