using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Main
{
	public static class StringParser
	{
		private static Regex sBoolRegex = new Regex("^\\d$");
		private static Regex sIntRegex = new Regex("^[+-]?\\d+$");
		private static Regex sColorRegex = new Regex("^\\d+(,\\d+)*$");
		private static Regex sVectorRegex = new Regex("^-?\\d*\\.?\\d+(,-?\\d*\\.?\\d+)*$");
		private static Regex sVector4Regex = new Regex("^-?\\d*\\.?\\d+(,-?\\d*\\.?\\d+){3,}$");

		public static T ListItemToType<T>(IList<string> list, int index)
		{
			return ListItemToType(list, index, default(T));
		}

		public static T ListItemToType<T>(IList<string> list, int index, T defaultValue)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToType<T>(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static T StringToType<T>(string str)
		{
			return StringToType(str, default(T));
		}

		public static T StringToType<T>(string str, T defaultValue)
		{
			Type type = typeof(T);
			if (str is T)
			{
				return (T) (object) str;
			}
			else if (type.IsEnum || type == typeof(int))
			{
				return (T) (object) StringToInt(str);
			}
			else if (type == typeof(long))
			{
				return (T) (object) StringToLong(str);
			}
			else if (type == typeof(short))
			{
				return (T) (object) StringToShort(str);
			}
			else if (type == typeof(float))
			{
				return (T) (object) StringToFloat(str);
			}
			else if (type == typeof(double))
			{
				return (T) (object) StringToDouble(str);
			}
			else if (type == typeof(bool))
			{
				return (T) (object) StringToBool(str);
			}
			else if (type == typeof(Vector2))
			{
				return (T) (object) StringToVector2(str);
			}
			else if (type == typeof(Vector3))
			{
				return (T) (object) StringToVector3(str);
			}
			else if (type == typeof(Vector4))
			{
				return (T) (object) StringToVector4(str);
			}
			else if (type == typeof(Quaternion))
			{
				return (T) (object) StringToQuaternion(str);
			}
			else if (type == typeof(SimpleJson.JsonArray))
			{
				return (T) (object) StringToJa(str);
			}
			else if (type == typeof(SimpleJson.JsonObject))
			{
				return (T) (object) StringToJo(str);
			}
			else
			{
				return defaultValue;
			}
		}

		public static string ListItemToString(IList<string> list, int index, string defaultValue = null)
		{
			if (index >= 0 && index < list.Count)
			{
				return list[index];
			}
			return defaultValue;
		}

		public static byte ListItemToByte(IList<string> list, int index, byte defaultValue = 0)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToByte(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static byte StringToByte(string str, byte defaultValue = 0)
		{
			try
			{
				if (string.IsNullOrEmpty(str))
				{
					Debugger.LogError("Can not convert null or empty to byte.");
					return defaultValue;
				}
				if (sIntRegex.IsMatch(str))
				{
					return Convert.ToByte(str);
				}
				return Convert.ToByte(Convert.ToInt32(Convert.ToDouble(str)));
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to int with {1}", str, e.GetType().Name));
			}
			return defaultValue;
		}

		public static int ListItemToInt(IList<string> list, int index, int defaultValue = 0)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToInt(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static int StringToInt(string str, int defaultValue = 0)
		{
			try
			{
				if (string.IsNullOrEmpty(str))
				{
					Debugger.LogWarning("Can not convert null or empty to int.");
					return defaultValue;
				}
				if (sIntRegex.IsMatch(str))
				{
					return Convert.ToInt32(str);
				}
				return Convert.ToInt32(Convert.ToDouble(str));
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to int with {1}", str, e.GetType().Name));
			}
			return defaultValue;
		}

		public static long ListItemToLong(IList<string> list, int index, long defaultValue = 0)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToLong(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static long StringToLong(string str, long defaultValue = 0)
		{
			try
			{
				if (string.IsNullOrEmpty(str))
				{
					Debugger.LogWarning("Can not convert null or empty to long.");
					return defaultValue;
				}
				if (sIntRegex.IsMatch(str))
				{
					return Convert.ToInt64(str);
				}
				return Convert.ToInt64(Convert.ToDouble(str));
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to long with {1}", str, e.GetType().Name));
			}
			return defaultValue;
		}

		public static short ListItemToShort(IList<string> list, int index, short defaultValue = 0)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToShort(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static short StringToShort(string str, short defaultValue = 0)
		{
			try
			{
				if (string.IsNullOrEmpty(str))
				{
					Debugger.LogWarning("Can not convert null or empty to short.");
					return defaultValue;
				}
				if (sIntRegex.IsMatch(str))
				{
					return Convert.ToInt16(str);
				}
				return Convert.ToInt16(Convert.ToDouble(str));
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to short with {1}", str, e.GetType().Name));
			}
			return defaultValue;
		}

		public static float ListItemToFloat(IList<string> list, int index, float defaultValue = 0)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToFloat(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static float StringToFloat(string str, float defaultValue = 0)
		{
			try
			{
				if (string.IsNullOrEmpty(str))
				{
					Debugger.LogWarning("Can not convert null or empty to float.");
					return defaultValue;
				}
				return Convert.ToSingle(str);
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to float with {1}", str, e.GetType().Name));
			}
			return defaultValue;
		}

		public static double ListItemToDouble(IList<string> list, int index, double defaultValue = 0)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToDouble(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static double StringToDouble(string str, double defaultValue = 0)
		{
			try
			{
				if (string.IsNullOrEmpty(str))
				{
					Debugger.LogWarning("Can not convert null or empty to double.");
					return defaultValue;
				}
				return Convert.ToDouble(str);
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to double with {1}", str, e.GetType().Name));
			}
			return defaultValue;
		}

		public static bool ListItemToBool(IList<string> list, int index, bool defaultValue = false)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToBool(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static bool StringToBool(string str, bool defaultValue = false)
		{
			try
			{
				if (string.IsNullOrEmpty(str))
				{
					Debugger.LogWarning("Can not convert null or empty to bool.");
					return defaultValue;
				}
				if (sBoolRegex.IsMatch(str))
				{
					return Convert.ToBoolean(Convert.ToDouble(str));
				}
				return Convert.ToBoolean(str);
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to bool with {1}", str, e.GetType().Name));
			}
			return defaultValue;
		}

		public static Color32 StringToColor32(string str)
		{
			Color32 defualtValue = Color.white;
			return StringToColor32(str, defualtValue);
		}

		public static Color32 StringToColor32(string str, Color32 defualtValue)
		{
			if (sColorRegex.IsMatch(str ?? ""))
			{
				string[] strComps = str.Split(',');
				byte r = ListItemToByte(strComps, 0);
				byte g = ListItemToByte(strComps, 1);
				byte b = ListItemToByte(strComps, 2);
				byte a = ListItemToByte(strComps, 3, 255);
				return new Color32(r, g, b, a);
			}
			return defualtValue;
		}

		public static Vector2 ListItemToVector2(IList<string> list, int index)
		{
			Vector2 defualtValue = Vector2.zero;
			return ListItemToVector2(list, index, defualtValue);
		}

		public static Vector2 ListItemToVector2(IList<string> list, int index, Vector2 defaultValue)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToVector2(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static Vector2 StringToVector2(string str)
		{
			Vector2 defualtValue = Vector2.zero;
			return StringToVector2(str, defualtValue);
		}

		public static Vector2 StringToVector2(string str, Vector2 defualtValue)
		{
			if (sVectorRegex.IsMatch(str ?? ""))
			{
				string[] strComps = str.Split(',');
				float x = ListItemToFloat(strComps, 0);
				float y = ListItemToFloat(strComps, 1);
				return new Vector2(x, y);
			}
			return defualtValue;
		}

		public static Vector3 ListItemToVector3(IList<string> list, int index)
		{
			Vector3 defualtValue = Vector3.zero;
			return ListItemToVector3(list, index, defualtValue);
		}

		public static Vector3 ListItemToVector3(IList<string> list, int index, Vector3 defaultValue)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToVector3(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static Vector3 StringToVector3(string str)
		{
			Vector3 defualtValue = Vector3.zero;
			return StringToVector3(str, defualtValue);
		}

		public static Vector3 StringToVector3(string str, Vector3 defualtValue)
		{
			if (sVectorRegex.IsMatch(str ?? ""))
			{
				string[] strComps = str.Split(',');
				float x = ListItemToFloat(strComps, 0);
				float y = ListItemToFloat(strComps, 1);
				float z = ListItemToFloat(strComps, 2);
				return new Vector3(x, y, z);
			}
			return defualtValue;
		}

		public static Vector4 ListItemToVector4(IList<string> list, int index)
		{
			Vector4 defualtValue = Vector4.zero;
			return ListItemToVector4(list, index, defualtValue);
		}

		public static Vector4 ListItemToVector4(IList<string> list, int index, Vector4 defaultValue)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToVector4(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static Vector3 StringToVector4(string str)
		{
			Vector4 defualtValue = Vector4.zero;
			return StringToVector4(str, defualtValue);
		}

		public static Vector4 StringToVector4(string str, Vector4 defualtValue)
		{
			if (sVectorRegex.IsMatch(str ?? ""))
			{
				string[] strComps = str.Split(',');
				float x = ListItemToFloat(strComps, 0);
				float y = ListItemToFloat(strComps, 1);
				float z = ListItemToFloat(strComps, 2);
				float w = ListItemToFloat(strComps, 3);
				return new Vector4(x, y, z, w);
			}
			return defualtValue;
		}

		public static Quaternion ListItemToQuaternion(IList<string> list, int index)
		{
			Quaternion defualtValue = Quaternion.identity;
			return ListItemToQuaternion(list, index, defualtValue);
		}

		public static Quaternion ListItemToQuaternion(IList<string> list, int index, Quaternion defaultValue)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToQuaternion(list[index], defaultValue);
			}
			return defaultValue;
		}

		public static Quaternion StringToQuaternion(string str)
		{
			Quaternion defualtValue = Quaternion.identity;
			return StringToQuaternion(str, defualtValue);
		}

		public static Quaternion StringToQuaternion(string str, Quaternion defualtValue)
		{
			if (sVectorRegex.IsMatch(str ?? ""))
			{
				string[] strComps = str.Split(',');
				float x = ListItemToFloat(strComps, 0);
				float y = ListItemToFloat(strComps, 1);
				float z = ListItemToFloat(strComps, 2);
				float w = ListItemToFloat(strComps, 3);
				return new Quaternion(x, y, z, w);
			}
			return defualtValue;
		}

		public static bool IsVector4(string trimedStr)
		{
			return sVector4Regex.IsMatch(trimedStr ?? "");
		}

		public static List<object> ListItemToJa(IList<string> list, int index)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToJa(list[index]);
			}
			return null;
		}

		public static SimpleJson.JsonArray StringToJa(string str, bool defaultNull = false)
		{
			try
			{
				if (!string.IsNullOrEmpty(str))
				{
					SimpleJson.JsonArray ja = SimpleJson.SimpleJson.DeserializeObject<SimpleJson.JsonArray>(str);
					if (ja != null)
					{
						return ja;
					}
				}
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to JsonArray with {1}", str, e));
			}
			return defaultNull ? null : new SimpleJson.JsonArray();
		}

		public static SimpleJson.JsonObject ListItemToJo(IList<string> list, int index)
		{
			if (index >= 0 && index < list.Count)
			{
				return StringToJo(list[index]);
			}
			return null;
		}

		public static SimpleJson.JsonObject StringToJo(string str, bool defaultNull = false)
		{
			try
			{
				if (!string.IsNullOrEmpty(str))
				{
					SimpleJson.JsonObject jo = SimpleJson.SimpleJson.DeserializeObject<SimpleJson.JsonObject>(str);
					if (jo != null)
					{
						return jo;
					}
				}
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to JsonObject with {1}", str, e));
			}
			return defaultNull ? null : new SimpleJson.JsonObject();
		}

		public class ConfigList<T>
		{
			public List<T> list;
		}

		public static List<T> StringToConfigList<T>(string jsonStr, bool defaultNull = false)
		{
			try
			{
				ConfigList<T> configList = JsonUtility.FromJson<ConfigList<T>>(jsonStr);
				if (configList != null)
				{
					return configList.list;
				}
				Debugger.LogError("Config is none!");
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to ConfigList with {1}", jsonStr, e));
			}
			return defaultNull ? null : new List<T>();
		}

		public static Dictionary<int, TValue> StringToConfigDict<TValue>(string jsonStr)
		{
			return StringToConfigDict<int, TValue>(jsonStr, (index, value) => index);
		}

		public static Dictionary<TKey, TValue> StringToConfigDict<TKey, TValue>(string jsonStr, Func<TValue, TKey> getKeyFunc)
		{
			return StringToConfigDict<TKey, TValue>(jsonStr, (index, value) => getKeyFunc(value));
		}

		private static Dictionary<TKey, TValue> StringToConfigDict<TKey, TValue>(string jsonStr, Func<int, TValue, TKey> getKeyFunc)
		{
			if (getKeyFunc == null)
			{
				return null;
			}

			Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
			List<TValue> list = StringToConfigList<TValue>(jsonStr, false);
			for (int index = 0; index < list.Count; index++)
			{
				dict.Add(getKeyFunc(index, list[index]), list[index]);
			}
			return dict;
		}
	}
}