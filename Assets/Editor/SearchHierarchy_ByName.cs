using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SearchHierarchy_ByName : SearchHierarchy {

	private string mSearchName = "";

	protected override bool OnSearchGUI()
	{
		mSearchName = EditorGUILayout.TextField("Object Name", mSearchName);

		GUILayout.BeginHorizontal();
			GUILayout.Space(10F);
			bool mNameSearch = GUILayout.Button("Search");
			GUILayout.Space(10F);
		GUILayout.EndHorizontal();

		return !string.IsNullOrEmpty(mSearchName) && mNameSearch;
	}

	protected override ICollection<Object> GetFilteredTargets()
	{
		string[] paths = mSearchName.Split('|');
		List<NameNode> nameNodeList = new List<NameNode>();
		foreach (string path in paths)
		{
			if (!string.IsNullOrEmpty(path))
			{
				nameNodeList.Add(NameNode.Parse(path));
			}
		}

		List<Object> list = new List<Object>();
		foreach (GameObject go in Selection.GetFiltered(typeof(GameObject), SelectionMode.Deep))
		{
			if (Contains(nameNodeList, go))
			{
				list.Add(go);
			}
		}
		return list;
	}

	private bool Contains(List<NameNode> nameNodeList, GameObject go)
	{
		foreach (NameNode nameNode in nameNodeList)
		{
			if (nameNode.IsNameMatch(go))
			{
				return true;
			}
		}
		return false;
	}

	[@MenuItem("Window/Search/Search Hierarchy By Name")]
	private static void ShowWindow()
	{
		ShowSearchWindow<SearchHierarchy_ByName>("Name Search");
	}

	private class NameNode
	{
		private Regex mNameRegex;
		private NameNode mParent;

		public NameNode(string name, NameNode parent = null)
		{
			mNameRegex = new Regex("^" + name + "$");
			mParent = parent;
		}

		public bool IsNameMatch(GameObject go)
		{
			return IsNameMatch(go.transform);
		}

		public bool IsNameMatch(Transform trans)
		{
			if (!mNameRegex.IsMatch(trans.name))
			{
				return false;
			}

			if (mParent == null)
			{
				return true;
			}

			Transform parentTrans = trans.parent;
			if (!parentTrans)
			{
				return false;
			}

			return mParent.IsNameMatch(parentTrans);
		}

		public static NameNode Parse(string path)
		{
			NameNode currentNode = null;
			string[] names = path.Split('/');
			foreach (string name in names)
			{
				if (!string.IsNullOrEmpty(name))
				{
					NameNode node = new NameNode(name, currentNode);
					currentNode = node;
				}
			}
			return currentNode;
		}
	}
}
