using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	public enum InjectionType
	{
		Space = -1,
		String = 0,
		Int = 1,
		Float = 2,
		Boolean = 3,

		Color = 11,
		Vector2 = 12,
		Vector3 = 13,
		Vector4 = 14,

		Curve = 21,

		Object = 30,
		GameObject = 31,
		Transform = 32,
		Behaviour = 33,
		OtherComp = 39,

		List = 98,
		Dict = 99
	}

	[System.Serializable]
	public class Injection
	{
		[SerializeField]
		protected string m_Name;
		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		[SerializeField]
		protected InjectionType m_Type;
		public InjectionType Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				if (value != m_Type)
				{
					m_Type = value;
					object v = GetValue();
					ResetValue();
					SetValue(v);
				}
			}
		}

		[SerializeField]
		protected string m_StringValue;
		[SerializeField]
		protected float[] m_NumsValue;
		[SerializeField]
		protected AnimationCurve m_CurveValue;
		[SerializeField]
		protected Object m_ObjectValue;
		public object Value
		{
			get
			{
				return GetValue();
			}
			set
			{
				ResetValue();
				SetValue(value);
			}
		}

		protected virtual object GetValue()
		{
			switch (Type)
			{
				case InjectionType.String:
					return m_StringValue;
				case InjectionType.Int:
					return m_NumsValue != null && m_NumsValue.Length > 0 ? (int) m_NumsValue[0] : 0;
				case InjectionType.Float:
					return m_NumsValue != null && m_NumsValue.Length > 0 ? m_NumsValue[0] : 0;
				case InjectionType.Boolean:
					return m_NumsValue != null && m_NumsValue.Length > 0 ? m_NumsValue[0] > 0 : false;
				case InjectionType.Curve:
					return m_CurveValue;
				case InjectionType.Color:
					{
						Color color = Color.white;
						int length = Mathf.Min(m_NumsValue == null ? 0 : m_NumsValue.Length, 4);
						for (int index = 0; index < length; index++)
						{
							color[index] = Mathf.Clamp01(m_NumsValue[index]);
						}
						return color;
					}
				case InjectionType.Vector2:
					{
						Vector2 vector = Vector2.zero;
						int length = Mathf.Min(m_NumsValue == null ? 0 : m_NumsValue.Length, 2);
						for (int index = 0; index < length; index++)
						{
							vector[index] = m_NumsValue[index];
						}
						return vector;
					}
				case InjectionType.Vector3:
					{
						Vector3 vector = Vector3.zero;
						int length = Mathf.Min(m_NumsValue == null ? 0 : m_NumsValue.Length, 3);
						for (int index = 0; index < length; index++)
						{
							vector[index] = m_NumsValue[index];
						}
						return vector;
					}
				case InjectionType.Vector4:
					{
						Vector4 vector = Vector4.zero;
						int length = Mathf.Min(m_NumsValue == null ? 0 : m_NumsValue.Length, 4);
						for (int index = 0; index < length; index++)
						{
							vector[index] = m_NumsValue[index];
						}
						return vector;
					}
				case InjectionType.Object:
				case InjectionType.GameObject:
				case InjectionType.Transform:
				case InjectionType.Behaviour:
				case InjectionType.OtherComp:
					return m_ObjectValue;
			}
			return null;
		}

		protected virtual void SetValue(object value)
		{
			switch (Type)
			{
				case InjectionType.String:
					m_StringValue = (value ?? "").ToString();
					break;
				case InjectionType.Int:
					if (value is int)
					{
						m_NumsValue = new float[] { (int) value };
					}
					break;
				case InjectionType.Float:
					if (value is float)
					{
						m_NumsValue = new float[] { (float) value };
					}
					break;
				case InjectionType.Boolean:
					if (value is bool)
					{
						m_NumsValue = new float[] { (bool) value ? 1 : 0 };
					}
					break;
				case InjectionType.Curve:
					if (value is AnimationCurve)
					{
						m_CurveValue = value as AnimationCurve;
					}
					break;
				case InjectionType.Color:
					if (value is Color)
					{
						Color color = (Color) value;
						m_NumsValue = new float[] { color.r, color.g, color.b, color.a };
					}
					break;
				case InjectionType.Vector2:
					if (value is Vector2)
					{
						Vector2 vector = (Vector2) value;
						m_NumsValue = new float[] { vector.x, vector.y };
					}
					break;
				case InjectionType.Vector3:
					if (value is Vector3)
					{
						Vector3 vector = (Vector3) value;
						m_NumsValue = new float[] { vector.x, vector.y, vector.z };
					}
					break;
				case InjectionType.Vector4:
					if (value is Vector4)
					{
						Vector4 vector = (Vector4) value;
						m_NumsValue = new float[] { vector.x, vector.y, vector.z, vector.w };
					}
					break;
				case InjectionType.Object:
				case InjectionType.GameObject:
				case InjectionType.Transform:
				case InjectionType.Behaviour:
				case InjectionType.OtherComp:
					if (value is Object)
					{
						m_ObjectValue = value as Object;
					}
					break;
			}
		}

		protected virtual void ResetValue()
		{
			m_StringValue = null;
			m_NumsValue = null;
			m_CurveValue = null;
			m_ObjectValue = null;
		}

		public static LuaTable ToValueTable(InjectionType type, object value)
		{
			if (value is List<Injection4>)
			{
				return ToValueTable(type, value as List<Injection4>);
			}
			else if (value is List<Injection3>)
			{
				return ToValueTable(type, value as List<Injection3>);
			}
			else if (value is List<Injection2>)
			{
				return ToValueTable(type, value as List<Injection2>);
			}
			else if (value is List<Injection1>)
			{
				return ToValueTable(type, value as List<Injection1>);
			}
			else if (value is List<Injection>)
			{
				return ToValueTable(type, value as List<Injection>);
			}
			return null;
		}

		private static LuaTable ToValueTable<T>(InjectionType type, List<T> array) where T : Injection
		{
			LuaTable table = LuaMain.LuaEnv.NewTable();
			for (int index = 0, realIndex = 0; index < array.Count; index++)
			{
				T injection = array[index];
				if (injection.Type != InjectionType.Space)
				{
					object value = injection.Value;
					if (injection.Type == InjectionType.List || injection.Type == InjectionType.Dict)
					{
						value = ToValueTable(injection.Type, injection.Value);
					}
					if (type == InjectionType.List)
					{
						table.Set(realIndex + 1, value);
					}
					else
					{
						table.Set(injection.Name, value);
					}
					realIndex++;
				}
			}
			return table;
		}
	}

	public interface IFoldable
	{
		bool IsFolded
		{
			get;
			set;
		}
	}

	[System.Serializable]
	public class Injection<T> : Injection, IFoldable
	{
		[SerializeField]
		private bool m_IsFolded;
		public bool IsFolded
		{
			get
			{
				return m_IsFolded;
			}
			set
			{
				m_IsFolded = value;
			}
		}

		[SerializeField]
		private List<T> m_ListOrDictValue;
		protected override object GetValue()
		{
			if (Type == InjectionType.List || Type == InjectionType.Dict)
			{
				return m_ListOrDictValue;
			}
			return base.GetValue();
		}
		protected override void SetValue(object value)
		{
			if (Type == InjectionType.List || Type == InjectionType.Dict)
			{
				if (value is List<T>)
				{
					m_ListOrDictValue = value as List<T>;
				}
			}
			else
			{
				base.SetValue(value);
			}
		}
		protected override void ResetValue()
		{
			base.ResetValue();
			m_ListOrDictValue = null;
		}
	}

	[System.Serializable]
	public class Injection1 : Injection<Injection>
	{
	}

	[System.Serializable]
	public class Injection2 : Injection<Injection1>
	{
	}

	[System.Serializable]
	public class Injection3 : Injection<Injection2>
	{
	}

	[System.Serializable]
	public class Injection4 : Injection<Injection3>
	{
	}
}
