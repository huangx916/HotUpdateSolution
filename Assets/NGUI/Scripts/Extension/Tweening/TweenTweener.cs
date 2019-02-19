//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the tweener's factor and value.
/// </summary>

[RequireComponent(typeof(UITweener))]
[AddComponentMenu("NGUI/Tween/Tween Tweener")]
public class TweenTweener : UITweener
{
	[Range(0f, 1f)] public float from = 1f;
	[Range(0f, 1f)] public float to = 1f;

	bool mCached = false;
	UITweener mTweener;

	void Cache()
	{
		mCached = true;

		UITweener[] tweeners = GetComponents<UITweener>();
		foreach (UITweener tweener in tweeners)
		{
			if (tweener != this)
			{
				mTweener = tweener;
			}
		}
	}

	[System.Obsolete("Use 'value' instead")]
	public float Factor { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public float value
	{
		get
		{
			if (!mCached) Cache();
			return mTweener ? mTweener.tweenFactor : 0;
		}
		set
		{
			if (!mCached) Cache();
			if (mTweener)
			{
				mTweener.tweenFactor = value;
				mTweener.Sample(value, true);
			}
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenTweener Begin (GameObject go, float duration, float tweener)
	{
		TweenTweener comp = UITweener.Begin<TweenTweener>(go, duration);
		comp.from = comp.value;
		comp.to = tweener;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = to; }
}
