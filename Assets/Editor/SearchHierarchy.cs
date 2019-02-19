using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class SearchHierarchy : EditorWindow
{
	protected class Node
	{
		public int index;
		public GameObject go;
		public string path;
		public bool isLeaf;
		public List<int> list;
		public Dictionary<int, Node> dict;

		public Node(int index, GameObject go, string path, bool isLeaf = false)
		{
			this.index = index;
			this.go = go;
			this.path = path;
			this.isLeaf = isLeaf;
			list = new List<int>();
			dict = new Dictionary<int, Node>();
		}

		public int depth
		{
			get
			{
				return path.Split('/').Length - 2;
			}
		}

		public string name
		{
			get
			{
				return go.name;
			}
		}

		public void Sort()
		{
			list.Clear();

			if (dict.Count <= 0)
			{
				return;
			}

			foreach (KeyValuePair<int, Node> pair in dict)
			{
				pair.Value.Sort();
				list.Add(pair.Key);
			}
			list.Sort();
		}
	}

	protected Node mRootNode = new Node(0, null, "");
	protected Dictionary<GameObject, bool> mSelectedDict = new Dictionary<GameObject, bool>();
	protected bool mTargetSelect;

	protected ICollection<Object> mFilteredTargets;
	protected Dictionary<string, bool> mPathStatusDict = new Dictionary<string, bool>();

	private Vector2 mScroll = Vector2.zero;

	protected abstract bool OnSearchGUI();

	private void OnGUI()
	{
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal();
		mTargetSelect = EditorPrefs.GetBool("Select Target", true);
		bool targetSelectTemp = EditorGUILayout.Toggle("Select Target", mTargetSelect);
		if (targetSelectTemp != mTargetSelect)
		{
			mTargetSelect = targetSelectTemp;
			EditorPrefs.SetBool("Select Target", mTargetSelect);
		}
		bool research = GUILayout.Button("Research");
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);

		bool search = OnSearchGUI();
		if (search)
		{
			Clear();
			mFilteredTargets = GetFilteredTargets();
		}
		if (search || research)
		{
			foreach (Object obj in mFilteredTargets)
			{
				Transform trans;
				if (obj is GameObject)
				{
					trans = (obj as GameObject).transform;
				}
				else if (obj is Component)
				{
					trans = (obj as Component).transform;
				}
				else
				{
					continue;
				}

				Stack<Transform> path = new Stack<Transform>();
				while (trans != null)
				{
					path.Push(trans);
					trans = trans.parent;
				}
				string pathStr = "";
				Node parentNode = mRootNode;
				while (path.Count > 0)
				{
					Transform tempTrans = path.Pop();
					int tempIndex = tempTrans.GetSiblingIndex();
					pathStr += tempIndex + "/";
					if (!parentNode.dict.ContainsKey(tempIndex))
					{
						GameObject tempGo = tempTrans.gameObject;
						if (path.Count == 0)
						{
							mSelectedDict[tempGo] = true;
							parentNode.dict.Add(tempIndex, new Node(tempIndex, tempGo, pathStr, true));
						}
						else
						{
							parentNode.dict.Add(tempIndex, new Node(tempIndex, tempGo, pathStr));
						}
					}
					else if (path.Count == 0)
					{
						Node tempNode = parentNode.dict[tempIndex];
						mSelectedDict[tempNode.go] = true;
						tempNode.isLeaf = true;
					}
					parentNode = parentNode.dict[tempIndex];
				}
			}
			OnButtonLateClick();
		}

		mScroll = EditorGUILayout.BeginScrollView(mScroll);
		GUILayout.Space(10F);
		foreach (int index in mRootNode.list)
		{
			DrawNode(mRootNode.dict[index]);
		}
		GUILayout.Space(20F);
		GUILayout.EndScrollView();
	}

	protected abstract ICollection<Object> GetFilteredTargets();

	protected void Clear()
	{
		mRootNode.dict.Clear();
		mSelectedDict.Clear();
		mPathStatusDict.Clear();
	}

	protected void OnButtonLateClick()
	{
		mRootNode.Sort();
		if (mTargetSelect)
		{
			List<GameObject> selectedList = new List<GameObject>();
			foreach (GameObject go in mSelectedDict.Keys)
			{
				selectedList.Add(go);
			}
			Selection.objects = selectedList.ToArray();
		}
	}

	protected void DrawNode(Node node)
	{
		if (!node.go)
		{
			return;
		}

		if (node.dict.Count > 0)
		{
			if (DrawHeader(node))
			{
				foreach (int index in node.list)
				{
					DrawNode(node.dict[index]);
				}
			}
		}
		else
		{
			int indentation = 20 * node.depth;

			GUILayout.BeginHorizontal();

			GUILayout.Space(5 + indentation);
			Color normalColor = GUI.contentColor;
			GUI.contentColor = new Color(1f, 0.3f, 0.3f, 1f);
			GUILayout.Label(GetDisplayName(node), new GUIStyle("PreToolbar2"), GUILayout.Width(EditorGUIUtility.currentViewWidth - 5 - indentation));
			GUILayout.Space(-EditorGUIUtility.currentViewWidth);
			GUI.contentColor = normalColor;

			DrawLine(node);
			GUILayout.EndHorizontal();
		}
	}

	protected virtual string GetDisplayName(Node node)
	{
		return node.name;
	}

	protected bool DrawHeader(Node node)
	{
		bool state = GetBool(node.path, true);
		int indentation = 20 * node.depth;

		GUILayout.BeginHorizontal();

		GUILayout.Space(5 + indentation);
		Color normalColor = GUI.contentColor;
		GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f, 1f) : new Color(0.2f, 0.2f, 0.2f, 1f);
		if (!GUILayout.Toggle(true, state ? "\u25BC" : "\u25BA", "PreToolbar2", GUILayout.Width(12f)))
			state = !state;
		GUILayout.Space(-17 - indentation);

		if (node.isLeaf)
		{
			GUILayout.Space(17 + indentation);
			GUI.contentColor = new Color(1f, 0.3f, 0.3f, 1f);
			GUILayout.Label(GetDisplayName(node), new GUIStyle("PreToolbar2"), GUILayout.Width(EditorGUIUtility.currentViewWidth - 17 - indentation));
			GUILayout.Space(-EditorGUIUtility.currentViewWidth);
		}
		else
		{
			GUILayout.Space(11 + indentation);
			GUILayout.Label(GetDisplayName(node), GUILayout.Width(EditorGUIUtility.currentViewWidth - 17 - indentation));
			GUILayout.Space(-EditorGUIUtility.currentViewWidth);
		}
		GUI.contentColor = normalColor;

		DrawLine(node);

		GUILayout.EndHorizontal();

		if (GUI.changed)
		{
			SetBool(node.path, state);
		}
		return state;
	}

	protected void DrawLine(Node node)
	{
		bool selected = node.go && mSelectedDict.ContainsKey(node.go);
		if (selected)
		{
			Color normalColor = GUI.color;
			GUI.color = new Color(0.4f, 0.7f, 1f, 0.4f);
			
			if (GUILayout.Button("", new GUIStyle("As TextArea"), GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
			{
				OnObjectSelected(node.go);
			}
			GUILayout.Space(-EditorGUIUtility.currentViewWidth);
			GUI.color = normalColor;
		}
		else
		{
			if (GUILayout.Button("", new GUIStyle("PreToolbar2"), GUILayout.Width(EditorGUIUtility.currentViewWidth)))
			{
				OnObjectSelected(node.go);
			}
			GUILayout.Space(-EditorGUIUtility.currentViewWidth);
		}
	}

	private void OnObjectSelected(GameObject go)
	{
		if (!Event.current.control)
		{
			mSelectedDict.Clear();
			mSelectedDict.Add(go, true);
			if (mTargetSelect)
			{
				Selection.objects = new Object[] { go };
			}
			return;
		}

		if (mSelectedDict.ContainsKey(go))
		{
			mSelectedDict.Remove(go);
		}
		else
		{
			mSelectedDict.Add(go, true);
		}
		if (mTargetSelect)
		{
			List<Object> selectedList = new List<Object>();
			foreach (Object obj in mSelectedDict.Keys)
			{
				selectedList.Add(obj);
			}
			Selection.objects = selectedList.ToArray();
		}
	}

	private bool GetBool(string key, bool defaultValue = true)
	{
		if (mPathStatusDict.ContainsKey(key))
		{
			return mPathStatusDict[key];
		}
		return defaultValue;
	}

	private void SetBool(string key, bool value)
	{
		mPathStatusDict[key] = value;
	}

	protected static void ShowSearchWindow<T>(string windowName, float minWidth = 320, float minHeight = 320) where T : SearchHierarchy
	{
		SearchHierarchy window = GetWindow(typeof(T), false, windowName) as SearchHierarchy;
		window.minSize = new Vector2(minWidth, minHeight);
		window.ShowTab();
	}
}
