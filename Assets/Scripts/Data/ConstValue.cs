using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Main
{
	public class ConstValue : MonoBehaviour
	{
		#region Scripting Define Symbols
		public const string GAME_NAME =
#if TENON_CUBE
				"TenonCube";
#elif TENON_JUMP
				"TenonJump";
#elif TENON_SOLITAIRE
				"TenonSolitaire";
#else
				"";
#endif
		public const bool SDK_SKILLZ =
#if SDK_SKILLZ
				true;
#else
				false;
#endif
		public const bool DEBUG_MACRO =
#if DEBUG_MACRO
				true;
#else
				false;
#endif
		public const bool DOWNLOAD =
#if DOWNLOAD
				true;
#else
				false;
#endif
		public const bool ASSET_LOAD_LOG =
#if ASSET_LOAD_LOG
				true;
#else
				false;
#endif
		public const bool BUNDLE_LOAD_LOG =
#if BUNDLE_LOAD_LOG
				true;
#else
				false;
#endif

        public const bool LOAD_FROM_RES =
#if LOAD_FROM_RES
				true;
#else
                false;
#endif

        public const bool UNITY_EDITOR = 
#if UNITY_EDITOR
                true;
#else
                false;
#endif

		public const bool UNITY_EDITOR_WIN =
#if UNITY_EDITOR_WIN
				true;
#else
				false;
#endif
		public const bool UNITY_EDITOR_OSX =
#if UNITY_EDITOR_OSX
				true;
#else
				false;
#endif
		public const RuntimePlatform PLATFORM =
#if UNITY_STANDALONE_WIN
				RuntimePlatform.WindowsPlayer;
#elif UNITY_STANDALONE_OSX
				RuntimePlatform.OSXPlayer;
#elif UNITY_ANDROID
				RuntimePlatform.Android;
#elif UNITY_IOS
				RuntimePlatform.IPhonePlayer;
#elif UNITY_WEBGL
				RuntimePlatform.WebGLPlayer;
#else
			0;
#endif
#endregion

		// Version
		public const string VERSION = "1.1.4";
		public const string BUNDLE_DIR =
#if UNITY_IOS
				"iOS";
#elif UNITY_ANDROID
				"Android";
#elif UNITY_WEBGL
				"WebGL";
#else
				"Other";
#endif
		public const string VERSION_KEY = "Version";
        public const string MODULE_KEY = "Module";
		public const string VERSION_NAME = "Version.json";
		public const string MANIFEST_NAME = "manifest";
		public const string FILE_LIST_NAME = "FileList.json";
		public const string FILE_LIST_SIZE_KEY = "Size";
		public const string FILE_LIST_MD5_KEY = "Md5";
		public const string ASSET_BUNDLE_VARIANT = "ab";
        [System.NonSerialized]
		public static string CDN_URL = 
#if UNITY_WEBGL
            STREAMING_DIR_PATH;
#else
#if DEBUG_MACRO
                "http://127.0.0.1:80/cdn";
                // "http://47.88.60.106:8090/games";
#else
                "http://127.0.0.1:80/cdn";
                // "http://47.88.60.106:8090/games";
#endif
#endif

        public static string PERSISTENT_DIR_PATH
		{
			get
			{
				return Application.persistentDataPath + "/" + BUNDLE_DIR;
			}
		}
		public static string STREAMING_DIR_PATH
		{
			get
			{
#if !UNITY_EDITOR && UNITY_ANDROID
				return Application.dataPath + "!assets/" + BUNDLE_DIR;
#else
				return Application.streamingAssetsPath + "/" + BUNDLE_DIR;
#endif
			}
		}

		// IO
		public static int BYTE_ARRAY_TEMP_LENGTH { get { return 4096; } }
	}
}