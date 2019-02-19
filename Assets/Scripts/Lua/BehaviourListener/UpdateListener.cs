using System;
using UnityEngine;
using XLua;

namespace Main
{
	public class UpdateListener : BehaviourListener
	{
		public string listenerTag;

		public Action<LuaTable> update;

		private LuaTable m_LuaTable;

		void Awake()
		{
			LuaBehaviour behaviour = gameObject.GetComponent<LuaBehaviour>();
			m_LuaTable = behaviour ? behaviour.LuaTable : null;
		}

		void Update()
		{
			if (update != null)
			{
				update(m_LuaTable);
			}
		}

		private static UpdateListener FindListener(UpdateListener[] listeners, string listenerTag)
		{
			foreach (UpdateListener listener in listeners)
			{
				if (string.Equals(listener.listenerTag, listenerTag))
				{
					return listener;
				}
			}
			return null;
		}

		public static UpdateListener Get(GameObject go, string listenerTag = null)
		{
			UpdateListener[] listeners = go.GetComponents<UpdateListener>();
			UpdateListener listener = FindListener(listeners, listenerTag);
			if (listener == null)
			{
				listener = go.AddComponent<UpdateListener>();
				listener.listenerTag = listenerTag;
			}
			return listener;
		}

		public static UpdateListener Get(Component comp, string listenerTag = null)
		{
			return Get(comp.gameObject, listenerTag);
		}

		public override void Dispose()
		{
			update = null;
			Destroy(this);
		}
	}

}