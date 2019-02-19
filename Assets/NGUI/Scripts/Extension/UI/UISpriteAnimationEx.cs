using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/UI/Sprite Animation Ex")]
public class UISpriteAnimationEx : MonoBehaviour
{
	public enum PlayMode
	{
		OnEnable,
		OnAwake,
		OnStart
	}

	[System.Serializable]
	public class Keyframe
	{
		public int frameIndex;
		public string spriteName;
	}

	[HideInInspector][SerializeField] protected int mFrameIndex = 0;
	[HideInInspector][SerializeField] protected int mFPS = 30;
	[HideInInspector][SerializeField] protected bool mLoop = true;
	[HideInInspector][SerializeField] protected bool mSnap = true;
	[HideInInspector][SerializeField] protected bool mAutoPlay = true;
	[HideInInspector][SerializeField] protected bool mRestart = false;
	[HideInInspector][SerializeField] protected PlayMode mAutoPlayMode = PlayMode.OnEnable;
	[HideInInspector][SerializeField] protected List<Keyframe> mKeyframes = new List<Keyframe>();

	protected UISprite mSprite;
	protected float mDelta = 0f;
	protected bool mActive = false;

	public int frameIndex
	{
		get
		{
			return mFrameIndex;
		}
		set
		{
			mFrameIndex = value;
		}
	}

	public int framesPerSecond
	{
		get
		{
			return mFPS;
		}
		set
		{
			mFPS = value;
		}
	}

	public bool snap
	{
		get
		{
			return mSnap;
		}
		set
		{
			mSnap = value;
		}
	}

	public bool loop
	{
		get
		{
			return mLoop;
		}
		set
		{
			mLoop = value;
		}
	}

	public bool isPlaying
	{
		get
		{
			return mActive;
		}
	}

	public List<Keyframe> keyframes
	{
		get
		{
			return mKeyframes;
		}
		set
		{
			mKeyframes = value;
		}
	}

	public UISprite sprite
	{
		get
		{
			return mSprite ?? (mSprite = GetComponent<UISprite>());
		}
	}

	protected virtual void OnEnable()
	{
		if (mAutoPlayMode == PlayMode.OnEnable)
		{
			mActive = mAutoPlay;
			if (mAutoPlay && mRestart)
			{
				mFrameIndex = 0;
			}
		}
	}

	protected virtual void Awake()
	{
		if (mAutoPlayMode == PlayMode.OnAwake)
		{
			mActive = mAutoPlay;
			if (mAutoPlay && mRestart)
			{
				mFrameIndex = 0;
			}
		}
	}

	protected virtual void Start()
	{
		if (mAutoPlayMode == PlayMode.OnStart)
		{
			mActive = mAutoPlay;
			if (mAutoPlay && mRestart)
			{
				mFrameIndex = 0;
			}
		}
	}

	protected virtual void Update ()
	{
		if (mActive && mKeyframes.Count > 1 && Application.isPlaying && mFPS > 0)
		{
			mDelta += Mathf.Min(1f, RealTime.deltaTime);
			float rate = 1f / mFPS;

			while (rate < mDelta)
			{
				mDelta = (rate > 0f) ? mDelta - rate : 0f;
				if (mKeyframes.Count > 0 && mActive)
				{
					Sample();
					frameIndex++;
					if (frameIndex > mKeyframes[mKeyframes.Count - 1].frameIndex)
					{
						frameIndex = 0;
						mActive = mLoop;
					}
				}
			}
		}
	}

	[ContextMenu("Play")]
	public void Play()
	{
		mActive = true;
	}

	[ContextMenu("Pause")]
	public void Pause()
	{
		mActive = false;
	}

	[ContextMenu("ResetToBeginning")]
	public void ResetToBeginning()
	{
		mFrameIndex = 0;
		Sample();
	}

	[ContextMenu("Sample")]
	public void Sample()
	{
		Sample(-1);
	}

	public void Sample(int frameIndex)
	{
		if (mKeyframes.Count > 0)
		{
			int fIndex = frameIndex == -1 ? this.frameIndex : frameIndex;
			sprite.spriteName = GetSpriteName(fIndex);
			if (mSnap)
			{
				sprite.MakePixelPerfect();
			}
		}
	}

	public string GetSpriteName(int frameIndex)
	{
		if (mKeyframes.Count > 0)
		{
			mKeyframes.Sort(SortKeyFrame);
			int index = mKeyframes.Count - 1;
			while (index > 0 && frameIndex < mKeyframes[index].frameIndex)
			{
				index--;
			}
			return mKeyframes[index].spriteName;
		}
		return null;
	}

	private int SortKeyFrame(Keyframe kf1, Keyframe kf2)
	{
		return kf1.frameIndex - kf2.frameIndex;
	}
}
