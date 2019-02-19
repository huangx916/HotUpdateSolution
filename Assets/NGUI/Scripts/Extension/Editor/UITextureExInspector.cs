//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UITextureEx), true)]
public class UITextureExInspector : UITextureInspector
{
	enum TextureType
	{
		Self,
		Reference,
	}

	UITextureEx mTexEx;
	TextureType mType = TextureType.Self;
	UITexture mReplacement = null;

	protected override void OnEnable()
	{
		base.OnEnable();
		mTexEx = target as UITextureEx;
	}

	void OnSelectTexture(Object obj)
	{
		// Undo doesn't work correctly in this case... so I won't bother.
		//NGUIEditorTools.RegisterUndo("Texture Change");
		//NGUIEditorTools.RegisterUndo("Texture Change", mTexEx);

		mTexEx.replacement = obj as UITexture;
		mReplacement = mTexEx.replacement;
		NGUITools.SetDirty(mTexEx);
	}

	protected override bool ShouldDrawProperties ()
	{
		if (mTexEx == null) return false;

		if (mTexEx.replacement != null)
		{
			mType = TextureType.Reference;
			mReplacement = mTexEx.replacement;
		}

		GUI.changed = false;
		GUILayout.BeginHorizontal();
		mType = (TextureType) EditorGUILayout.EnumPopup("Texture Type", mType);
		NGUIEditorTools.DrawPadding();
		GUILayout.EndHorizontal();

		if (GUI.changed)
		{
			if (mType == TextureType.Self)
				OnSelectTexture(null);
		}

		if (mType == TextureType.Reference)
		{
			GUI.changed = false;
			Object obj = EditorGUILayout.ObjectField("Replacement", mTexEx.replacement, typeof(UITexture), true);
			if (GUI.changed)
			{
				OnSelectTexture(obj);
			}

			GUILayout.Space(6f);
			EditorGUILayout.HelpBox("You can have one texture simply point to " +
				"another one. This is useful if you want to be " +
				"able to quickly replace the contents of one " +
				"texture with another one, for example for " +
				"swapping an SD font with an HD one, or " +
				"replacing an English texture with a Chinese " +
				"one. All the textures referencing this texture " +
				"will update their references to the new one.", MessageType.Info);

			if (mReplacement != mTexEx && mTexEx.replacement != mReplacement)
			{
				NGUIEditorTools.RegisterUndo("Texture Change", mTexEx);
				mTexEx.replacement = mReplacement;
				NGUITools.SetDirty(mTexEx);
			}
			return true;
		}
		else
		{
			return base.ShouldDrawProperties();
		}
	}
}

/// <summary>
/// UITextureInspector's extension.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UITexture), true)]
public class UITextureInspectorEx : UITextureInspector
{

	protected override void DrawFinalProperties()
	{
		base.DrawFinalProperties();

		UITexture mTex = target as UITexture;
		if (target.GetType() == typeof(UITexture))
		{
			GUILayout.Space(3f);

			if (GUILayout.Button("Upgrade to a Texture Ex"))
			{
				NGUIEditorTools.ReplaceClass(serializedObject, typeof(UITextureEx));
				mTex.SendMessage("Start");
				Selection.activeGameObject = null;
				EditorApplication.delayCall = () => Selection.activeGameObject = mTex.gameObject;
			}
		}
	}
}