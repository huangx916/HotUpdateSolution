using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SearchHierarchy_ByDepth : SearchHierarchy {

	private int mDepthMin = 1000;
	private int mDepthMax = 1000;
	private Dictionary<System.Type, bool> mTypeSelectDict = new Dictionary<System.Type, bool>() {
		{ typeof(UIWidget), false }, { typeof(UITexture), true }, { typeof(UISprite), true },
		{ typeof(UI2DSprite), false }, { typeof(UIBlank), false }, { typeof(UIEffect), false },
	};
	private List<System.Type> mTypeList = new List<System.Type>();
	private Dictionary<GameObject, int> mWidgetDepthDict = new Dictionary<GameObject, int>();

	protected override bool OnSearchGUI()
	{
		mTypeList.Clear();
		foreach (System.Type type in mTypeSelectDict.Keys)
		{
			mTypeList.Add(type);
		}
		foreach (System.Type type in mTypeList)
		{
			mTypeSelectDict[type] = GUILayout.Toggle(mTypeSelectDict[type], type.Name);
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 85;
		mDepthMin = EditorGUILayout.IntField("Select depth:", mDepthMin);
		EditorGUIUtility.labelWidth = 18;
		mDepthMax = EditorGUILayout.IntField("~", mDepthMax);
		EditorGUILayout.EndHorizontal();

		bool search = GUILayout.Button("Search");

		return search;
	}

	protected override ICollection<Object> GetFilteredTargets()
	{
		mWidgetDepthDict.Clear();
		List<Object> list = new List<Object>();
		foreach (UIWidget widget in Selection.GetFiltered(typeof(UIWidget), SelectionMode.Deep))
		{
			int depth = widget.depth;
			if (depth >= mDepthMin && depth <= mDepthMax)
			{
				System.Type type = widget.GetType();
				if (mTypeSelectDict.ContainsKey(type) && mTypeSelectDict[type])
				{
					GameObject go = widget.gameObject;
					list.Add(go);
					mWidgetDepthDict.Add(go, depth);
				}
			}
		}
		return list;
	}

	protected override string GetDisplayName(Node node)
	{
		string name = base.GetDisplayName(node);
		if (mWidgetDepthDict.ContainsKey(node.go))
		{
			return name + ": " + mWidgetDepthDict[node.go];
		}
		return name;
	}

	[@MenuItem("Window/Search/Search Hierarchy By Depth")]
	private static void Window()
	{
		ShowSearchWindow<SearchHierarchy_ByDepth>("Search By Depth");
	}
}
