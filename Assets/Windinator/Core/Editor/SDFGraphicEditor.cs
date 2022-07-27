using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Riten.Windinator.Shapes;

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
        EditorGUI.BeginChangeCheck();

        graphic.Texture = EditorGUILayout.ObjectField("Texture", graphic.Texture, typeof(Texture), allowSceneObjects: true) as Texture;
        graphic.color = EditorGUILayout.ColorField("Base Color", graphic.color);
        graphic.Alpha = EditorGUILayout.Slider("Alpha Multiplier", graphic.Alpha, 0, 1);
        graphic.raycastTarget = EditorGUILayout.Toggle("Raycast Target", graphic.raycastTarget);

        GUILayout.Space(10f);

        BeginGroup("Outline Settings");

        graphic.OutlineColor = EditorGUILayout.ColorField("Outline Color", graphic.OutlineColor);
        graphic.OutlineSize = EditorGUILayout.FloatField("Outline Size", graphic.OutlineSize);

        if (graphic.OutlineSize < 0) graphic.OutlineSize = 0f;

        EndGroup();

        BeginGroup("Shadow Settings");

        graphic.ShadowColor = EditorGUILayout.ColorField("Shadow Color", graphic.ShadowColor);
        graphic.ShadowSize = EditorGUILayout.FloatField("Shadow Size", graphic.ShadowSize);
        graphic.ShadowBlur = EditorGUILayout.FloatField("Shadow Blur", graphic.ShadowBlur);
        graphic.ShadowPower = EditorGUILayout.FloatField("Shadow Power", graphic.ShadowPower);

        if (graphic.ShadowPower < 1) graphic.ShadowPower = 1f;
        if (graphic.ShadowBlur < 0) graphic.ShadowBlur = 0f;
        if (graphic.ShadowSize < 0) graphic.ShadowSize = 0f;

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
            graphic.CircleColor = EditorGUILayout.ColorField(
                new GUIContent("Circle Color"), 
                graphic.CircleColor, true, false, true
            );
            
            graphic.CircleSize = EditorGUILayout.FloatField("Circle Radius", graphic.CircleSize);
            graphic.CircleAlpha = EditorGUILayout.Slider("Alpha Multiplier", graphic.CircleAlpha, 0, 1);
            graphic.CirclePos = EditorGUILayout.Vector2Field("Circle Pos", graphic.CirclePos);
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

            rect = EditorGUILayout.RectField(
                "Mask Rect", rect
            );

            graphic.MaskRect = new Vector4(rect.x, rect.y, rect.width, rect.height);

            Rect offset = new Rect(graphic.MaskOffset.x, graphic.MaskOffset.y, graphic.MaskOffset.z, graphic.MaskOffset.w);

            offset = EditorGUILayout.RectField(
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
            graphic.LeftUpColor = EditorGUILayout.ColorField("Top Left", graphic.LeftUpColor);
            graphic.RightUpColor = EditorGUILayout.ColorField("Top Right", graphic.RightUpColor);
            graphic.RightDownColor = EditorGUILayout.ColorField("Bottom Right", graphic.RightDownColor);
            graphic.LeftDownColor = EditorGUILayout.ColorField("Bottom Left",graphic.LeftDownColor);
        }

        EditorGUI.indentLevel -= 1;

        EndGroup();

        if (EditorGUI.EndChangeCheck())
        {
            graphic.SetAllDirty();
            Undo.RecordObject(graphic, "SDF Changed");
        }

        EditorUtility.SetDirty(graphic);
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

        g.SetMargin(EditorGUILayout.FloatField("Expand Borders", g.Margin));
        g.Quality = EditorGUILayout.Slider("Quality", g.Quality, 0.001f, 1f);
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

        graphic.MaxRoundess = EditorGUILayout.Toggle("Max Roundness", graphic.MaxRoundess);
        if (!graphic.MaxRoundess)
        {
            graphic.UniformRoundness = EditorGUILayout.Toggle("Uniform Roundness", graphic.UniformRoundness);
            if (graphic.UniformRoundness)
            {
                var r = graphic.Roundness;
                r.x = EditorGUILayout.FloatField("Roundness", r.x);
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
        EditorGUI.BeginChangeCheck();
        SDFGraphicEditor.DrawSDFGUI(target as SignedDistanceFieldGraphic);

        PolygonGraphic graphic = target as PolygonGraphic;

        SDFGraphicEditor.BeginGroup("Polygon Settings");

        graphic.Roundness = EditorGUILayout.FloatField("Roundness", graphic.Roundness);

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
                graphic.Points[i] = EditorGUILayout.Vector2Field("Point " + i, graphic.Points[i]);

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

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(graphic, "Polygon Point Updated");
        }

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
        EditorGUI.BeginChangeCheck();
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

            for (int i = 0; i < graphic.Points.Length; ++i)
            {
                Vector2 point = graphic.Points[i];

                Vector2 localPoint = Vector2.Scale(point, rectTransform.rect.size);
                
                float pivotX = (-rectTransform.pivot.x) * rectTransform.rect.width;
                float pivotY = (-rectTransform.pivot.y) * rectTransform.rect.height;

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

                actualPos = Handles.FreeMoveHandle(actualPos, Quaternion.identity, 5f, Vector3.zero, Handles.SphereHandleCap);
                var p = Vector2.Scale(graphic.transform.InverseTransformPoint(new Vector2(
                    actualPos.x - pivotX,
                    actualPos.y - pivotY
                )), new Vector2(1f / rectTransform.rect.size.x, 1f / rectTransform.rect.size.y));

                p = Vector4.Max(p, default);
                p = Vector4.Min(p, Vector4.one);

                if (p != point)
                {
                    Undo.RecordObject(graphic, "Polygon Updated From Scene");
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
        EditorGUI.BeginChangeCheck();
        SDFGraphicEditor.DrawSDFGUI(target as SignedDistanceFieldGraphic);

        LineGraphic graphic = target as LineGraphic;

        SDFGraphicEditor.BeginGroup("Line Settings");

        graphic.Roundness = EditorGUILayout.FloatField("Roundness", graphic.Roundness);
        graphic.Size = EditorGUILayout.FloatField("Size", graphic.Size);

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
                graphic.Points[i] = EditorGUILayout.Vector2Field("Point " + i, graphic.Points[i]);

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

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(graphic, "Line Point Updated");
        }

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
        EditorGUI.BeginChangeCheck();
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

                actualPos = Handles.FreeMoveHandle(actualPos, Quaternion.identity, 5f, Vector3.zero, Handles.SphereHandleCap);
                var p = Vector2.Scale(graphic.transform.InverseTransformPoint(new Vector2(
                    actualPos.x - pivotX,
                    actualPos.y - pivotY
                )), new Vector2(1f / rectTransform.rect.size.x, 1f / rectTransform.rect.size.y));

                p = Vector4.Max(p, default);
                p = Vector4.Min(p, Vector4.one);

                if (p != point)
                {
                    Undo.RecordObject(graphic, "Line Updated From Scene");
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