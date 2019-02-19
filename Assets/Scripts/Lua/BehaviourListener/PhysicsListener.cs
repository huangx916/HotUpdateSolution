using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	public class PhysicsListener : BehaviourListener
	{
		public string listenerTag;

		public Action<LuaTable, Collider> onTriggerEnter;
		public Action<LuaTable, Collider> onTriggerStay;
		public Action<LuaTable, Collider> onTriggerExit;
		public Action<LuaTable, Collision> onCollisionEnter;
		public Action<LuaTable, Collision> onCollisionStay;
		public Action<LuaTable, Collision> onCollisionExit;

		private LuaTable m_LuaTable;

		void Awake()
		{
			LuaBehaviour behaviour = gameObject.GetComponent<LuaBehaviour>();
			m_LuaTable = behaviour ? behaviour.LuaTable : null;
		}

		void OnTriggerEnter(Collider other)
		{
			if (onTriggerEnter != null)
			{
				onTriggerEnter(m_LuaTable, other);
			}
		}

		void OnTriggerStay(Collider other)
		{
			if (onTriggerStay != null)
			{
				onTriggerStay(m_LuaTable, other);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (onTriggerExit != null)
			{
				onTriggerExit(m_LuaTable, other);
			}
		}

		void OnCollisionEnter(Collision collision)
		{
			if (onCollisionEnter != null)
			{
				onCollisionEnter(m_LuaTable, collision);
			}
		}

		void OnCollisionStay(Collision collision)
		{
			if (onCollisionStay != null)
			{
				onCollisionStay(m_LuaTable, collision);
			}
		}

		void OnCollisionExit(Collision collision)
		{
			if (onCollisionExit != null)
			{
				onCollisionExit(m_LuaTable, collision);
			}
		}

		private static PhysicsListener FindListener(PhysicsListener[] listeners, string listenerTag)
		{
			foreach (PhysicsListener listener in listeners)
			{
				if (string.Equals(listener.listenerTag, listenerTag))
				{
					return listener;
				}
			}
			return null;
		}

		public static PhysicsListener Get(GameObject go, string listenerTag = null)
		{
			PhysicsListener[] listeners = go.GetComponents<PhysicsListener>();
			PhysicsListener listener = FindListener(listeners, listenerTag);
			if (listener == null)
			{
				listener = go.AddComponent<PhysicsListener>();
				listener.listenerTag = listenerTag;
			}
			return listener;
		}

		public static PhysicsListener Get(Component comp, string listenerTag = null)
		{
			return Get(comp.gameObject, listenerTag);
		}

		public override void Dispose()
		{
			onTriggerEnter = null;
			onTriggerStay = null;
			onTriggerExit = null;
			onCollisionEnter = null;
			onCollisionStay = null;
			onCollisionExit = null;
			Destroy(this);
		}
	}

}