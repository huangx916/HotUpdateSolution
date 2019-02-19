using System;
using UnityEngine;
using XLua;

namespace Main
{
	public class ApplicationListener : BehaviourListener
	{
		public string listenerTag;

		public Action<LuaTable, bool> onApplicationFocus;
		public Action<LuaTable, bool> onApplicationPause;
		public Action<LuaTable> onApplicationQuit;

		private LuaTable m_LuaTable;

		void Awake()
		{
			LuaBehaviour behaviour = gameObject.GetComponent<LuaBehaviour>();
			m_LuaTable = behaviour ? behaviour.LuaTable : null;
		}

		void OnApplicationFocus(bool hasFocus)
		{
			if (onApplicationFocus != null)
			{
				onApplicationFocus(m_LuaTable, hasFocus);
			}
		}

		void OnApplicationPause(bool pauseStatus)
		{
			if (onApplicationPause != null)
			{
				onApplicationPause(m_LuaTable, pauseStatus);
			}
		}

		void OnApplicationQuit()
		{
			if (onApplicationQuit != null)
			{
				onApplicationQuit(m_LuaTable);
			}
		}

		private static ApplicationListener FindListener(ApplicationListener[] listeners, string listenerTag)
		{
			foreach (ApplicationListener listener in listeners)
			{
				if (string.Equals(listener.listenerTag, listenerTag))
				{
					return listener;
				}
			}
			return null;
		}

		public static ApplicationListener Get(GameObject go, string listenerTag = null)
		{
			ApplicationListener[] listeners = go.GetComponents<ApplicationListener>();
			ApplicationListener listener = FindListener(listeners, listenerTag);
			if (listener == null)
			{
				listener = go.AddComponent<ApplicationListener>();
				listener.listenerTag = listenerTag;
			}
			return listener;
		}

		public static ApplicationListener Get(Component comp, string listenerTag = null)
		{
			return Get(comp.gameObject, listenerTag);
		}

		public override void Dispose()
		{
			onApplicationFocus = null;
			onApplicationPause = null;
			onApplicationQuit = null;
			Destroy(this);
		}
	}

}