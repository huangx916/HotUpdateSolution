using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using System.IO;
using XLua;

namespace Main
{
	public enum AssetPathType
	{
		Resources,
		Streaming,
		Persistent,
		Temporary
	}

	public enum UpdateStepType
	{
		UpdateFailed = -1,
		UpdateCancel,
		VersionCompare,
		VersionCompared,
		FileCompare,
		FileDownload,
		FileDelete,
		VersionWrite,
		UpdateFinished
	}

	public class HotUpdateManager : MonoBehaviour
	{
		public static HotUpdateManager Instance
		{
			get
			{
                return GameMain.Instance.HotUpdateManager;
			}
		}

		private string m_CdnCurVerUrl;
        private string m_ModuleName = "common";

		private AssetPathType m_AssetPathType = AssetPathType.Streaming;
		private UpdateStepType m_UpdateStepType = UpdateStepType.VersionCompare;
		public AssetPathType AssetPathType { get { return m_AssetPathType; } }
		public UpdateStepType UpdateStepType { get { return m_UpdateStepType; } }

		private string m_CdnVersion;
		private string m_Version;
		public string Version { get { return m_Version; } }

		private List<string> m_UpdateFileNameList;
		private List<string> m_DeleteFileNameList;
		public List<string> UpdateFileNameList { get { return m_UpdateFileNameList; } }
		public List<string> DeleteFileNameList { get { return m_DeleteFileNameList; } }

		private int m_Index;
		private WWWLoader m_CurrentLoader;
		public int Index { get { return m_Index; } }
		public WWWLoader CurrentLoader { get { return m_CurrentLoader; } }
		
		private JsonObject m_CdnFileList;
		public JsonObject CdnFileList { get { return m_CdnFileList; } }
		public JsonObject m_PersistentFileList;
		public JsonObject m_StreamingFileList;
		public JsonObject FileList
		{
			get
			{
#if DOWNLOAD
				if (m_AssetPathType == AssetPathType.Persistent)
				{
					return m_PersistentFileList;
				}
#endif
				return m_StreamingFileList;
			}
		}

        private LuaTable m_LoadingUI = null;
        private int m_UpdateIndex = 0;

        void Awake()
        {
#if DOWNLOAD
            m_UpdateStepType = UpdateStepType.VersionCompare;
            NativeVersionCompare();

            if (IsPersistentHasVersion())
            {
                // 如果有Version字段，则有Persistent数据，加载Persistent的FileList
                byte[] persistentFileListBytes = FileManager.Read(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.FILE_LIST_NAME);
                string persistentFileListStr = persistentFileListBytes == null ? null : ConvertExt.BytesToString(persistentFileListBytes);
                m_PersistentFileList = StringParser.StringToJo(persistentFileListStr, false);
            }
            else
            {
                // 否则，Persistent的FileList为空
                m_PersistentFileList = new JsonObject();
            }
#else
            m_UpdateStepType = UpdateStepType.UpdateCancel;
            m_Version = ConstValue.VERSION;
#endif
            if (FileManager.IsFileExist(ConstValue.STREAMING_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.FILE_LIST_NAME))
            {
                byte[] streamingFileListBytes = FileManager.Read(ConstValue.STREAMING_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.FILE_LIST_NAME);
                string streamingFileListStr = streamingFileListBytes == null ? null : ConvertExt.BytesToString(streamingFileListBytes);
                m_StreamingFileList = StringParser.StringToJo(streamingFileListStr, false);
            }
            else 
            {
                m_StreamingFileList = new JsonObject();
            }
        }

#if DOWNLOAD
        private void NativeVersionCompare()
		{
			Debugger.Log("Native version compare.");
#if UNITY_WEBGL
			m_AssetPathType = AssetPathType.Persistent;
			m_Version = GetPersistentVersion(null);
			Debugger.Log("Persistent version: " + (m_Version ?? "null"));
			Debugger.Log("Last native version in Persistent.");
#else
            string streamingVersion = ConstValue.VERSION;
            Debugger.Log("Streaming version: " + streamingVersion);
            string[] streamingVersions = ConstValue.VERSION.Split('.');
            int streamingHotVersion = StringParser.StringToInt(streamingVersions[2]);

            string persistentVersion = GetPersistentVersion(ConstValue.VERSION);
            Debugger.Log("Persistent version: " + persistentVersion);
            string[] persistentVersions = persistentVersion.Split('.');
            int persistentHotVersion = StringParser.StringToInt(persistentVersions[2]);

            if (string.Equals(persistentVersions[0], streamingVersions[0]) &&
                string.Equals(persistentVersions[1], streamingVersions[1]) &&
                persistentHotVersion > streamingHotVersion)
            {
                m_AssetPathType = AssetPathType.Persistent;
                m_Version = persistentVersion;
                Debugger.Log("Last native version in Persistent.");
            }
            else
            {
                m_AssetPathType = AssetPathType.Streaming;
                m_Version = streamingVersion;
                Debugger.Log("Last native version in Streaming.");
            }
#endif
		}
#endif

		public void VersionCompare(Action updateAction)
        {
#if DOWNLOAD
            CoroutineManager.Start(DoVersionCompare(updateAction), this);
#endif
        }

        private IEnumerator DoVersionCompare(Action updateAction)
        {
            m_UpdateIndex++;
            // 等待UIRoot加载完毕
            yield return null;

            if(m_UpdateIndex == 1)
            {
                LuaTable UIManager = LuaMain.LuaEnv.Global.GetInPath<LuaTable>("LuaClass.UIManager.Instance");
                LuaTable moduleTypeClass = LuaMain.LuaEnv.Global.GetInPath<LuaTable>("LuaClass.ModuleType");
                m_LoadingUI = LuaMain.Instance.FuncInvoke(UIManager, "Open", UIManager, moduleTypeClass.Get<object>("Common"), "LoadingUI") as LuaTable;
            }

            // 对比版本
            m_UpdateStepType = UpdateStepType.VersionCompare;
            Debugger.Log("Start cdn version compare.");

            // 下载CDN服务器上的版本信息
            string[] streamingVersions = ConstValue.VERSION.Split('.');
            string cdnCurVerUrl = ConstValue.CDN_URL + "/" + ConstValue.GAME_NAME + "/" + ConstValue.BUNDLE_DIR + "/" + streamingVersions[0] + "." + streamingVersions[1];
            WWWLoader cdnVersionLoader = WWWManager.Load(cdnCurVerUrl + "/" + m_ModuleName + "/" + ConstValue.VERSION_NAME, this);
            yield return cdnVersionLoader.Wait;
            if (cdnVersionLoader.Error != null)
            {
                // 下载失败，需要整包更新或其他原因
                //Debugger.Log("Update failed by: " + cdnVersionLoader.Error);
                //m_UpdateStepType = UpdateStepType.UpdateFailed;
                //OnUpdateFaild(ConstValue.VERSION_NAME, cdnVersionLoader.Error);
                LuaMain.Instance.FuncInvoke(m_LoadingUI, "PlayDefault", m_LoadingUI);
                m_UpdateStepType = UpdateStepType.UpdateCancel;
                yield break;
            }

            // 解析CDN上的版本号
            JsonObject cdnVersionInfoJo = StringParser.StringToJo(cdnVersionLoader.Text, false);
            m_CdnVersion = JsonParser.JoItemToString(cdnVersionInfoJo, ConstValue.VERSION_KEY, ConstValue.VERSION);
            Debugger.Log("Cdn version: " + m_CdnVersion);
            string[] cdnVersions = m_CdnVersion.Split('.');
            int cdnHotVersion = StringParser.StringToInt(cdnVersions[2]);

#if UNITY_WEBGL
			if (string.IsNullOrEmpty(m_Version))
			{
				m_Version = cdnVersions[0] + '.' + cdnVersions[1] + '.' + -1;
			}
#endif
			// 解析本地对比后的版本号
			string[] nativeVersions = m_Version.Split('.');
            int nativeHotVersion = StringParser.StringToInt(nativeVersions[2]);


            // 大版本号对比
            if (!string.Equals(cdnVersions[0], nativeVersions[0]) || 
                !string.Equals(cdnVersions[1], nativeVersions[1]))
            {
                // 如果资源服上的大版本号和本地的不同，则提示需要重新下载安装游戏
                LuaMain.Instance.FuncInvoke(m_LoadingUI, "ShowReDownloadDialog", m_LoadingUI, m_CdnVersion);
                //Application.Quit();
                yield break;
            }

            // 对比CDN上的和本地对比后的版本号
            if (nativeHotVersion >= cdnHotVersion)
            {
                if(m_UpdateIndex == 1)
                {
                    LuaMain.Instance.FuncInvoke(m_LoadingUI, "PlayDefault", m_LoadingUI);
                }
                else
                {
                    LuaMain.Instance.FuncInvoke(m_LoadingUI, "CheckModule", m_LoadingUI);
                }
                // 本地是最新的热更版本，不需要更新
                if (m_AssetPathType == AssetPathType.Streaming)
                {
                    // 如果最新的热更版本来自Streaming，则清空Persistent，清完后删除Version字段
                    // StartCoroutine(ClearPersistent());
                    if (m_PersistentFileList.Count > 0)
                    {
                        foreach (string fileName in m_PersistentFileList.Keys)
                        {
                            FileManager.Delete(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + fileName);
                            yield return null;
                        }
                        FileManager.Delete(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.FILE_LIST_NAME);
                    }
                    if (IsPersistentHasVersion())
                    {
                        FileManager.Delete(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.VERSION_NAME);
                    }
                }
                Debugger.Log("Last version in native.");
                // 跳过更新
                m_UpdateStepType = UpdateStepType.UpdateCancel;
                yield break;
            }
            Debugger.Log("Last version in cdn.");
            m_UpdateStepType = UpdateStepType.VersionCompared;

            if (updateAction != null)
            {
                CoroutineManager.MainThread(updateAction);
            }
        }

        public void VersionUpdate(Action resetAction)
        {
#if DOWNLOAD
            CoroutineManager.Start(DoVersionUpdate(resetAction), this);
#endif
        }

        private IEnumerator DoVersionUpdate(Action callback)
        {
            // 对比文件
            m_UpdateStepType = UpdateStepType.FileCompare;

            string[] streamingVersions = ConstValue.VERSION.Split('.');
            string cdnCurVerUrl = ConstValue.CDN_URL + "/" + ConstValue.GAME_NAME + "/" + ConstValue.BUNDLE_DIR + "/" + streamingVersions[0] + "." + streamingVersions[1];

            // 下载并解析CDN上的文件列表和Manifest
            WWWLoader cdnFileListLoader = WWWManager.Load(cdnCurVerUrl + "/" + m_ModuleName + "/" + ConstValue.FILE_LIST_NAME, this);
            yield return cdnFileListLoader.Wait;
            m_CdnFileList = StringParser.StringToJo(cdnFileListLoader.Text, false);

            // 统计需要下载的和需要删除的
            CountFiles();

            // 下载
            if(m_UpdateIndex > 1)
            {
                LuaTable UIManager = LuaMain.LuaEnv.Global.GetInPath<LuaTable>("LuaClass.UIManager.Instance");
                LuaTable moduleTypeClass = LuaMain.LuaEnv.Global.GetInPath<LuaTable>("LuaClass.ModuleType");
                m_LoadingUI = LuaMain.Instance.FuncInvoke(UIManager, "Open", UIManager, moduleTypeClass.Get<object>("Common"), "LoadingUI") as LuaTable;
            }
            m_UpdateStepType = UpdateStepType.FileDownload;
            for (m_Index = 0; m_Index < UpdateFileNameList.Count; m_Index++)
            {
                string fileName = UpdateFileNameList[m_Index];
                m_CurrentLoader = WWWManager.Load(cdnCurVerUrl + "/" + fileName, this);
                yield return m_CurrentLoader.Wait;

                if (m_CurrentLoader.Error != null)
                {
                    // 下载失败，需要重新更新
                    Debugger.Log("Update failed by: " + m_CurrentLoader.Error);
                    m_UpdateStepType = UpdateStepType.UpdateFailed;
                    OnUpdateFaild(fileName, m_CurrentLoader.Error);
                    yield break;
                }

                byte[] bytes = m_CurrentLoader.Bytes;
                if (bytes != null)
                {
                    FileManager.Write(ConstValue.PERSISTENT_DIR_PATH + "/" + fileName, bytes);
                }
            }

            // 删除
            m_UpdateStepType = UpdateStepType.FileDelete;
            for (m_Index = 0; m_Index < DeleteFileNameList.Count; m_Index++)
            {
                string fileName = DeleteFileNameList[m_Index];
                FileManager.Delete(ConstValue.PERSISTENT_DIR_PATH + "/" + fileName);
                yield return null;
            }

            // 将CDN上的Version文件和Manifest文件写入本地
            m_UpdateStepType = UpdateStepType.VersionWrite;

            // 将Persistent的文件列表替换成Cdn上的文件列表，并写入文件
            m_PersistentFileList = m_CdnFileList;
            SetPersistentFileList();

            // 将Version标记为最新的版本
            string[] cdnVersions = m_CdnVersion.Split('.');
            string[] nativeVersions = m_Version.Split('.');
            m_Version = nativeVersions[0] + "." + nativeVersions[1] + "." + cdnVersions[2];
            SetPersistentVersion(m_Version);

            // 热更完成
            Action action = () =>
            {
                m_UpdateStepType = UpdateStepType.UpdateFinished;
                //优先读Persistent
                m_AssetPathType = AssetPathType.Persistent;
            };
            action += callback;
            CoroutineManager.MainThread(action);
        }

        private IEnumerator ClearPersistent()
        {
            if (m_PersistentFileList.Count > 0)
            {
                foreach (string fileName in m_PersistentFileList.Keys)
                {
                    FileManager.Delete(ConstValue.PERSISTENT_DIR_PATH + "/" + fileName);
                    yield return null;
                }
                FileManager.Delete(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.FILE_LIST_NAME);
            }
            if (IsPersistentHasVersion())
            {
                FileManager.Delete(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.VERSION_NAME);
            }
        }

        private void CountFiles()
        {
            m_UpdateFileNameList = new List<string>();
            m_DeleteFileNameList = new List<string>();

            // 遍历Cdn上的每一个bundle
            foreach (string fileName in m_CdnFileList.Keys)
            {
                JsonObject cdnInfoJo = JsonParser.JoItemToJo(m_CdnFileList, fileName);
                string cdnMd5 = JsonParser.JoItemToString(cdnInfoJo, ConstValue.FILE_LIST_MD5_KEY);
                if (m_PersistentFileList.ContainsKey(fileName))
                {
                    // 如果Persistent存在
                    JsonObject persistentInfoJo = JsonParser.JoItemToJo(m_PersistentFileList, fileName);
                    string persistentMd5 = JsonParser.JoItemToString(persistentInfoJo, ConstValue.FILE_LIST_MD5_KEY);
                    if (!string.Equals(persistentMd5, cdnMd5))
                    {
                        // 如果Persistent与Cdn上的不一样
                        if (m_StreamingFileList.ContainsKey(fileName))
                        {
                            // 如果Streaming存在
                            JsonObject streamingInfoJo = JsonParser.JoItemToJo(m_StreamingFileList, fileName);
                            string streamingMd5 = JsonParser.JoItemToString(streamingInfoJo, ConstValue.FILE_LIST_MD5_KEY);
                            if (string.Equals(streamingMd5, cdnMd5))
                            {
                                // 如果Streaming与Cdn上的一样，则删除Persistent
                                m_DeleteFileNameList.Add(fileName);
                            }
                            else
                            {
                                // 如果Streaming与Cdn上的不一样，则下载覆盖Persistent
                                m_UpdateFileNameList.Add(fileName);
                            }
                        }
                        else
                        {
                            // 如果Streaming不存在，则下载覆盖Persistent
                            m_UpdateFileNameList.Add(fileName);
                        }
                    }
                }
                else
                {
                    // 如果Persistent不存在
                    if (m_StreamingFileList.ContainsKey(fileName))
                    {
                        // 如果Streaming存在
                        JsonObject streamingInfoJo = JsonParser.JoItemToJo(m_StreamingFileList, fileName);
                        string streamingMd5 = JsonParser.JoItemToString(streamingInfoJo, ConstValue.FILE_LIST_MD5_KEY);
                        if (!string.Equals(streamingMd5, cdnMd5))
                        {
                            // 如果Streaming与Cdn上的不一样，则下载到Persistent
                            m_UpdateFileNameList.Add(fileName);
                        }
                    }
                    else
                    {
                        // 如果Streaming不存在，则下载到Persistent
                        m_UpdateFileNameList.Add(fileName);
                    }
                }
            }
            // 遍历Persistent里的每一个bundle
            foreach (string fileName in m_PersistentFileList.Keys)
            {
                if (!m_CdnFileList.ContainsKey(fileName))
                {
                    // 本地存在该文件，但CDN上不存在该文件，则删除
                    m_DeleteFileNameList.Add(fileName);
                }
                else
                {
                    // 双方都存在，在Cdn循环里已经处理过了
                }
            }
        }

		public bool IsStreamingBundleExist(string relativePath)
		{
#if UNITY_EDITOR
			string fullPath = ConstValue.STREAMING_DIR_PATH + "/" + m_ModuleName + "/" + relativePath;
			return FileManager.IsFileExist(fullPath + "." + ConstValue.ASSET_BUNDLE_VARIANT);
#else
			//return m_StreamingFileList.ContainsKey(relativePath);
			return false;
#endif
		}

		private bool IsPersistentHasVersion()
		{
            return FileManager.IsFileExist(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.VERSION_NAME);
		}

		private string GetPersistentVersion(string defaultVersion)
		{
			if (IsPersistentHasVersion())
			{
                byte[] persistentVersionInfoBytes = FileManager.Read(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.VERSION_NAME);
				string persistentVersionInfoStr = persistentVersionInfoBytes == null ? null : ConvertExt.BytesToString(persistentVersionInfoBytes);
				JsonObject persistentVersionInfoJo = StringParser.StringToJo(persistentVersionInfoStr, false);
				return JsonParser.JoItemToString(persistentVersionInfoJo, ConstValue.VERSION_KEY, defaultVersion);
			}
			return defaultVersion;
		}

		private void SetPersistentFileList()
		{
			byte[] persistentFileListBytes = ConvertExt.StringToBytes(m_PersistentFileList.ToString());
            FileManager.Write(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.FILE_LIST_NAME, persistentFileListBytes);
		}

		private void SetPersistentVersion(string version)
		{
			JsonObject persistentVersionInfoJo = new JsonObject();
			persistentVersionInfoJo.Add(ConstValue.VERSION_KEY, version);
			byte[] persistentVersionInfoBytes = ConvertExt.StringToBytes(persistentVersionInfoJo.ToString());
            FileManager.Write(ConstValue.PERSISTENT_DIR_PATH + "/" + m_ModuleName + "/" + ConstValue.VERSION_NAME, persistentVersionInfoBytes);
		}
		
		private void OnUpdateFaild(string fileName, string error)
		{
            LuaMain.Instance.FuncInvoke(m_LoadingUI, "ShowOfflineDialog", m_LoadingUI);
            //UIManager.Instance.OpenUI<DialogUI>(dialogUI =>
            //{
            //    if (dialogUI)
            //    {
            //        if (string.Equals(fileName, ConstValue.VERSION_NAME))
            //        {
            //            if (error.StartsWith("404"))
            //            {
            //                // 找不到版本文件
            //                dialogUI.InitConfirm("VersionUpdateUI", null, "VersionObsolete");
            //                dialogUI.ConfirmAction = Application.Quit;
            //            }
            //            else if(error.EndsWith("unreachable"))
            //            {
            //                // 版本文件，无法连接
            //                dialogUI.InitConfirm("VersionUpdateUI", null, "VersionUnreachable");
            //                dialogUI.ConfirmAction = GameMain.Instance.VersionCompare;
            //            }
            //            else
            //            {
            //                // 版本文件，其他失败原因
            //                dialogUI.InitConfirm("VersionUpdateUI", null, "VersionCompareFailed");
            //                dialogUI.ConfirmAction = GameMain.Instance.VersionCompare;
            //            }
            //        }
            //        else
            //        {
            //            if (error.StartsWith("404"))
            //            {
            //                // 找不到版本文件
            //                dialogUI.InitConfirm("VersionUpdateUI", null, "FileObsolete");
            //                dialogUI.ConfirmAction = Application.Quit;
            //            }
            //            else if (error.EndsWith("unreachable"))
            //            {
            //                // 无法连接
            //                dialogUI.InitConfirm("VersionUpdateUI", null, "FileUnreachable");
            //                dialogUI.ConfirmAction = GameMain.Instance.VersionCompare;
            //            }
            //            else
            //            {
            //                // 默认，更新失败
            //                dialogUI.InitConfirm("VersionUpdateUI", null, "FileUpdateFailed");
            //                dialogUI.ConfirmAction = GameMain.Instance.VersionCompare;
            //            }
            //        }
            //    }
            //});
            //ListenerManager.Trigger<string>(ListenerType.UpdateFailed, error);
		}
		
	}
}
