using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	public class EnabledListener : BehaviourListener
	{
		public string listenerTag;

		public Action<LuaTable> onDisable;
		public Action<LuaTable> onEnable;

		private LuaTable m_LuaTable;

		void Awake()
		{
			LuaBehaviour behaviour = gameObject.GetComponent<LuaBehaviour>();
			m_LuaTable = behaviour ? behaviour.LuaTable : null;
		}

		void OnDisable()
		{
			if (onDisable != null)
			{
				onDisable(m_LuaTable);
			}
		}

		void OnEnable()
		{
			if (onEnable != null)
			{
				onEnable(m_LuaTable);
			}
		}

		private static EnabledListener FindListener(EnabledListener[] listeners, string listenerTag)
		{
			foreach (EnabledListener listener in listeners)
			{
				if (string.Equals(listener.listenerTag, listenerTag))
				{
					return listener;
				}
			}
			return null;
		}

		public static EnabledListener Get(GameObject go, string listenerTag = null)
		{
			EnabledListener[] listeners = go.GetComponents<EnabledListener>();
			EnabledListener listener = FindListener(listeners, listenerTag);
			if (listener == null)
			{
				listener = go.AddComponent<EnabledListener>();
				listener.listenerTag = listenerTag;
			}
			return listener;
		}

		public static EnabledListener Get(Component comp, string listenerTag = null)
		{
			return Get(comp.gameObject, listenerTag);
		}

		public override void Dispose()
		{
			onDisable = null;
			onEnable = null;
			Destroy(this);
		}
	}

}