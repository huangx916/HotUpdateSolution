using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
	public class ListenerData
	{
	}

	public class ListenerData<T1> : ListenerData
	{
		protected T1 mValue1;
		public T1 Value1
		{
			get
			{
				return mValue1;
			}
		}
		public ListenerData(T1 value1) : base()
		{
			mValue1 = value1;
		}
	}

	public class ListenerData<T1, T2> : ListenerData<T1>
	{
		protected T2 mValue2;
		public T2 Value2
		{
			get
			{
				return mValue2;
			}
		}
		public ListenerData(T1 value1, T2 value2) : base(value1)
		{
			mValue2 = value2;
		}
	}

	public class ListenerData<T1, T2, T3> : ListenerData<T1, T2>
	{
		protected T3 mValue3;
		public T3 Value3
		{
			get
			{
				return mValue3;
			}
		}
		public ListenerData(T1 value1, T2 value2, T3 value3) : base(value1, value2)
		{
			mValue3 = value3;
		}
	}

	public class ListenerData<T1, T2, T3, T4> : ListenerData<T1, T2, T3>
	{
		protected T4 mValue4;
		public T4 Value4
		{
			get
			{
				return mValue4;
			}
		}
		public ListenerData(T1 value1, T2 value2, T3 value3, T4 value4) : base(value1, value2, value3)
		{
			mValue4 = value4;
		}
	}

	public class ExpandDelegate
	{
		public Delegate Listener
		{
			get;
			set;
		}

		public Delegate OncePerFrameListener
		{
			get;
			set;
		}

		public IList OncePerFrameArgList
		{
			get;
			set;
		}

		public string Name
		{
			get
			{
				StringBuilder name = new StringBuilder();
				if (Listener != null)
				{
					Delegate[] listeners = Listener.GetInvocationList();
					if (listeners.Length > 0)
					{
						name.Append(listeners[0].Method.Name);
						for (int index = 1, length = listeners.Length; index < length; ++index)
						{
							name.Append("|");
							name.Append(listeners[index].Method.Name);
						}
					}
				}
				if (OncePerFrameListener != null)
				{
					Delegate[] oncePerFramelisteners = OncePerFrameListener.GetInvocationList();
					if (oncePerFramelisteners.Length > 0)
					{
						if (name.Length > 0)
						{
							name.Append(",");
						}
						name.Append(oncePerFramelisteners[0].Method.Name);
						for (int index = 1, length = oncePerFramelisteners.Length; index < length; ++index)
						{
							name.Append("|");
							name.Append(oncePerFramelisteners[index].Method.Name);
						}
					}
				}
				return name.ToString();
			}
		}
	}

	public class ListenerManager : MonoBehaviour
	{
		public static ListenerManager Instance
		{
			get
			{
				return GameMain.Instance.ManagerCollection.GetManager<ListenerManager>();
			}
		}

		private Dictionary<string, Dictionary<object, ExpandDelegate>> mListenerDict = new Dictionary<string, Dictionary<object, ExpandDelegate>>();

		void LateUpdate()
		{
			StartCoroutine(DoOncePerFrameTrigger());
		}

		private IEnumerator DoOncePerFrameTrigger()
		{
			yield return new WaitForEndOfFrame();

			RemoveInvalidListener();

			List<ExpandDelegate> epDelegateList = new List<ExpandDelegate>();
			foreach (Dictionary<object, ExpandDelegate> dict in mListenerDict.Values)
			{
				foreach (ExpandDelegate epDelegate in dict.Values)
				{
					if (epDelegate.OncePerFrameArgList != null && epDelegate.OncePerFrameArgList.Count > 0 && epDelegate.OncePerFrameListener != null)
					{
						epDelegateList.Add(epDelegate);
					}
				}
			}

			foreach (ExpandDelegate epDelegate in epDelegateList)
			{
				Type argListType = epDelegate.OncePerFrameArgList.GetType();
				MethodInfo toArrayMethod = argListType.GetMethod("ToArray");
				object args = toArrayMethod.Invoke(epDelegate.OncePerFrameArgList, null);
				epDelegate.OncePerFrameListener.DynamicInvoke(new object[] { args });
				epDelegate.OncePerFrameArgList.Clear();
			}
		}

		#region AddListener

		/// <summary>
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddListener(string type, object observer, Action method)
		{
			AddListenerExt(type, observer, method);
		}

		/// <summary>
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddListener<T>(string type, object observer, Action<T> method)
		{
			AddListenerExt(type, observer, method);
		}

		/// <summary>
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddListener<T1, T2>(string type, object observer, Action<T1, T2> method)
		{
			AddListenerExt(type, observer, method);
		}

		/// <summary>
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddListener<T1, T2, T3>(string type, object observer, Action<T1, T2, T3> method)
		{
			AddListenerExt(type, observer, method);
		}

		/// <summary>
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddListener<T1, T2, T3, T4>(string type, object observer, Action<T1, T2, T3, T4> method)
		{
			AddListenerExt(type, observer, method);
		}

		/// <summary>
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddListenerExt(string type, object observer, Delegate method)
		{
			if (method != null && ListenerAddingCheck(type, observer, method.GetType(), false))
			{
				ExpandDelegate epDelegate = mListenerDict[type][observer];
				epDelegate.Listener = Delegate.Combine(epDelegate.Listener, method);
			}
		}

		private bool ListenerAddingCheck(string type, object observer, Type argsType, bool oncePerFrame)
		{
			if (observer == null)
			{
				Debugger.LogError("Observer is null!");
				return false;
			}

			if (!mListenerDict.ContainsKey(type))
			{
				Dictionary<object, ExpandDelegate> observerListenerDict = new Dictionary<object, ExpandDelegate>();
				observerListenerDict.Add(observer, new ExpandDelegate());
				mListenerDict.Add(type, observerListenerDict);
				return true;
			}

			Dictionary<object, ExpandDelegate> dict = mListenerDict[type];
			if (!dict.ContainsKey(observer))
			{
				dict.Add(observer, new ExpandDelegate());
				return true;
			}

			ExpandDelegate epDelegate = dict[observer];
			Delegate listener = oncePerFrame ? epDelegate.OncePerFrameListener : epDelegate.Listener;
			if (listener == null || listener.GetType() == argsType)
			{
				return true;
			}

			Debugger.LogError("Incompatible delegate Types!");
			return false;
		}

		#endregion

		#region AddOncePerFrameListener

		/// <summary>
		/// Once-per-frame-listener will invoke once on end of current frame.
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddOncePerFrameListener(string type, object observer, Action<ListenerData[]> method)
		{
			AddOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// Once-per-frame-listener will invoke once on end of current frame.
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddOncePerFrameListener<T>(string type, object observer, Action<ListenerData<T>[]> method)
		{
			AddOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// Once-per-frame-listener will invoke once on end of current frame.
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddOncePerFrameListener<T1, T2>(string type, object observer, Action<ListenerData<T1, T2>[]> method)
		{
			AddOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// Once-per-frame-listener will invoke once on end of current frame.
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddOncePerFrameListener<T1, T2, T3>(string type, object observer, Action<ListenerData<T1, T2, T3>[]> method)
		{
			AddOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// Once-per-frame-listener will invoke once on end of current frame.
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddOncePerFrameListener<T1, T2, T3, T4>(string type, object observer, Action<ListenerData<T1, T2, T3, T4>[]> method)
		{
			AddOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// Once-per-frame-listener will invoke once on end of current frame.
		/// </summary>
		/// <param name="type">The type which want to listen.</param>
		/// <param name="observer">The key is used to remove listener easily.</param>
		/// <param name="method">The method which want to invoke while event been triggered.</param>
		public void AddOncePerFrameListenerExt<T>(string type, object observer, Action<T[]> method) where T : ListenerData
		{
			if (method != null && ListenerAddingCheck(type, observer, typeof(Action<T[]>), true))
			{
				ExpandDelegate epDelegate = mListenerDict[type][observer];
				epDelegate.OncePerFrameListener = Delegate.Combine(epDelegate.OncePerFrameListener, method);
				epDelegate.OncePerFrameArgList = new List<T>();
			}
		}

		#endregion

		#region TriggerListener

		public void TriggerListener(string type)
		{
			Debugger.Log("Trigger " + type);

			Instance.RemoveInvalidListener(type);

			if (mListenerDict.ContainsKey(type))
			{
				List<Delegate> list = new List<Delegate>();
				foreach (ExpandDelegate epDelegate in mListenerDict[type].Values)
				{
					if (epDelegate == null)
					{
						continue;
					}
					if (epDelegate.Listener is Action)
					{
						list.Add(epDelegate.Listener);
					}
					if (epDelegate.OncePerFrameListener != null)
					{
						Type elementType = GetFirstArgumentElementType(epDelegate.OncePerFrameListener);
						if (elementType.IsAssignableFrom(typeof(ListenerData)))
						{
							epDelegate.OncePerFrameArgList.Add(new ListenerData());
						}
					}
				}

				foreach (Action listener in list)
				{
					listener();
				}
			}
		}

		public void TriggerListener<T>(string type, T arg)
		{
			Debugger.Log("Trigger " + type + ": " + arg);

			RemoveInvalidListener(type);

			if (mListenerDict.ContainsKey(type))
			{
				List<Delegate> list = new List<Delegate>();
				foreach (ExpandDelegate epDelegate in mListenerDict[type].Values)
				{
					if (epDelegate == null)
					{
						continue;
					}
					if (epDelegate.Listener is Action<T>)
					{
						list.Add(epDelegate.Listener);
					}
					else
					{
						Type[] types = epDelegate.Listener.GetType().GetGenericArguments();
						if (IsTypesAssignableFrom(types, typeof(T)))
						{
							list.Add(epDelegate.Listener);
						}
					}
					if (epDelegate.OncePerFrameListener != null)
					{
						Type elementType = GetFirstArgumentElementType(epDelegate.OncePerFrameListener);
						if (elementType.IsAssignableFrom(typeof(ListenerData<T>)))
						{
							epDelegate.OncePerFrameArgList.Add(new ListenerData<T>(arg));
						}
					}
				}

				foreach (Delegate listener in list)
				{
					if (listener is Action<T>)
					{
						(listener as Action<T>)(arg);
					}
					else
					{
						listener.DynamicInvoke(arg);
					}
				}
			}
		}

		public void TriggerListener<T1, T2>(string type, T1 arg1, T2 arg2)
		{
			Debugger.Log("Trigger " + type + ": " + arg1 + ", " + arg2);

			RemoveInvalidListener(type);

			if (mListenerDict.ContainsKey(type))
			{
				List<Delegate> list = new List<Delegate>();
				foreach (ExpandDelegate epDelegate in mListenerDict[type].Values)
				{
					if (epDelegate == null)
					{
						continue;
					}
					if (epDelegate.Listener is Action<T1, T2>)
					{
						list.Add(epDelegate.Listener);
					}
					else
					{
						Type[] types = epDelegate.Listener.GetType().GetGenericArguments();
						if (IsTypesAssignableFrom(types, typeof(T1), typeof(T2)))
						{
							list.Add(epDelegate.Listener);
						}
					}
					if (epDelegate.OncePerFrameListener != null)
					{
						Type elementType = GetFirstArgumentElementType(epDelegate.OncePerFrameListener);
						if (elementType.IsAssignableFrom(typeof(ListenerData<T1, T2>)))
						{
							epDelegate.OncePerFrameArgList.Add(new ListenerData<T1, T2>(arg1, arg2));
						}
					}
				}

				foreach (Delegate listener in list)
				{
					if (listener is Action<T1, T2>)
					{
						(listener as Action<T1, T2>)(arg1, arg2);
					}
					else
					{
						listener.DynamicInvoke(arg1, arg2);
					}
				}
			}
		}

		public void TriggerListener<T1, T2, T3>(string type, T1 arg1, T2 arg2, T3 arg3)
		{
			Debugger.Log("Trigger " + type + ": " + arg1 + ", " + arg2 + ", " + arg3);

			RemoveInvalidListener(type);

			if (mListenerDict.ContainsKey(type))
			{
				List<Delegate> list = new List<Delegate>();
				foreach (ExpandDelegate epDelegate in mListenerDict[type].Values)
				{
					if (epDelegate == null)
					{
						continue;
					}
					if (epDelegate.Listener is Action<T1, T2, T3>)
					{
						list.Add(epDelegate.Listener);
					}
					else
					{
						Type[] types = epDelegate.Listener.GetType().GetGenericArguments();
						if (IsTypesAssignableFrom(types, typeof(T1), typeof(T2), typeof(T3)))
						{
							list.Add(epDelegate.Listener);
						}
					}
					if (epDelegate.OncePerFrameListener != null)
					{
						Type elementType = GetFirstArgumentElementType(epDelegate.OncePerFrameListener);
						if (elementType.IsAssignableFrom(typeof(ListenerData<T1, T2, T3>)))
						{
							epDelegate.OncePerFrameArgList.Add(new ListenerData<T1, T2, T3>(arg1, arg2, arg3));
						}
					}
				}

				foreach (Delegate listener in list)
				{
					if (listener is Action<T1, T2, T3>)
					{
						(listener as Action<T1, T2, T3>)(arg1, arg2, arg3);
					}
					else
					{
						listener.DynamicInvoke(arg1, arg2, arg3);
					}
				}
			}
		}

		public void TriggerListener<T1, T2, T3, T4>(string type, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			Debugger.Log("Trigger " + type + ": " + arg1 + ", " + arg2 + ", " + arg3 + ", " + arg4);

			RemoveInvalidListener(type);

			if (mListenerDict.ContainsKey(type))
			{
				List<Delegate> list = new List<Delegate>();
				foreach (ExpandDelegate epDelegate in mListenerDict[type].Values)
				{
					if (epDelegate == null)
					{
						continue;
					}
					if (epDelegate.Listener is Action<T1, T2, T3, T4>)
					{
						list.Add(epDelegate.Listener);
					}
					else
					{
						Type[] types = epDelegate.Listener.GetType().GetGenericArguments();
						if (IsTypesAssignableFrom(types, typeof(T1), typeof(T2), typeof(T3), typeof(T4)))
						{
							list.Add(epDelegate.Listener);
						}
					}
					if (epDelegate.OncePerFrameListener != null)
					{
						Type elementType = GetFirstArgumentElementType(epDelegate.OncePerFrameListener);
						if (elementType.IsAssignableFrom(typeof(ListenerData<T1, T2, T3, T4>)))
						{
							epDelegate.OncePerFrameArgList.Add(new ListenerData<T1, T2, T3, T4>(arg1, arg2, arg3, arg4));
						}
					}
				}

				foreach (Delegate listener in list)
				{
					if (listener is Action<T1, T2, T3, T4>)
					{
						(listener as Action<T1, T2, T3, T4>)(arg1, arg2, arg3, arg4);
					}
					else
					{
						listener.DynamicInvoke(arg1, arg2, arg3, arg4);
					}
				}
			}
		}

		private void TriggerListenerExt(bool isSilently, string type, params object[] args)
		{
			string argsStr = "";
			if (args != null)
			{
				for (int index = 0; index < args.Length; index++)
				{
					if (index != 0)
					{
						argsStr += ", ";
					}
					argsStr += args[index] == null ? "null" : args[index].ToString();
				}
			}
			if (!isSilently)
			{
				Debugger.Log("Trigger " + type + ": " + argsStr);
			}

			RemoveInvalidListener(type);

			Type[] argTypes = new Type[args.Length];
			for (int index = 0; index < args.Length; index++)
			{
				if (args[index] == null)
				{
					argTypes[index] = null;
				}
				else
				{
					argTypes[index] = args[index].GetType();
				}
			}
			if (mListenerDict.ContainsKey(type))
			{
				List<Delegate> list = new List<Delegate>();
				foreach (ExpandDelegate epDelegate in mListenerDict[type].Values)
				{
					if (epDelegate == null)
					{
						continue;
					}

					if (epDelegate.Listener != null)
					{
						Type[] types = epDelegate.Listener.GetType().GetGenericArguments();
						if (IsTypesAssignableFrom(types, argTypes))
						{
							list.Add(epDelegate.Listener);
						}
					}

					if (epDelegate.OncePerFrameListener != null)
					{
						Type listenerDataDefinitionType = null;
						switch (argTypes.Length)
						{
							case 0:
								listenerDataDefinitionType = typeof(ListenerData);
								break;
							case 1:
								listenerDataDefinitionType = typeof(ListenerData<>);
								break;
							case 2:
								listenerDataDefinitionType = typeof(ListenerData<,>);
								break;
							case 3:
								listenerDataDefinitionType = typeof(ListenerData<,,>);
								break;
							case 4:
								listenerDataDefinitionType = typeof(ListenerData<,,,>);
								break;
						}
						if (listenerDataDefinitionType != null)
						{
							Type listenerDataType = listenerDataDefinitionType.MakeGenericType(argTypes);
							Type elementType = GetFirstArgumentElementType(epDelegate.OncePerFrameListener);
							if (elementType.IsAssignableFrom(listenerDataType))
							{
								ConstructorInfo constructor = listenerDataType.GetConstructor(argTypes);
								if (constructor != null)
								{
									ListenerData listenerData = constructor.Invoke(args) as ListenerData;
									epDelegate.OncePerFrameArgList.Add(listenerData);
								}
							}
						}
					}
				}

				foreach (Delegate listener in list)
				{
#region For lua&WebGL
					switch (args.Length)
					{
						case 0:
							if (listener is Action)
							{
								(listener as Action)();
							}
							continue;
						case 1:
							if (listener is Action<object>)
							{
								(listener as Action<object>)(args[0]);
							}
							continue;
						case 2:
							if (listener is Action<object, object>)
							{
								(listener as Action<object, object>)(args[0], args[1]);
							}
							continue;
						case 3:
							if (listener is Action<object, object, object>)
							{
								(listener as Action<object, object, object>)(args[0], args[1], args[2]);
							}
							continue;
						case 4:
							if (listener is Action<object, object, object, object>)
							{
								(listener as Action<object, object, object, object>)(args[0], args[1], args[2], args[3]);
							}
							continue;
					}
#endregion
					listener.DynamicInvoke(args);
				}
			}
		}

		private Type GetFirstArgumentElementType(Delegate oncePerFrameListener)
		{
			MethodInfo info = oncePerFrameListener.GetType().GetMethod("Invoke");
			ParameterInfo[] parameters = info.GetParameters();
			return parameters.Length == 1 ? parameters[0].ParameterType.GetElementType() : null;
		}

		#endregion

		private bool IsTypesAssignableFrom(Type[] paramTypes, params Type[] argTypes)
		{
			if (paramTypes.Length != argTypes.Length)
			{
				return false;
			}
			for (int index = 0; index < paramTypes.Length; index++)
			{
				if (argTypes[index] == null)
				{
					if (paramTypes[index].IsValueType)
					{
						return false;
					}
				}
				else
				{
					if (!paramTypes[index].IsAssignableFrom(argTypes[index]))
					{
						return false;
					}
				}
			}
			return true;
		}

		private void RemoveInvalidListener()
		{
			foreach (KeyValuePair<string, Dictionary<object, ExpandDelegate>> typeObserverPair in mListenerDict)
			{
				string type = typeObserverPair.Key;
				RemoveInvalidListener(type);
			}
		}

		private void RemoveInvalidListener(string type)
		{
			if (!mListenerDict.ContainsKey(type))
			{
				return;
			}

			List<object> list = new List<object>();
			foreach (KeyValuePair<object, ExpandDelegate> pair in mListenerDict[type])
			{
				if (IsObserverNull(pair.Key))
				{
					list.Add(pair.Key);
					string logMessage = "The observer has been destroyed but you are still trying to trigger listener [{0}.{1}] with type: {2}.";
					Debugger.LogError(string.Format(logMessage, pair.Key.GetType().Name, pair.Value.Name, type));
				}
			}
			foreach (object observer in list)
			{
				mListenerDict[type].Remove(observer);
			}
		}

		private bool IsObserverNull(object obj)
		{
			if (obj == null)
			{
				return true;
			}
			if (obj is UnityEngine.Object)
			{
				return obj as UnityEngine.Object == null;
			}
			return false;
		}

		#region RemoveListener

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveListener(string type, object observer, Action method)
		{
			RemoveListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveListener<T>(string type, object observer, Action<T> method)
		{
			RemoveListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveListener<T1, T2>(string type, object observer, Action<T1, T2> method)
		{
			RemoveListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveListener<T1, T2, T3>(string type, object observer, Action<T1, T2, T3> method)
		{
			RemoveListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveListener<T1, T2, T3, T4>(string type, object observer, Action<T1, T2, T3, T4> method)
		{
			RemoveListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		/// <param name="oncePerFrame">Specify to remove which listener in ExpandDelegate.</param>
		public void RemoveListenerExt(string type, object observer, Delegate method = null)
		{
			if (observer == null || !mListenerDict.ContainsKey(type) || !mListenerDict[type].ContainsKey(observer))
			{
				return;
			}

			if (method == null)
			{
				mListenerDict[type].Remove(observer);
				return;
			}

			ExpandDelegate epDelegate = mListenerDict[type][observer];
			Delegate listener = epDelegate.Listener;
			if (listener == null)
			{
				Debugger.LogError("Listener is null!");
				return;
			}

			if (listener.GetType() != method.GetType())
			{
				Debugger.LogError("Incompatible delegate Types!");
				return;
			}

			epDelegate.Listener = Delegate.Remove(listener, method);
			if (epDelegate.Listener == null && epDelegate.OncePerFrameListener == null)
			{
				mListenerDict[type].Remove(observer);
			}
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveOncePerFrameListener(string type, object observer, Action<ListenerData[]> method)
		{
			RemoveOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveOncePerFrameListener<T>(string type, object observer, Action<ListenerData<T>[]> method)
		{
			RemoveOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveOncePerFrameListener<T1, T2>(string type, object observer, Action<ListenerData<T1, T2>[]> method)
		{
			RemoveOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveOncePerFrameListener<T1, T2, T3>(string type, object observer, Action<ListenerData<T1, T2, T3>[]> method)
		{
			RemoveOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		public void RemoveOncePerFrameListener<T1, T2, T3, T4>(string type, object observer, Action<ListenerData<T1, T2, T3, T4>[]> method)
		{
			RemoveOncePerFrameListenerExt(type, observer, method);
		}

		/// <summary>
		/// If can not find specified type or specified observer, do nothing.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		/// <param name="method">If method is null, remove observer.</param>
		/// <param name="oncePerFrame">Specify to remove which listener in ExpandDelegate.</param>
		public void RemoveOncePerFrameListenerExt<T>(string type, object observer, Action<T[]> method = null) where T : ListenerData
		{
			if (observer == null || !mListenerDict.ContainsKey(type) || !mListenerDict[type].ContainsKey(observer))
			{
				return;
			}

			if (method == null)
			{
				mListenerDict[type].Remove(observer);
				return;
			}

			ExpandDelegate epDelegate = mListenerDict[type][observer];
			Delegate listener = epDelegate.OncePerFrameListener;
			if (listener == null)
			{
				Debugger.LogError("Listener is null!");
				return;
			}

			if (listener.GetType() != method.GetType())
			{
				Debugger.LogError("Incompatible delegate Types!");
				return;
			}

			epDelegate.OncePerFrameListener = Delegate.Remove(listener, method);
			if (epDelegate.Listener == null && epDelegate.OncePerFrameListener == null)
			{
				mListenerDict[type].Remove(observer);
			}
		}

		/// <summary>
		/// Remove observer in all types.
		/// </summary>
		/// <param name="observer">If observer is null, do nothing.</param>
		public void RemoveAllListenerExt(object observer)
		{
			if (observer == null)
			{
				return;
			}
			foreach (Dictionary<object, ExpandDelegate> observerDelegateDict in mListenerDict.Values)
			{
				if (observerDelegateDict.ContainsKey(observer))
				{
					observerDelegateDict.Remove(observer);
				}
			}
		}

		#endregion

		public bool IsListenerExist(string type, object observer, Delegate method)
		{
			if (mListenerDict.ContainsKey(type) && mListenerDict[type].ContainsKey(observer))
			{
				if (mListenerDict[type][observer].Listener != null)
				{
					foreach (Delegate item in mListenerDict[type][observer].Listener.GetInvocationList())
					{
						if (item == method)
						{
							return true;
						}
					}
				}
				if (mListenerDict[type][observer].OncePerFrameListener != null)
				{
					foreach (Delegate item in mListenerDict[type][observer].OncePerFrameListener.GetInvocationList())
					{
						if (item == method)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		#region Shortcut by static

		public static bool IsExist(string type, object observer, Delegate method)
		{
			return Instance.IsListenerExist(type, observer, method);
		}

		public static void AddLua1(string type, object observer, Action<object> method)
		{
			Add<object>(type, observer, method);
		}

		public static void AddLua2(string type, object observer, Action<object, object> method)
		{
			Add<object, object>(type, observer, method);
		}

		public static void AddLua3(string type, object observer, Action<object, object, object> method)
		{
			Add<object, object, object>(type, observer, method);
		}

		public static void AddLua4(string type, object observer, Action<object, object, object, object> method)
		{
			Add<object, object, object, object>(type, observer, method);
		}

		public static void Add(string type, object observer, Action method)
		{
			Instance.AddListener(type, observer, method);
		}

		public static void Add<T>(string type, object observer, Action<T> method)
		{
			Instance.AddListener(type, observer, method);
		}

		public static void Add<T1, T2>(string type, object observer, Action<T1, T2> method)
		{
			Instance.AddListener(type, observer, method);
		}

		public static void Add<T1, T2, T3>(string type, object observer, Action<T1, T2, T3> method)
		{
			Instance.AddListener(type, observer, method);
		}

		public static void Add<T1, T2, T3, T4>(string type, object observer, Action<T1, T2, T3, T4> method)
		{
			Instance.AddListener(type, observer, method);
		}

		public static void AddExt(string type, object observer, Action<object[]> method)
		{
			Instance.AddListenerExt(type, observer, method);
		}

		public static void AddOncePerFrame(string type, object observer, Action<ListenerData[]> method)
		{
			Instance.AddOncePerFrameListener(type, observer, method);
		}

		public static void AddOncePerFrame<T>(string type, object observer, Action<ListenerData<T>[]> method)
		{
			Instance.AddOncePerFrameListener(type, observer, method);
		}

		public static void AddOncePerFrame<T1, T2>(string type, object observer, Action<ListenerData<T1, T2>[]> method)
		{
			Instance.AddOncePerFrameListener(type, observer, method);
		}

		public static void AddOncePerFrame<T1, T2, T3>(string type, object observer, Action<ListenerData<T1, T2, T3>[]> method)
		{
			Instance.AddOncePerFrameListener(type, observer, method);
		}

		public static void AddOncePerFrame<T1, T2, T3, T4>(string type, object observer, Action<ListenerData<T1, T2, T3, T4>[]> method)
		{
			Instance.AddOncePerFrameListener(type, observer, method);
		}

		public static void AddOncePerFrameExt<T>(string type, object observer, Action<T[]> method) where T : ListenerData
		{
			Instance.AddOncePerFrameListenerExt(type, observer, method);
		}

		public static void Trigger(string type)
		{
			Instance.TriggerListener(type);
		}

		public static void Trigger<T>(string type, T arg)
		{
			Instance.TriggerListener(type, arg);
		}

		public static void Trigger<T1, T2>(string type, T1 arg1, T2 arg2)
		{
			Instance.TriggerListener(type, arg1, arg2);
		}

		public static void Trigger<T1, T2, T3>(string type, T1 arg1, T2 arg2, T3 arg3)
		{
			Instance.TriggerListener(type, arg1, arg2, arg3);
		}

		public static void Trigger<T1, T2, T3, T4>(string type, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			Instance.TriggerListener(type, arg1, arg2, arg3, arg4);
		}

		public static void TriggerExt(string type, params object[] args)
		{
			Instance.TriggerListenerExt(false, type, args);
		}

		public static void TriggerExtSilently(string type, params object[] args)
		{
			Instance.TriggerListenerExt(true, type, args);
		}

		public static void Remove(string type, object observer, Action method)
		{
			Instance.RemoveListener(type, observer, method);
		}

		public static void Remove<T>(string type, object observer, Action<T> method)
		{
			Instance.RemoveListener(type, observer, method);
		}

		public static void Remove<T1, T2>(string type, object observer, Action<T1, T2> method)
		{
			Instance.RemoveListener(type, observer, method);
		}

		public static void Remove<T1, T2, T3>(string type, object observer, Action<T1, T2, T3> method)
		{
			Instance.RemoveListener(type, observer, method);
		}

		public static void Remove<T1, T2, T3, T4>(string type, object observer, Action<T1, T2, T3, T4> method)
		{
			Instance.RemoveListener(type, observer, method);
		}

		public static void RemoveExt(string type, object observer, Delegate method = null)
		{
			Instance.RemoveListenerExt(type, observer, method);
		}

		public static void RemoveOncePerFrame(string type, object observer, Action<ListenerData[]> method)
		{
			Instance.RemoveOncePerFrameListener(type, observer, method);
		}

		public static void RemoveOncePerFrame<T>(string type, object observer, Action<ListenerData<T>[]> method)
		{
			Instance.RemoveOncePerFrameListener(type, observer, method);
		}

		public static void RemoveOncePerFrame<T1, T2>(string type, object observer, Action<ListenerData<T1, T2>[]> method)
		{
			Instance.RemoveOncePerFrameListener(type, observer, method);
		}

		public static void RemoveOncePerFrame<T1, T2, T3>(string type, object observer, Action<ListenerData<T1, T2, T3>[]> method)
		{
			Instance.RemoveOncePerFrameListener(type, observer, method);
		}

		public static void RemoveOncePerFrame<T1, T2, T3, T4>(string type, object observer, Action<ListenerData<T1, T2, T3, T4>[]> method)
		{
			Instance.RemoveOncePerFrameListener(type, observer, method);
		}

		public static void RemoveOncePerFrameExt<T>(string type, object observer, Action<T[]> method = null) where T : ListenerData
		{
			Instance.RemoveOncePerFrameListenerExt(type, observer, method);
		}

		public static void RemoveAllExt(object observer)
		{
			Instance.RemoveAllListenerExt(observer);
		}
		#endregion
	}
}