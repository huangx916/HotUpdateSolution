using UnityEditor;
using System;
using CSObjectWrapEditor;
using System.Threading;
class TenonBuildScript
{
     static void PerformBuild ()
     {
		string baseMacro = "TENON_CUBE";
		string[] arguments = Environment.GetCommandLineArgs ();
		for (int i = 0; i < arguments.Length; i++) {
			if (arguments[i] == "-tenonMacro") {
				baseMacro = arguments [i + 1];
				i = i + 1;
			}
		}
		string gameMacro = "HOTFIX_ENABLE;LOAD_FROM_RES;DEBUG_MACRO;" + baseMacro;
		Console.WriteLine("Tenon Used Macro: {0}", gameMacro);

		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, gameMacro);
		Generator.GenAll ();
        string[] scenes = { "Assets/Scenes/Main.unity" };
        string path = "cube.apk";	
        BuildPipeline.BuildPlayer(scenes, path, BuildTarget.Android, BuildOptions.None);
     }
}