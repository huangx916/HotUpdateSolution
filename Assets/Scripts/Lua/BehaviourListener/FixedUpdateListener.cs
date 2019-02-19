using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	public class FixedUpdateListener : BehaviourListener
	{
		public string listenerTag;

		public Action<LuaTable> fixedUpdate;

		private LuaTable m_LuaTable;

		void Awake()
		{
			LuaBehaviour behaviour = gameObject.GetComponent<LuaBehaviour>();
			m_LuaTable = behaviour ? behaviour.LuaTable : null;
		}

		void FixedUpdate()
		{
			if (fixedUpdate != null)
			{
				fixedUpdate(m_LuaTable);
			}
		}

		private static FixedUpdateListener FindListener(FixedUpdateListener[] listeners, string listenerTag)
		{
			foreach (FixedUpdateListener listener in listeners)
			{
				if (string.Equals(listener.listenerTag, listenerTag))
				{
					return listener;
				}
			}
			return null;
		}

		public static FixedUpdateListener Get(GameObject go, string listenerTag = null)
		{
			FixedUpdateListener[] listeners = go.GetComponents<FixedUpdateListener>();
			FixedUpdateListener listener = FindListener(listeners, listenerTag);
			if (listener == null)
			{
				listener = go.AddComponent<FixedUpdateListener>();
				listener.listenerTag = listenerTag;
			}
			return listener;
		}

		public static FixedUpdateListener Get(Component comp, string listenerTag = null)
		{
			return Get(comp.gameObject, listenerTag);
		}

		public override void Dispose()
		{
			fixedUpdate = null;
			Destroy(this);
		}
	}

}