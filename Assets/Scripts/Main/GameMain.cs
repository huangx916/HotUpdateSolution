using UnityEngine;
using System.Collections.Generic;

namespace Main
{
	public class GameMain : MonoBehaviour
	{
		private static GameMain s_Instance;
		public static GameMain Instance
		{
			[System.Diagnostics.DebuggerHidden]
			[System.Diagnostics.DebuggerStepThrough]
			get
			{
				return s_Instance;
			}
		}

		void Awake()
		{
#if UNITY_EDITOR || DEBUG_MACRO
			Debugger.EnableLog = true;
			Debugger.EnableDraw = true;
#endif

			s_Instance = this;

            m_HotUpdateManager = gameObject.AddComponent<HotUpdateManager>();

			m_ManagerCollection = AddChild<ManagerCollection>();
			m_LuaMain = AddChild<LuaMain>();
			m_LuaMain.StartLua();
            //VersionCompare();
            CoroutineManager.EndOfFrame(VersionCompare);
            //CoroutineManager.Delay(0, VersionCompare, this);
		}

		public void VersionCompare()
		{
#if DOWNLOAD
            HotUpdateManager.VersionCompare(VersionUpdate);
#endif
		}

		public void VersionUpdate()
		{
#if DOWNLOAD
            HotUpdateManager.VersionUpdate(() =>
			{
                DestroyImmediate(m_ManagerCollection);
				m_ManagerCollection = AddChild<ManagerCollection>();
				m_LuaMain.Restart();
				//VersionCompare();
                CoroutineManager.EndOfFrame(VersionCompare);
			});
#endif
		}

		private T AddChild<T>() where T : MonoBehaviour
		{
			return CompAgent.AddChild<T>(this);
		}

		private ManagerCollection m_ManagerCollection;
		public ManagerCollection ManagerCollection
		{
			get
			{
				return m_ManagerCollection;
			}
		}

        private HotUpdateManager m_HotUpdateManager;
        public HotUpdateManager HotUpdateManager
        {
            get
            {
                return m_HotUpdateManager;
            }
        }

		private LuaMain m_LuaMain;
		public LuaMain LuaMain
		{
			get
			{
				return m_LuaMain;
			}
		}
	}
}