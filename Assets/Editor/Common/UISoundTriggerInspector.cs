using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XLua;
using Main;
using SimpleJson;
using System.IO;

[CanEditMultipleObjects]
[CustomEditor(typeof(UISoundTrigger), true)]
public class UISoundTriggerInspector : Editor {

	private string m_RootPath = "Assets/AssetBundle/Resources";
	private string m_AudioConfigPath = "{0}/Config/AudioConfig";

	public override void OnInspectorGUI()
	{
		List<string> moduleNameList = GetModuleNameList();
		SerializedProperty moduleNameSp = serializedObject.FindProperty("m_ModuleName");
		int moduleIndex = moduleNameList.IndexOf(moduleNameSp.stringValue);
		int newModuleIndex = EditorGUILayout.Popup("Module Name", moduleIndex == -1 ? 0 : moduleIndex, moduleNameList.ToArray());
		if (newModuleIndex != moduleIndex)
		{
			if (newModuleIndex < moduleNameList.Count)
			{
				moduleNameSp.stringValue = moduleNameList[newModuleIndex];
			}
		}

		List<string> uiSoundNameList = GetSoundNameList(string.Format(m_AudioConfigPath, moduleNameSp.stringValue));
		SerializedProperty soundNameSp = serializedObject.FindProperty("m_SoundName");
		int soundIndex = uiSoundNameList.IndexOf(soundNameSp.stringValue);
		int newSoundIndex = EditorGUILayout.Popup("Sound Name", soundIndex, uiSoundNameList.ToArray());
		if (newSoundIndex == -1)
		{
			newSoundIndex = 0;
		}
		if (newSoundIndex != soundIndex)
		{
			if (newSoundIndex < uiSoundNameList.Count)
			{
				soundNameSp.stringValue = uiSoundNameList[newSoundIndex];
			}
		}
		NGUIEditorTools.DrawProperty("Trigger", serializedObject, "m_Trigger");
		NGUIEditorTools.DrawProperty("Volume", serializedObject, "m_Volume");

		if (GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
		}
	}

	private List<string> GetModuleNameList()
	{
		List<string> moduleNameList = new List<string>();
		moduleNameList.Add("Common");
		DirectoryInfo rootDir = new DirectoryInfo(m_RootPath);
		foreach (DirectoryInfo dir in rootDir.GetDirectories())
		{
			if (!string.Equals(dir.Name, "Common"))
			{
				moduleNameList.Add(dir.Name);
			}
		}
		return moduleNameList;
	}

	private List<string> GetSoundNameList(string path)
	{
		List<string> uiSoundNameList = new List<string>();
		TextAsset textAsset = Resources.Load<TextAsset>(path);
		if (textAsset)
		{
			string jsonStr = textAsset.text;
			if (!string.IsNullOrEmpty(jsonStr))
			{
				JsonArray ja = StringParser.StringToJa(jsonStr, false);
				foreach (object item in ja)
				{
					if (item is JsonObject)
					{
						JsonObject configJo = item as JsonObject;
						uiSoundNameList.Add(JsonParser.JoItemToString(configJo, "name"));
					}
				}
			}
		}
		return uiSoundNameList;
	}
}
