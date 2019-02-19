//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the texture's border.
/// </summary>

[RequireComponent(typeof(UITexture))]
[AddComponentMenu("NGUI/Tween/Tween Border")]
public class TweenBorder : UITweener
{
	public Vector4 from = Vector4.zero;
	public Vector4 to = Vector4.zero;

	bool mCached = false;
	UITexture mTexture;

	void Cache()
	{
		mCached = true;
		mTexture = GetComponent<UITexture>();
	}

	[System.Obsolete("Use 'value' instead")]
	public Vector4 border { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Vector4 value
	{
		get
		{
			if (!mCached) Cache();
			return mTexture.border;
		}
		set
		{
			if (!mCached) Cache();
			mTexture.border = value;
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Vector4.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenBorder Begin (GameObject go, float duration, Vector4 border)
	{
		TweenBorder comp = UITweener.Begin<TweenBorder>(go, duration);
		comp.from = comp.value;
		comp.to = border;

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
