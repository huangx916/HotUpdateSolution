//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UISpriteAnimations.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UISpriteAnimationEx))]
public class UISpriteAnimationExInspector : Editor
{
	private UISpriteAnimationEx mSpriteAnim;

	public void OnEnable()
	{
		mSpriteAnim = target as UISpriteAnimationEx;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.Space(3f);
		NGUIEditorTools.SetLabelWidth(80f);
		serializedObject.Update();

		NGUIEditorTools.DrawProperty("Frame Index", serializedObject, "mFrameIndex");
		NGUIEditorTools.DrawProperty("Frame Rate", serializedObject, "mFPS");
		NGUIEditorTools.DrawProperty("Loop", serializedObject, "mLoop");
		NGUIEditorTools.DrawProperty("Pixel Snap", serializedObject, "mSnap");
		SerializedProperty sp = NGUIEditorTools.DrawProperty("Auto Play", serializedObject, "mAutoPlay");
		if (sp.boolValue)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			GUILayout.BeginVertical();
			NGUIEditorTools.DrawProperty("Play Mode", serializedObject, "mAutoPlayMode");
			NGUIEditorTools.DrawProperty("Restart", serializedObject, "mRestart");
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		List<UISpriteAnimationEx.Keyframe> keyframes = mSpriteAnim.keyframes;
		for (int index = 0; index < keyframes.Count; index++)
		{
			GUILayout.BeginHorizontal();
			Undo.RecordObject(mSpriteAnim, "UISpriteAnimationEx.Keyframe.Remove");
			bool remove = GUILayout.Button("×", GUILayout.Width(20F), GUILayout.Height(14F));

			UISpriteAnimationEx.Keyframe keyframe = mSpriteAnim.keyframes[index];
			Undo.RecordObject(mSpriteAnim, "UISpriteAnimationEx.Keyframe.FrameIndex");
			keyframe.frameIndex = EditorGUILayout.IntField(keyframe.frameIndex, GUILayout.Width(40F));
			NGUIEditorTools.DrawAdvancedSpriteField(mSpriteAnim.sprite.atlas, keyframe.spriteName, spriteName =>
			{
				keyframe.spriteName = spriteName;
			}, false);

			GUI.enabled = index > 0;
			bool up = GUILayout.Button("▲", GUILayout.Width(20F), GUILayout.Height(14F));
			GUI.enabled = index < keyframes.Count - 1;
			bool down = GUILayout.Button("▼", GUILayout.Width(20F), GUILayout.Height(14F));
			GUI.enabled = true;

			GUILayout.EndHorizontal();

			if (remove)
			{
				keyframes.RemoveAt(index);
			}
			else if (up)
			{
				keyframes[index] = keyframes[index - 1];
				keyframes[index - 1] = keyframe;
			}
			else if (down)
			{
				keyframes[index] = keyframes[index + 1];
				keyframes[index + 1] = keyframe;
			}
		}
		GUILayout.BeginHorizontal();
		Undo.RecordObject(mSpriteAnim, "UISpriteAnimationEx.Keyframe.Append");
		if (GUILayout.Button("+"))
		{
			keyframes.Add(new UISpriteAnimationEx.Keyframe());
		}
		GUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();

		if (Application.isPlaying)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Play"))
			{
				mSpriteAnim.Play();
			}
			if (GUILayout.Button("Pause"))
			{
				mSpriteAnim.Pause();
			}
			GUILayout.EndHorizontal();
		}
	}
}
