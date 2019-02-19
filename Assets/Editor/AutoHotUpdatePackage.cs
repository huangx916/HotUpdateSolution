using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using UnityEditor;
using SimpleJson;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AutoHotUpdatePackage : EditorWindow
{
    [System.NonSerialized]
    string m_CdnUrl = "http://47.88.60.106:8090/games";
    //string m_CdnUrl = "http://127.0.0.1:80/cdn";

    public JsonObject m_CdnFileList;
    public JsonObject m_StreamingFileList;

    string m_CdnNativePath = "";
    string m_PackagePath = "";

    string m_CdnVersion;
    string m_streamingVersion;

    string m_ModuleName = "";

    private UnityEngine.Object folderObj = null;

    public void OnGUI()
    {
        

        if (GUILayout.Button("Init"))
        {
            OnInit();
        }
        if (GUILayout.Button("Clear NativeCdn & Package Directory"))
        {
            OnClear();
        }

        DrawFolderPopup();

        if(GUILayout.Button("Get Cur FileList & Version From Cdn"))
        {
            OnGetFromCdn(m_ModuleName);
        }
        if (GUILayout.Button("ExportChange"))
        {
            OnExportChange(m_ModuleName);
        }
    }

    private int selectedIndex = -1;
    private void DrawFolderPopup()
    {
        Color contentColor = GUI.backgroundColor;
        GUI.backgroundColor = selectedIndex == -1 ? new Color(1, 0.5F, 0.5F) : new Color(0.5F, 1, 0.5F);
        DirectoryInfo directory = new DirectoryInfo(ConstValue.STREAMING_DIR_PATH);
        if (directory.Exists)
        {
            DirectoryInfo[] list = directory.GetDirectories();
            List<string> listName = new List<string>();
            foreach (var dir in list)
            {
                listName.Add(dir.Name);
            }
            selectedIndex = selectedIndex >= listName.Count ? -1 : selectedIndex;
            selectedIndex = EditorGUILayout.Popup("Folder Name", selectedIndex, listName.ToArray());
            if (selectedIndex == -1)
            {
                m_ModuleName = "";
            }
            else
            {
                //SetDir(false, listName[selectedIndex]);
                m_ModuleName = listName[selectedIndex];
            }
        }
        GUI.backgroundColor = contentColor;
    }

    void OnInit()
    {
        string[] streamingVersions = ConstValue.VERSION.Split('.');
        m_CdnNativePath = Application.streamingAssetsPath
                + "/" + "_CdnVersion"
                + "/" + ConstValue.BUNDLE_DIR
                + "/" + streamingVersions[0] + "." + streamingVersions[1];
        m_PackagePath = Application.streamingAssetsPath
                + "/" + "_HotUpdatePackage"
                + "/" + ConstValue.BUNDLE_DIR
                + "/" + streamingVersions[0] + "." + streamingVersions[1];
    }

    void OnClear()
    {
        DirectoryInfo cdnDirInfo = new DirectoryInfo(m_CdnNativePath);
        if(cdnDirInfo.Exists)
        {
            cdnDirInfo.Delete(true);
        }
        DirectoryInfo packageDirInfo = new DirectoryInfo(m_PackagePath);
        if (packageDirInfo.Exists)
        {
            packageDirInfo.Delete(true);
        }
        AssetDatabase.Refresh();
    }

    void OnGetFromCdn(string moduleName)
    {
        ConstValue.CDN_URL = m_CdnUrl;
        EditorDelayedCallManager.Instance.StartCoroutine(DoGetFromCdn(moduleName, ConstValue.VERSION_NAME), this);
        EditorDelayedCallManager.Instance.StartCoroutine(DoGetFromCdn(moduleName, ConstValue.FILE_LIST_NAME), this);
    }

    IEnumerator DoGetFromCdn(string moduleName, string fileName)
    {
        // 下载CDN服务器上的版本信息
        string[] streamingVersions = ConstValue.VERSION.Split('.');
        string cdnCurVerUrl = ConstValue.CDN_URL + "/" + ConstValue.GAME_NAME + "/" + ConstValue.BUNDLE_DIR + "/" + streamingVersions[0] + "." + streamingVersions[1];
        WWW cdnVersionLoader = new WWW(cdnCurVerUrl + "/" + moduleName + "/" + fileName);
        yield return cdnVersionLoader;
        if (cdnVersionLoader.error != null)
        {
            // 下载失败，需要整包更新或其他原因
            Debug.LogError("Update failed by: " + cdnVersionLoader.error);
            yield break;
        }
        byte[] bytes = cdnVersionLoader.bytes;
        if (bytes != null)
        {
            string destPath = m_CdnNativePath + "/" + moduleName + "/" + fileName;
            FileManager.Write(destPath, bytes);
        }

        AssetDatabase.Refresh();
    }

    void OnExportChange(string moduleName)
    {
        if(!ExportVersion(moduleName))
        {
            ShowNotification(new GUIContent("Version File's Version Need Add"));
            AssetDatabase.Refresh();
            return;
        }
        if(!ExportFileList(moduleName))
        {
            ShowNotification(new GUIContent("FileList Not Changed"));
            AssetDatabase.Refresh();
            return;
        }

        ExportAssetBundles(moduleName);

        AssetDatabase.Refresh();
    }

    bool ExportVersion(string moduleName)
    {
        int streamingHotVersion = 0;
        int cdnHotVersion = 0;

        if (FileManager.IsFileExist(ConstValue.STREAMING_DIR_PATH + "/" + moduleName + "/" + ConstValue.VERSION_NAME))
        {
            byte[] streamingVersionBytes = FileManager.Read(ConstValue.STREAMING_DIR_PATH + "/" + moduleName + "/" + ConstValue.VERSION_NAME);
            string streamingVersionStr = streamingVersionBytes == null ? null : ConvertExt.BytesToString(streamingVersionBytes);
            //Debug.LogError("streamingVersionStr = " + streamingVersionStr);

            JsonObject streamingVersionInfoJo = StringParser.StringToJo(streamingVersionStr, false);
            m_streamingVersion = JsonParser.JoItemToString(streamingVersionInfoJo, ConstValue.VERSION_KEY, ConstValue.VERSION);
            //Debug.LogError("streaming version: " + m_streamingVersion);
            string[] streamingVersions = m_streamingVersion.Split('.');
            streamingHotVersion = StringParser.StringToInt(streamingVersions[2]);
        }
        else
        {
            return false;
        }

        if (FileManager.IsFileExist(m_CdnNativePath + "/" + moduleName + "/" + ConstValue.VERSION_NAME))
        {
            byte[] cdnVersionBytes = FileManager.Read(m_CdnNativePath + "/" + moduleName + "/" + ConstValue.VERSION_NAME);
            string cdnVersionStr = cdnVersionBytes == null ? null : ConvertExt.BytesToString(cdnVersionBytes);
            //Debug.LogError("cdnVersionStr = " + cdnVersionStr);

            JsonObject cdnVersionInfoJo = StringParser.StringToJo(cdnVersionStr, false);
            m_CdnVersion = JsonParser.JoItemToString(cdnVersionInfoJo, ConstValue.VERSION_KEY, ConstValue.VERSION);
            //Debug.LogError("Cdn version: " + m_CdnVersion);
            string[] cdnVersions = m_CdnVersion.Split('.');
            cdnHotVersion = StringParser.StringToInt(cdnVersions[2]);
        }
        else
        {
            return false;
        }

        if (streamingHotVersion <= cdnHotVersion)
        {
            return false;
        }

        FileCopyTo(ConstValue.STREAMING_DIR_PATH + "/" + moduleName + "/" + ConstValue.VERSION_NAME,
            m_PackagePath + "/" + moduleName + "/" + ConstValue.VERSION_NAME);

        return true;
    }

    bool ExportFileList(string moduleName)
    {
        string streamingFileListMd5 = GetMd5(ConstValue.STREAMING_DIR_PATH + "/" + moduleName + "/" + ConstValue.FILE_LIST_NAME);
        string cdnFileListMd5 = GetMd5(m_CdnNativePath + "/" + moduleName + "/" + ConstValue.FILE_LIST_NAME);
        if(string.Equals(streamingFileListMd5, cdnFileListMd5))
        {
            return false;
        }

        FileCopyTo(ConstValue.STREAMING_DIR_PATH + "/" + moduleName + "/" + ConstValue.FILE_LIST_NAME,
            m_PackagePath + "/" + moduleName + "/" + ConstValue.FILE_LIST_NAME);

        return true;
    }

    void ExportAssetBundles(string moduleName)
    {
        if (FileManager.IsFileExist(ConstValue.STREAMING_DIR_PATH + "/" + moduleName + "/" + ConstValue.FILE_LIST_NAME))
        {
            byte[] streamingFileListBytes = FileManager.Read(ConstValue.STREAMING_DIR_PATH + "/" + moduleName + "/" + ConstValue.FILE_LIST_NAME);
            string streamingFileListStr = streamingFileListBytes == null ? null : ConvertExt.BytesToString(streamingFileListBytes);
            m_StreamingFileList = StringParser.StringToJo(streamingFileListStr, false);
        }
        else
        {
            m_StreamingFileList = new JsonObject();
        }

        if (FileManager.IsFileExist(m_CdnNativePath + "/" + moduleName + "/" + ConstValue.FILE_LIST_NAME))
        {
            byte[] cdnFileListBytes = FileManager.Read(m_CdnNativePath + "/" + moduleName + "/" + ConstValue.FILE_LIST_NAME);
            string cdnFileListStr = cdnFileListBytes == null ? null : ConvertExt.BytesToString(cdnFileListBytes);
            m_CdnFileList = StringParser.StringToJo(cdnFileListStr, false);
        }
        else
        {
            m_CdnFileList = new JsonObject();
        }

        foreach (string fileName in m_StreamingFileList.Keys)
        {
            string srcPath = ConstValue.STREAMING_DIR_PATH + "/" + fileName;
            string destPath = m_PackagePath + "/" + fileName;
            JsonObject streamingInfoJo = JsonParser.JoItemToJo(m_StreamingFileList, fileName);
            string streamingMd5 = JsonParser.JoItemToString(streamingInfoJo, ConstValue.FILE_LIST_MD5_KEY);
            if (m_CdnFileList.ContainsKey(fileName))
            {
                JsonObject cdnInfoJo = JsonParser.JoItemToJo(m_CdnFileList, fileName);
                string cdnMd5 = JsonParser.JoItemToString(cdnInfoJo, ConstValue.FILE_LIST_MD5_KEY);
                if(!string.Equals(streamingMd5, cdnMd5))
                {
                    FileCopyTo(srcPath, destPath);
                }
            }
            else 
            {
                FileCopyTo(srcPath, destPath);
            }
        }
        AssetDatabase.Refresh();
    }

    string GetMd5(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        FileStream fileStream = fileInfo.Open(FileMode.Open);
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] bytes = md5.ComputeHash(fileStream);
        fileStream.Close();
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x2"));
        }
        return sb.ToString();
    }

    void FileCopyTo(string srcPath, string destPath)
    {
        FileInfo srcFileInfo = new FileInfo(srcPath);
        FileInfo destFileInfo = new FileInfo(destPath);
        if(!destFileInfo.Directory.Exists)
        {
            destFileInfo.Directory.Create();
        }
        if(destFileInfo.Exists)
        {
            destFileInfo.Delete();
        }
        srcFileInfo.CopyTo(destPath);
    }





















    [MenuItem("Window/HotUpdateFile Package Auto")]
    private static void Window()
    {
        AutoHotUpdatePackage window = GetWindow(typeof(AutoHotUpdatePackage), false, "HotUpdateFile Package Auto") as AutoHotUpdatePackage;
        window.minSize = new Vector2(200, 320);
        window.ShowTab();
    }
}
