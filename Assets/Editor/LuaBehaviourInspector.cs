using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Main;

[CanEditMultipleObjects]
[CustomEditor(typeof(LuaBehaviour), true)]
public class LuaBehaviourInspector : Editor
{
	protected LuaBehaviour m_LuaBehaviour;

	public void OnEnable()
	{
		m_LuaBehaviour = target as LuaBehaviour;
	}

	public override void OnInspectorGUI()
	{
		GUI.changed = false;

		EditorGUIUtility.labelWidth = 80;

		GUILayout.BeginHorizontal();
		GUILayout.Label("Lua Path:", GUILayout.Width(60F));
		Undo.RecordObject(m_LuaBehaviour, "LuaBehaviour.LuaPath");
		m_LuaBehaviour.m_LuaPath = EditorGUILayout.TextField(m_LuaBehaviour.m_LuaPath ?? "");
		GUILayout.EndHorizontal();

		if (GUI.changed)
		{
			EditorUtility.SetDirty(m_LuaBehaviour);
		}
	}
}
