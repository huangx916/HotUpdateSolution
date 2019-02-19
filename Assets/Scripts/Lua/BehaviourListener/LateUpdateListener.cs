using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	public class LateUpdateListener : BehaviourListener
	{
		public string listenerTag;

		public Action<LuaTable> lateUpdate;

		private LuaTable m_LuaTable;

		void Awake()
		{
			LuaBehaviour behaviour = gameObject.GetComponent<LuaBehaviour>();
			m_LuaTable = behaviour ? behaviour.LuaTable : null;
		}

		void LateUpdate()
		{
			if (lateUpdate != null)
			{
				lateUpdate(m_LuaTable);
			}
		}

		private static LateUpdateListener FindListener(LateUpdateListener[] listeners, string listenerTag)
		{
			foreach (LateUpdateListener listener in listeners)
			{
				if (string.Equals(listener.listenerTag, listenerTag))
				{
					return listener;
				}
			}
			return null;
		}

		public static LateUpdateListener Get(GameObject go, string listenerTag = null)
		{
			LateUpdateListener[] listeners = go.GetComponents<LateUpdateListener>();
			LateUpdateListener listener = FindListener(listeners, listenerTag);
			if (listener == null)
			{
				listener = go.AddComponent<LateUpdateListener>();
				listener.listenerTag = listenerTag;
			}
			return listener;
		}

		public static LateUpdateListener Get(Component comp, string listenerTag = null)
		{
			return Get(comp.gameObject, listenerTag);
		}

		public override void Dispose()
		{
			lateUpdate = null;
			Destroy(this);
		}
	}

}