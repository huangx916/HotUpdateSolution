//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenGradientColor))]
public class TweenGradientColorEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		NGUIEditorTools.DrawProperty("Gradient", serializedObject, "gradient");

		DrawCommonProperties();

		serializedObject.ApplyModifiedProperties();
	}
}
