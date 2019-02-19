using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(DefaultAsset))]
public class StreamingAssetsInspector : Editor
{
	private const int MAX_STRING_LENGTH = 16382;
	private const string ASSET_BUNDLE_MANIFEST_FILENAME = "manifest.ab";
	private const string ASSET_BUNDLE_EXTENSION = ".ab";
	private static string[] s_TextFileExtensions = new string[] { ".txt", ".manifest", ".json", ".xml", ".lua" };

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		foreach (Object item in targets)
		{
			DrawTarget(item);
		}
	}

	private void DrawTarget(Object target)
	{
		string path = AssetDatabase.GetAssetPath(target);
		if (File.Exists(path) && path.StartsWith("Assets/StreamingAssets/"))
		{
			if (IsTextFile(path))
			{
				string text = File.ReadAllText(path);
				DrawItem(path, text);
			}
			else if (path.EndsWith(ASSET_BUNDLE_EXTENSION))
			{
				DrawAssetBundle(path);
			}
		}
	}

	private bool IsTextFile(string path)
	{
		foreach (string textFileExtension in s_TextFileExtensions)
		{
			if (path.EndsWith(textFileExtension))
			{
				return true;
			}
		}
		return false;
	}

	private void DrawAssetBundle(string path)
	{
		if (path.EndsWith(ASSET_BUNDLE_MANIFEST_FILENAME))
		{
			AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
			if (assetBundle)
			{
				AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
				string[] bundleNames = manifest.GetAllAssetBundles();
				StringBuilder sb = new StringBuilder();
				foreach (string bundleName in bundleNames)
				{
					sb.Append("- ");
					sb.AppendLine(bundleName);
					string[] dependencies = manifest.GetDirectDependencies(bundleName);
					foreach (string dependency in dependencies)
					{
						sb.Append("\t");
						sb.AppendLine(dependency);
					}
				}
				int lineEndLength = System.Environment.NewLine.Length;
				if (sb.Length > lineEndLength)
				{
					sb.Remove(sb.Length - lineEndLength, lineEndLength);
				}
				assetBundle.Unload(true);
				DrawItem(path, sb.ToString());
			}
		}
		else
		{
			Dictionary<string, string> txtDict = new Dictionary<string, string>();
			StringBuilder sb = new StringBuilder();

			AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
			if (assetBundle)
			{
				string[] assetNames = assetBundle.GetAllAssetNames();

				foreach (string assetPath in assetNames)
				{
					if (IsTextFile(assetPath))
					{
						TextAsset textAsset = assetBundle.LoadAsset<TextAsset>(assetPath);
						txtDict[assetPath] = textAsset.text ?? "";
					}
				}

				sb.AppendLine("Assets:");
				foreach (string assetName in assetNames)
				{
					sb.Append("- ");
					sb.AppendLine(assetName);
				}
				int lineEndLength = System.Environment.NewLine.Length;
				if (sb.Length > lineEndLength)
				{
					sb.Remove(sb.Length - lineEndLength, lineEndLength);
				}

				assetBundle.Unload(true);
			}
			if (txtDict.Count > 0)
			{
				foreach (string assetPath in txtDict.Keys)
				{
					DrawItem(assetPath, txtDict[assetPath]);
				}
			}
			else
			{
				DrawItem(path, sb.ToString());
			}
		}
	}

	private void DrawItem(string title, string content)
	{
		GUILayout.TextField(title);

		int totalLength = content.Length;
		int length = 0;
		while (length < totalLength)
		{
			int currentLength = Mathf.Min(totalLength - length, MAX_STRING_LENGTH);
			GUILayout.TextArea(content.Substring(length, currentLength));
            length += currentLength;
		}

        GUILayout.Space(10);
	}
}