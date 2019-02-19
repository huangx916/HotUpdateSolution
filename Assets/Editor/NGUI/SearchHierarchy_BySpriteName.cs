using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SearchHierarchy_BySpriteName : SearchHierarchy {

	private UIAtlas mAtlas = null;
	private string[] mSprites = new string[0];

	protected override bool OnSearchGUI()
	{
		if (!mAtlas)
		{
			mAtlas = NGUISettings.atlas;
		}
		ComponentSelector.Draw<UIAtlas>("Atlas", mAtlas, obj => {
			if (mAtlas != obj)
			{
				mAtlas = obj as UIAtlas;
			}
		}, true, GUILayout.MinWidth(80f));

		if (!mAtlas)
		{
			mSprites = new string[0];
			return false;
		}

		GUILayout.BeginHorizontal();
		GUILayout.Space(5F);
		EditorGUILayout.BeginVertical();
		if (NGUIEditorTools.DrawMinimalisticHeader("Sprites"))
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(15F);
			EditorGUILayout.BeginVertical();
			GUILayout.Space(2F);

			int newLength = EditorGUILayout.IntField("Size", mSprites.Length);
			int oldLength = mSprites.Length;
			if (newLength != oldLength)
			{
				int minLength = Mathf.Min(newLength, oldLength);
				string[] sprites = new string[newLength];
				for (int i = 0; i < minLength; i++)
				{
					sprites[i] = mSprites[i];
				}
				mSprites = sprites;
			}
			for (int i = 0; i < mSprites.Length; i++)
			{
				int index = i;
				NGUIEditorTools.DrawAdvancedSpriteField(mAtlas, mSprites[index], spriteName =>
				{
					mSprites[index] = spriteName;
					Repaint();
				}, false);
			}

			GUILayout.Space(2F);
			EditorGUILayout.EndVertical();
			GUILayout.Space(3F);
			EditorGUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(10F);

		GUILayout.BeginHorizontal();
		GUILayout.Space(10F);
		bool search = GUILayout.Button("Search");
		GUILayout.Space(10F);
		GUILayout.EndHorizontal();

		return search;
	}

	protected override ICollection<Object> GetFilteredTargets()
	{
		List<string> names = new List<string>();
		for (int index = 0; index < mSprites.Length; index++)
		{
			if (!string.IsNullOrEmpty(mSprites[index]))
			{
				names.Add(mSprites[index]);
			}
		}

		List<Object> list = new List<Object>();
		foreach (UISprite sprite in Selection.GetFiltered(typeof(UISprite), SelectionMode.Deep))
		{
			if (sprite.atlas == mAtlas)
			{
				if (names.Count == 0 || names.Contains(sprite.spriteName))
				{
					list.Add(sprite.gameObject);
				}
			}
		}
		return list;
	}

	[@MenuItem("Window/Search/Search Hierarchy By Sprite Name")]
	private static void Window()
	{
		ShowSearchWindow<SearchHierarchy_BySpriteName>("Search By Sprite Name");
	}
}
