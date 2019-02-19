using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SearchHierarchy_ByAtlas : SearchHierarchy {

	private int mNullCount;
	private bool mNullSelect;
	private Dictionary<UIAtlas, int> mAtlasCountDict = new Dictionary<UIAtlas, int>();
	private Dictionary<UIAtlas, bool> mAtlasSelectDict = new Dictionary<UIAtlas, bool>();
	private List<Object> mSprites = new List<Object>();
	private List<Object> mLabels = new List<Object>();
	private bool mSearchSprite = true;
	private bool mSearchLabel = true;
	private bool mShowSprite = true;
	private bool mShowLabel = true;

	protected override bool OnSearchGUI()
	{
		EditorGUILayout.BeginHorizontal();
		mSearchSprite = GUILayout.Toggle(mSearchSprite, "Count Sprite");
		mSearchLabel = GUILayout.Toggle(mSearchLabel, "Count Label");
		EditorGUILayout.EndHorizontal();

		bool selectedAnyWidget = mSearchSprite || mSearchLabel;
		GUI.enabled = selectedAnyWidget;
		bool count = GUILayout.Button("Count Atlas");
		GUI.enabled = true;

		if (count)
		{
			mSprites = mSearchSprite ? new List<Object>(Selection.GetFiltered(typeof(UISprite), SelectionMode.Deep)) : new List<Object>();
			mLabels = mSearchLabel ? new List<Object>(Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep)) : new List<Object>();

			mNullSelect = false;
			mAtlasSelectDict.Clear();
			CountAtlas();
		}
		else
		{
			RemoveInvalid();
		}

		float btnWidth = 50F;
		float toggleWidth = EditorGUIUtility.currentViewWidth - 20 - btnWidth;
		if (mNullCount > 0)
		{
			mNullSelect = GUILayout.Toggle(mNullSelect, "Null × " + mNullCount);
		}
		foreach (KeyValuePair<UIAtlas, int> pair in mAtlasCountDict)
		{
			EditorGUILayout.BeginHorizontal();
			mAtlasSelectDict[pair.Key] = GUILayout.Toggle(mAtlasSelectDict[pair.Key], pair.Key + " × " + pair.Value, GUILayout.Width(toggleWidth));
			if (GUILayout.Button("Select", GUILayout.Width(btnWidth)))
			{
				Selection.activeObject = pair.Key.gameObject;
			}
			EditorGUILayout.EndHorizontal();
		}

		bool selectedAnyAtlas = mNullSelect;
		if (!selectedAnyAtlas)
		{
			foreach (bool selected in mAtlasSelectDict.Values)
			{
				if (selected)
				{
					selectedAnyAtlas = true;
					break;
				}
			}
		}

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		if (mSprites.Count > 0)
		{
			mShowSprite = GUILayout.Toggle(mShowSprite, "Show Sprite");
		}
		else
		{
			GUILayout.Label("                     ");
		}
		if (mLabels.Count > 0)
		{
			mShowLabel = GUILayout.Toggle(mShowLabel, "Show Label");
		}
		else
		{
			GUILayout.Label("                     ");
		}
		EditorGUILayout.EndHorizontal();

		GUI.enabled = selectedAnyAtlas;
		bool search = GUILayout.Button("Show Selected");
		GUI.enabled = true;

		return search;
	}

	private void CountAtlas()
	{
		Dictionary<UIAtlas, bool> atlasSelectDict = new Dictionary<UIAtlas, bool>();
		mNullCount = 0;
		mAtlasCountDict.Clear();
		foreach (UISprite sprite in mSprites)
		{
			UIAtlas atlas = sprite.atlas;
			if (atlas)
			{
				if (!mAtlasCountDict.ContainsKey(atlas))
				{
					mAtlasCountDict[atlas] = 1;
					atlasSelectDict[atlas] = mAtlasSelectDict.ContainsKey(atlas) ? mAtlasSelectDict[atlas] : false;
				}
				else
				{
					++mAtlasCountDict[atlas];
				}
			}
			else
			{
				++mNullCount;
			}
		}
		foreach (UILabel label in mLabels)
		{
			UIFont font = label.bitmapFont;
			if (font)
			{
				if (font.dynamicFont == null)
				{
					UIAtlas atlas = font.atlas;
					if (atlas)
					{
						if (!mAtlasCountDict.ContainsKey(atlas))
						{
							mAtlasCountDict[atlas] = 1;
							atlasSelectDict[atlas] = mAtlasSelectDict.ContainsKey(atlas) ? mAtlasSelectDict[atlas] : false;
						}
						else
						{
							++mAtlasCountDict[atlas];
						}
					}
					else if (!font.material)
					{
						++mNullCount;
					}
				}
			}
			else
			{
				++mNullCount;
			}
		}
		mAtlasSelectDict = atlasSelectDict;
	}

	private void RemoveInvalid()
	{
		bool changed = false;
		for (int index = mSprites.Count - 1; index >= 0; --index)
		{
			if (!mSprites[index])
			{
				mSprites.RemoveAt(index);
				changed = true;
			}
		}
		for (int index = mLabels.Count - 1; index >= 0; --index)
		{
			if (!mLabels[index])
			{
				mLabels.RemoveAt(index);
				changed = true;
			}
		}
		if (changed)
		{
			CountAtlas();
		}
	}

	protected override ICollection<Object> GetFilteredTargets()
	{
		List<Object> list = new List<Object>();
		if (mShowSprite)
		{
			foreach (UISprite sprite in mSprites)
			{
				UIAtlas atlas = sprite.atlas;
				if (atlas)
				{
					if (mAtlasSelectDict.ContainsKey(atlas) && mAtlasSelectDict[atlas])
					{
						list.Add(sprite.gameObject);
					}
				}
				else
				{
					if (mNullSelect)
					{
						list.Add(sprite.gameObject);
					}
				}
			}
		}
		if (mShowLabel)
		{
			foreach (UILabel label in mLabels)
			{
				UIFont font = label.bitmapFont;
				if (font)
				{
					if (font.dynamicFont == null)
					{
						UIAtlas atlas = font.atlas;
						if (atlas)
						{
							if (mAtlasSelectDict.ContainsKey(atlas) && mAtlasSelectDict[atlas])
							{
								list.Add(label.gameObject);
							}
						}
						else if (!font.material)
						{
							if (mNullSelect)
							{
								list.Add(label.gameObject);
							}
						}
					}
				}
				else
				{
					if (mNullSelect)
					{
						list.Add(label.gameObject);
					}
				}
			}
		}
		return list;
	}

	[@MenuItem("Window/Search/Search Hierarchy By Atlas")]
	private static void Window()
	{
		ShowSearchWindow<SearchHierarchy_ByAtlas>("Search Atlas");
	}
}
