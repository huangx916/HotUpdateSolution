//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UIBasicSprite), true)]
public class UIBasicSpriteEditor : UIWidgetInspector
{
	/// <summary>
	/// Draw all the custom properties such as sprite type, flip setting, fill direction, etc.
	/// </summary>

	protected override void DrawCustomProperties ()
	{
		GUILayout.Space(6f);

		SerializedProperty sp = NGUIEditorTools.DrawProperty("Type", serializedObject, "mType", GUILayout.MinWidth(20f));

		UISprite.Type type = (UISprite.Type) sp.intValue;

		if (type == UISprite.Type.Simple || type == UISprite.Type.Sliced)
		{
			NGUIEditorTools.DrawBorderProperty("Trim", serializedObject, "mBorder");
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
			NGUIEditorTools.DrawProperty("Mirror", serializedObject, "mMirror");

			ShowFillOptions();

			EditorGUI.BeginDisabledGroup(sp.hasMultipleDifferentValues);
			{
				sp = serializedObject.FindProperty("centerType");
				bool val = (sp.intValue != (int) UISprite.AdvancedType.Invisible);

				if (val != EditorGUILayout.Toggle("Fill Center", val))
				{
					sp.intValue = val ? (int) UISprite.AdvancedType.Invisible : (int) UISprite.AdvancedType.Sliced;
				}

				NGUIEditorTools.DrawProperty("Fill Border", serializedObject, "mFillBorder");
			}
			EditorGUI.EndDisabledGroup();
		}
		else if (type == UISprite.Type.Tiled)
		{
			NGUIEditorTools.DrawBorderProperty("Trim", serializedObject, "mBorder");
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
			NGUIEditorTools.DrawProperty("Mirror", serializedObject, "mMirror");
			NGUIEditorTools.DrawProperty("Tile Border", serializedObject, "mTileBorder");
		}
		else if (type == UISprite.Type.Filled)
		{
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
			NGUIEditorTools.DrawProperty("Fill Dir", serializedObject, "mFillDirection", GUILayout.MinWidth(20f));
			UISprite.FillDirection fillDir = (UISprite.FillDirection) serializedObject.FindProperty("mFillDirection").intValue;
			if (fillDir != UISprite.FillDirection.Nothing)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(10f);
				GUILayout.BeginVertical();
				NGUIEditorTools.DrawProperty("Fill Amount", serializedObject, "mFillAmount", GUILayout.MinWidth(20f));
				NGUIEditorTools.DrawProperty("Invert Fill", serializedObject, "mInvert", GUILayout.MinWidth(20f));
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
		}
		else if (type == UISprite.Type.Advanced)
		{
			SerializedProperty mirrorSp = serializedObject.FindProperty("mMirror");
			mirrorSp.intValue = (int) UISprite.Flip.Nothing;

			NGUIEditorTools.DrawBorderProperty("Border", serializedObject, "mBorder");
			NGUIEditorTools.DrawProperty("  Left", serializedObject, "leftType");
			NGUIEditorTools.DrawProperty("  Right", serializedObject, "rightType");
			NGUIEditorTools.DrawProperty("  Top", serializedObject, "topType");
			NGUIEditorTools.DrawProperty("  Bottom", serializedObject, "bottomType");
			NGUIEditorTools.DrawProperty("  Center", serializedObject, "centerType");
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
		}
		
		if (type == UIBasicSprite.Type.Simple || type == UIBasicSprite.Type.Sliced) // Gradients get too complicated for tiled and filled.
		{
			GUILayout.BeginHorizontal();
			SerializedProperty gr = NGUIEditorTools.DrawProperty("Gradient", serializedObject, "mApplyGradient", GUILayout.Width(95f));

			EditorGUI.BeginDisabledGroup(!gr.hasMultipleDifferentValues && !gr.boolValue);
			{
				NGUIEditorTools.SetLabelWidth(30f);
				serializedObject.DrawProperty("mGradientTop", "Top", GUILayout.MinWidth(40f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				NGUIEditorTools.SetLabelWidth(50f);
				GUILayout.Space(79f);

				serializedObject.DrawProperty("mGradientBottom", "Bottom", GUILayout.MinWidth(40f));
				NGUIEditorTools.SetLabelWidth(80f);
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}
		base.DrawCustomProperties();
	}

	private void ShowFillOptions()
	{
		int length = (int) UISprite.FillDirection.Vertical + 1;
		string[] fillOptionNames = new string[length];
		int[] fillOptionValues = new int[length];
		string[] enumNames = System.Enum.GetNames(typeof(UISprite.FillDirection));
		for (int i = 0; i < length; i++)
		{
			fillOptionNames[i] = enumNames[i];
			fillOptionValues[i] = i;
		}

		int fillDirInt = serializedObject.FindProperty("mFillDirection").intValue;
		if (fillDirInt >= length)
		{
			fillDirInt = 0;
		}

		fillDirInt = EditorGUILayout.IntPopup("Fill Dir", fillDirInt, fillOptionNames, fillOptionValues);
		serializedObject.FindProperty("mFillDirection").intValue = fillDirInt;

		UISprite.FillDirection fillDir = (UISprite.FillDirection) fillDirInt;
		if (fillDir != UISprite.FillDirection.Nothing)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginVertical();
			NGUIEditorTools.DrawProperty("Fill Amount", serializedObject, "mFillAmount", GUILayout.MinWidth(20f));
			NGUIEditorTools.DrawProperty("Invert Fill", serializedObject, "mInvert", GUILayout.MinWidth(20f));
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}
}
