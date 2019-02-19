/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	[RequireComponent(typeof(LuaInjectionData))]
	public class LuaBehaviour : MonoBehaviour
	{
		public string m_LuaPath;

		protected LuaTable m_LuaTable;
		public LuaTable LuaTable
		{
			get
			{
				return m_LuaTable;
			}
		}

		protected Action<LuaTable> m_LuaAwake;
		protected Action<LuaTable> m_LuaStart;
		protected Action<LuaTable> m_LuaOnDestroy;

		private HashSet<IDisposable> m_BehaviourListenerSet = new HashSet<IDisposable>();

		public virtual void InitLua(params object[] args)
		{
			string luaPath = GetLuaPath();
			if (!string.IsNullOrEmpty(luaPath))
			{
				LuaTable luaClassTable = LuaMain.Instance.Require(luaPath);
				if (luaClassTable != null)
				{
					m_LuaTable = LuaMain.Instance.FuncInvoke(luaClassTable, args) as LuaTable;
					if (m_LuaTable != null)
					{
						OnTableRequired();

						if (m_LuaAwake != null)
						{
							m_LuaAwake(m_LuaTable);
						}
					}
					else
					{
						Debugger.LogError("LuaObject has not instanced: " + luaPath);
						Dispose();
					}
				}
				else
				{
					Debugger.LogError("LuaTable not returned: " + luaPath);
					Dispose();
				}
			}
		}

		protected virtual void OnTableRequired()
		{
			m_LuaTable.Set("m_CSBehaviour", this);
			InjectData();

			m_LuaAwake = m_LuaTable.Get<Action<LuaTable>>("Awake");
			m_LuaStart = m_LuaTable.Get<Action<LuaTable>>("Start");
			m_LuaOnDestroy = m_LuaTable.Get<Action<LuaTable>>("OnDestroy");

			AddUpdateListener();
			AddFixedUpdateListener();
			AddLateUpdateListener();
			AddApplicationListener();
			AddEnabledListener();
			AddPhysicsListener();
			AddGUIListener();
		}

		[ContextMenu("Inject Data")]
		private void InjectData()
		{
			LuaInjectionData injections = GetComponent<LuaInjectionData>();
			if (injections)
			{
				injections.Data.ForEach<object, object>((key, value) => m_LuaTable.Set(key, value));
			}
		}

		private void AddUpdateListener()
		{
			Action<LuaTable> luaUpdate = m_LuaTable.Get<Action<LuaTable>>("Update");
			if (luaUpdate != null)
			{
				UpdateListener listener = UpdateListener.Get(this);
				listener.update = luaUpdate;
				if (!m_BehaviourListenerSet.Contains(listener))
				{
					m_BehaviourListenerSet.Add(listener);
				}
			}
		}

		private void AddFixedUpdateListener()
		{
			Action<LuaTable> luaFixedUpdate = m_LuaTable.Get<Action<LuaTable>>("FixedUpdate");
			if (luaFixedUpdate != null)
			{
				FixedUpdateListener listener = FixedUpdateListener.Get(this);
				listener.fixedUpdate = luaFixedUpdate;
				if (!m_BehaviourListenerSet.Contains(listener))
				{
					m_BehaviourListenerSet.Add(listener);
				}
			}
		}

		private void AddLateUpdateListener()
		{
			Action<LuaTable> luaLateUpdate = m_LuaTable.Get<Action<LuaTable>>("LateUpdate");
			if (luaLateUpdate != null)
			{
				LateUpdateListener listener = LateUpdateListener.Get(this);
				listener.lateUpdate = luaLateUpdate;
				if (!m_BehaviourListenerSet.Contains(listener))
				{
					m_BehaviourListenerSet.Add(listener);
				}
			}
		}

		private void AddApplicationListener()
		{
			Action<LuaTable, bool> luaOnApplicationFocus = m_LuaTable.Get<Action<LuaTable, bool>>("OnApplicationFocus");
			Action<LuaTable, bool> luaOnApplicationPause = m_LuaTable.Get<Action<LuaTable, bool>>("OnApplicationPause");
			Action<LuaTable> luaOnApplicationQuit = m_LuaTable.Get<Action<LuaTable>>("OnApplicationQuit");
			if (luaOnApplicationFocus != null || luaOnApplicationPause != null || luaOnApplicationQuit != null)
			{
				ApplicationListener listener = ApplicationListener.Get(this);
				listener.onApplicationFocus = luaOnApplicationFocus;
				listener.onApplicationPause = luaOnApplicationPause;
				listener.onApplicationQuit = luaOnApplicationQuit;
				if (!m_BehaviourListenerSet.Contains(listener))
				{
					m_BehaviourListenerSet.Add(listener);
				}
			}
		}

		private void AddPhysicsListener()
		{
			Action<LuaTable, Collision> luaOnCollisionEnter = m_LuaTable.Get<Action<LuaTable, Collision>>("OnCollisionEnter");
			Action<LuaTable, Collision> luaOnCollisionExit = m_LuaTable.Get<Action<LuaTable, Collision>>("OnCollisionExit");
			Action<LuaTable, Collision> luaOnCollisionStay = m_LuaTable.Get<Action<LuaTable, Collision>>("OnCollisionStay");
			Action<LuaTable, Collider> luaOnTriggerEnter = m_LuaTable.Get<Action<LuaTable, Collider>>("OnTriggerEnter");
			Action<LuaTable, Collider> luaOnTriggerExit = m_LuaTable.Get<Action<LuaTable, Collider>>("OnTriggerExit");
			Action<LuaTable, Collider> luaOnTriggerStay = m_LuaTable.Get<Action<LuaTable, Collider>>("OnTriggerStay");
			if (luaOnCollisionEnter != null || luaOnCollisionExit != null || luaOnCollisionStay != null ||
				luaOnTriggerEnter != null || luaOnTriggerExit != null || luaOnTriggerStay != null)
			{
				PhysicsListener listener = PhysicsListener.Get(this);
				listener.onCollisionEnter = luaOnCollisionEnter;
				listener.onCollisionExit = luaOnCollisionExit;
				listener.onCollisionStay = luaOnCollisionStay;
				listener.onTriggerEnter = luaOnTriggerEnter;
				listener.onTriggerExit = luaOnTriggerExit;
				listener.onTriggerStay = luaOnTriggerStay;
				if (!m_BehaviourListenerSet.Contains(listener))
				{
					m_BehaviourListenerSet.Add(listener);
				}
			}
		}

		private void AddEnabledListener()
		{
			Action<LuaTable> luaOnEnable = m_LuaTable.Get<Action<LuaTable>>("OnEnable");
			Action<LuaTable> luaOnDisable = m_LuaTable.Get<Action<LuaTable>>("OnDisable");
			if (luaOnEnable != null || luaOnDisable != null)
			{
				EnabledListener listener = EnabledListener.Get(this);
				listener.onEnable = luaOnEnable;
				listener.onDisable = luaOnDisable;
				if (!m_BehaviourListenerSet.Contains(listener))
				{
					m_BehaviourListenerSet.Add(listener);
				}
			}
		}

		private void AddGUIListener()
		{
			Action<LuaTable> luaOnGUI = m_LuaTable.Get<Action<LuaTable>>("OnGUI");
			if (luaOnGUI != null)
			{
				GUIListener listener = GUIListener.Get(this);
				listener.onGUI = luaOnGUI;
				if (!m_BehaviourListenerSet.Contains(listener))
				{
					m_BehaviourListenerSet.Add(listener);
				}
			}
		}

		protected virtual void OnEnable()
		{
			foreach (MonoBehaviour behaviourListener in m_BehaviourListenerSet)
			{
				behaviourListener.enabled = true;
			}
		}

		protected virtual void OnDisable()
		{
			foreach (MonoBehaviour behaviourListener in m_BehaviourListenerSet)
			{
				behaviourListener.enabled = false;
			}
		}

		protected virtual string GetLuaPath()
		{
			return m_LuaPath;
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			if (m_LuaStart != null)
			{
				m_LuaStart(m_LuaTable);
			}
		}

		protected virtual void OnDestroy()
		{
			if (m_LuaOnDestroy != null)
			{
				m_LuaOnDestroy(m_LuaTable);
			}
			Dispose();
		}

		protected virtual void Dispose()
		{
			m_LuaAwake = null;
			m_LuaStart = null;
			m_LuaOnDestroy = null;
			foreach (IDisposable behaviourListener in m_BehaviourListenerSet)
			{
				behaviourListener.Dispose();
			}
			m_BehaviourListenerSet.Clear();
			if (m_LuaTable != null)
			{
				m_LuaTable.Dispose();
				m_LuaTable = null;
			}
		}
	}
}