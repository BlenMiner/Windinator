using System.Reflection;
using Riten.Windinator;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Riten.Windinator.Swatch), true)]
public class SwatchPropDrawer : PropertyDrawer
{
    SerializedProperty CustomColor;

    SerializedProperty PaletteColor;

    SerializedProperty UseCustomColor;

    SerializedProperty Alpha;

    SerializedProperty Saturation;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 60;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.HelpBox(position, "", MessageType.None);

        const float INSIDE = 50;

        Vector2 size = new Vector2(position.width - 30f - INSIDE, 20f);
        Vector2 pos = position.position + new Vector2(10, 10);

        if (CustomColor != property.FindPropertyRelative("CustomColor"))
        {
            CustomColor = property.FindPropertyRelative("CustomColor");
            PaletteColor = property.FindPropertyRelative("PaletteColor");
            UseCustomColor = property.FindPropertyRelative("UseCustomColor");
            Alpha = property.FindPropertyRelative("Alpha");
            Saturation = property.FindPropertyRelative("Saturation");
        }

        if (UseCustomColor.boolValue)
             CustomColor.colorValue = EditorGUI.ColorField(new Rect(pos, size), label, CustomColor.colorValue);
        else EditorGUI.PropertyField(new Rect(pos, size), PaletteColor, label);

        UseCustomColor.boolValue = EditorGUI.Toggle(new Rect(pos + Vector2.right * (size.x), new Vector2(10f, 20f)), UseCustomColor.boolValue);

        pos.y += 22;

        const float LABEL_SPACE = 40f;

        size.x += 25;

        var firstHalf = new Rect(pos + new Vector2(LABEL_SPACE, 0), new Vector2(size.x * 0.5f - LABEL_SPACE, size.y));
        var secondHalf = new Rect(pos + new Vector2(size.x * 0.5f + LABEL_SPACE + 5, 0), new Vector2(size.x * 0.5f - LABEL_SPACE - 5, size.y));

        Alpha.floatValue = EditorGUI.Slider(firstHalf, Alpha.floatValue, 0f, 1f);
        firstHalf.x -= LABEL_SPACE;
        EditorGUI.LabelField(firstHalf, "Alpha");

        Saturation.floatValue = EditorGUI.Slider(secondHalf, Saturation.floatValue, 0f, 1f);
        secondHalf.x -= LABEL_SPACE;
        EditorGUI.LabelField(secondHalf, "Satur");

        Color c = UseCustomColor.boolValue ? CustomColor.colorValue : ((Colors)PaletteColor.enumValueIndex).ToColor((MonoBehaviour)property.serializedObject.targetObject);

        c = Color.Lerp(Color.white, c, Saturation.floatValue);
        c.a = Alpha.floatValue;

        var colorOverlay = new Rect(position.position + new Vector2(position.width - INSIDE + 10, 10),
            new Vector2(INSIDE - 20, position.height - 20));

        EditorGUI.HelpBox(colorOverlay, "", MessageType.None);

        const float PAD = 2;
        colorOverlay.x += PAD;
        colorOverlay.width -= PAD * 2;
        colorOverlay.y += PAD;
        colorOverlay.height -= PAD * 2;

        EditorGUI.DrawRect(colorOverlay, c);

        EditorGUI.EndProperty();
    }
}
