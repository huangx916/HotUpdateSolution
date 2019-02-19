//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenClipRegion))]
public class TweenClipRegionEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		TweenClipRegion tw = target as TweenClipRegion;
		GUI.changed = false;

		EditorGUILayout.LabelField("From");
		Vector4 from = tw.from;
		Vector2 centerFrom = EditorGUILayout.Vector2Field("    Center", new Vector2(from.x, from.y));
		Vector2 sizeFrom = EditorGUILayout.Vector2Field("    Size", new Vector2(from.z, from.w));
		from.x = centerFrom.x;
		from.y = centerFrom.y;
		from.z = sizeFrom.x;
		from.w = sizeFrom.y;
		EditorGUILayout.LabelField("To");
		Vector4 to = tw.to;
		Vector2 centerTo = EditorGUILayout.Vector2Field("    Center", new Vector2(to.x, to.y));
		Vector2 sizeTo = EditorGUILayout.Vector2Field("    Size", new Vector2(to.z, to.w));
		to.x = centerTo.x;
		to.y = centerTo.y;
		to.z = sizeTo.x;
		to.w = sizeTo.y;

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
