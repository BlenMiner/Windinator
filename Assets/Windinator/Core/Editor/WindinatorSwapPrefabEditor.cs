#if UNITY_2018_3_OR_NEWER
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class WindinatorSwapPrefabEditor
{
    static WindinatorSwapPrefabEditor()
    {
        EditorSettings.prefabUIEnvironment = Resources.Load<SceneAsset>("CanvasEnvironment");
    }
}
#endif
