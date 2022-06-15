using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomPropertyDrawer(typeof(Riten.Windinator.CanvasScalerPreset), true)]
public class CanvasScalerPropDrawer : PropertyDrawer
{
    SerializedProperty m_UiScaleMode;
    SerializedProperty m_ScaleFactor;
    SerializedProperty m_ReferenceResolution;
    SerializedProperty m_ScreenMatchMode;
    SerializedProperty m_MatchWidthOrHeight;
    SerializedProperty m_PhysicalUnit;
    SerializedProperty m_FallbackScreenDPI;
    SerializedProperty m_DefaultSpriteDPI;
    SerializedProperty m_DynamicPixelsPerUnit;
    SerializedProperty m_ReferencePixelsPerUnit;

    const int kSliderEndpointLabelsHeight = 12;

    private class Styles
    {
        public GUIContent matchContent;
        public GUIContent widthContent;
        public GUIContent heightContent;
        public GUIStyle leftAlignedLabel;
        public GUIStyle rightAlignedLabel;

        public Styles()
        {
            matchContent = new GUIContent("Match");
            widthContent = new GUIContent("Width");
            heightContent = new GUIContent("Height");

            leftAlignedLabel = new GUIStyle(EditorStyles.label);
            rightAlignedLabel = new GUIStyle(EditorStyles.label);
            rightAlignedLabel.alignment = TextAnchor.MiddleRight;
        }
    }
    private static Styles s_Styles;

    void Init(SerializedProperty serializedObject)
    {
        if (m_UiScaleMode != serializedObject.FindPropertyRelative("UIScaleMode"))
        {
            m_UiScaleMode = serializedObject.FindPropertyRelative("UIScaleMode");
            m_ScaleFactor = serializedObject.FindPropertyRelative("ScaleFactor");
            m_ReferenceResolution = serializedObject.FindPropertyRelative("ReferenceResolution");
            m_ScreenMatchMode = serializedObject.FindPropertyRelative("ScreenMatchMode");
            m_MatchWidthOrHeight = serializedObject.FindPropertyRelative("Match");
            m_PhysicalUnit = serializedObject.FindPropertyRelative("Physicalunit");
            m_FallbackScreenDPI = serializedObject.FindPropertyRelative("FallBackScreenDPI");
            m_DefaultSpriteDPI = serializedObject.FindPropertyRelative("DefaultSpriteDPI");
            m_ReferencePixelsPerUnit = serializedObject.FindPropertyRelative("ReferencePixelsPerUnit");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

        float size = 60f;

        if (!m_UiScaleMode.hasMultipleDifferentValues)
        {
            // Constant pixel size
            if (m_UiScaleMode.enumValueIndex == (int)CanvasScaler.ScaleMode.ConstantPixelSize)
                size += 20f;
            // Scale with screen size
            else if (m_UiScaleMode.enumValueIndex == (int)CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                size += 20f;
                size += 20f;
                if (m_ScreenMatchMode.enumValueIndex == (int)CanvasScaler.ScreenMatchMode.MatchWidthOrHeight && !m_ScreenMatchMode.hasMultipleDifferentValues)
                {
                    size += 40f;
                }
            }
            // Constant physical size
            else if (m_UiScaleMode.enumValueIndex == (int)CanvasScaler.ScaleMode.ConstantPhysicalSize)
            {
                size += 20f;
                size += 20f;
                size += 20f;
            }

            size += 20f;
        }

        return size + 10f;
    }

    public override void OnGUI(Rect position, SerializedProperty serializedObject, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, serializedObject);
        EditorGUI.HelpBox(position, "", MessageType.None);

        position.x += 10f;
        position.width -= 20f;

        position.y += 5f;
        position.height -= 10f;

        Init(serializedObject);

        if (s_Styles == null)
            s_Styles = new Styles();

        Vector2 size = new Vector2(position.width, 20f);
        Vector2 pos = position.position;

        EditorGUI.LabelField(new Rect(pos + Vector2.right * (position.width * 0.4f), size), label);

        pos.y += 40f;
        EditorGUI.PropertyField(new Rect(pos, size), m_UiScaleMode);

        pos.y += 20f;

        if (!m_UiScaleMode.hasMultipleDifferentValues)
        {
            // Constant pixel size
            if (m_UiScaleMode.enumValueIndex == (int)CanvasScaler.ScaleMode.ConstantPixelSize)
            {
                EditorGUI.PropertyField(new Rect(pos, size), m_ScaleFactor);
                pos.y += 20f;
            }
            // Scale with screen size
            else if (m_UiScaleMode.enumValueIndex == (int)CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                EditorGUI.PropertyField(new Rect(pos, size), m_ReferenceResolution); pos.y += 20f;
                EditorGUI.PropertyField(new Rect(pos, size), m_ScreenMatchMode); pos.y += 20f;
                if (m_ScreenMatchMode.enumValueIndex == (int)CanvasScaler.ScreenMatchMode.MatchWidthOrHeight && !m_ScreenMatchMode.hasMultipleDifferentValues)
                {
                    DualLabeledSlider(new Rect(pos, size), m_MatchWidthOrHeight, s_Styles.matchContent, s_Styles.widthContent, s_Styles.heightContent);
                    pos.y += 40f;
                }
            }
            // Constant physical size
            else if (m_UiScaleMode.enumValueIndex == (int)CanvasScaler.ScaleMode.ConstantPhysicalSize)
            {
                EditorGUI.PropertyField(new Rect(pos, size),m_PhysicalUnit); pos.y += 20f;
                EditorGUI.PropertyField(new Rect(pos, size),m_FallbackScreenDPI); pos.y += 20f;
                EditorGUI.PropertyField(new Rect(pos, size),m_DefaultSpriteDPI); pos.y += 20f;
            }

            EditorGUI.PropertyField(new Rect(pos, size), m_ReferencePixelsPerUnit);
            pos.y += 20f;
        }

        EditorGUI.EndProperty();
    }

    private static void DualLabeledSlider(Rect position, SerializedProperty property, GUIContent mainLabel, GUIContent labelLeft, GUIContent labelRight)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        Rect pos = position;

        position.y += 20f;
        position.xMin += EditorGUIUtility.labelWidth;
        position.xMax -= EditorGUIUtility.fieldWidth;

        GUI.Label(position, labelLeft, s_Styles.leftAlignedLabel);
        GUI.Label(position, labelRight, s_Styles.rightAlignedLabel);

        EditorGUI.PropertyField(pos, property, mainLabel);
    }
}
