using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SpritesToMaterials : EditorWindow {

	private UIAtlas mAtlas = null;
	private string[] mSprites = new string[0];

	private void OnGUI()
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
		}
		else
		{
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
		}

		GUILayout.BeginHorizontal();
		GUILayout.Space(10F);
		bool start = GUILayout.Button("Create/Update Materials");
		GUILayout.Space(10F);
		GUILayout.EndHorizontal();

		if (start && mAtlas && mSprites.Length > 0)
		{
			string atlasPath = AssetDatabase.GetAssetPath(mAtlas);
			string dirPath = atlasPath.Replace("/UIAtlas/", "/Materials/").Replace(".prefab", "/");
			DirectoryInfo dir = new DirectoryInfo(dirPath);
			if (!dir.Exists)
			{
				dir.Create();
			}
			bool anyAssetsChanged = false;
			UISpriteData[] spriteDatas = new UISpriteData[mSprites.Length];
			for (int index = 0; index < mSprites.Length; index++)
			{
				string spriteName = mSprites[index];
				if (!string.IsNullOrEmpty(spriteName))
				{
					UISpriteData spriteData = mAtlas.GetSprite(spriteName);
					if (spriteData != null)
					{
						string path = dirPath + spriteName + ".mat";
						bool exist = File.Exists(path);
						Material mat = exist ? AssetDatabase.LoadAssetAtPath<Material>(path) : new Material(mAtlas.spriteMaterial);
						float scaleX = (float) spriteData.width / mAtlas.width;
						float scaleY = (float) spriteData.height / mAtlas.height;
						float offsetX = (float) spriteData.x / mAtlas.width;
						float offsetY = (float) (mAtlas.height - spriteData.y - spriteData.height) / mAtlas.height;
						mat.SetTextureScale("_MainTex", new Vector2(scaleX, scaleY));
						mat.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
						if (exist)
						{
							anyAssetsChanged = true;
						}
						else
						{
							AssetDatabase.CreateAsset(mat, path);
						}
					}
				}
			}
			if (anyAssetsChanged)
			{
				AssetDatabase.SaveAssets();
			}
		}
	}

	[@MenuItem("Window/Sprites To Materials")]
	private static void Window()
	{
		SpritesToMaterials window = GetWindow(typeof(SpritesToMaterials), false, "Sprites To Materials") as SpritesToMaterials;
		window.minSize = new Vector2(420, 320);
		window.ShowTab();
	}
}
