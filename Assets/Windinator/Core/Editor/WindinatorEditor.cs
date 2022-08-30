using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Riten.Windinator.LayoutBuilder;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Riten.Windinator
{
    public static class WindinatorEditorUtils
    {
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for(int i =0;i<guids.Length;i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }
    }

    public class WindinatorEditor : Editor
    {
        const string TEMPLATE_SRC =
    @"using UnityEngine;
using Riten.Windinator;

public class {0} : WindinatorBehaviour
{{
    // We recommend a setup function to pass in your data
    public void Setup(/* int exampleParam */)
    {{

    }}
    
    // Use your usual Unity callback if you need, aka void Start(), Update(), etc.
}}
";

        const string LAYOUT_TEMPLATE_SRC =
    @"using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;

public class {0} : LayoutBaker
{{
    public override Element Bake()
    {{
        return null; // Place your elements here
    }}

    // Use your usual Unity callback if you need, aka void Start(), Update(), etc.
}}
";
        public static Type TypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }

            return null;
        }

        static void Error(string msg)
        {
            Debug.LogError($"[<b>Windinator</b>] {msg}");
        }

        static void CleanList()
        {
            try
            {
                WindinatorConfig config = Resources.Load<WindinatorConfig>("WindinatorConfig");

                if (config.Windows == null) config.Windows = new List<WindinatorBehaviour>();

                for (int i = 0; i < config.Windows.Count; ++i)
                {
                    if (config.Windows[i] == null)
                        config.Windows.RemoveAt(i--);
                }

                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch
            {
                Error("Failed to clean prefabs, make sure 'WindinatorConfig' exists in a Resources folder.");
            }
        }

        static void RegisterWindow(WindinatorBehaviour comp)
        {
            if (comp == null) Error("Gameobject doesn't have a WindinatorBehaviour");
            try
            {
                WindinatorConfig config = Resources.Load<WindinatorConfig>("WindinatorConfig");

                CleanList();

                if (!config.Windows.Contains(comp))
                    config.Windows.Add(comp);

                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch
            {
                Error("Failed to register prefab, make sure 'WindinatorConfig' exists in a Resources folder.");
            }
        }

        static void RegisterPrefab(LayoutBaker comp)
        {
            if (comp == null) Error("Gameobject doesn't have a LayoutBaker");
            try
            {
                WindinatorConfig config = Resources.Load<WindinatorConfig>("WindinatorConfig");
                
                CleanList();

                if (config.Prefabs == null) config.Prefabs = new List<LayoutBaker>();

                for (int i = 0; i < config.Prefabs.Count; ++i)
                {
                    if (config.Prefabs[i] == null)
                        config.Prefabs.RemoveAt(i--);
                }

                if (!config.Prefabs.Contains(comp))
                    config.Prefabs.Add(comp);

                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch
            {
                Error("Failed to register prefab, make sure 'WindinatorConfig' exists in a Resources folder.");
            }
        }

        static string GetRelativePath(string absolutePath)
        {
            string dataPath = Application.dataPath.Replace('\\', '/');
            absolutePath = absolutePath.Replace('\\', '/');

            if (absolutePath.StartsWith(dataPath))
            {
                absolutePath = "Assets" + absolutePath.Substring(dataPath.Length);
            }
            return absolutePath;
        }

        static string GetCurrentFolder()
        {
            string folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if (folderPath.Contains("."))
                folderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
            return folderPath;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptsReloaded()
        {
            if(EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += OnScriptsReloaded;
                return;
            }
        
            LoadWindow();

            LoadElement();

            RefreshPrefabs();
        }

        private static void LoadElement()
        {
            string prefabToAdd = EditorPrefs.GetString("Windinator.Element", null);
            string className = EditorPrefs.GetString("Windinator.Element.Class", null);

            try
            {
                if (!string.IsNullOrWhiteSpace(prefabToAdd) &&
                    !string.IsNullOrWhiteSpace(className))
                {
                    GameObject go = PrefabUtility.LoadPrefabContents(prefabToAdd);
                    var newType = TypeByName(className);
                    go.AddComponent(newType);

                    var saved = PrefabUtility.SaveAsPrefabAsset(go, prefabToAdd);
                    PrefabUtility.UnloadPrefabContents(go);

                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                    // await Task.Delay(100);

                    GameObject reloadedGo = AssetDatabase.LoadAssetAtPath<GameObject>(prefabToAdd);
                    var prefab = reloadedGo.GetComponent<LayoutBaker>();

                    RegisterPrefab(prefab);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            }

            EditorPrefs.DeleteKey("Windinator.Element");
            EditorPrefs.DeleteKey("Windinator.Element.Class");
        }

        private static void LoadWindow()
        {
            string prefabToAdd = EditorPrefs.GetString("Windinator.Window", null);
            string className = EditorPrefs.GetString("Windinator.Class", null);

            try
            {
                if (!string.IsNullOrWhiteSpace(prefabToAdd) &&
                    !string.IsNullOrWhiteSpace(className))
                {
                    GameObject go = PrefabUtility.LoadPrefabContents(prefabToAdd);
                    var newType = TypeByName(className);
                    go.AddComponent(newType);

                    var saved = PrefabUtility.SaveAsPrefabAsset(go, prefabToAdd);
                    PrefabUtility.UnloadPrefabContents(go);

                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                    //await Task.Delay(100);

                    GameObject reloadedGo = AssetDatabase.LoadAssetAtPath<GameObject>(prefabToAdd);
                    var window = reloadedGo.GetComponent<WindinatorBehaviour>();

                    RegisterWindow(window);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            }

            EditorPrefs.DeleteKey("Windinator.Window");
            EditorPrefs.DeleteKey("Windinator.Class");
        }

        static SHA256 shaHash;

        static string GetSha256Hash(byte[] input)
        {
            if (shaHash == null) shaHash = SHA256.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = shaHash.ComputeHash(input);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        static void RefreshPrefabs(bool force = false)
        {
            WindinatorConfig config = Resources.Load<WindinatorConfig>("WindinatorConfig");

            if (config == null) return;

            for (int i = 0; i < config.Prefabs.Count; ++i)
                if (config.Prefabs[i] == null)
                    config.Prefabs.RemoveAt(i--);

            foreach (var p in config.Prefabs)
            {
                BakeElement(force, p.gameObject);
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void BakeElement(bool force, GameObject p)
        {
            string assetPath = AssetDatabase.GetAssetPath(p);

            var mono = p.GetComponent<LayoutBaker>();

            string oldHash = mono.ScriptHash;
            string hash = GetSha256Hash(MonoScript.FromMonoBehaviour(mono).bytes);

            if (hash == oldHash && !force)
                return;

            // The editing scope will automatically save, reimport prefab and unload contents
            using (var editingScope = new PrefabUtility.EditPrefabContentsScope(assetPath))
            {
                var builder = editingScope.prefabContentsRoot.GetComponent<LayoutBaker>();
                var script = MonoScript.FromMonoBehaviour(builder);

                builder.ScriptHash = hash;

                var cs = editingScope.prefabContentsRoot.GetComponent<ContentSizeFitter>();
                if (cs == null)
                {
                    cs = editingScope.prefabContentsRoot.AddComponent<ContentSizeFitter>();
                }
                cs.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                cs.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                cs.hideFlags = HideFlags.HideInInspector;

                var h = editingScope.prefabContentsRoot.GetComponent<HorizontalLayoutGroup>();
                if (h == null)
                    h = editingScope.prefabContentsRoot.AddComponent<HorizontalLayoutGroup>();
                h.childControlWidth = true;
                h.childControlHeight = true;
                h.childScaleWidth = false;
                h.childScaleHeight = false;
                h.childForceExpandWidth = false;
                h.childForceExpandHeight = false;
                h.hideFlags = HideFlags.HideInInspector;

                builder.ClearContents();
                builder.Build();
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Windinator/Force Re-bake Elements", priority = 200)]
        public static void Rebake()
        {
            RefreshPrefabs(true);
        }

        [MenuItem("Assets/Windinator/Re-bake Dirty Elements", priority = 200)]
        public static void RefreshElements()
        {
            RefreshPrefabs();
        }

        [MenuItem("Assets/Windinator/Windinator Config")]
        public static void NewConfig()
        {
            string folderPath = GetCurrentFolder();

            var savePath = EditorUtility.SaveFilePanel("Create windinator config file", folderPath, "WindinatorConfig", "asset");

            if (string.IsNullOrWhiteSpace(savePath)) return;

            savePath = GetRelativePath(savePath);

            AssetDatabase.CreateAsset(CreateInstance<WindinatorConfig>(), savePath);
        }

        [MenuItem("Assets/Windinator/New Window", priority = -100)]
        public static void NewWindow()
        {
            string folderPath = GetCurrentFolder();

            var savePath = EditorUtility.SaveFilePanel("Save window script", folderPath, "NewWindow", "cs");

            if (string.IsNullOrWhiteSpace(savePath)) return;

            FileInfo fileInfo = new FileInfo(savePath);
            string name = fileInfo.Name.Replace(' ', '_');
            var ext = fileInfo.Extension;

            string className = name.Substring(0, name.Length - ext.Length);

            File.WriteAllText(savePath, string.Format(TEMPLATE_SRC, className));

            savePath = GetRelativePath(savePath);

            AssetDatabase.ImportAsset(savePath);

            GameObject window = new GameObject($"[W] {className}",
                typeof(Canvas),
                typeof(UnityEngine.UI.CanvasScaler),
                typeof(GraphicRaycaster)
            );

            var c = window.GetComponent<Canvas>();
            var cs = window.GetComponent<CanvasScaler>();

            Windinator.SetupCanvas(c, cs);

            string prefab = $"{fileInfo.Directory.FullName}/[W] {className}.prefab";
            PrefabUtility.SaveAsPrefabAsset(window, prefab);
            EditorPrefs.SetString("Windinator.Window", GetRelativePath(prefab));
            EditorPrefs.SetString("Windinator.Class", className);

            DestroyImmediate(window);
        }

        [MenuItem("Assets/Windinator/New Element", priority = -100)]
        public static void NewElement()
        {
            string folderPath = GetCurrentFolder();

            var savePath = EditorUtility.SaveFilePanel("Save element script", folderPath, "NewElement", "cs");

            if (string.IsNullOrWhiteSpace(savePath)) return;

            FileInfo fileInfo = new FileInfo(savePath);
            string name = fileInfo.Name.Replace(' ', '_');
            var ext = fileInfo.Extension;

            string className = name.Substring(0, name.Length - ext.Length);

            File.WriteAllText(savePath, string.Format(LAYOUT_TEMPLATE_SRC, className));

            savePath = GetRelativePath(savePath);

            AssetDatabase.ImportAsset(savePath);

            GameObject window = new GameObject($"[E] {className}",
                typeof(RectTransform)
            );

            var transform = window.GetComponent<RectTransform>();

            transform.sizeDelta = new Vector2(800, 600);

            string prefab = $"{fileInfo.Directory.FullName}/[E] {className}.prefab";
            PrefabUtility.SaveAsPrefabAsset(window, prefab);
            EditorPrefs.SetString("Windinator.Element", GetRelativePath(prefab));
            EditorPrefs.SetString("Windinator.Element.Class", className);

            DestroyImmediate(window);
        }

        [MenuItem("Assets/Windinator/Link Selected Prefabs", validate = true)]
        public static bool LinkAvailable()
        {
            bool validSelection = Selection.gameObjects != null && Selection.gameObjects.Length > 0;

            if (validSelection)
            {
                foreach(var go in Selection.gameObjects)
                    if (go.GetComponent<LayoutBaker>() != null || go.GetComponent<WindinatorBehaviour>() != null) return true;
            }

            return false;
        }

        [MenuItem("Assets/Windinator/Re-bake Selection", true, 100)]
        public static bool RebakePossible()
        {
            bool validSelection = Selection.gameObjects != null && Selection.gameObjects.Length > 0;

            if (validSelection)
            {
                foreach(var go in Selection.gameObjects)
                    if (go.GetComponent<LayoutBaker>() != null) return true;
            }

            return false;
        }

        [MenuItem("Assets/Windinator/Re-bake Selection", false, 100)]
        public static void RebakeSelection()
        {
            var selections = Selection.gameObjects;

            foreach(var go in selections)
            {
                if (go.GetComponent<LayoutBaker>() != null)
                    BakeElement(true, go);
            }
        }


        [MenuItem("Assets/Windinator/Link Selected Prefabs")]
        public static void LinkPrefabWindow()
        {
            var selections = Selection.gameObjects;
            StringBuilder completion = new StringBuilder();

            completion.Append("Successfuly linked windows:\n\n");

            foreach (var s in selections)
            {
                var w = s.GetComponent<WindinatorBehaviour>();
                if (w != null)
                {
                    completion.Append(w.GetType().Name);
                    completion.AppendLine();
                    RegisterWindow(w);
                }
            }

            completion.Append("\nSuccessfuly linked prefabs:\n\n");

            foreach (var s in selections)
            {
                var w = s.GetComponent<LayoutBaker>();
                if (w != null)
                {
                    completion.Append(w.GetType().Name);
                    completion.AppendLine();
                    RegisterPrefab(w);
                }
            }

            EditorUtility.DisplayDialog("Link Action", completion.ToString(), "Ok");
        }
    }
}