using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Riten.Windinator.Shapes;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public static class UndoUtils
{
    public static T ObjectField<T>(this Object target, string label, T obj) where T : Object
    {
        EditorGUI.BeginChangeCheck();

        var newVal = EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects: true) as T;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"{label} Changed");
            EditorUtility.SetDirty(target);
        }

        return newVal;
    }

    public static Color ColorField(this Object target, string label, Color color, bool showEyedropper = true, bool showAlpha = true, bool hdr = false)
    {
        EditorGUI.BeginChangeCheck();

        var newVal = EditorGUILayout.ColorField(new GUIContent(label), color, showEyedropper, showAlpha, hdr);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"{label} Changed");
            EditorUtility.SetDirty(target);
        }

        return newVal;
    }

    public static float FloatField(this Object target, string label, float value)
    {
        EditorGUI.BeginChangeCheck();

        var newVal = EditorGUILayout.FloatField(label, value);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"{label} Changed");
            EditorUtility.SetDirty(target);
        }

        return newVal;
    }

    public static int IntField(this Object target, string label, int value)
    {
        EditorGUI.BeginChangeCheck();

        var newVal = EditorGUILayout.IntField(label, value);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"{label} Changed");
            EditorUtility.SetDirty(target);
        }

        return newVal;
    }

    public static Rect RectField(this Object target, string label, Rect value)
    {
        EditorGUI.BeginChangeCheck();

        var newVal = EditorGUILayout.RectField(label, value);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"{label} Changed");
            EditorUtility.SetDirty(target);
        }

        return newVal;
    }

    public static float Slider(this Object target, string label, float value, float min, float max)
    {
        EditorGUI.BeginChangeCheck();

        var newVal = EditorGUILayout.Slider(label, value, min, max);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"{label} Changed");
            EditorUtility.SetDirty(target);
        }

        return newVal;
    }

    public static bool Toggle(this Object target, string label, bool value)
    {
        EditorGUI.BeginChangeCheck();

        var newVal = EditorGUILayout.Toggle(label, value);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"{label} Changed");
            EditorUtility.SetDirty(target);
        }

        return newVal;
    }

    public static Vector2 Vector2Field(this Object target, string label, Vector2 value)
    {
        EditorGUI.BeginChangeCheck();

        var newVal = EditorGUILayout.Vector2Field(label, value);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"{label} Changed");
            EditorUtility.SetDirty(target);
        }

        return newVal;
    }
}

[CustomEditor(typeof(SignedDistanceFieldGraphic))]
public class SDFGraphicEditor : Editor
{
    static bool gradientSettings;

    static bool circleSettings;

    static bool maskSettings;

    static BoxBoundsHandle boxBounds;

    public static void BeginGroup(string title)
    {
        EditorGUILayout.BeginVertical(style: "helpbox");
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(title);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel += 1;
    }

    public static void EndGroup()
    {
        EditorGUI.indentLevel -= 1;
        EditorGUILayout.EndVertical();
    }

    public static void DrawSDFGUI(SignedDistanceFieldGraphic graphic)
    {
        if (!(graphic is CanvasGraphic))
            graphic.Texture = graphic.ObjectField("Texture", graphic.Texture);
        graphic.color = graphic.ColorField("Base Color", graphic.color);
        graphic.Alpha = graphic.Slider("Alpha Multiplier", graphic.Alpha, 0, 1);
        graphic.raycastTarget = graphic.Toggle("Raycast Target", graphic.raycastTarget);

        GUILayout.Space(10f);

        BeginGroup("Outline Settings");

        graphic.OutlineColor = graphic.ColorField("Outline Color", graphic.OutlineColor);
        graphic.OutlineSize = graphic.FloatField("Outline Size", graphic.OutlineSize);

        if (graphic.OutlineSize < 0) graphic.OutlineSize = 0f;

        EndGroup();

        BeginGroup("Shadow Settings");

        graphic.ShadowColor = graphic.ColorField("Shadow Color", graphic.ShadowColor);
        graphic.ShadowSize = graphic.FloatField("Shadow Size", graphic.ShadowSize);
        graphic.ShadowBlur = graphic.FloatField("Shadow Blur", graphic.ShadowBlur);

        EndGroup();

        BeginGroup("Emboss Settings");

        graphic.EmbossDirection = graphic.Slider("Emboss Direction", graphic.EmbossDirection, 0, 1);

        GUILayout.Space(10f);

        graphic.EmbossHighlightColor = graphic.ColorField("Emboss Highlight", graphic.EmbossHighlightColor, true, true, false);
        graphic.EmbossLowlightColor = graphic.ColorField("Emboss Lowlight", graphic.EmbossLowlightColor, true, true, false);

        GUILayout.Space(10f);

        graphic.EmbossBlurTop = graphic.Slider("Emboss Blur Top", graphic.EmbossBlurTop, -1, 1);
        graphic.EmbossBlurBottom = graphic.Slider("Emboss Blur Bottom", graphic.EmbossBlurBottom, -1, 1);
        graphic.EmbossDistance = graphic.FloatField("Emboss Distance", graphic.EmbossDistance);
        graphic.EmbossSize = graphic.FloatField("Emboss Size", graphic.EmbossSize);
        graphic.EmbossPower = graphic.Slider("Emboss Power", graphic.EmbossPower, 0, 1);

        EndGroup();

        BeginGroup("Advanced Settings");

        circleSettings = EditorGUILayout.Foldout(circleSettings, "Circle Effect");

        Rect foldRect = GUILayoutUtility.GetLastRect();
        if (Event.current.type == EventType.MouseUp && foldRect.Contains (Event.current.mousePosition)) 
        {
            circleSettings = !circleSettings;
            GUI.changed = true;
            Event.current.Use ();
        }

        EditorGUI.indentLevel += 1;
        if (circleSettings)
        {
            graphic.CircleColor = graphic.ColorField("Circle Color", graphic.CircleColor, true, false, false);
            graphic.CircleSize = graphic.FloatField("Circle Radius", graphic.CircleSize);
            graphic.CircleAlpha = graphic.Slider("Alpha Multiplier", graphic.CircleAlpha, 0, 1);
            graphic.CirclePos = graphic.Vector2Field("Circle Pos", graphic.CirclePos);
        }
        EditorGUI.indentLevel -= 1;

        // Mask settings

        maskSettings = EditorGUILayout.Foldout(maskSettings, "Mask Effect");

        foldRect = GUILayoutUtility.GetLastRect();
        if (Event.current.type == EventType.MouseUp && foldRect.Contains (Event.current.mousePosition)) 
        {
            maskSettings = !maskSettings;
            GUI.changed = true;
            Event.current.Use ();
        }

        EditorGUI.indentLevel += 1;
        if (maskSettings)
        {
            Rect rect = new Rect(graphic.MaskRect.x, graphic.MaskRect.y, graphic.MaskRect.z, graphic.MaskRect.w);

            rect = graphic.RectField(
                "Mask Rect", rect
            );

            graphic.MaskRect = new Vector4(rect.x, rect.y, rect.width, rect.height);

            Rect offset = new Rect(graphic.MaskOffset.x, graphic.MaskOffset.y, graphic.MaskOffset.z, graphic.MaskOffset.w);

            offset = graphic.RectField(
                "Mask Offset", offset
            );

            graphic.MaskOffset = new Vector4(offset.x, offset.y, offset.width, offset.height);
        }
        EditorGUI.indentLevel -= 1;

        gradientSettings = EditorGUILayout.Foldout(gradientSettings, "Gradient Effect");

        foldRect = GUILayoutUtility.GetLastRect();
        if (Event.current.type == EventType.MouseUp && foldRect.Contains (Event.current.mousePosition)) 
        {
            gradientSettings = !gradientSettings;
            GUI.changed = true;
            Event.current.Use ();
        }

        EditorGUI.indentLevel += 1;

        if (gradientSettings)
        {
            graphic.LeftUpColor = graphic.ColorField("Top Left", graphic.LeftUpColor);
            graphic.RightUpColor = graphic.ColorField("Top Right", graphic.RightUpColor);
            graphic.RightDownColor = graphic.ColorField("Bottom Right", graphic.RightDownColor);
            graphic.LeftDownColor = graphic.ColorField("Bottom Left",graphic.LeftDownColor);
        }

        EditorGUI.indentLevel -= 1;

        EndGroup();
    }

    public static void DrawSDFScene(SignedDistanceFieldGraphic graphic)
    {
        RectTransform rt = graphic.transform as RectTransform;

        float pivotX = 0.5f - rt.pivot.x;
        float pivotY = 0.5f - rt.pivot.y;

        Vector3 actualPos = graphic.transform.TransformPoint(new Vector3(
            pivotX * rt.rect.width + graphic.CirclePos.x,
            pivotY * rt.rect.height + graphic.CirclePos.y
        ));

        Handles.color = new Color(1 - graphic.color.r, 1 - graphic.color.g, 1 - graphic.color.b, 1f);

        if (circleSettings)
        {
            float transformedSize = graphic.transform.TransformVector(new Vector3(graphic.CircleSize, 0, 0)).x;

            transformedSize = Handles.RadiusHandle(Quaternion.identity, actualPos, transformedSize, true);
            actualPos = Handles.FreeMoveHandle(actualPos, Quaternion.identity, 3f, Vector3.one, Handles.SphereHandleCap);

            graphic.CirclePos = graphic.transform.InverseTransformPoint(new Vector3(
                actualPos.x - pivotX * rt.rect.width,
                actualPos.y - pivotY * rt.rect.height
            ));

            graphic.CircleSize = graphic.transform.InverseTransformVector(new Vector3(transformedSize, 0, 0)).x;
        }
        
        if (maskSettings)
        {
            Vector2 maskPos = graphic.transform.TransformPoint(new Vector2(
                pivotX * rt.rect.width + graphic.MaskRect.x,
                pivotY * rt.rect.height + graphic.MaskRect.y
            ));

            Vector2 maskSize = graphic.transform.TransformVector(new Vector2(
                graphic.MaskRect.z,
                graphic.MaskRect.w
            ));
            
            if (boxBounds == null) boxBounds = new BoxBoundsHandle();

            maskPos += maskSize * 0.5f;
            maskPos = Handles.FreeMoveHandle(maskPos, Quaternion.identity, 3f, Vector3.one, Handles.SphereHandleCap);

            boxBounds.center = maskPos;// + maskSize * 0.5f;
            boxBounds.size = maskSize;
            boxBounds.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;

            boxBounds.DrawHandle();

            maskPos = new Vector2(boxBounds.center.x, boxBounds.center.y) - new Vector2(boxBounds.size.x, boxBounds.size.y) * 0.5f;

            Vector2 pos = graphic.transform.InverseTransformPoint(new Vector3(
                maskPos.x - pivotX * rt.rect.width,
                maskPos.y - pivotY * rt.rect.height
            ));

            Vector2 size = graphic.transform.InverseTransformVector(new Vector3(boxBounds.size.x, boxBounds.size.y, 0));

            graphic.MaskRect = new Vector4(pos.x, pos.y, size.x, size.y);
        }

        if (Event.current.type == EventType.MouseUp)
        {
            Undo.RecordObject(graphic, "SDF Updated From Scene");
        }
    }

    public void OnSceneGUI()
    {
        DrawSDFScene(target as SignedDistanceFieldGraphic);
    }

    public override void OnInspectorGUI()
    {
        DrawSDFGUI(target as SignedDistanceFieldGraphic);
    }
}

[CustomEditor(typeof(CanvasGraphic), true)]
public class CanvasGraphicEditor : Editor
{
    public void OnSceneGUI()
    {
        SDFGraphicEditor.DrawSDFScene(target as SignedDistanceFieldGraphic);
    }

    public override void OnInspectorGUI()
    {
        CanvasGraphic g = target as CanvasGraphic;

        SDFGraphicEditor.DrawSDFGUI(target as SignedDistanceFieldGraphic);

        g.SetMargin(g.FloatField("Expand Borders", g.GetMargin()));
        g.Quality = g.Slider("Quality", g.Quality, 0.0001f, 1f);
    }
}

[CustomEditor(typeof(RectangleGraphic), true)]
public class RectangleGraphicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SDFGraphicEditor.DrawSDFGUI(target as SignedDistanceFieldGraphic);

        RectangleGraphic graphic = target as RectangleGraphic;

        SDFGraphicEditor.BeginGroup("Roundness Settings");

        graphic.MaxRoundess = graphic.Toggle("Max Roundness", graphic.MaxRoundess);
        if (!graphic.MaxRoundess)
        {
            graphic.UniformRoundness = graphic.Toggle("Uniform Roundness", graphic.UniformRoundness);
            if (graphic.UniformRoundness)
            {
                var r = graphic.Roundness;
                r.x = graphic.FloatField("Roundness", r.x);
                graphic.Roundness = r;
            }
            else
            {
                graphic.Roundness = EditorGUILayout.Vector4Field("Roundness", graphic.Roundness);
            }
        }

        SDFGraphicEditor.EndGroup();
    }

    public void OnSceneGUI()
    {
        SDFGraphicEditor.DrawSDFScene(target as SignedDistanceFieldGraphic);
    }
}

[CustomEditor(typeof(PolygonGraphic), true)]
public class PolygonGraphicEditor : Editor
{
    bool m_showPoints = false;

    public override void OnInspectorGUI()
    {
        SDFGraphicEditor.DrawSDFGUI(target as SignedDistanceFieldGraphic);

        PolygonGraphic graphic = target as PolygonGraphic;

        SDFGraphicEditor.BeginGroup("Polygon Settings");

        graphic.Roundness = graphic.FloatField("Roundness", graphic.Roundness);

        m_showPoints = EditorGUILayout.Foldout(m_showPoints, "Points List");

        Rect foldRect = GUILayoutUtility.GetLastRect();
        if (Event.current.type == EventType.MouseUp && foldRect.Contains (Event.current.mousePosition)) 
        {
            m_showPoints = !m_showPoints;
            GUI.changed = true;
            Event.current.Use ();
        }

        EditorGUI.indentLevel += 1;
        if (m_showPoints)
        {
            GUI.SetNextControlName("MyTextField");
            graphic.Points.Length = EditorGUILayout.IntField("Count", graphic.Points.Length);

            EditorGUI.indentLevel += 1;
            for (int i = 0; i < graphic.Points.Length; ++i)
            {
                graphic.Points[i] = graphic.Vector2Field("Point " + i, graphic.Points[i]);

                graphic.Points[i] = Vector4.Max(graphic.Points[i], default);
                graphic.Points[i] = Vector4.Min(graphic.Points[i], Vector4.one); 
            }
            EditorGUI.indentLevel -= 1;

            if (GUILayout.Button("Add Point"))
            {
                graphic.Points.Add(graphic.Points.Length == 0 ? default : graphic.Points[graphic.Points.Length - 1]);
            }
        }
        EditorGUI.indentLevel -= 1;

        SDFGraphicEditor.EndGroup();
    }

    Tool prevTool;

    void OnDisable()
    {
        if (prevTool != default)
            Tools.current = prevTool;
    }

    public void OnSceneGUI()
    {
        PolygonGraphic graphic = target as PolygonGraphic;
        SDFGraphicEditor.DrawSDFScene(target as SignedDistanceFieldGraphic);

        RectTransform rectTransform = graphic.transform as RectTransform;

        if (m_showPoints)
        {
            if (Tools.current != Tool.None)
            {
                prevTool = Tools.current;
                Tools.current = Tool.None;
            }

            var canvas = graphic.canvas;

            for (int i = 0; i < graphic.Points.Length; ++i)
            {
                Vector2 point = graphic.Points[i];

                Vector2 localPoint = Vector2.Scale(point, rectTransform.rect.size);
                
                float pivotX = (-rectTransform.pivot.x) * rectTransform.rect.width;
                float pivotY = (-rectTransform.pivot.y) * rectTransform.rect.height;

                var pivot = graphic.transform.TransformVector(new Vector2(pivotX, pivotY));

                Vector3 actualPos = graphic.transform.TransformPoint(new Vector3(
                    pivotX + localPoint.x,
                    pivotY + localPoint.y
                ));

                if (i == graphic.Points.Length - 1)
                {
                    Color.RGBToHSV(Handles.color, out var H, out var S, out var V);
                    H += 90f / 360f;
                    Handles.color = Color.HSVToRGB(H % 1f, 1f, 1f);
                }

                EditorGUI.BeginChangeCheck();

                actualPos = Handles.FreeMoveHandle(actualPos, Quaternion.identity, 5f, Vector3.zero, Handles.SphereHandleCap);

                var p = Vector2.Scale(graphic.transform.InverseTransformPoint(new Vector2(
                    actualPos.x - pivot.x,
                    actualPos.y - pivot.y
                )), new Vector2(1f / rectTransform.rect.size.x, 1f / rectTransform.rect.size.y));

                p = Vector4.Max(p, default);
                p = Vector4.Min(p, Vector4.one);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(graphic, $"Polygon {i} Updated From Scene");
                    graphic.Points[i] = p;
                }
            }

            graphic.SetAllDirty();
        }
        else
        {
            if (Tools.current == Tool.None)
            {
                Tools.current = prevTool;
            }
        }

        if (Event.current.type == EventType.MouseUp || EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(graphic);
        }
    }
}


[CustomEditor(typeof(LineGraphic), true)]
public class LineGraphicEditor : Editor
{
    bool m_showPoints = false;

    public override void OnInspectorGUI()
    {
        SDFGraphicEditor.DrawSDFGUI(target as SignedDistanceFieldGraphic);

        LineGraphic graphic = target as LineGraphic;

        SDFGraphicEditor.BeginGroup("Line Settings");

        graphic.Roundness = graphic.FloatField("Roundness", graphic.Roundness);
        graphic.Size = graphic.FloatField("Size", graphic.Size);

        m_showPoints = EditorGUILayout.Foldout(m_showPoints, "Points List");

        Rect foldRect = GUILayoutUtility.GetLastRect();
        if (Event.current.type == EventType.MouseUp && foldRect.Contains (Event.current.mousePosition)) 
        {
            m_showPoints = !m_showPoints;
            GUI.changed = true;
            Event.current.Use ();
        }

        EditorGUI.indentLevel += 1;
        if (m_showPoints)
        {
            GUI.SetNextControlName("MyTextField");
            graphic.Points.Length = graphic.IntField("Count", graphic.Points.Length);

            EditorGUI.indentLevel += 1;
            for (int i = 0; i < graphic.Points.Length; ++i)
            {
                graphic.Points[i] = graphic.Vector2Field("Point " + i, graphic.Points[i]);

                graphic.Points[i] = Vector4.Max(graphic.Points[i], default);
                graphic.Points[i] = Vector4.Min(graphic.Points[i], Vector4.one); 
            }
            EditorGUI.indentLevel -= 1;

            if (GUILayout.Button("Add Point"))
            {
                Undo.RecordObject(graphic, "Line Point Count Updated");
                EditorUtility.SetDirty(graphic);

                graphic.Points.Add(graphic.Points.Length == 0 ? default : graphic.Points[graphic.Points.Length - 1]);
            }
        }
        EditorGUI.indentLevel -= 1;

        SDFGraphicEditor.EndGroup();
    }

    Tool prevTool;

    void OnDisable()
    {
        if (prevTool != default)
            Tools.current = prevTool;
    }

    public void OnSceneGUI()
    {
        LineGraphic graphic = target as LineGraphic;
        SDFGraphicEditor.DrawSDFScene(target as SignedDistanceFieldGraphic);

        RectTransform rectTransform = graphic.transform as RectTransform;

        if (m_showPoints)
        {
            if (Tools.current != Tool.None)
            {
                prevTool = Tools.current;
                Tools.current = Tool.None;
            }

            for (int i = 0; i < graphic.Points.Length; ++i)
            {
                Vector2 point = graphic.Points[i];

                Vector2 localPoint = Vector2.Scale(point, rectTransform.rect.size);
                
                float pivotX = (-rectTransform.pivot.x) * rectTransform.rect.width;
                float pivotY = (-rectTransform.pivot.y) * rectTransform.rect.height;

                var pivot = graphic.transform.TransformVector(new Vector2(pivotX, pivotY));

                Vector3 actualPos = graphic.transform.TransformPoint(new Vector3(
                    pivotX + localPoint.x,
                    pivotY + localPoint.y
                ));

                if (i == graphic.Points.Length - 1)
                {
                    Color.RGBToHSV(Handles.color, out var H, out var S, out var V);
                    H += 90f / 360f;
                    Handles.color = Color.HSVToRGB(H % 1f, 1f, 1f);
                }

                EditorGUI.BeginChangeCheck();

                actualPos = Handles.FreeMoveHandle(actualPos, Quaternion.identity, 5f, Vector3.zero, Handles.SphereHandleCap);

                var p = Vector2.Scale(graphic.transform.InverseTransformPoint(new Vector2(
                    actualPos.x - pivot.x,
                    actualPos.y - pivot.y
                )), new Vector2(1f / rectTransform.rect.size.x, 1f / rectTransform.rect.size.y));

                p = Vector4.Max(p, default);
                p = Vector4.Min(p, Vector4.one);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(graphic, $"Line {i} Updated From Scene");
                    graphic.Points[i] = p;
                }
            }

            graphic.SetAllDirty();
        }
        else
        {
            if (Tools.current == Tool.None)
            {
                Tools.current = prevTool;
            }
        }

        if (Event.current.type == EventType.MouseUp)
        {
            EditorUtility.SetDirty(graphic);
        }
    }
}