//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the progress-bar's value.
/// </summary>

[RequireComponent(typeof(UIProgressBar))]
[AddComponentMenu("NGUI/Tween/Tween Progress")]
public class TweenProgress : UITweener
{
	[Range(0f, 1f)] public float from = 1f;
	[Range(0f, 1f)] public float to = 1f;

	bool mCached = false;
	UIProgressBar mBar;

	void Cache()
	{
		mCached = true;
		mBar = GetComponent<UIProgressBar>();
	}

	[System.Obsolete("Use 'value' instead")]
	public float progress { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public float value
	{
		get
		{
			if (!mCached) Cache();
			return mBar.value;
		}
		set
		{
			if (!mCached) Cache();
			mBar.value = value;
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenProgress Begin (GameObject go, float duration, float progress)
	{
		TweenProgress comp = UITweener.Begin<TweenProgress>(go, duration);
		comp.from = comp.value;
		comp.to = progress;

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
