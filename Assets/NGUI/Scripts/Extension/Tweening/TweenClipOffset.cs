//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the panel's clip offset.
/// </summary>

[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Tween/Tween Clip Offset")]
public class TweenClipOffset : UITweener
{
	public Vector2 from;
	public Vector2 to;

	bool mCached = false;
	UIPanel mPanel;

	void Cache()
	{
		mCached = true;
		mPanel = GetComponent<UIPanel>();
	}

	[System.Obsolete("Use 'value' instead")]
	public Vector2 clipOffset { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Vector2 value
	{
		get
		{
			if (!mCached) Cache();
			return mPanel.clipOffset;
		}
		set
		{
			if (!mCached) Cache();
			mPanel.clipOffset = value;
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Vector2.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenClipOffset Begin (GameObject go, float duration, Vector2 clipOffset)
	{
		TweenClipOffset comp = UITweener.Begin<TweenClipOffset>(go, duration);
		comp.from = comp.value;
		comp.to = clipOffset;

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
