//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenClipOffset))]
public class TweenClipOffsetEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		TweenClipOffset tw = target as TweenClipOffset;
		GUI.changed = false;

		Vector2 from = EditorGUILayout.Vector2Field("From", tw.from);
		Vector2 to = EditorGUILayout.Vector2Field("To", tw.to);

		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
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
