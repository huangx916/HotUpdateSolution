using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	public class LuaMain : MonoBehaviour
	{
		private const float GC_INTERVAL = 1;//1 second

		private LuaEnv m_LuaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
		private float m_LastGcTime = 0;

		private LuaTable m_LuaTable;
		public LuaTable LuaTable
		{
			get
			{
				return m_LuaTable;
			}
		}

		private Func<string, LuaTable> m_Require;
		private Func<string, LuaTable> m_DoFile;
		private Func<string, LuaFunction> m_LoadFile;

		private Func<string, int, string> m_TraceBack;

		private Func<object, object[], object> m_FuncInvoke;
		private HashSet<string> m_GlobalVarBuiltInSet = new HashSet<string>();

		private LuaTable m_LoadedTable;
		private HashSet<string> m_LoadedBuiltInSet = new HashSet<string>();
		private LuaTable m_PreloadTable;
		private HashSet<string> m_PreloadBuiltInSet = new HashSet<string>();

		private Action<LuaTable, object, LuaFunction> m_HotFix;
		private Dictionary<LuaTable, HashSet<string>> m_HotFixDict = new Dictionary<LuaTable, HashSet<string>>();

		private Action<LuaTable, object, LuaFunction> m_HotFixEx;
		private Dictionary<LuaTable, HashSet<string>> m_HotFixExDict = new Dictionary<LuaTable, HashSet<string>>();

		void Awake()
		{
			m_LuaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
			m_LuaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
			m_LuaEnv.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProfobuf);

			m_Require = m_LuaEnv.Global.Get<Func<string, LuaTable>>("require");
			m_DoFile = m_LuaEnv.Global.Get<Func<string, LuaTable>>("dofile");
			m_LoadFile = m_LuaEnv.Global.Get<Func<string, LuaFunction>>("loadfile");

			m_TraceBack = m_LuaEnv.Global.GetInPath<Func<string, int, string>>("debug.traceback");

			LuaTable package = m_LuaEnv.Global.Get<LuaTable>("package");
			m_LoadedTable = package.Get<LuaTable>("loaded");
			m_LoadedTable.ForEach<string, object>((key, value) => m_LoadedBuiltInSet.Add(key));
			m_PreloadTable = package.Get<LuaTable>("preload");
			m_PreloadTable.ForEach<string, object>((key, value) => m_PreloadBuiltInSet.Add(key));

			LuaTable xlua = m_LuaEnv.Global.Get<LuaTable>("xlua");
			m_LuaEnv.Global.Set<string, LuaTable>("xlua", xlua);
			m_HotFix = xlua.Get<Action<LuaTable, object, LuaFunction>>("hotfix");
			xlua.Set<string, Action<LuaTable, object, LuaFunction>>("hotfix", HotFix);

			LuaTable util = Require("xlua.util");
			m_LuaEnv.Global.Set<string, LuaTable>("util", util);
			m_HotFixEx = util.Get<Action<LuaTable, object, LuaFunction>>("hotfix_ex");
			util.Set<string, Action<LuaTable, object, LuaFunction>>("hotfix_ex", HotFixEx);

			LuaTable csHelp = Require("LuaUtil.CSHelp");
			m_LuaEnv.Global.Set<string, LuaTable>("CSHelp", csHelp);
			m_FuncInvoke = csHelp.Get<Func<object, object[], object>>("FuncInvoke");

			m_LuaEnv.Global.ForEach<string, object>((key, value) => m_GlobalVarBuiltInSet.Add(key));

			m_LuaEnv.AddLoader(CustomLoad);

			Debugger.LogMessageFormat = message => m_TraceBack(message.ToString(), 1);
		}

		public void StartLua()
		{
			m_LuaTable = LuaHelp.AddLuaComponent(transform, "Common.LuaScripts.Main.LuaMain.lua");
		}

		public void Restart()
		{
			ResetLua();
			CoroutineManager.EndOfFrame(StartLua);
		}

		private void ResetLua()
		{
			if (m_LuaTable != null)
			{
				CompAgent.ClearChildren(transform);
				CompAgent.ClearCompnents<LuaBehaviour>(transform);
				m_LuaTable = null;

				List<string> loadedRemoveList = new List<string>();
				m_LoadedTable.ForEach<string, object>((luaName, value) => loadedRemoveList.Add(luaName));
				foreach (string luaName in loadedRemoveList)
				{
					if (!m_LoadedBuiltInSet.Contains(luaName))
					{
						m_LoadedTable.Set<string, object>(luaName, null);
					}
				}
				List<string> preloadRemoveList = new List<string>();
				m_PreloadTable.ForEach<string, object>((luaName, value) => preloadRemoveList.Add(luaName));
				foreach (string luaName in preloadRemoveList)
				{
					if (!m_PreloadBuiltInSet.Contains(luaName))
					{
						m_PreloadTable.Set<string, object>(luaName, null);
					}
				}

				foreach (LuaTable csTable in m_HotFixDict.Keys)
				{
					foreach (string fieldName in m_HotFixDict[csTable])
					{
						m_HotFix(csTable, fieldName, null);
					}
				}
				m_HotFixDict.Clear();
				foreach (LuaTable csTable in m_HotFixExDict.Keys)
				{
					foreach (string fieldName in m_HotFixExDict[csTable])
					{
						m_HotFixEx(csTable, fieldName, null);
					}
				}
				m_HotFixExDict.Clear();

				List<string> globalVarRemoveList = new List<string>();
				m_LuaEnv.Global.ForEach<string, object>((key, value) => globalVarRemoveList.Add(key));
				foreach (string varName in globalVarRemoveList)
				{
					if (!m_GlobalVarBuiltInSet.Contains(varName))
					{
						m_LuaEnv.Global.Set<string, object>(varName, null);
					}
				}

				FuncInvoke(m_LuaEnv.Global, "collectgarbage");
			}
		}

		void Update()
		{
			if (Time.time - m_LastGcTime > GC_INTERVAL)
			{
				m_LuaEnv.Tick();
				m_LastGcTime = Time.time;
			}
		}

		public void HotFix(LuaTable csTable, object field, LuaFunction fixFunc = null)
		{
			if (m_HotFix != null)
			{
				HashSet<string> set;
				if (m_HotFixDict.ContainsKey(csTable))
				{
					set = m_HotFixDict[csTable];
				}
				else
				{
					set = new HashSet<string>();
					m_HotFixDict[csTable] = set;
				}

				if (field is string)
				{
					if (fixFunc != null)
					{
						string fieldName = field.ToString();
						if (!set.Contains(fieldName))
						{
							set.Add(fieldName);
						}
					}
				}
				else if (field is LuaTable)
				{
					LuaTable fixTable = field as LuaTable;
					fixTable.ForEach<string, object>((fieldName, function) =>
					{
						if (!set.Contains(fieldName))
						{
							set.Add(fieldName);
						}
					});
				}

				m_HotFix(csTable, field, fixFunc);
			}
		}

		public void HotFixEx(LuaTable csTable, object field, LuaFunction fixFunc)
		{
			if (m_HotFixEx != null)
			{
				HashSet<string> set;
				if (m_HotFixExDict.ContainsKey(csTable))
				{
					set = m_HotFixExDict[csTable];
				}
				else
				{
					set = new HashSet<string>();
					m_HotFixExDict[csTable] = set;
				}

				if (field is string)
				{
					string fieldName = field.ToString();
					if (!set.Contains(fieldName))
					{
						set.Add(fieldName);
					}
					m_HotFixEx(csTable, field, fixFunc);
				}
			}
		}

		/// <summary>
		/// HotFix始终替换这个函数，用于标记是否能够hotfix成功
		/// </summary>
		public void HotFixLog()
		{
			Debug.Log("C# version is " + ConstValue.VERSION + ", no HotFix!");
		}

		private byte[] CustomLoad(ref string filename)
		{
			string extension = ".lua";
			filename = filename.EndsWith(extension) ? filename.Substring(0, filename.Length - extension.Length) : filename;
			string relativePath = filename.Replace(".", "/") + extension;
			TextAsset textAsset = LoadLua(relativePath);
			if (textAsset)
			{
				return ConvertExt.StringToBytes(textAsset.text);
			}
			return null;
		}

#region load lua
		public static TextAsset LoadLua(string relativePath)
		{
#if !LOAD_FROM_RES
			string longRelativePath = relativePath + "." + ConstValue.ASSET_BUNDLE_VARIANT;
#if DOWNLOAD
			string persistentPath = ConstValue.PERSISTENT_DIR_PATH + "/" + longRelativePath.ToLower();
			if (FileManager.IsFileExist(persistentPath))
			{
#if ASSET_LOAD_LOG
				Debugger.Log("<b>Load lua form persistent:</b> " + relativePath);
#endif
				int index = relativePath.LastIndexOf('/');
				string assetName = relativePath.Substring(index + 1);
				return LoadLuaBundle(persistentPath, assetName);
			}
#endif
			string streamingPath = ConstValue.STREAMING_DIR_PATH + "/" + longRelativePath.ToLower();
			if (FileManager.IsFileExist(streamingPath))
			{
#if ASSET_LOAD_LOG
				Debugger.Log("<b>Load lua form streaming:</b> " + relativePath);
#endif
				int index = relativePath.LastIndexOf('/');
				string assetName = relativePath.Substring(index + 1);
				return LoadLuaBundle(streamingPath, assetName);
			}
#endif

#if ASSET_LOAD_LOG
			Debugger.Log("<b>Load lua form resources:</b> " + relativePath);
#endif
			TextAsset asset = Resources.Load<TextAsset>(relativePath);
			if (asset)
			{
				return asset;
			}
			Debugger.LogError("<b>Resources.Load lua failed:</b> " + relativePath);
			return null;
		}

		public static TextAsset LoadLuaBundle(string path, string assetName)
		{
			AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
			if (assetBundle)
			{
				TextAsset asset = assetBundle.LoadAsset<TextAsset>(assetName);
				assetBundle.Unload(false);
				return asset;
			}
			Debugger.LogError("<b>AssetBundl.Load lua failed:</b> " + path);
			return null;
		}
#endregion

		public LuaTable Require(string luaPath)
		{
			try
			{
				Debugger.Log("Require: " + luaPath);
				return m_Require(luaPath);
			}
			catch (LuaException le)
			{
				Debugger.LogError(le);
				return null;
			}
		}

		public LuaTable DoFile(string luaPath)
		{
			try
			{
				Debugger.Log("DoFile: " + luaPath);
				return m_DoFile(luaPath);
			}
			catch (LuaException le)
			{
				Debugger.LogError(le);
				return null;
			}
		}

		public LuaFunction LoadFile(string luaPath)
		{
			try
			{
				Debugger.Log("LoadFile: " + luaPath);
				return m_LoadFile(luaPath);
			}
			catch (LuaException le)
			{
				Debugger.LogError(le);
				return null;
			}
		}

		public object FuncInvoke(string path, params object[] args)
		{
			return FuncInvoke(m_LuaEnv.Global, path, args);
		}

		public object FuncInvoke(LuaTable table, params object[] args)
		{
			return FuncInvoke(table as object, args);
		}

		public object FuncInvoke(LuaTable table, string path, params object[] args)
		{
			if (table != null)
			{
				object func = table.GetInPath<object>(path);
				return FuncInvoke(func, args);
			}
			else
			{
				Debugger.LogError("FuncInvoke: table is null!");
				return null;
			}
		}

		private object FuncInvoke(object func, params object[] args)
		{
			try
			{
				return m_FuncInvoke(func, args);
			}
			catch (LuaException le)
			{
				Debugger.LogError(le);
				return null;
			}
		}

		public static LuaEnv LuaEnv
		{
			get
			{
				return Instance.m_LuaEnv;
			}
		}

		public static LuaMain Instance
		{
			[System.Diagnostics.DebuggerHidden]
			[System.Diagnostics.DebuggerStepThrough]
			get
			{
				return GameMain.Instance.LuaMain;
			}
		}
	}
}