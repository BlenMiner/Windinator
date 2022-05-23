using UnityEditor;
using UnityEngine;

public static class MaterialUIEditor
{
	[MenuItem("GameObject/UI/Material/Card", false, 1)]
	public static void AddRectangle(MenuCommand menuCommand)
	{
		CreateShapeGO("Card", "Material Card", menuCommand);
	}

	[MenuItem("GameObject/UI/Material/Action Button", false, 1)]
	public static void AddActionutton(MenuCommand menuCommand)
	{
		CreateShapeGO("Action Button", "Action Button", menuCommand);
	}

	[MenuItem("GameObject/UI/Material/Button", false, 1)]
	public static void AddButton(MenuCommand menuCommand)
	{
		CreateShapeGO("Button", "Material Button", menuCommand);
	}

	[MenuItem("GameObject/UI/Material/Label", false, 1)]
	public static void AddLabel(MenuCommand menuCommand)
	{
		CreateShapeGO("Label", "Material Label", menuCommand);
	}

	[MenuItem("GameObject/UI/Material/Text Field", false, 1)]
	public static void AddTextField(MenuCommand menuCommand)
	{
		CreateShapeGO("Button", "Material Text Field", menuCommand);
	}

	public static GameObject CreateShapeGO(string name, string prefab, MenuCommand menuCommand)
	{
		GameObject shapeGO = Object.Instantiate(Resources.Load<GameObject>($"Windinator.Material.UI/{prefab}"));
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
