using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SearchSprite_ByUnused : EditorWindow {

	private Vector2 mScroll = Vector2.zero;

	private UIAtlas m_atlas = null;
	private List<string> m_spriteNames = new List<string>();

	private void OnGUI()
	{
		GUILayout.Space(10F);

		if (!m_atlas)
		{
			m_atlas = NGUISettings.atlas;
		}
		ComponentSelector.Draw<UIAtlas>("Atlas", m_atlas, obj => {
			if (m_atlas != obj)
			{
				m_atlas = obj as UIAtlas;
			}
		}, true, GUILayout.MinWidth(80f));

		if (m_atlas != null)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			if (GUILayout.Button("Search"))
			{
				m_spriteNames.Clear();

				Object[] uiSprites = Selection.GetFiltered(typeof(UISprite), SelectionMode.Deep);
				Object[] uiLabels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
				foreach (UISpriteData spriteData in m_atlas.spriteList)
				{
					bool found = false;
					foreach (UISprite uiSprite in uiSprites)
					{
						if (uiSprite.atlas == m_atlas && string.Equals(uiSprite.spriteName, spriteData.name))
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						foreach (UILabel uiLabel in uiLabels)
						{
							if (uiLabel.bitmapFont)
							{
								foreach (BMSymbol symbol in uiLabel.bitmapFont.symbols)
								{
									if (string.Equals(symbol.spriteName, spriteData.name))
									{
										found = true;
										break;
									}
								}
								if (found)
								{
									break;
								}
							}
						}
					}
					if (!found)
					{
						m_spriteNames.Add(spriteData.name);
					}
				}
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);

			GUILayout.BeginHorizontal();
			GUILayout.Space(5F);
			EditorGUILayout.BeginVertical();
			if (NGUIEditorTools.DrawMinimalisticHeader("Sprites"))
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(15f);
				EditorGUILayout.BeginVertical();
				mScroll = EditorGUILayout.BeginScrollView(mScroll);
				GUILayout.Space(2f);

				int oldLength = m_spriteNames.Count;
				int newLength = EditorGUILayout.IntField("Size", oldLength);
				if (newLength != oldLength)
				{
					int minLength = Mathf.Min(newLength, oldLength);
					string[] sprites = new string[newLength];
					for (int i = 0; i < minLength; i++)
					{
						sprites[i] = m_spriteNames[i];
					}
					m_spriteNames = new List<string>(sprites);
				}
				for (int i = 0; i < newLength; i++)
				{
					int index = i;
					NGUIEditorTools.DrawAdvancedSpriteField(m_atlas, m_spriteNames[index], spriteName => {
						m_spriteNames[index] = spriteName;
						Repaint();
					}, false);
				}

				GUILayout.Space(2f);
				GUILayout.EndScrollView();
				EditorGUILayout.EndVertical();
				GUILayout.Space(3f);
				EditorGUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
		}
	}

	[@MenuItem("Window/Search/Search Sprite Unused")]
	private static void Window()
	{
		SearchSprite_ByUnused window = GetWindow(typeof(SearchSprite_ByUnused), false, "Unused Sprite Search") as SearchSprite_ByUnused;
		window.minSize = new Vector2(200, 320);
		window.ShowTab();
	}
}
