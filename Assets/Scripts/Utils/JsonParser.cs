using System;
using System.Collections.Generic;
using SimpleJson;

namespace Main
{
	public static class JsonParser
	{
		public static T ObjectToType<T>(object data, T defaultValue)
		{
			Type type = typeof(T);
			if (data is T)
			{
				return (T) data;
			}
			else if (type.IsEnum || type == typeof(int))
			{
				return (T) (object) ObjectToInt(data);
			}
			else if (type == typeof(float))
			{
				return (T) (object) ObjectToFloat(data);
			}
			else if (type == typeof(bool))
			{
				return (T) (object) ObjectToBool(data);
			}
			else if (type == typeof(long))
			{
				return (T) (object) ObjectToLong(data);
			}
			else if (type == typeof(double))
			{
				return (T) (object) ObjectToBool(data);
			}
			else
			{
				return StringParser.StringToType<T>(data.ToString(), defaultValue);
			}
		}

		public static string ObjectToString(object data, string defaultValue = null)
		{
			if (data == null)
			{
				Debugger.LogWarning("data is null");
				return defaultValue;
			}
			return data.ToString();
		}

		public static int ObjectToInt(object data, int defaultValue = 0)
		{
			try
			{
				if (data == null)
				{
					Debugger.LogWarning("Can not convert null to int.");
					return defaultValue;
				}
				if (data is string)
				{
					string dataStr = data.ToString();
					return StringParser.StringToInt(dataStr, defaultValue);
				}
				return Convert.ToInt32(data);
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to int with {1}", data, e.GetType().Name));
			}
			return defaultValue;
		}

		public static long ObjectToLong(object data, long defaultValue = 0)
		{
			try
			{
				if (data == null)
				{
					Debugger.LogWarning("Can not convert null to long.");
					return defaultValue;
				}
				if (data is string)
				{
					string dataStr = data.ToString();
					return StringParser.StringToLong(dataStr, defaultValue);
				}
				return Convert.ToInt64(data);
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to long with {1}", data, e.GetType().Name));
			}
			return defaultValue;
		}

		public static float ObjectToFloat(object data, float defaultValue = 0)
		{
			try
			{
				if (data == null)
				{
					Debugger.LogWarning("Can not convert null to float.");
					return defaultValue;
				}
				return Convert.ToSingle(data);
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to float with {1}", data, e.GetType().Name));
			}
			return defaultValue;
		}

		public static double ObjectToDouble(object data, double defaultValue = 0)
		{
			try
			{
				if (data == null)
				{
					Debugger.LogWarning("Can not convert null to double.");
					return defaultValue;
				}
				return Convert.ToDouble(data);
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to double with {1}", data, e.GetType().Name));
			}
			return defaultValue;
		}

		public static bool ObjectToBool(object data, bool defaultValue = false)
		{
			try
			{
				if (data == null)
				{
					Debugger.LogWarning("Can not convert null to bool.");
					return defaultValue;
				}
				if (data is string)
				{
					string dataStr = data.ToString();
					return StringParser.StringToBool(dataStr, defaultValue);
				}
				return Convert.ToBoolean(data);
			}
			catch (Exception e)
			{
				Debugger.LogError(string.Format("Can not convert {0} to bool with {1}", data, e.GetType().Name));
			}
			return defaultValue;
		}

		public static Dictionary<string, TValue> JoToDict<TValue>(JsonObject data)
		{
			Dictionary<string, TValue> result = new Dictionary<string, TValue>();
			foreach (string key in data.Keys)
			{
				object value = data[key];
				result.Add(key, ObjectToType(data[key], default(TValue)));
			}
			return result;
		}

		public static T[] JaToArray<T>(JsonArray data)
		{
			T[] result = new T[data.Count];
			for (int index = 0; index < data.Count; index++)
			{
				result[index] = ObjectToType(data[index], default(T));
			}
			return result;
		}

		public static JsonObject JaToJo(JsonArray data)
		{
			JsonObject result = new JsonObject();
			for (int index = 0; index < data.Count; index++)
			{
				result[index.ToString()] = data[index];
			}
			return result;
		}

		public static string JoItemToString(JsonObject data, string key, string defaultValue = null)
		{
			if (data.ContainsKey(key))
			{
				return ObjectToString(data[key], defaultValue);
			}
			return defaultValue;
		}

		public static int JoItemToInt(JsonObject data, string key, int defaultValue = 0)
		{
			if (data.ContainsKey(key))
			{
				return ObjectToInt(data[key], defaultValue);
			}
			return defaultValue;
		}

		public static long JoItemToLong(JsonObject data, string key, long defaultValue = 0)
		{
			if (data.ContainsKey(key))
			{
				return ObjectToLong(data[key], defaultValue);
			}
			return defaultValue;
		}

		public static float JoItemToFloat(JsonObject data, string key, float defaultValue = 0f)
		{
			if (data.ContainsKey(key))
			{
				return ObjectToFloat(data[key], defaultValue);
			}
			return defaultValue;
		}

		public static double JoItemToDouble(JsonObject data, string key, double defaultValue = 0f)
		{
			if (data.ContainsKey(key))
			{
				return ObjectToDouble(data[key], defaultValue);
			}
			return defaultValue;
		}

		public static bool JoItemToBool(JsonObject data, string key, bool defaultValue = false)
		{
			if (data.ContainsKey(key))
			{
				return ObjectToBool(data[key], defaultValue);
			}
			return defaultValue;
		}

		public static JsonObject JoItemToJo(JsonObject data, string key, bool defaultNull = false)
		{
			if (data.ContainsKey(key) && data[key] is JsonObject)
			{
				return data[key] as JsonObject;
			}
			return defaultNull ? null : new JsonObject();
		}

		public static Dictionary<string, TValue> JoItemToDict<TValue>(JsonObject data, string key, bool defaultNull = false)
		{
			JsonObject jo = JoItemToJo(data, key, defaultNull);
			if (jo != null)
			{
				return JoToDict<TValue>(jo);
			}
			return defaultNull ? null : new Dictionary<string, TValue>();
		}

		public static JsonArray JoItemToJa(JsonObject data, string key, bool defaultNull = false)
		{
			if (data.ContainsKey(key) && data[key] is JsonArray)
			{
				return data[key] as JsonArray;
			}
			return defaultNull ? null : new JsonArray();
		}

		public static T[] JoItemToArray<T>(JsonObject data, string key, bool defaultNull = false)
		{
			JsonArray ja = JoItemToJa(data, key, defaultNull);
			if (ja != null)
			{
				return JaToArray<T>(ja);
			}
			return defaultNull ? null : new T[0];
		}

		public static string JaItemToString(JsonArray data, int index, string defaultValue = null)
		{
			if (index >= 0 && index < data.Count)
			{
				return ObjectToString(data[index]);
			}
			return defaultValue;
		}

		public static int JaItemToInt(JsonArray data, int index, int defaultValue = 0)
		{
			if (index >= 0 && index < data.Count)
			{
				return ObjectToInt(data[index]);
			}
			return defaultValue;
		}

		public static long JaItemToLong(JsonArray data, int index, long defaultValue = 0)
		{
			if (index >= 0 && index < data.Count)
			{
				return ObjectToLong(data[index]);
			}
			return defaultValue;
		}

		public static bool JaItemToBool(JsonArray data, int index, bool defaultValue = false)
		{
			if (index >= 0 && index < data.Count)
			{
				return ObjectToBool(data[index]);
			}
			return defaultValue;
		}

		public static JsonObject JaItemToJo(JsonArray data, int index, bool defaultNull = false)
		{
			if (index >= 0 && index < data.Count && data[index] is JsonObject)
			{
				return data[index] as JsonObject;
			}
			return defaultNull ? null : new JsonObject();
		}

		public static Dictionary<string, TValue> JaItemToDict<TValue>(JsonArray data, int index, bool defaultNull = false)
		{
			JsonObject jo = JaItemToJo(data, index, defaultNull);
			if (jo != null)
			{
				return JoToDict<TValue>(jo);
			}
			return defaultNull ? null : new Dictionary<string, TValue>();
		}

		public static JsonArray JaItemToJa(JsonArray data, int index, bool defaultNull = false)
		{
			if (index >= 0 && index < data.Count && data[index] is JsonArray)
			{
				return data[index] as JsonArray;
			}
			return defaultNull ? null : new JsonArray();
		}

		public static T[] JaItemToArray<T>(JsonArray data, int index, bool defaultNull = false)
		{
			JsonArray ja = JaItemToJa(data, index, defaultNull);
			if (ja != null)
			{
				return JaToArray<T>(ja);
			}
			return defaultNull ? null : new T[0];
		}

		public static T[] JoItemToEnumArray<T>(JsonObject wall, string key)
		{
			JsonArray ja = JoItemToJa(wall, key, false);
			T[] result = new T[ja.Count];
			for (int index = 0; index < ja.Count; index++)
			{
				object value = ja[index];
				if (value is string)
				{
					result[index] = (T) Enum.Parse(typeof(T), value.ToString());
				}
				else
				{
					result[index] = (T) (object) ObjectToInt(value);
				}
			}
			return result;
		}

        public static T[][] JoItem2Array2<T>(JsonObject wall, string key, int groupLength)
        {
            JsonArray groupsJa = JoItemToJa(wall, key, false);
            T[][] result = new T[groupsJa.Count][];
            for (int groupIndex = 0; groupIndex < groupsJa.Count; groupIndex++)
            {
                result[groupIndex] = new T[groupLength];
                JsonArray cardsJa = groupsJa[groupIndex] as JsonArray;
                for (int cardIndex = 0; cardIndex < groupLength; cardIndex++)
                {
                    result[groupIndex][cardIndex] = cardsJa == null ? default(T) : ObjectToType(cardsJa[cardIndex], default(T));
                }
            }
            return result;
        }
	}
}