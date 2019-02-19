using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class SearchHierarchy_ByScript : SearchHierarchy {

	private MonoScript mScript = null;
	private string mClassName = "";
	private bool mScriptSearch;
	private bool mClassNameSearch;

	protected override bool OnSearchGUI()
	{
		mScript = EditorGUILayout.ObjectField("Script", mScript, typeof(MonoScript), true) as MonoScript;

		GUILayout.BeginHorizontal();
			GUILayout.Space(10F);
			mScriptSearch = GUILayout.Button("Search");
			GUILayout.Space(10F);
		GUILayout.EndHorizontal();

		GUILayout.Space(10F);

		mClassName = EditorGUILayout.TextField("Class Name", mClassName);

		GUILayout.BeginHorizontal();
			GUILayout.Space(10F);
			mClassNameSearch = GUILayout.Button("Search");
			GUILayout.Space(10F);
		GUILayout.EndHorizontal();

		return (mScript && mScriptSearch) || (!string.IsNullOrEmpty(mClassName) && mClassNameSearch);
	}

	protected override ICollection<Object> GetFilteredTargets()
	{
		if (mScriptSearch)
		{
			return Selection.GetFiltered(mScript.GetClass(), SelectionMode.Deep);
		}
		if (mClassNameSearch)
		{
			List<Object> list = new List<Object>();
			foreach (GameObject go in Selection.GetFiltered(typeof(GameObject), SelectionMode.Deep))
			{
				if (go.GetComponent(mClassName))
				{
					list.Add(go);
				}
			}
			return list;
		}
		return new Object[0];
	}

	[@MenuItem("Window/Search/Search Hierarchy By Script")]
	private static void ShowWindow()
	{
		ShowSearchWindow<SearchHierarchy_ByScript>("Search By Script");
	}
}
