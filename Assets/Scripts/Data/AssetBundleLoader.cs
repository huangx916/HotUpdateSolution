using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
	public abstract class AssetBundleLoader : AsyncOperation
	{
		protected string m_Path;
		protected int m_PointerCount;
		protected bool m_Unloaded;

		public abstract void Load();

		protected List<string> m_DependencyList = new List<string>();
		public void AddDependency(string loader)
		{
			m_DependencyList.Add(loader);
		}
		public void ForEachDependency(Action<string> action)
		{
			m_DependencyList.ForEach(action);
		}
		public void ClearDependencies()
		{
			m_DependencyList.Clear();
		}

		public abstract WaitUntil Wait
		{
			 get;
		}

		public string Path
		{
			get
			{
				return m_Path;
			}
		}

		public int PointerCount
		{
			get
			{
				return m_PointerCount;
			}
			set
			{
				m_PointerCount = value;
			}
		}

		public abstract AssetBundle AssetBundle
		{
			get;
		}

		public abstract bool IsDone
		{
			get;
		}

		public abstract float Progress
		{
			get;
		}

		public void Unload()
		{
			if (AssetBundle)
			{
#if BUNDLE_LOAD_LOG
				Debugger.Log("Unload assetBundle: " + m_Path);
#endif
				AssetBundle.Unload(false);
			}
			IsUnloaded = true;
		}

		public bool IsUnloaded
		{
			get
			{
				return m_Unloaded;
			}
			private set
			{
				m_Unloaded = value;
			}
		}
	}

	public class AssetBundleLoaderSync : AssetBundleLoader
	{
		private AssetBundle m_AssetBundle;

		public AssetBundleLoaderSync(string path)
		{
			m_Path = path;
		}

		public override void Load()
		{
			if (!IsUnloaded)
			{
				m_AssetBundle = AssetBundle.LoadFromFile(m_Path);
				if (m_AssetBundle)
				{
#if BUNDLE_LOAD_LOG
					Debugger.Log("<b>AssetBundle loaded:<b> " + m_Path);
#endif
				}
				else
				{
					Debugger.LogError("<b>AssetBundl.Load failed:<b> " + m_Path);
				}
			}
		}

		public override WaitUntil Wait
		{
			get
			{
				return null;
			}
		}

		public override AssetBundle AssetBundle
		{
			get
			{
				return m_AssetBundle;
			}
		}

		public override bool IsDone
		{
			get
			{
				return IsUnloaded || m_AssetBundle;
			}
		}

		public override float Progress
		{
			get
			{
				return IsDone ? 1 : 0;
			}
		}
	}

	public class AssetBundleLoaderAsync : AssetBundleLoader
	{
		private AssetBundleCreateRequest m_Request;

		public AssetBundleLoaderAsync(string path)
		{
			m_Path = path;
			m_Wait = new WaitUntil(() => m_Request != null && m_Request.isDone);
		}

		public override void Load()
		{
			if (!IsUnloaded)
			{
				m_Request = AssetBundle.LoadFromFileAsync(m_Path);
				CoroutineManager.Delay(m_Request, () =>
				{
					if (IsDone)
					{
#if BUNDLE_LOAD_LOG
						Debugger.Log("<b>AssetBundle async loaded:<b> " + m_Path);
#endif
					}
					else
					{
						Debugger.LogError("<b>AssetBundl.Load async failed:<b> " + m_Path);
					}
					if (IsUnloaded)
					{
						if (AssetBundle != null)
						{
							AssetBundle.Unload(false);
						}
					}
				});
			}
		}

		private WaitUntil m_Wait;
		public override WaitUntil Wait
		{
			get
			{
				return m_Wait;
			}
		}

		public override AssetBundle AssetBundle
		{
			get
			{
				if (m_Request == null)
				{
					return null;
				}
				return m_Request.assetBundle;
			}
		}

		public override bool IsDone
		{
			get
			{
				if (m_Request == null)
				{
					return false;
				}
				return m_Request.isDone;
			}
		}

		public override float Progress
		{
			get
			{
				if (m_Request == null)
				{
					return 0;
				}
				return m_Request.progress;
			}
		}
	}
}