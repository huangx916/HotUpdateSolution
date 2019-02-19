//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenMaterialColor))]
public class TweenMaterialColorEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		TweenMaterialColor tw = target as TweenMaterialColor;
		GUI.changed = false;

		string colorName = EditorGUILayout.TextField("Color Name", tw.colorName);
		Color from = EditorGUILayout.ColorField("From", tw.from);
		Color to = EditorGUILayout.ColorField("To", tw.to);

		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.colorName = colorName;
			tw.from = from;
			tw.to = to;
			if (preview)
			{
				tw.Sample(tw.tweenFactor, false);
			}
			NGUITools.SetDirty(tw);
		}

		DrawCommonProperties();
	}
}
