//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's color with gradient.
/// </summary>

[AddComponentMenu("NGUI/Tween/Tween Gradient Color")]
public class TweenGradientColor : UITweener
{
	public Gradient gradient;

	bool mCached = false;
	UIWidget mWidget;
	Material mMat;
	Light mLight;
	SpriteRenderer mSr;

	void Cache ()
	{
		mCached = true;
		mWidget = GetComponent<UIWidget>();
		if (mWidget != null) return;

		mSr = GetComponent<SpriteRenderer>();
		if (mSr != null) return;

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		Renderer ren = renderer;
#else
		Renderer ren = GetComponent<Renderer>();
#endif
		if (ren != null)
		{
			mMat = ren.material;
			return;
		}

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		mLight = light;
#else
		mLight = GetComponent<Light>();
#endif
		if (mLight == null) mWidget = GetComponentInChildren<UIWidget>();
	}

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Color value
	{
		get
		{
			if (!mCached) Cache();
			if (mWidget != null) return mWidget.color;
			if (mMat != null) return mMat.color;
			if (mSr != null) return mSr.color;
			if (mLight != null) return mLight.color;
			return Color.black;
		}
		set
		{
			if (!mCached) Cache();
			if (mWidget != null) mWidget.color = value;
			else if (mMat != null) mMat.color = value;
			else if (mSr != null) mSr.color = value;
			else if (mLight != null)
			{
				mLight.color = value;
				mLight.enabled = (value.r + value.g + value.b) > 0.01f;
			}
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = gradient.Evaluate(factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenGradientColor Begin (GameObject go, float duration, Gradient gradient)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return null;
#endif
		TweenGradientColor comp = UITweener.Begin<TweenGradientColor>(go, duration);
		comp.gradient = gradient;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = gradient.Evaluate(0); }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = gradient.Evaluate(1); }
}
