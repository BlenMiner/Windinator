using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Riten.Windinator;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Swatch), true)]
public class SwatchPropDrawer : PropertyDrawer
{
    SerializedProperty CustomColor;

    SerializedProperty PaletteColor;

    SerializedProperty UseCustomColor;

    SerializedProperty Alpha;

    SerializedProperty Saturation;

    /*public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 60;
    }*/

    /*public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
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
             CustomColor.colorValue = EditorGUI.ColorField(new Rect(pos, size), label, CustomColor.colorValue, true, false, false);
        else EditorGUI.PropertyField(new Rect(pos, size), PaletteColor, label);

        var togglePos = pos + Vector2.right * (size.x + 10);

        pos.y += 22;

        const float LABEL_SPACE = 40f;

        size.x += 25;

        var firstHalf = new Rect(pos + new Vector2(LABEL_SPACE, 0), new Vector2(size.x * 0.5f - LABEL_SPACE, size.y));
        var secondHalf = new Rect(pos + new Vector2(size.x * 0.5f + LABEL_SPACE + 5, 0), new Vector2(size.x * 0.5f - LABEL_SPACE - 5, size.y));

        Alpha.floatValue = EditorGUI.Slider(firstHalf, Alpha.floatValue, 0f, 1f);
        firstHalf.x -= LABEL_SPACE;
        EditorGUI.LabelField(firstHalf, "A");

        Saturation.floatValue = EditorGUI.Slider(secondHalf, Saturation.floatValue, 0f, 1f);
        secondHalf.x -= LABEL_SPACE;
        EditorGUI.LabelField(secondHalf, "S");

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

        UseCustomColor.boolValue = GUI.Toggle(new Rect(togglePos, new Vector2(20f, 20f)), UseCustomColor.boolValue, "");
        EditorGUI.EndProperty();
    }*/

    public object GetParent(SerializedProperty prop)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements.Take(elements.Length - 1))
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue(obj, elementName, index);
            }
            else
            {
                obj = GetValue(obj, element);
            }
        }
        return obj;
    }

    public object GetValue(object source, string name, int index)
    {
        var enumerable = GetValue(source, name) as IEnumerable;
        var enm = enumerable.GetEnumerator();
        while (index-- >= 0)
            enm.MoveNext();
        return enm.Current;
    }


    public object GetValue(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();
        var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (f == null)
        {
            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null)
                return null;
            return p.GetValue(source, null);
        }
        return f.GetValue(source);
    }



    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (CustomColor != property.FindPropertyRelative("CustomColor"))
        {
            CustomColor = property.FindPropertyRelative("CustomColor");
            PaletteColor = property.FindPropertyRelative("PaletteColor");
            UseCustomColor = property.FindPropertyRelative("UseCustomColor");
            Alpha = property.FindPropertyRelative("Alpha");
            Saturation = property.FindPropertyRelative("Saturation");
        }

        var indented = EditorGUI.IndentedRect(position);

        var half = new Rect(position.position, new Vector2(position.size.x * 0.6f, position.size.y));
        var secondHalf = new Rect(half.position + Vector2.right * (half.size.x + 15f), new Vector2(position.size.x - (half.width + 15f), position.size.y));

        var alphaRect = new Rect(secondHalf.position, new Vector2(secondHalf.size.x * 0.5f, secondHalf.size.y));
        var satRect = new Rect(secondHalf.position + Vector2.right * alphaRect.size.x, new Vector2(secondHalf.size.x - alphaRect.width, secondHalf.size.y));

        var alphaRectContent = new Rect(alphaRect.position + Vector2.right * 10f, alphaRect.size + Vector2.left * 10f);
        var satRectContent = new Rect(satRect.position + Vector2.right * 10f, satRect.size + Vector2.left * 10f);

        if (UseCustomColor.boolValue)
             CustomColor.colorValue = EditorGUI.ColorField(half, label, CustomColor.colorValue, true, false, false);
        else PaletteColor.enumValueIndex = (int)(Colors)EditorGUI.EnumPopup(half, label, (Colors)PaletteColor.enumValueIndex);

        /*Alpha.floatValue =      GUI.HorizontalSlider(alphaRectContent, Alpha.floatValue, 0f, 1f);
        Saturation.floatValue = GUI.HorizontalSlider(satRectContent, Saturation.floatValue, 0f, 1f);*/

        int i = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 15f;

        UseCustomColor.boolValue = GUI.Toggle(new Rect(secondHalf.position + Vector2.left * 8.5f, new Vector2(20f, 20f)), UseCustomColor.boolValue, "");

        Alpha.floatValue = EditorGUI.Slider(alphaRectContent, "A", Alpha.floatValue, 0f, 1f);
        Saturation.floatValue = EditorGUI.Slider(satRectContent, "S", Saturation.floatValue, 0f, 1f);

        EditorGUI.indentLevel = i;

        Color c = UseCustomColor.boolValue ? CustomColor.colorValue : ((Colors)PaletteColor.enumValueIndex).ToColor((MonoBehaviour)property.serializedObject.targetObject);
        Color.RGBToHSV(c, out var h, out var s, out var v);
        c = Color.HSVToRGB(h, s * Saturation.floatValue, v);
        c.a = Alpha.floatValue;

        EditorGUI.DrawRect(new Rect(indented.position + Vector2.left * 10f, new Vector2(5f, indented.height)), c);

        EditorGUI.EndProperty();
    }
}
