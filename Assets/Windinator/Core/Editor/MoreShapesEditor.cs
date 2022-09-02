using UnityEditor;
using UnityEngine;

public static class MoreShapesEditor
{
	[MenuItem("GameObject/UI/More Shapes/Rectangle", false, 1)]
	public static void AddRectangle(MenuCommand menuCommand)
	{
		CreateShapeGO("Rectangle", "Rectangle", menuCommand);
	}

    [MenuItem("GameObject/UI/More Shapes/Polygon", false, 1)]
	public static void AddPolygon(MenuCommand menuCommand)
	{
		CreateShapeGO("Polygon", "Polygon", menuCommand);
	}

    [MenuItem("GameObject/UI/More Shapes/Line", false, 1)]
	public static void AddLine(MenuCommand menuCommand)
	{
		CreateShapeGO("Line", "Line", menuCommand);
	}

    [MenuItem("GameObject/UI/More Shapes/Canvas", false, 1)]
	public static void AddCanvas(MenuCommand menuCommand)
	{
		CreateShapeGO("Dynamic Canvas", "Canvas", menuCommand);
	}

	public static GameObject CreateShapeGO(string name, string prefab, MenuCommand menuCommand)
	{
		GameObject shapeGO = Object.Instantiate(Resources.Load<GameObject>($"Windinator.Presets/{prefab}"));
		shapeGO.name = name;
		Undo.RegisterCreatedObjectUndo(shapeGO, "Created " + name + " shape");

		GameObject parent = (GameObject)menuCommand.context;

		if (
			parent != null &&
			(parent.GetComponent<Canvas>() || parent.GetComponentInParent<Canvas>())
		)
		{
			Undo.SetTransformParent(
				shapeGO.transform,
				parent.transform,
				"Set " + name + " parent"
			);

			Undo.RecordObject(shapeGO.transform, "centered " + name);
			shapeGO.transform.localPosition = Vector3.zero;
			shapeGO.transform.localScale = Vector3.one;
		}

		Selection.activeGameObject = shapeGO;

		return shapeGO;
	}
}
