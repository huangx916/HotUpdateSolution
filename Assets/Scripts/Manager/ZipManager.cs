using System;
using System.IO;
using UnityEngine;
using ICSharpCode.SharpZipLib.GZip;
using System.Threading;

namespace Main
{
	public class ZipManager : MonoBehaviour
	{
		public static byte[] Decompress(byte[] gzBytes)
		{
			if (gzBytes == null || gzBytes.Length <= 0)
			{
				return gzBytes;
			}

			MemoryStream ms = null;
			GZipInputStream gzis = null;
			MemoryStream msTemp = null;
			try
			{
				ms = new MemoryStream(gzBytes);
				gzis = new GZipInputStream(ms);
				msTemp = new MemoryStream();

				byte[] bytesTemp = new byte[ConstValue.BYTE_ARRAY_TEMP_LENGTH];
				int readLength = 0;
				while ((readLength = gzis.Read(bytesTemp, 0, ConstValue.BYTE_ARRAY_TEMP_LENGTH)) != 0)
				{
					msTemp.Write(bytesTemp, 0, readLength);
				}
				gzis.Close();
				gzis = null;

				return msTemp.ToArray();
			}
			catch (Exception e)
			{
				Debugger.LogError("Data can not be decompressed: " + e);
				return null;
			}
			finally
			{
				if (msTemp != null)
				{
					msTemp.Close();
				}
				if (gzis != null)
				{
					gzis.Close();
				}
				if (ms != null)
				{
					ms.Close();
				}
			}
		}

		public static void DecompressAsync(byte[] gzBytes, Action<byte[]> callback)
		{
			if (callback == null)
			{
				return;
			}

#if UNITY_WEBGL
			byte[] bytes = Decompress(gzBytes);
			callback(bytes);
#else
			ThreadPool.QueueUserWorkItem(obj =>
			{
				byte[] bytes = Decompress(gzBytes);
				CoroutineManager.MainThread(() => callback(bytes));
			});
#endif
		}

		public static byte[] Compress(byte[] bytes)
		{
			if (bytes == null || bytes.Length <= 0)
			{
				return bytes;
			}

			MemoryStream ms = null;
			GZipOutputStream gzos = null;
			try
			{
				ms = new MemoryStream();
				gzos = new GZipOutputStream(ms);
				gzos.Write(bytes, 0, bytes.Length);
				gzos.Close();
				gzos = null;
				return ms.ToArray();
			}
			catch (Exception e)
			{
				Debugger.LogError("Data can not be compressed: " + e);
				return null;
			}
			finally
			{
				if (gzos != null)
				{
					gzos.Close();
				}
				if (ms != null)
				{
					ms.Close();
				}
			}
		}

		public static void CompressAsync(byte[] bytes, Action<byte[]> callback)
		{
			if (callback == null)
			{
				return;
			}

#if UNITY_WEBGL
			byte[] gzBytes = Compress(bytes);
			callback(gzBytes);
#else
			ThreadPool.QueueUserWorkItem(obj =>
			{
				byte[] gzBytes = Compress(bytes);
				CoroutineManager.MainThread(() => callback(gzBytes));
			});
#endif
		}
	}
}