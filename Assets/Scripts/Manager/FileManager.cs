using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Main
{
	public class FileManager : MonoBehaviour
	{
		private class DataContainer<T>
		{
			private bool m_IsDone;
			public bool IsDone
			{
				get
				{
					return m_IsDone;
				}
			}

			private T m_Data;
			public T Data
			{
				get
				{
					return m_Data;
				}

				set
				{
					m_Data = value;
					m_IsDone = true;
				}
			}
		}

		private static Dictionary<string, DataContainer<byte[]>> s_ReadDataDict = new Dictionary<string, DataContainer<byte[]>>();
		private static HashSet<string> s_WritePathSet = new HashSet<string>();
#if !UNITY_EDITOR && UNITY_ANDROID
		private static AndroidJavaClass s_StreamingLoader = new AndroidJavaClass("UnityPlugins.IO.StreamingLoader");
#endif

		public static bool IsFileExist(string path)
		{
#if !UNITY_EDITOR && UNITY_ANDROID
			if (path.StartsWith(ConstValue.STREAMING_DIR_PATH))
			{
				string relativePath = path.Substring(ConstValue.STREAMING_DIR_PATH.Length);
				return IsStreamingExist(relativePath);
			}
#endif
			return File.Exists(path);
		}

		public static bool IsDirectoryExist(string path)
		{
#if !UNITY_EDITOR && UNITY_ANDROID
			if (path.StartsWith(ConstValue.STREAMING_DIR_PATH))
			{
				string relativePath = path.Substring(ConstValue.STREAMING_DIR_PATH.Length);
				return IsStreamingExist(relativePath);
			}
#endif
			return Directory.Exists(path);
		}

		private static FileInfo GetFile(string path)
		{
			FileInfo file = new FileInfo(path);
			string directoryName = file.DirectoryName;
			if (!IsDirectoryExist(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			return file;
		}

		public static void Write(string path, byte[] bytes)
		{
			FileStream fs = null;
			try
			{
				Debugger.Log("Write: " + path);

				FileInfo file = GetFile(path);
				fs = file.Open(FileMode.Create);
				fs.Write(bytes, 0, bytes.Length);
			}
			catch (Exception e)
			{
				Debugger.LogError("Write [" + path + "] error: " + e);
			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
				}
			}
		}

#if !UNITY_EDITOR && UNITY_ANDROID
		private static bool IsStreamingExist(string relativePath)
		{
			int slashIndex = relativePath.LastIndexOf('/');
            string dirPath = slashIndex == -1 ? "Android" : "Android" + relativePath.Substring(0, slashIndex).ToLower();
			string fileName = slashIndex == -1 ? relativePath : relativePath.Substring(slashIndex + 1);
			string[] fileNames = s_StreamingLoader.CallStatic<string[]>("list", dirPath);

            //Debugger.Log("<b>Load form ANDROID dirPath:</b> " + dirPath);
            //Debugger.Log("<b>Load form ANDROID fileName:</b> " + fileName);
            //foreach (string name in fileNames)
            //{
            //    Debugger.Log("<b>Load form ANDROID fileNames:</b> " + name);
            //}

			if (fileNames != null)
			{
				if (fileNames.Contains(fileName))
				{
                    //Debugger.Log("<b>Load form ANDROID true:</b> ");
					return true;
				}
				if (fileNames.Contains(fileName.ToLower()))
				{
                    //Debugger.Log("<b>Load form ANDROID true:</b> ");
					return true;
				}
			}
            //Debugger.Log("<b>Load form ANDROID false:</b> ");
			return false;
		}
#endif

        public static byte[] Read(string path)
		{
			if (!IsFileExist(path))
			{
				Debugger.LogError("File is not exist: " + path);
				return null;
			}

#if !UNITY_EDITOR && UNITY_ANDROID
			if (path.StartsWith(ConstValue.STREAMING_DIR_PATH))
			{
				string relativePath = path.Replace(ConstValue.STREAMING_DIR_PATH, "Android");
				return s_StreamingLoader.CallStatic<byte[]>("read", relativePath, ConstValue.BYTE_ARRAY_TEMP_LENGTH);
			}
#endif

			FileStream fs = null;
			MemoryStream ms = null;
			try
			{
				Debugger.Log("Read: " + path);

				FileInfo file = new FileInfo(path);
				fs = file.OpenRead();
				ms = new MemoryStream();
				byte[] bytesTemp = new byte[ConstValue.BYTE_ARRAY_TEMP_LENGTH];
				int readLength = 0;
				while ((readLength = fs.Read(bytesTemp, 0, ConstValue.BYTE_ARRAY_TEMP_LENGTH)) > 0)
				{
					ms.Write(bytesTemp, 0, readLength);
				}
				ms.Flush();
				return ms.ToArray();
			}
			catch (Exception e)
			{
				Debugger.LogError("Read [" + path + "] error: " + e);
				return null;
			}
			finally
			{
				if (ms != null)
				{
					ms.Close();
				}
				if (fs != null)
				{
					fs.Close();
				}
			}
		}

		public static void WriteAsync(string path, byte[] bytes, Action callback = null)
		{
			if (!s_WritePathSet.Contains(path))
			{
				FileStream fs = null;
				try
				{
					Debugger.Log("WriteAsync: " + path);
					s_WritePathSet.Add(path);

					FileInfo file = GetFile(path);
					fs = file.Open(FileMode.Create);
					fs.BeginWrite(bytes, 0, bytes.Length, ar =>
					{
						try
						{
							fs.EndWrite(ar);

							if (callback != null)
							{
								CoroutineManager.MainThread(callback);
							}
						}
						catch (Exception e)
						{
							Debugger.LogError("WriteAsync [" + path + "] error: " + e);
						}
						finally
						{
							fs.Close();
							s_WritePathSet.Remove(path);
						}
					}, null);
				}
				catch (Exception e)
				{
					Debugger.LogError("WriteAsync [" + path + "] error: " + e);
					if (fs != null)
					{
						fs.Close();
					}
					s_WritePathSet.Remove(path);
				}
			}
		}

		public static void ReadAsync(string path, Action<byte[]> callback)
		{
			if (!IsFileExist(path))
			{
				Debugger.LogError("File is not exist: " + path);
				if (callback != null)
				{
					callback(null);
				}
				return;
			}
			if (s_ReadDataDict.ContainsKey(path))
			{
				if (callback != null)
				{
					DataContainer<byte[]> dataContainer = s_ReadDataDict[path];
					CoroutineManager.Wait(() => dataContainer.IsDone, () => callback(dataContainer.Data));
				}
			}
			else
			{
				Debugger.Log("ReadAsync: " + path);
				DataContainer<byte[]> dataContainer = new DataContainer<byte[]>();
				s_ReadDataDict.Add(path, dataContainer);

#if !UNITY_EDITOR && UNITY_ANDROID
				if (path.StartsWith(ConstValue.STREAMING_DIR_PATH))
				{
					ThreadPool.QueueUserWorkItem(obj =>
					{
						string relativePath = path.Replace(ConstValue.STREAMING_DIR_PATH, "Android");
						byte[] bytes = s_StreamingLoader.CallStatic<byte[]>("read", path, ConstValue.BYTE_ARRAY_TEMP_LENGTH);
						if (callback != null)
						{
							CoroutineManager.MainThread(() => callback(bytes));
						}
						s_ReadDataDict.Remove(path);
					});
					return;
				}
#endif

				FileStream fs = null;
				try
				{
					FileInfo file = new FileInfo(path);
					fs = file.OpenRead();
					byte[] bytesTemp = new byte[ConstValue.BYTE_ARRAY_TEMP_LENGTH];
					fs.BeginRead(bytesTemp, 0, ConstValue.BYTE_ARRAY_TEMP_LENGTH, ar =>
					{
						MemoryStream ms = null;
						try
						{
							ms = new MemoryStream();
							int readLength = fs.EndRead(ar);
							while (readLength > 0)
							{
								ms.Write(bytesTemp, 0, readLength);
								readLength = fs.Read(bytesTemp, 0, ConstValue.BYTE_ARRAY_TEMP_LENGTH);
							}
							ms.Flush();
							dataContainer.Data = ms.ToArray();

							if (callback != null)
							{
								CoroutineManager.MainThread(() => callback(dataContainer.Data));
							}
						}
						catch (Exception e)
						{
							Debugger.LogError("ReadAsync [" + path + "] error: " + e);
						}
						finally
						{
							if (ms != null)
							{
								ms.Close();
							}
							fs.Close();
							s_ReadDataDict.Remove(path);
						}
					}, null);
				}
				catch (Exception e)
				{
					Debugger.LogError("ReadAsync [" + path + "] error: " + e);
					if (fs != null)
					{
						fs.Close();
					}
					s_ReadDataDict.Remove(path);
				}
			}
		}

		public static void DeleteChildren(string dirPath, string searchPattern = null, Func<string, bool> match = null)
		{
			DirectoryInfo directory = new DirectoryInfo(dirPath);
			if (directory.Exists)
			{
				FileSystemInfo[] infos = string.IsNullOrEmpty(searchPattern) ?
					directory.GetFileSystemInfos() : directory.GetFileSystemInfos(searchPattern);
				for (int index = infos.Length - 1; index >= 0; index--)
				{
					if (match == null || match(infos[index].Name))
					{
						infos[index].Delete();
					}
				}
			}
			else
			{
				Debugger.LogError("Directory is not exist: " + dirPath);
			}
		}

		public static void Delete(string path)
		{
			if (IsFileExist(path))
			{
				File.Delete(path);
			}
			else if (IsDirectoryExist(path))
			{
				Directory.Delete(path);
			}
			else
			{
				Debugger.LogError("File or directory is not exist: " + path);
			}
		}
	}
}