using Riten.Windinator;
using UnityEngine;

public class WindinatorWarmuper : MonoBehaviour
{
    private void Awake()
    {
        Windinator.Warmup();
    }
}
