using Riten.Windinator;
using UnityEngine;

public class WindinatorBootstrapper : MonoBehaviour
{
    [SerializeField, Header("This just pushes a window prefab on Awake for convinience purposes.")]
    WindinatorBehaviour m_rootWindow;

    private void Awake()
    {
        Windinator.PushPrefab(m_rootWindow);
    }
}
