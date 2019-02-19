using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
	public class WWWManager : MonoBehaviour
	{
		private static Dictionary<string, WWWLoader> s_LoaderDict = new Dictionary<string, WWWLoader>();

		private static WWWLoader GetLoader(string url)
		{
			if (!s_LoaderDict.ContainsKey(url))
			{
				Debugger.Log("Load: " + url);
				s_LoaderDict[url] = new WWWLoader(url);
			}
			return s_LoaderDict[url];
		}

		public static WWWLoader Load(string url, MonoBehaviour behaviour = null)
		{
			WWWLoader loader = GetLoader(url);
			CoroutineManager.Wait(loader.Wait, () => s_LoaderDict.Remove(url), behaviour);
			return loader;
		}

		public static WaitUntil Load(string url, Action<WWWLoader> callback, MonoBehaviour behaviour = null)
		{
			WWWLoader loader = GetLoader(url);
			CoroutineManager.Wait(loader.Wait, () =>
			{
				if (callback != null)
				{
					callback(loader);
				}
				s_LoaderDict.Remove(url);
			}, behaviour);
			return loader.Wait;
		}

		public static WaitUntil LoadBytes(string url, Action<byte[]> callback, MonoBehaviour behaviour = null)
		{
			return Load(url, loader =>
			{
				if (callback != null)
				{
					callback(loader.Bytes);
				}
			}, behaviour);
		}

		public static WaitUntil LoadBundle(string url, Action<AssetBundle> callback, MonoBehaviour behaviour = null)
		{
			return Load(url, loader =>
			{
				if (callback != null)
				{
					callback(loader.AssetBundle);
				}
			}, behaviour);
		}

		public static WaitUntil LoadText(string url, Action<string> callback, MonoBehaviour behaviour = null)
		{
			return Load(url, loader =>
			{
				if (callback != null)
				{
					callback(loader.Text);
				}
			}, behaviour);
		}

		public static WaitUntil LoadTexture(string url, Action<Texture2D> callback, MonoBehaviour behaviour = null)
		{
			return Load(url, loader =>
			{
				if (callback != null)
				{
					callback(loader.Texture);
				}
			}, behaviour);
		}
	}
}