//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the material's color.
/// </summary>

[AddComponentMenu("NGUI/Tween/Tween Material Color")]
[RequireComponent(typeof(Renderer))]
public class TweenMaterialColor : UITweener
{
	public Color from = Color.white;
	public Color to = Color.white;
	public string colorName;

	bool mCached = false;
	Renderer ren;
	Material mMat;

	void Cache ()
	{
		mCached = true;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		renderer = renderer;
#else
		ren = GetComponent<Renderer>();
#endif
		if (ren != null)
		{
			mMat = ren.material;
			return;
		}
	}

	[System.Obsolete("Use 'value' instead")]
	public Color color { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Color value
	{
		get
		{
			if (!Cached) Cache();
			if (mMat != null && mMat.HasProperty(colorName))
			{
				return mMat.GetColor(colorName);
			}
			return Color.black;
		}
		set
		{
			if (!Cached) Cache();
			if (mMat != null && mMat.HasProperty(colorName))
			{
				mMat.SetColor(colorName, value);
			}
		}
	}

	private bool Cached
	{
		get
		{
			if (!mCached)
			{
				return false;
			}
			if (ren == null)
			{
				return false;
			}
			if (mMat != ren.sharedMaterial)
			{
				return false;
			}
			return true;
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Color.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenMaterialColor Begin (GameObject go, float duration, Color color)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return null;
#endif
		TweenMaterialColor comp = UITweener.Begin<TweenMaterialColor>(go, duration);
		comp.from = comp.value;
		comp.to = color;

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
