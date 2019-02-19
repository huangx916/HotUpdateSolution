using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	public class GUIListener : BehaviourListener
	{
		public string listenerTag;

		public Action<LuaTable> onGUI;

		private LuaTable m_LuaTable;

		void Awake()
		{
			LuaBehaviour behaviour = gameObject.GetComponent<LuaBehaviour>();
			m_LuaTable = behaviour ? behaviour.LuaTable : null;
		}

		void OnGUI()
		{
			if (onGUI != null)
			{
				onGUI(m_LuaTable);
			}
		}

		private static GUIListener FindListener(GUIListener[] listeners, string listenerTag)
		{
			foreach (GUIListener listener in listeners)
			{
				if (string.Equals(listener.listenerTag, listenerTag))
				{
					return listener;
				}
			}
			return null;
		}

		public static GUIListener Get(GameObject go, string listenerTag = null)
		{
			GUIListener[] listeners = go.GetComponents<GUIListener>();
			GUIListener listener = FindListener(listeners, listenerTag);
			if (listener == null)
			{
				listener = go.AddComponent<GUIListener>();
				listener.listenerTag = listenerTag;
			}
			return listener;
		}

		public static GUIListener Get(Component comp, string listenerTag = null)
		{
			return Get(comp.gameObject, listenerTag);
		}

		public override void Dispose()
		{
			onGUI = null;
			Destroy(this);
		}
	}

}