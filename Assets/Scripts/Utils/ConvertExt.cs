using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace Main
{
	public static class ConvertExt
	{
		public static Color ParseColor24(string text, int offset)
		{
			StringBuilder builder = new StringBuilder();
			for (int index = text.Length; index < 6; ++index)
			{
				builder.Append('0');
			}
			text += builder;
			int r = (HexToDecimal(text[offset]) << 4) | HexToDecimal(text[offset + 1]);
			int g = (HexToDecimal(text[offset + 2]) << 4) | HexToDecimal(text[offset + 3]);
			int b = (HexToDecimal(text[offset + 4]) << 4) | HexToDecimal(text[offset + 5]);
			float f = 1f / 255f;
			return new Color(f * r, f * g, f * b);
		}

		public static int HexToDecimal(char ch)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch - '0';
			}
			if (ch >= 'a' && ch <= 'f')
			{
				return ch - 'a' + 10;
			}
			if (ch >= 'A' && ch <= 'F')
			{
				return ch - 'A' + 10;
			}
			return 0x0;
		}

		public static string EncodeColor24(Color c)
		{
			int i = 0xFFFFFF & (ColorToInt(c) >> 8);
			return DecimalToHex24(i);
		}

		public static int ColorToInt(Color c)
		{
			int retVal = 0;
			retVal |= Mathf.RoundToInt(c.r * 255f) << 24;
			retVal |= Mathf.RoundToInt(c.g * 255f) << 16;
			retVal |= Mathf.RoundToInt(c.b * 255f) << 8;
			retVal |= Mathf.RoundToInt(c.a * 255f);
			return retVal;
		}

		public static string DecimalToHex24(int num)
		{
			num &= 0xFFFFFF;
			return num.ToString("X6");
		}

		public static byte[] StringToBytes(string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		public static string BytesToString(byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		public static byte[] IntToBytes(int num)
		{
			return BitConverter.GetBytes(num);
		}

		public static int BytesToInt(byte[] bytes)
		{
			return BitConverter.ToInt32(bytes, 0);
		}

		public static byte[] UIntToBytes(uint num)
		{
			return BitConverter.GetBytes(num);
		}

		public static uint BytesToUInt(byte[] bytes)
		{
			return BitConverter.ToUInt32(bytes, 0);
		}

		public static string StringToMd5(string srcStr)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] srcBytes = StringToBytes(srcStr);
			byte[] dstBytes = md5.ComputeHash(srcBytes);
			StringBuilder sb = new StringBuilder();
			for (int index = 0; index < dstBytes.Length; index++)
			{
				sb.Append(dstBytes[index].ToString("x2"));
			}
			return sb.ToString();
		}

		#region url

		public static string UriEncode(string url)
		{
			return Uri.EscapeUriString(url);
		}

		#endregion

		private const long TICKS_PER_SECOND = 10000000;
		private const long TICKS_PER_MILLISECOND = 10000;
		private const long TIME_TICKS_OFFSET = 621356256000000000;

		public static long SecondsToMilliseconds(long time)
		{
			return time * TICKS_PER_SECOND / TICKS_PER_MILLISECOND;
		}

		public static long MillisecondsToSeconds(long time)
		{
			return time * TICKS_PER_MILLISECOND / TICKS_PER_SECOND;
		}

		public static DateTime SecondsToDateTime(long time)
		{
			return new DateTime(time * TICKS_PER_SECOND + TIME_TICKS_OFFSET);
		}

		public static long DateTimeToSeconds(DateTime data)
		{
			return (data.Ticks - TIME_TICKS_OFFSET) / TICKS_PER_SECOND;
		}

		public static TimeSpan SecondsToTimeSpan(float duration)
		{
			return new TimeSpan((long) (duration * TICKS_PER_SECOND));
		}

		public static float TimeSpanToSeconds(TimeSpan timeSpan)
		{
			return timeSpan.Ticks / (float) TICKS_PER_SECOND;
		}

		public static DateTime MillisecondsToDateTime(long time)
		{
			return new DateTime(time * TICKS_PER_MILLISECOND + TIME_TICKS_OFFSET);
		}

		public static long DateTimeToMilliseconds(DateTime data)
		{
			return (data.Ticks - TIME_TICKS_OFFSET) / TICKS_PER_MILLISECOND;
		}

		public static TimeSpan MillisecondsToTimeSpan(long duration)
		{
			return new TimeSpan(duration * TICKS_PER_MILLISECOND);
		}

		public static long TimeSpanToMilliseconds(TimeSpan timeSpan)
		{
			return timeSpan.Ticks / TICKS_PER_MILLISECOND;
		}
	}
}