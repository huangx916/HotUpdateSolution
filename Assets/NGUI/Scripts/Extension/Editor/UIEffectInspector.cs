//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UIEffects.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UIEffect), true)]
public class UIEffectInspector : UIBlankInspector
{

	/// <summary>
	/// Draw all the custom properties.
	/// </summary>

	protected override void DrawCustomProperties()
	{
		EditorGUIUtility.labelWidth = 100F;

		NGUIEditorTools.DrawProperty("Clone Material", serializedObject, "mMatClone");
		SerializedProperty renderersSp = serializedObject.FindProperty("mRenderers");
		EditorGUILayout.PropertyField(renderersSp, new GUIContent("Renderers"), true);

		base.DrawCustomProperties();
	}
}
