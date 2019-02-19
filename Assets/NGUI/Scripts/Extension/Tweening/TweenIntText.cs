//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using System;
using UnityEngine;

/// <summary>
/// Tween the label's number text.
/// </summary>

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Tween/Tween Int Text")]
public class TweenIntText : UITweener
{
	public long from = 0;
	public long to = 0;
	public Func<long, string> formatFunc;

	bool mCached = false;
	UILabel mLabel;
	long mNum;

	void Cache()
	{
		mCached = true;
		mLabel = GetComponent<UILabel>();
	}

	[System.Obsolete("Use 'value' instead")]
	public long num { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public long value
	{
		get
		{
			return mNum;
		}
		set
		{
			mNum = value;
			if (!mCached) Cache();
			mLabel.text = formatFunc == null ? mNum.ToString() : formatFunc(mNum);
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate(float factor, bool isFinished)
	{
		double dValue = from + (to - from) * (double) factor;
		value = (long) Math.Round(dValue);
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenIntText Begin (GameObject go, float duration, long num)
	{
		TweenIntText comp = UITweener.Begin<TweenIntText>(go, duration);
		comp.from = comp.value;
		comp.to = num;

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
