using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderCache : MonoBehaviour {

	private static Dictionary<string, Shader> mCache = new Dictionary<string, Shader>();

	public static void Add(string name, Shader shader)
	{
		mCache[name] = shader;
	}

	public static Shader Find(string name)
	{
		if (mCache.ContainsKey(name))
		{
			return mCache[name];
		}
		return Shader.Find(name);
	}

	public static void Clear()
	{
		mCache.Clear();
	}
}
