using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MissingScriptsEditor : EditorWindow
{
	private static EditorWindow window;
	private List<GameObject> lstTmp = new List<GameObject>();

	[MenuItem("Custom/MissingScripteEditor")]
	private static void Execute()
	{
		if (window == null)
			window = (MissingScriptsEditor) GetWindow(typeof(MissingScriptsEditor));
		window.Show();
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical("box");
		if (GUILayout.Button("CleanUpSelection", GUILayout.Height(30f)))
		{
			CleanUpSelection();
		}
		GUILayout.EndVertical();
	}

	private void CleanUpSelection()
	{
		var lstSelection = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);

		for (int i = 0; i < lstSelection.Length; ++i)
		{
			EditorUtility.DisplayProgressBar("Checking", "逐个分析中，请勿退出！", (float) i / (float) lstSelection.Length);
			var gameObject = lstSelection[i] as GameObject;
			var components = gameObject.GetComponents<Component>();

			for (int j = 0; j < components.Length; j++)
			{
				// 如果组建为null
				if (components[j] == null)
				{
					CleanUpAsset(gameObject);
					break;
				}
			}
		}
		EditorUtility.ClearProgressBar();
		AssetDatabase.Refresh();

		foreach (var go in lstTmp)
		{
			GameObject.DestroyImmediate(go);
		}
		lstTmp.Clear();
	}

	private void CleanUpAsset(Object asset)
	{
		GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

		// 创建一个序列化对象
		SerializedObject serializedObject = new SerializedObject(go);
		// 获取组件列表属性
		SerializedProperty prop = serializedObject.FindProperty("m_Component");

		var components = go.GetComponents<Component>();
		int r = 0;
		for (int j = 0; j < components.Length; j++)
		{
			// 如果组建为null
			if (components[j] == null)
			{
				// 按索引删除
				prop.DeleteArrayElementAtIndex(j - r);
				r++;
			}
		}

		// 应用修改到对象
		serializedObject.ApplyModifiedProperties();

		// 将数据替换到asset
		// PrefabUtility.ReplacePrefab(go, asset);
		PrefabUtility.CreatePrefab(AssetDatabase.GetAssetPath(asset), go);

		go.hideFlags = HideFlags.HideAndDontSave;

		// 删除临时实例化对象
		lstTmp.Add(go);
	}

}