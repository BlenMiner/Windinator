using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riten.Windinator;
using UnityEngine.Experimental.GlobalIllumination;

public class BootstrapUI : MonoBehaviour
{
    void Start()
    {
        var window = Windinator.Push<ExampleWindow>();
        window.Setup("BlenMiner");
    }
}
