using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJson;
using Main;
using System.Security.Cryptography;
using System.Text;

public class AssetBundleBuilder : EditorWindow {

	[Flags]
	private enum StepType
	{
		None = 0,
		SetName = 1,
		BuildBundle = 2,
		CreateFileList = 4,
		CreateVersionFile = 8,
		CheckFileExsit = 16
	}

	private struct Platform
	{
		public string name;
		public BuildTarget buildTarget;
		public Color backgroundColor;
		public Color contentColor;

		public Platform(string name, BuildTarget buildTarget) : this(name, buildTarget, Color.white, Color.gray)
		{
		}

		public Platform(string name, BuildTarget buildTarget, Color backgroundColor, Color contentColor)
		{
			this.name = name;
			this.buildTarget = buildTarget;
			this.backgroundColor = backgroundColor;
			this.contentColor = contentColor;
		}

		private static Platform mAndroid = new Platform("Android", BuildTarget.Android, new Color(1, 0.9F, 0.9F), new Color(0.85F, 1, 1));
		public static Platform Android { get { return mAndroid; } }

		private static Platform mIPhone = new Platform("IOS", BuildTarget.iOS, new Color(0.85F, 1, 1), new Color(0.8F, 0.9F, 0.8F));
		public static Platform IPhone { get { return mIPhone; } }

		private static Platform mWindows = new Platform("Windows", BuildTarget.StandaloneWindows64, new Color(1, 1, 0.85F), new Color(0.8F, 0.9F, 0.8F));
		public static Platform Windows { get { return mWindows; } }

		private static Platform mWebGL = new Platform("WebGL", BuildTarget.WebGL, new Color(1, 0.9F, 0.95F), new Color(0.9F, 0.9F, 0.95F));
		public static Platform WebGL { get { return mWebGL; } }
	}
	private static Platform CurrentPlatform
	{
		get
		{
#if UNITY_IPHONE
			return Platform.IPhone;
#elif UNITY_ANDROID
			return Platform.Android;
#elif UNITY_WEBGL
			return Platform.WebGL;
#else
			return Platform.Windows;
#endif
		}
	}

	private const char DIR_BUNDLE_PREFIX = '@';
	private const char SUB_SEPARATOR = '`';

	private const string SRC_DIR = "Assets/AssetBundle/Resources";
	private const string OUTPUT_PATH = "Assets/StreamingAssets" + "/" + ConstValue.BUNDLE_DIR;
	private static string[] s_ModuleNames = new string[0];
	private string mModuleName = "";

	private const BuildAssetBundleOptions OPTIONS = BuildAssetBundleOptions.DeterministicAssetBundle;

	private static string NOTICE_FOLDER_DRAG = "Select module here.";
	private static string NOTICE_ONE_CLICK = "Click following buttons one by one.";
	private static string NOTICE_NAME = "Set asset bundle name by asset name.\n" +
										"If file name starts with \"" + DIR_BUNDLE_PREFIX + "\", it will use directory name.\n" +
										"If file path contains \"" + SUB_SEPARATOR + "\", it will sub-name before first \"" + SUB_SEPARATOR + "\".";
	private static string NOTICE_BUILD = "Build asset bundle, assets has same bundle-name will be built in same bundle.";
	private static string NOTICE_VERSION = "Create files of version-info and file-list.";

	private static string[] s_CopyDirs = { "Share" };
	private static HashSet<string> s_ExceptExtensionSet = new HashSet<string>() { ".cginc" };
	//private static Dictionary<string, string> s_ExtensionReplaceDict = new Dictionary<string, string>() { { ".lua.txt", ".lua" } };

	private bool m_AssetsChanged = false;
	private StepType m_Step;

	private Vector2 m_Scroll = Vector2.zero;

	void OnGUI()
	{
		GUILayout.Space(5F);
		GUI.backgroundColor = CurrentPlatform.backgroundColor;
		DrawHeader(CurrentPlatform.name);
		m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll);
		GUILayout.BeginVertical("As TextArea");

		GUILayout.Space(5F);
		GUI.backgroundColor = CurrentPlatform.contentColor;

		m_Step = StepType.None;

		BeginVerticalArea("Module Path", NOTICE_FOLDER_DRAG);
		DrawFolderToggle();
		EndVerticalArea();

		GUILayout.Space(10F);

		BeginVerticalArea("One Click", NOTICE_ONE_CLICK);
		DrawOneClick();
		EndVerticalArea();

		GUILayout.Space(10F);

		BeginVerticalArea("Set Name", NOTICE_NAME);
		DrawSetName();
		EndVerticalArea();

		GUILayout.Space(10F);

		BeginVerticalArea("Build Bundle", NOTICE_BUILD);
		DrawBuildBundle();
		EndVerticalArea();

		GUILayout.Space(10F);

		BeginVerticalArea("Create Md5", NOTICE_BUILD);
		DrawCreateMd5();
		EndVerticalArea();

		GUILayout.Space(10F);

		BeginVerticalArea("Create Version", NOTICE_VERSION);
		DrawCreateVersion();
		EndVerticalArea();

		GUILayout.Space(5F);

		GUILayout.EndVertical();
		GUILayout.EndScrollView();

		if (m_Step != StepType.None)
		{
			OnStepTrigger();
		}
	}

	private void OnStepTrigger()
	{
		DateTime startDt = DateTime.Now;

		int count = s_ModuleNames.Length;
		for (int index = 0; index < count; index++)
		{
			string moduleName = s_ModuleNames[index];
			if (moduleName != null)
			{
				Debug.Log("Module: " + moduleName);
				SetModule(moduleName);
				if ((m_Step & StepType.SetName) != 0)
				{
					Debug.Log("Set Name");
					SetNames();
				}
				if ((m_Step & StepType.BuildBundle) != 0)
				{
					Debug.Log("Build Bundle");
					BuildBundles();
				}
				if ((m_Step & StepType.CreateFileList) != 0)
				{
					Debug.Log("Create File List");
					CreateFileList();
				}
				if ((m_Step & StepType.CreateVersionFile) != 0)
				{
					Debug.Log("Create Version File");
					CreateVersion();
				}
			}
		}

		if (m_AssetsChanged)
		{
			m_AssetsChanged = false;
			AssetDatabase.Refresh();
		}

		Debug.Log("Finished! Cost " + (DateTime.Now - startDt).TotalMilliseconds + "ms");
	}

	private void SetModule(string moduleName)
	{
		mModuleName = moduleName;
	}

	private int selectedIndex = -1;
	private void DrawFolderToggle()
	{
		Color contentColor = CurrentPlatform.contentColor;
		GUI.backgroundColor = selectedIndex == -1 ? new Color(1, 0.5F, 0.5F) : new Color(0.5F, 1, 0.5F);
		DirectoryInfo directory = new DirectoryInfo(SRC_DIR);
		if (directory.Exists)
		{
			DirectoryInfo[] moduleDirs = directory.GetDirectories();
			int count = moduleDirs.Length;
			int oldCount = s_ModuleNames.Length;
			if (oldCount != count)
			{
				string[] newNames = new string[count];
				for (int index = 0; index < oldCount; index++)
				{
					newNames[index] = s_ModuleNames[index];
				}
				s_ModuleNames = newNames;
			}
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20F);
			EditorGUILayout.BeginVertical();
			for (int index = 0; index < count; index++)
			{
				string name = moduleDirs[index].Name;
				bool selected = EditorGUILayout.ToggleLeft(" " + name, !string.IsNullOrEmpty(s_ModuleNames[index]));
				if (selected)
				{
					s_ModuleNames[index] = name;
				}
				else
				{
					s_ModuleNames[index] = null;
				}
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			//List<string> listName = new List<string>();
			//foreach (var dir in moduleDirs)
			//{
			//	listName.Add(dir.Name);
			//}
			//selectedIndex = selectedIndex >= listName.Count ? -1 : selectedIndex;
			//selectedIndex = EditorGUILayout.Popup("Folder Name", selectedIndex, listName.ToArray());
			//if (selectedIndex == -1)
			//{
			//	SetDir(true);
			//}
			//else
			//{
			//	SetDir(false, listName[selectedIndex]);
			//}
		}
		GUI.backgroundColor = contentColor;
	}


	private void DrawOneClick()
	{
		Color contentColor = CurrentPlatform.contentColor;
		GUI.backgroundColor = new Color(1, 0.85F, 0.7F);
		if (GUILayout.Button("One Click"))
		{
			m_Step = (StepType) 0 - 1;
		}
		GUI.backgroundColor = contentColor;
	}

	private void DrawSetName()
	{
		if (GUILayout.Button("Set Name"))
		{
			m_Step = StepType.SetName;
		}
	}

	private void DrawBuildBundle()
	{
		if (GUILayout.Button("Build Bundle"))
		{
			m_Step = StepType.BuildBundle;
		}
	}

	private void DrawCreateMd5()
	{
		if (GUILayout.Button("Create File List"))
		{
			m_Step = StepType.CreateFileList;
		}
	}

	private void DrawCreateVersion()
	{
		if (GUILayout.Button("Create Version File"))
		{
			m_Step = StepType.CreateVersionFile;
		}
	}

	private void SetNames()
	{
		string srcModuleDir = SRC_DIR + "/" + mModuleName;
		DirectoryInfo dir = new DirectoryInfo(srcModuleDir);
		List<FileInfo> fileList = dir.Exists ? GetAllFiles(dir, ".", ".meta") : new List<FileInfo>();
		foreach (FileInfo file in fileList)
		{
			string relativePath = file.FullName.Substring(dir.FullName.Length + 1).Replace("\\", "/");
			string assetPath = srcModuleDir + "/" + relativePath;
			if (IsCopyDir(relativePath))
			{
				SetBundleName(assetPath, "", "");
			}
			else if (s_ExceptExtensionSet.Contains(file.Extension))
			{
				SetBundleName(assetPath, "", "");
			}
			else
			{
				string bundleName = mModuleName + "/" + GetBundleName(dir, file);
				SetBundleName(assetPath, bundleName, ConstValue.ASSET_BUNDLE_VARIANT);
			}
		}
	}

	private bool IsCopyDir(string relativePath)
	{
		foreach (string copyDir in s_CopyDirs)
		{
			if (relativePath.StartsWith(copyDir))
			{
				return true;
			}
		}
		return false;
	}

	private void SetBundleName(string assetPath, string bundleName, string variant)
	{
		AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
		bool changed = false;
		if (!string.Equals(assetImporter.assetBundleName.ToLower(), bundleName.ToLower()))
		{
			assetImporter.assetBundleName = bundleName;
			changed = true;
		}
		if (!string.Equals(assetImporter.assetBundleVariant.ToLower(), variant.ToLower()))
		{
			assetImporter.assetBundleVariant = variant;
			changed = true;
		}
		if (changed)
		{
			m_AssetsChanged = true;
			assetImporter.SaveAndReimport();
		}
	}

	private string GetBundleName(DirectoryInfo dir, FileInfo file)
	{
		// 如果文件名以DIR_BUNDLE_PREFIX开头，则使用文件夹名字
		FileSystemInfo info = file;
		if (file.Directory != dir && file.Name[0] == DIR_BUNDLE_PREFIX)
		{
			info = file.Directory;
		}

		string relativePath = info.FullName.Substring(dir.FullName.Length + 1).Replace('\\', '/');
		int discardLength = GetNameDiscardLength(relativePath);
		string bundleName = relativePath.Substring(0, relativePath.Length - discardLength);
		return bundleName;
	}

	private int GetNameDiscardLength(string relativePath)
	{
		//foreach (string key in s_ExtensionReplaceDict.Keys)
		//{
		//	if (fileName.EndsWith(key))
		//	{
		//		string pureName = fileName.Substring(0, fileName.Length - key.Length);
		//		fileName = pureName + s_ExtensionReplaceDict[key];
		//		break;
		//	}
		//}
		int length = relativePath.Length;
		int indexSlash = relativePath.LastIndexOf('/'); // 为了找到文件名
		int fileNameLength = length - (indexSlash + 1);
		int indexPoint = relativePath.LastIndexOf('.', length - 1, fileNameLength); // 为了去掉扩展名
		int indexSeparator = relativePath.IndexOf(SUB_SEPARATOR); // 舍去SUB_SEPARATOR之后的所有路径

		if (indexPoint == -1)
		{
			indexPoint = length;
		}
		if (indexSeparator == -1)
		{
			indexSeparator = length;
		}

		return length - Mathf.Min(indexPoint, indexSeparator);
	}

	private void BuildBundles()
	{
        if (!Directory.Exists(OUTPUT_PATH))
		{
            Directory.CreateDirectory(OUTPUT_PATH);
		}
        BuildPipeline.BuildAssetBundles(OUTPUT_PATH, GetBuildMap(), OPTIONS, CurrentPlatform.buildTarget);
		RenameManifest();
		CopyAssets();
		m_AssetsChanged = true;
	}

	private AssetBundleBuild[] GetBuildMap()
	{
		Dictionary<string, List<string>> buildDic = new Dictionary<string, List<string>>();
		string srcModuleDir = SRC_DIR + "/" + mModuleName;
		DirectoryInfo dir = new DirectoryInfo(srcModuleDir);
		List<FileInfo> fileList = dir.Exists ? GetAllFiles(dir, ".", ".meta") : new List<FileInfo>();
		foreach (FileInfo file in fileList)
		{
			string relativePath = file.FullName.Substring(dir.FullName.Length + 1);
			string path = srcModuleDir + "/" + relativePath;
			AssetImporter asset = AssetImporter.GetAtPath(path);
			if (!string.IsNullOrEmpty(asset.assetBundleName))
			{
				string bundleName = asset.assetBundleName + "." + asset.assetBundleVariant;
				if (!buildDic.ContainsKey(bundleName))
				{
					buildDic.Add(bundleName, new List<string>());
				}
				buildDic[bundleName].Add(path);
			}
		}
		AssetBundleBuild[] buildMap = new AssetBundleBuild[buildDic.Count];
		int index = 0;
		foreach (string bundleNameVariant in buildDic.Keys)
		{
			int pointIndex = bundleNameVariant.LastIndexOf('.');
			string bundleName = pointIndex == -1 ? bundleNameVariant : bundleNameVariant.Substring(0, pointIndex);
			string Variant = pointIndex == -1 ? "" : bundleNameVariant.Substring(pointIndex + 1);
			buildMap[index].assetBundleName = bundleName;
			buildMap[index].assetBundleVariant = Variant;
			buildMap[index].assetNames = buildDic[bundleNameVariant].ToArray();
			index++;
		}
		return buildMap;
	}

	private void RenameManifest()
	{
        string currentManifestPath = OUTPUT_PATH + "/" + ConstValue.BUNDLE_DIR;
		string currentManifestDisplayPath = currentManifestPath + ".manifest";
		FileInfo currentManifestFile = new FileInfo(currentManifestPath);
		FileInfo currentManifestDisplayFile = new FileInfo(currentManifestDisplayPath);

		string expectManifestPath = OUTPUT_PATH + "/" + mModuleName + "/" + ConstValue.MANIFEST_NAME + "." + ConstValue.ASSET_BUNDLE_VARIANT;
		string expectManifestDisplayPath = expectManifestPath + ".manifest";
		FileInfo expectManifestFile = new FileInfo(expectManifestPath);
		FileInfo expectManifestDisplayFile = new FileInfo(expectManifestDisplayPath);

		if (currentManifestFile.Exists)
		{
			if (expectManifestFile.Exists)
			{
				expectManifestFile.Delete();
			}
			currentManifestFile.MoveTo(expectManifestPath);

			if (expectManifestDisplayFile.Exists)
			{
				expectManifestDisplayFile.Delete();
			}
			if (currentManifestDisplayFile.Exists)
			{
				currentManifestDisplayFile.MoveTo(expectManifestDisplayPath);
			}
		}
	}

	private void CopyAssets()
	{
		string srcModuleDir = SRC_DIR + "/" + mModuleName;
		foreach (string copyDir in s_CopyDirs)
		{
			DirectoryInfo srcDir = new DirectoryInfo(srcModuleDir + "/" + copyDir);
			List<FileInfo> srcFileList = srcDir.Exists ? GetAllFiles(srcDir, ".", ".meta") : new List<FileInfo>();
			foreach (FileInfo srcfile in srcFileList)
			{
				string relativePath = copyDir + "/" + srcfile.FullName.Substring(srcDir.FullName.Length + 1);
				string dstPath = OUTPUT_PATH + "/" + mModuleName + "/" + relativePath;
				FileInfo dstFile = new FileInfo(dstPath);
				if (dstFile.Exists)
				{
					string srcMd5 = GetMd5(srcfile);
					string dstMd5 = GetMd5(dstFile);
					if (string.Equals(srcMd5, dstMd5))
					{
						continue;
					}
					dstFile.Delete();
				}
				if (!dstFile.Directory.Exists)
				{
					dstFile.Directory.Create();
				}
				srcfile.CopyTo(dstPath);
			}
			DirectoryInfo dstDir = new DirectoryInfo(OUTPUT_PATH + "/" + mModuleName + "/" + copyDir);
			List<FileInfo> dstFileList = dstDir.Exists ? GetAllFiles(dstDir, ".", ".meta") : new List<FileInfo>();
			foreach (FileInfo dstFile in dstFileList)
			{
				string relativePath = copyDir + "/" + dstFile.FullName.Substring(dstDir.FullName.Length + 1);
				string srcPath = srcModuleDir + "/" + relativePath;
				FileInfo srcFile = new FileInfo(srcPath);
				if (!srcFile.Exists)
				{
					dstFile.Delete();
				}
			}
		}
	}

	private void CreateFileList()
	{
		JsonObject fileListJo = new JsonObject();

		string manifestPath = OUTPUT_PATH + "/" + mModuleName + "/" + ConstValue.MANIFEST_NAME + "." + ConstValue.ASSET_BUNDLE_VARIANT;
		FileInfo manifestFile = new FileInfo(manifestPath);
		JsonObject manifestInfoJo = new JsonObject();
		manifestInfoJo.Add(ConstValue.FILE_LIST_SIZE_KEY, manifestFile.Length);
		manifestInfoJo.Add(ConstValue.FILE_LIST_MD5_KEY, GetMd5(manifestFile));
		fileListJo.Add(mModuleName.ToLower() + "/" + ConstValue.MANIFEST_NAME + "." + ConstValue.ASSET_BUNDLE_VARIANT, manifestInfoJo);

		AssetBundle manifestAssetBundle = AssetBundle.LoadFromFile(manifestPath);
		if (manifestAssetBundle)
		{
			AssetBundleManifest manifest = manifestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
			if (manifest)
			{
				foreach (string bundleName in manifest.GetAllAssetBundles())
				{
                    FileInfo file = new FileInfo(OUTPUT_PATH + "/" + bundleName);

					JsonObject fileInfoJo = new JsonObject();
					fileInfoJo.Add(ConstValue.FILE_LIST_SIZE_KEY, file.Exists ? file.Length : 0);
					fileInfoJo.Add(ConstValue.FILE_LIST_MD5_KEY, file.Exists ? GetMd5(file) : "");

					fileListJo.Add(bundleName, fileInfoJo);
				}
			}
			manifestAssetBundle.Unload(true);
		}
		foreach (string copyDir in s_CopyDirs)
		{
			DirectoryInfo dir = new DirectoryInfo(OUTPUT_PATH + "/" + mModuleName + "/" + copyDir);
			List<FileInfo> fileList = dir.Exists ? GetAllFiles(dir, ".", ".meta") : new List<FileInfo>();
			foreach (FileInfo file in fileList)
			{
				JsonObject fileInfoJo = new JsonObject();
				fileInfoJo.Add(ConstValue.FILE_LIST_SIZE_KEY, file.Exists ? file.Length : 0);
				fileInfoJo.Add(ConstValue.FILE_LIST_MD5_KEY, file.Exists ? GetMd5(file) : "");

				string relativePath = copyDir + "/" + file.FullName.Substring(dir.FullName.Length + 1).Replace("\\", "/");
				fileListJo.Add(relativePath, fileInfoJo);
			}
		}

		string fileListStr = ToFileListString(fileListJo);
		byte[] fileListBytes = ConvertExt.StringToBytes(fileListStr);
		FileManager.Write(OUTPUT_PATH + "/" + mModuleName + "/" + ConstValue.FILE_LIST_NAME, fileListBytes);

		m_AssetsChanged = true;
	}

	public static string GetMd5(FileInfo file)
	{
		FileStream fileStream = file.Open(FileMode.Open);
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		byte[] retVal = md5.ComputeHash(fileStream);
		fileStream.Close();
		StringBuilder sb = new StringBuilder();
		for (int index = 0; index < retVal.Length; index++)
		{
			sb.Append(retVal[index].ToString("x2"));
		}
		return sb.ToString();
	}

	private string ToFileListString(JsonObject fileListJo)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("{\r\n");
		char[] chars = fileListJo.ToString().ToCharArray();
		int leftBracket = 0;
		for (int index = 1; index < chars.Length - 1; index++)
		{
			sb.Append(chars[index]);
			if (chars[index] == '{')
			{
				leftBracket++;
			}
			if (chars[index] == '}')
			{
				leftBracket--;
			}
			if (chars[index] == ',' && leftBracket == 0)
			{
				sb.Append("\r\n");
			}
		}
		sb.Append("\r\n}");
		return sb.ToString();
	}

	private void CreateVersion()
	{
		JsonObject versionInfoJo = new JsonObject();
		versionInfoJo.Add(ConstValue.MODULE_KEY, mModuleName);
		versionInfoJo.Add(ConstValue.VERSION_KEY, ConstValue.VERSION);
		byte[] versionInfoBytes = ConvertExt.StringToBytes(versionInfoJo.ToString());
		FileManager.Write(OUTPUT_PATH + "/" + mModuleName + "/" + ConstValue.VERSION_NAME, versionInfoBytes);

		m_AssetsChanged = true;
	}

	private static List<FileInfo> GetAllFiles(DirectoryInfo dir, string exceptPrefix = null, string exceptSuffix = null)
	{
		List<FileInfo> list = new List<FileInfo>();
		FileSystemInfo[] infos = dir.GetFileSystemInfos();
		foreach (FileSystemInfo info in infos)
		{
			if (exceptPrefix != null && info.Name.StartsWith(exceptPrefix))
			{
				continue;
			}
			if (exceptSuffix != null && info.Name.EndsWith(exceptSuffix))
			{
				continue;
			}

			if (info is FileInfo)
			{
				list.Add(info as FileInfo);
			}
			else if (info is DirectoryInfo)
			{
				list.AddRange(GetAllFiles(info as DirectoryInfo, exceptPrefix, exceptSuffix));
			}
		}
		return list;
	}

	private void DrawHeader(string text, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
		GUILayout.Button(CurrentPlatform.name, "TL Selection H1");
		GUILayout.EndHorizontal();
	}

	private void BeginVerticalArea(string title, string notice)
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(2F);
		title = "<b><size=11>" + title + "</size></b>";
		GUILayout.Toggle(true, title, "DragTab", GUILayout.MinWidth(20f));
		GUILayout.Space(2F);
		EditorGUILayout.EndHorizontal();

		if (!string.IsNullOrEmpty(notice))
		{
			GUILayout.Space(-5F);
			EditorGUILayout.HelpBox(notice, MessageType.Info);
			GUILayout.Space(-5F);
		}

		GUILayout.BeginHorizontal();
		GUILayout.Space(2F);
		EditorGUILayout.BeginVertical("As TextArea", GUILayout.MinHeight(18));
		GUILayout.Space(10F);
	}

	private void EndVerticalArea()
	{
		GUILayout.Space(10F);
		GUILayout.EndVertical();
		GUILayout.Space(2F);
		GUILayout.EndHorizontal();
	}

	[@MenuItem("Window/Asset Bundle Builder")]
	private static void Window()
	{
		AssetBundleBuilder window = GetWindow(typeof(AssetBundleBuilder), false, "Asset Bundle Builder") as AssetBundleBuilder;
		window.minSize = new Vector2(200, 320);
		window.ShowTab();
	}

	public class MyPostprocessor : AssetPostprocessor
	{
		void OnPostprocessAssetbundleNameChanged(string path, string previous, string next)
		{
			Debug.Log("AB: " + path + " [old: " + previous + "] [new: " + next + "]");
		}
	}
}
