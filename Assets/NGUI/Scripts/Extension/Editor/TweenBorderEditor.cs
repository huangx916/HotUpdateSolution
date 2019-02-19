//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenBorder))]
public class TweenBorderEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		TweenBorder tw = target as TweenBorder;
		GUI.changed = false;

		NGUIEditorTools.DrawBorderProperty("From", serializedObject, "from");
		NGUIEditorTools.DrawBorderProperty("To", serializedObject, "to");
		serializedObject.ApplyModifiedProperties();

		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			if (preview)
			{
				tw.Sample(tw.tweenFactor, false);
			}
			NGUITools.SetDirty(tw);
		}

		DrawCommonProperties();
	}
}
