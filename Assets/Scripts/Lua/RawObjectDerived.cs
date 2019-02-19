using XLua;

namespace Main
{
	public class BytesObject : RawObject
	{
		private byte[] m_Target;

		public BytesObject(byte[] bytes)
		{
			m_Target = bytes;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class FloatObject : RawObject
	{
		private float m_Target;

		public FloatObject(float num)
		{
			m_Target = num;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class DoubleObject : RawObject
	{
		private double m_Target;

		public DoubleObject(double num)
		{
			m_Target = num;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class SByteObject : RawObject
	{
		private sbyte m_Target;

		public SByteObject(sbyte num)
		{
			m_Target = num;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class ByteObject : RawObject
	{
		private byte m_Target;

		public ByteObject(byte num)
		{
			m_Target = num;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class CharObject : RawObject
	{
		private char m_Target;

		public CharObject(char c)
		{
			m_Target = c;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class ShortObject : RawObject
	{
		private short m_Target;

		public ShortObject(short num)
		{
			m_Target = num;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class UShortObject : RawObject
	{
		private ushort m_Target;

		public UShortObject(ushort num)
		{
			m_Target = num;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class IntObject : RawObject
	{
		private int m_Target;

		public IntObject(int num)
		{
			m_Target = num;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class UIntObject : RawObject
	{
		private uint m_Target;

		public UIntObject(uint num)
		{
			m_Target = num;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}

	public class ULongObject : RawObject
	{
		private ulong m_Target;

		public ULongObject(ulong num)
		{
			m_Target = num;
		}

		public object Target
		{
			get
			{
				return m_Target;
			}
		}
	}
}