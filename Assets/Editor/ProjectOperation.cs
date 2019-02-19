using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Collections.Generic;

public class ProjectOperation
{
	[MenuItem("Assets/Split bitmap font")]
	public static void SplitBitmapFont()
	{
		Object selection = Selection.activeObject;
		if (selection is GameObject)
		{
			UIFont font = (selection as GameObject).GetComponent<UIFont>();
			if (font)
			{
				Material mat = font.material;
				Texture2D tex = mat.GetTexture("_AlphaTex") as Texture2D;
				Debug.LogError(tex);
				tex = NGUIEditorTools.ImportTexture(AssetDatabase.GetAssetPath(tex), true, true, true);
				int texW = tex.width;
				int texH = tex.height;
				Color32[] cols = tex.GetPixels32();
				int fontSize = font.bmFont.charSize;

				string fontPath = AssetDatabase.GetAssetPath(font);
				int slashIndex = fontPath.LastIndexOf('.');
				string dirPath = slashIndex == -1 ? fontPath : fontPath.Substring(0, slashIndex);
				DirectoryInfo dir = new DirectoryInfo(dirPath);
				if (!dir.Exists)
				{
					dir.Create();
				}

				foreach (BMGlyph glyph in font.bmFont.glyphs)
				{
					int glyphW = glyph.advance;
					if (glyphW > 0)
					{
						int glyphH = fontSize;
						int w = Mathf.Min(glyph.width, glyphW);
						int h = Mathf.Min(glyph.height, glyphH);
						int x = glyph.x;
						int y = glyph.y;
						int offsetX = glyphW - w - Mathf.Max(Mathf.Min(glyph.offsetX, glyphW - w), 0);
						int offsetY = glyphH - h - Mathf.Max(Mathf.Min(glyph.offsetY, glyphH - h), 0);
						Color32[] glyphCols = new Color32[glyphW * glyphH];
						Debug.LogErrorFormat("{0}: x {1}, y {2}, w {3}, h {4}, ox {5}, oy {6}, advance {7}, channel {8}, kerning {9}", (char) glyph.index, glyph.x, glyph.y, glyph.width, glyph.height, glyph.offsetX, glyph.offsetY, glyph.advance, glyph.channel, glyph.kerning.Count);
						for (int i = 0; i < h; i++)
						{
							for (int j = 0; j < w; j++)
							{
								try
								{
									byte c = cols[(texH - 1 - y - i) * texW + (x + j)].r;
									glyphCols[(h - 1 - i + offsetY) * glyphW + j + offsetX] = new Color32(255, 255, 255, c);
								}
								catch (Exception e)
								{
									Debug.LogError(e);
									Debug.LogError(h - 1 - i + glyph.offsetY);
									Debug.LogError(j + glyph.offsetX);
									return;
								}
							}
						}
						Texture2D newTex = new Texture2D(glyphW, glyphH);
						newTex.SetPixels32(glyphCols);
						byte[] bytes = newTex.EncodeToPNG();
						string path = dirPath + "/" + glyph.index + ".png";
						File.WriteAllBytes(path, bytes);
						NGUIEditorTools.ImportTexture(path, false, true, false);
					}
				}

				NGUIEditorTools.ImportTexture(AssetDatabase.GetAssetPath(tex), false, true, false);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			}
		}
	}

	[MenuItem("Assets/Count CS Script Lines")]
	public static void CountCSLines()
	{
		Object[] selections = Selection.GetFiltered(typeof(MonoScript), SelectionMode.DeepAssets);
		int count = 0;
		foreach (MonoScript obj in selections)
		{
			string path = AssetDatabase.GetAssetPath(obj);
			if (path.EndsWith(".cs"))
			{
				count += obj.text.Split('\n').Length;
			}
		}
		Debug.LogError(count);
	}
	[MenuItem("Assets/Count Lua Script Lines")]
	public static void CountLuaLines()
	{
		Object[] selections = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);
		int count = 0;
		foreach (TextAsset obj in selections)
		{
			string path = AssetDatabase.GetAssetPath(obj);
			if (path.EndsWith(".lua.txt"))
			{
				count += obj.text.Split('\n').Length;
			}
		}
		Debug.LogError(count);
	}

	[MenuItem("Assets/Log MD5")]
	public static void LogMD5()
	{
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		FileInfo fileInfo = new FileInfo(path);
		FileStream fileStream = fileInfo.Open(FileMode.Open);
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		byte[] retVal = md5.ComputeHash(fileStream);
		fileStream.Close();
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < retVal.Length; i++)
		{
			sb.Append(retVal[i].ToString("x2"));
		}
		Debug.LogError(sb);
	}

	public static void CopyFields(object from, object to)
	{
		System.Type fromType = from.GetType();
		System.Type toType = to.GetType();
		foreach (FieldInfo toInfo in toType.GetFields())
		{
			FieldInfo fromInfo = fromType.GetField(toInfo.Name);
			object value = fromInfo.GetValue(from);
			toInfo.SetValue(to, value);
		}
	}

	[MenuItem("Assets/Select Prefabs")]
	public static void SelectPrefabs()
	{
		List<GameObject> selections = new List<GameObject>();
		foreach (Object selection in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
		{
			if (selection is GameObject)
			{
				GameObject go = selection as GameObject;
				Component[] comps = go.GetComponentsInChildren<Component>();
				foreach (Component comp in comps)
				{
					if (!comp)
					{
						Debug.LogError(comp);
						Object.DestroyImmediate(comp);
					}
				}
			}
		}
	}
	[MenuItem("Assets/Remove Missing Components")]
	public static void RemoveMissingScript()
	{
		foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
		{
			if (obj is GameObject)
			{
				GameObject go = obj as GameObject;
				foreach (Transform child in go.GetComponentsInChildren<Transform>(true))
				{
					RemoveMissingScript(child.gameObject);
				}
			}
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Debug.Log("清理完成!");
	}

	private static void RemoveMissingScript(GameObject go)
	{
		SerializedObject so = new SerializedObject(go);
		SerializedProperty soProperty = so.FindProperty("m_Component");
		Component[] comps = go.GetComponents<Component>();
		for (int propertyIndex = comps.Length - 1; propertyIndex >= 0; propertyIndex--)
		{
			if (!comps[propertyIndex])
			{
				soProperty.DeleteArrayElementAtIndex(propertyIndex);
			}
		}
		so.ApplyModifiedProperties();
	}
}
