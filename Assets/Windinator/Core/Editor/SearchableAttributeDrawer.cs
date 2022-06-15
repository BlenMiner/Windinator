using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using WindinatorEditorUtils;
using System.Reflection;

[CustomPropertyDrawer(typeof(SearchableAttribute))]

public class SearchableAttributeDrawer : PropertyDrawer
{
    struct EnumStringValuePair : IComparable<EnumStringValuePair>
    {
        public string strValue;
        public int intValue;

        public int CompareTo(EnumStringValuePair another)
        {
            if (intValue < another.intValue)
                return -1;
            else if (intValue > another.intValue)
                return 1;
            return 0;
        }
    }


    static Dictionary<int, StringSearchTree> m_cachedTrees = new Dictionary<int, StringSearchTree>();

    GUIStyle m_searchStyle;

    string SearchField(SerializedProperty property, GUIContent label, string id, string glabel, string value, Rect position)
    {
        if (m_searchStyle == null)
            m_searchStyle = GUI.skin.FindStyle("ToolbarSeachTextField");

        GUI.SetNextControlName(id);
        return EditorGUI.TextField(new Rect(position.position, new Vector2(position.width, base.GetPropertyHeight(property, label))), glabel, value, m_searchStyle);
    }

    void DrawOptions(SerializedProperty property, List<ValueName> opts, Vector2 pos, float width, ref string value)
    {
        for (int i = 0; i < opts.Count; i++)
        {
            if (GUI.Button(new Rect(pos + new Vector2(0, ITEMHEIGHT * i), new Vector2(width, 20)), opts[i].Name))
            {
                value = opts[i].Name;
                property.enumValueIndex = (int)opts[i].Value;
                GUI.FocusControl(null);
            }
        }
    }

    const float ITEMHEIGHT = 20;

    bool selected = false;

    string value = null;

    List<ValueName> cache;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float def = base.GetPropertyHeight(property, label);

        return def + (selected && cache != null ? (cache.Count * ITEMHEIGHT) : 0);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var enumType = GetType(property);

        if (value == null)
            value = Enum.ToObject(enumType, property.enumValueIndex).ToString();
        int y = (int)position.position.y;
        string id = y.ToString();

        if (!m_cachedTrees.ContainsKey(y))
            m_cachedTrees[y] = new StringSearchTree(enumType);

        var tree = m_cachedTrees[y];

        EditorGUI.BeginProperty(position, label, property);

        string newValue = SearchField(property, label, id, property.name, value, position);

        selected = GUI.GetNameOfFocusedControl() == id;

        if (newValue != value || (cache == null && selected))
        {
            value = newValue;
            cache = tree.GetPossibleResults(value);
        }

        if (selected && cache != null && cache.Count > 0)
        {
            DrawOptions(property, cache, position.position + new Vector2(0, base.GetPropertyHeight(property, label)), position.width, ref value);

            if (Event.current.isKey && Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)
            {
                value = cache[0].Name;
                property.enumValueIndex = (int)cache[0].Value;
                GUI.FocusControl(null);
            }
        }

        EditorGUI.EndProperty();
    }

    public static Type GetType(SerializedProperty property)
    {
        Type parentType = property.serializedObject.targetObject.GetType();
        FieldInfo fi = parentType.GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return fi.FieldType;
    }
}
