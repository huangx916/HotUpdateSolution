//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the panel's clip region.
/// </summary>

[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Tween/Tween Clip Region")]
public class TweenClipRegion : UITweener
{
	public Vector4 from;
	public Vector4 to;

	bool mCached = false;
	UIPanel mPanel;

	void Cache()
	{
		mCached = true;
		mPanel = GetComponent<UIPanel>();
	}

	[System.Obsolete("Use 'value' instead")]
	public Vector4 clipRegion { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Vector4 value
	{
		get
		{
			if (!mCached) Cache();
			return mPanel.baseClipRegion;
		}
		set
		{
			if (!mCached) Cache();
			mPanel.baseClipRegion = value;
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Vector4.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenClipRegion Begin (GameObject go, float duration, Vector4 clipRegion)
	{
		TweenClipRegion comp = UITweener.Begin<TweenClipRegion>(go, duration);
		comp.from = comp.value;
		comp.to = clipRegion;

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
