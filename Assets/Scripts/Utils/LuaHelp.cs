using System;
using System.Reflection;
using System.Collections;
using UnityEngine;
using UObject = UnityEngine.Object;
using XLua;

namespace Main
{
	public static class LuaHelp
	{
		public static int GetHash(object obj)
		{
			return obj.GetHashCode();
		}

		public static string StringFormat(string format, params object[] args)
		{
			int argLength = args.Length;
			for (int i = 0; i < argLength; ++i)
			{
				args[i] = args[i] ?? "null";
			}
			return string.Format(format ?? "", args);
		}

		public static string[] StringSplit(string str, params string[] separator)
		{
			if (str != null)
			{
				return str.Split(separator, StringSplitOptions.None);
			}
			return null;
		}

		public static string StringReplace(string str, string oldValue, string newValue)
		{
			if (str != null)
			{
				return str.Replace(oldValue, newValue);
			}
			return null;
		}

		public static AnimationState GetAnimState(Animation anim, string name)
		{
			return anim[name];
		}

		public static void Foreach(IEnumerable collection, Action<object> action)
		{
			if (action != null)
			{
				foreach (object item in collection)
				{
					action(item);
				}
			}
		}

		public static int RandomRangeInt(int min, int max)
		{
			return UnityEngine.Random.Range(min, max);
		}

		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity,
			int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
		}

		public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity,
			int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return Physics.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
		}

		public static LuaTable AddLuaChild(GameObject go, string name, string luaPath, params object[] args)
		{
			return AddLuaChild(go.transform, name, luaPath, args);
		}

		public static LuaTable AddLuaChild(Component comp, string name, string luaPath, params object[] args)
		{
			GameObject go = CompAgent.AddChild(comp, name);
			return AddLuaComponent(go, luaPath, args);
		}

		public static LuaTable AddLuaComponent(GameObject go, string luaPath, params object[] args)
		{
			LuaBehaviour luaBehaviour = go.AddComponent<LuaBehaviour>();
			luaBehaviour.m_LuaPath = luaPath;
			luaBehaviour.InitLua(args);
			return luaBehaviour.LuaTable;
		}

		public static LuaTable AddLuaComponent(Component comp, string luaPath, params object[] args)
		{
			return AddLuaComponent(comp.gameObject, luaPath, args);
		}

		public static bool IsNull(UObject uobj)
		{
			return !uobj;
		}

		public static bool IsNotNull(UObject uobj)
		{
			return uobj;
		}

		#region generic delegate

		private static MethodInfo MakeGenericMethod(string methodName, params Type[] types)
		{
			BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
			MethodInfo mi = typeof(LuaHelp).GetMethod(methodName, flags);
			return mi.MakeGenericMethod(types);
		}

		#region action
		public static object MakeAction1(Action<object> action, Type type)
		{
			MethodInfo genericMi = MakeGenericMethod("MakeAction1", type);
			return genericMi.Invoke(null, new object[] { action });
		}

		private static Action<T> MakeAction1<T>(Action<object> action)
		{
			return arg => action(arg);
		}

		public static object MakeAction2(Action<object, object> action, Type type1, Type type2)
		{
			MethodInfo genericMi = MakeGenericMethod("MakeAction2", type1, type2);
			return genericMi.Invoke(null, new object[] { action });
		}

		private static Action<T1, T2> MakeAction2<T1, T2>(Action<object, object> action)
		{
			return (arg1, arg2) => action(arg1, arg2);
		}

		public static object MakeAction3(Action<object, object, object> action, Type type1, Type type2, Type type3)
		{
			MethodInfo genericMi = MakeGenericMethod("MakeAction3", type1, type2, type3);
			return genericMi.Invoke(null, new object[] { action });
		}

		private static Action<T1, T2, T3> MakeAction3<T1, T2, T3>(Action<object, object, object> action)
		{
			return (arg1, arg2, arg3) => action(arg1, arg2, arg3);
		}

		public static object MakeAction4(Action<object, object, object, object> action, Type type1, Type type2, Type type3, Type type4)
		{
			MethodInfo genericMi = MakeGenericMethod("MakeAction4", type1, type2, type3, type4);
			return genericMi.Invoke(null, new object[] { action });
		}

		private static Action<T1, T2, T3, T4> MakeAction4<T1, T2, T3, T4>(Action<object, object, object, object> action)
		{
			return (arg1, arg2, arg3, arg4) => action(arg1, arg2, arg3, arg4);
		}
		#endregion

		#region func
		public static object MakeFunc(Func<object> action, Type type)
		{
			MethodInfo genericMi = MakeGenericMethod("MakeFunc", type);
			return genericMi.Invoke(null, new object[] { action });
		}

		private static Func<T> MakeFunc<T>(Func<object> func)
		{
			return () =>
			{
				object obj = func();
				if (obj is T)
				{
					return (T) obj;
				}
				else
				{
					return default(T);
				}
			};
		}

		public static object MakeFunc1(Action<object, object> action, Type type, Type typeResult)
		{
			MethodInfo genericMi = MakeGenericMethod("MakeFunc1", type, typeResult);
			return genericMi.Invoke(null, new object[] { action });
		}

		private static Func<T, TResult> MakeFunc1<T, TResult>(Func<object, object> func)
		{
			return (arg) =>
			{
				object result = func(arg);
				if (result is TResult)
				{
					return (TResult) result;
				}
				else
				{
					return default(TResult);
				}
			};
		}

		public static object MakeFunc2(Action<object, object> action, Type type1, Type type2, Type typeResult)
		{
			MethodInfo genericMi = MakeGenericMethod("MakeFunc2", type1, type2, typeResult);
			return genericMi.Invoke(null, new object[] { action });
		}

		private static Func<T1, T2, TResult> MakeFunc2<T1, T2, TResult>(Func<object, object, object> func)
		{
			return (arg1, arg2) =>
			{
				object result = func(arg1, arg2);
				if (result is TResult)
				{
					return (TResult) result;
				}
				else
				{
					return default(TResult);
				}
			};
		}

		public static object MakeFunc3(Action<object, object> action, Type type1, Type type2, Type type3, Type typeResult)
		{
			MethodInfo genericMi = MakeGenericMethod("MakeFunc3", type1, type2, type3, typeResult);
			return genericMi.Invoke(null, new object[] { action });
		}

		private static Func<T1, T2, T3, TResult> MakeFunc3<T1, T2, T3, TResult>(Func<object, object, object, object> func)
		{
			return (arg1, arg2, arg3) =>
			{
				object result = func(arg1, arg2, arg3);
				if (result is TResult)
				{
					return (TResult) result;
				}
				else
				{
					return default(TResult);
				}
			};
		}

		public static object MakeFunc4(Action<object, object> action, Type type1, Type type2, Type type3, Type type4, Type typeResult)
		{
			MethodInfo genericMi = MakeGenericMethod("MakeFunc4", type1, type2, type3, type4, typeResult);
			return genericMi.Invoke(null, new object[] { action });
		}

		private static Func<T1, T2, T3, T4, TResult> MakeFunc4<T1, T2, T3, T4, TResult>(Func<object, object, object, object, object> func)
		{
			return (arg1, arg2, arg3, arg4) =>
			{
				object result = func(arg1, arg2, arg3, arg4);
				if (result is TResult)
				{
					return (TResult) result;
				}
				else
				{
					return default(TResult);
				}
			};
		}
		#endregion

		#endregion
	}
}
