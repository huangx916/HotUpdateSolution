using System.Collections;
using UnityEngine;

namespace Main
{
	public class WWWLoader
	{
		private WWW m_WWW;
		private IEnumerator m_WaitIe;
		private string m_Url;

		private WaitUntil m_Wait;
		public WaitUntil Wait
		{
			get
			{
				return m_Wait;
			}
		}

		private bool m_Disposed;
		public bool Disposed
		{
			get
			{
				return m_Disposed;
			}
		}

		public WWWLoader(string url)
		{
			m_Url = url;
			m_WWW = new WWW(ConvertExt.UriEncode(url));
			m_Wait = new WaitUntil(() => IsDone);
			m_WaitIe = DoWait();
			CoroutineManager.Start(m_WaitIe);
		}

		private IEnumerator DoWait()
		{
			yield return m_WWW;
			if (!string.IsNullOrEmpty(Error))
			{
				Debugger.LogError("WWW load [" + Url + "] error: " + Error);
			}
		}

		public void Dispose()
		{
			m_WWW.Dispose();
			CoroutineManager.Stop(m_WaitIe);
			m_Disposed = true;
		}

		public string Url
		{
			get
			{
				return m_Url;
			}
		}

		public string Error
		{
			get
			{
				return m_WWW.error;
			}
		}

		public bool IsDone
		{
			get
			{
				return m_WWW.isDone;
			}
		}

		public float Progress
		{
			get
			{
				return m_WWW.progress;
			}
		}

		public int Size
		{
			get
			{
				return m_WWW.size;
			}
		}

		public byte[] Bytes
		{
			get
			{
				if (!string.IsNullOrEmpty(m_WWW.error))
				{
					return null;
				}
				return m_WWW.bytes;
			}
		}

		private AssetBundle m_AssetBundle;
		public AssetBundle AssetBundle
		{
			get
			{
				if (!string.IsNullOrEmpty(m_WWW.error))
				{
					return null;
				}
				if (m_AssetBundle == null)
				{
					m_AssetBundle = m_WWW.assetBundle;
				}
				return m_AssetBundle;
			}
		}

		private string m_Text;
		public string Text
		{
			get
			{
				if (!string.IsNullOrEmpty(m_WWW.error))
				{
					return null;
				}
				if (m_Text == null)
				{
					m_Text = m_WWW.text;
				}
				return m_Text;
			}
		}

		private Texture2D m_Texture;
		public Texture2D Texture
		{
			get
			{
				if (!string.IsNullOrEmpty(m_WWW.error))
				{
					return null;
				}
				if (m_Texture == null)
				{
					m_Texture = m_WWW.texture;
				}
				return m_Texture;
			}
		}
	}
}