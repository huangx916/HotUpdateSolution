using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
	public class ManagerCollection : MonoBehaviour
	{
		private Dictionary<Type, MonoBehaviour> m_ManagerDict = new Dictionary<Type, MonoBehaviour>();

		void Awake()
		{
			AddManager<CoroutineManager>();
			AddManager<ListenerManager>();
		}

		public T AddManager<T>() where T : MonoBehaviour
		{
			T manager = CompAgent.AddChild<T>(transform);
			m_ManagerDict.Add(typeof(T), manager);
			return manager;
		}

		public T GetManager<T>() where T : MonoBehaviour
		{
			Type type = typeof(T);
			if (m_ManagerDict.ContainsKey(type))
			{
				return m_ManagerDict[type] as T;
			}

			T manager = gameObject.GetComponent<T>();
			if (manager)
			{
				m_ManagerDict[type] = manager;
				return manager;
			}

			Debugger.LogError(typeof(T).Name + " is not exist!");
			return null;
		}
	}
}