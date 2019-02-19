using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Functionality common to both NGUI and 2D sprites brought out into a single common parent.
/// Mostly contains everything related to drawing the sprite.
/// </summary>

public abstract class UIBasicSprite : UIWidget
{
	public enum Type
	{
		Simple,
		Sliced,
		Tiled,
		Filled,
		Advanced,
	}

	public enum FillDirection
	{
		Nothing,
		Horizontal,
		Vertical,
		Radial90,
		Radial180,
		Radial360,
	}

	public enum AdvancedType
	{
		Invisible,
		Sliced,
		Tiled,
	}

	public enum Flip
	{
		Nothing,
		Horizontally,
		Vertically,
		Both,
	}

	[HideInInspector][SerializeField] protected Type mType = Type.Simple;
	[HideInInspector][SerializeField] protected FillDirection mFillDirection = FillDirection.Radial360;
	[Range(0f, 1f)]
	[HideInInspector][SerializeField] protected float mFillAmount = 1f;
	[HideInInspector][SerializeField] protected bool mInvert = false;
	[HideInInspector][SerializeField] protected Flip mFlip = Flip.Nothing;
	[HideInInspector][SerializeField] protected bool mApplyGradient = false;
	[HideInInspector][SerializeField] protected Color mGradientTop = Color.white;
	[HideInInspector][SerializeField] protected Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);
	[HideInInspector][SerializeField] protected Flip mMirror = Flip.Nothing;
	[HideInInspector][SerializeField] protected bool mFillBorder = true;
	[HideInInspector][SerializeField] protected bool mTileBorder = false;

	// Cached to avoid allocations
	[System.NonSerialized] Rect mInnerUV = new Rect();
	[System.NonSerialized] Rect mOuterUV = new Rect();

	/// <summary>
	/// When the sprite type is advanced, this determines whether the center is tiled or sliced.
	/// </summary>

	public AdvancedType centerType = AdvancedType.Sliced;

	/// <summary>
	/// When the sprite type is advanced, this determines whether the left edge is tiled or sliced.
	/// </summary>

	public AdvancedType leftType = AdvancedType.Sliced;

	/// <summary>
	/// When the sprite type is advanced, this determines whether the right edge is tiled or sliced.
	/// </summary>

	public AdvancedType rightType = AdvancedType.Sliced;

	/// <summary>
	/// When the sprite type is advanced, this determines whether the bottom edge is tiled or sliced.
	/// </summary>

	public AdvancedType bottomType = AdvancedType.Sliced;

	/// <summary>
	/// When the sprite type is advanced, this determines whether the top edge is tiled or sliced.
	/// </summary>

	public AdvancedType topType = AdvancedType.Sliced;

	/// <summary>
	/// How the sprite is drawn. It's virtual for legacy reasons (UISlicedSprite, UITiledSprite, UIFilledSprite).
	/// </summary>

	public virtual Type type
	{
		get
		{
			return mType;
		}
		set
		{
			if (mType != value)
			{
				mType = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Sprite flip setting.
	/// </summary>

	public Flip flip
	{
		get
		{
			return mFlip;
		}
		set
		{
			if (mFlip != value)
			{
				mFlip = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Sprite flip setting.
	/// </summary>

	public Flip mirror
	{
		get
		{
			return mMirror;
		}
		set
		{
			if (mMirror != value)
			{
				mMirror = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Direction of the cut procedure.
	/// </summary>

	public FillDirection fillDirection
	{
		get
		{
			return mFillDirection;
		}
		set
		{
			if (mFillDirection != value)
			{
				mFillDirection = value;
				mChanged = true;
			}
		}
	}

	/// <summary>
	/// Amount of the sprite shown. 0-1 range with 0 being nothing shown, and 1 being the full sprite.
	/// </summary>

	public float fillAmount
	{
		get
		{
			return mFillAmount;
		}
		set
		{
			float val = Mathf.Clamp01(value);

			if (mFillAmount != val)
			{
				mFillAmount = val;
				mChanged = true;
			}
		}
	}

	/// <summary>
	/// Minimum allowed width for this widget.
	/// </summary>

	override public int minWidth
	{
		get
		{
			if (type == Type.Sliced)
			{
				Vector4 b = border * pixelSize;
				int min;
				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
				{
					if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
						min = Mathf.RoundToInt(b.z + b.z);
					else
						min = Mathf.RoundToInt(b.x + b.x);
				}
				else
				{
					min = Mathf.RoundToInt(b.x + b.z);
				}
				return Mathf.Max(base.minWidth, ((min & 1) == 1) ? min + 1 : min);
			}
			else if (type == Type.Advanced)
			{
				Vector4 b = border * pixelSize;
				int min = Mathf.RoundToInt(b.x + b.z);
				return Mathf.Max(base.minWidth, ((min & 1) == 1) ? min + 1 : min);
			}
			return base.minWidth;
		}
	}

	/// <summary>
	/// Minimum allowed height for this widget.
	/// </summary>

	override public int minHeight
	{
		get
		{
			if (type == Type.Sliced)
			{
				Vector4 b = border * pixelSize;
				int min;
				if (mMirror == Flip.Vertically || mMirror == Flip.Both)
				{
					if (mFlip == Flip.Vertically || mFlip == Flip.Both)
						min = Mathf.RoundToInt(b.w + b.w);
					else
						min = Mathf.RoundToInt(b.y + b.y);
				}
				else
				{
					min = Mathf.RoundToInt(b.y + b.w);
				}
				return Mathf.Max(base.minHeight, ((min & 1) == 1) ? min + 1 : min);
			}
			else if (type == Type.Advanced)
			{
				Vector4 b = border * pixelSize;
				int min = Mathf.RoundToInt(b.y + b.w);
				return Mathf.Max(base.minHeight, ((min & 1) == 1) ? min + 1 : min);
			}
			return base.minHeight;
		}
	}

	/// <summary>
	/// Whether the sprite should be filled in the opposite direction.
	/// </summary>

	public bool invert
	{
		get
		{
			return mInvert;
		}
		set
		{
			if (mInvert != value)
			{
				mInvert = value;
				mChanged = true;
			}
		}
	}

	/// <summary>
	/// Whether the sprite should be filled 8 block on the edge for 9-slicing.
	/// </summary>

	public bool fillBorder
	{
		get
		{
			return mFillBorder;
		}
		set
		{
			if (mFillBorder != value)
			{
				mFillBorder = value;
				mChanged = true;
			}
		}
	}

	/// <summary>
	/// Whether the widget has a border for 9-slicing.
	/// </summary>

	public bool hasBorder
	{
		get
		{
			Vector4 br = border;
			return (br.x != 0f || br.y != 0f || br.z != 0f || br.w != 0f);
		}
	}

	/// <summary>
	/// Whether the sprite's material is using a pre-multiplied alpha shader.
	/// </summary>

	public virtual bool premultipliedAlpha { get { return false; } }

	/// <summary>
	/// Size of the pixel. Overwritten in the NGUI sprite to pull a value from the atlas.
	/// </summary>

	public virtual float pixelSize { get { return 1f; } }

	/// <summary>
	/// return a border flipped by mFlip;
	/// </summary>
	/// <param name="border"></param>
	/// <returns></returns>
	public Vector4 FlipBorder(Vector4 border)
	{
		switch (mFlip)
		{
			case Flip.Horizontally: return new Vector4(border.z, border.y, border.x, border.w);
			case Flip.Vertically: return new Vector4(border.x, border.w, border.z, border.y);
			case Flip.Both: return new Vector4(border.z, border.w, border.x, border.y);
			default: return border;
		}
	}

#if UNITY_EDITOR
	/// <summary>
	/// Keep sane values.
	/// </summary>

	protected override void OnValidate ()
	{
		base.OnValidate();
		mFillAmount = Mathf.Clamp01(mFillAmount);
	}
#endif

#region Fill Functions
	// Static variables to reduce garbage collection
	static protected Vector2[] mTempPos = new Vector2[5];
	static protected Vector2[] mTempUVs = new Vector2[5];

	/// <summary>
	/// Convenience function that returns the drawn UVs after flipping gets considered.
	/// X = left, Y = bottom, Z = right, W = top.
	/// </summary>

	Vector4 drawingUVs
	{
		get
		{
			switch (mFlip)
			{
				case Flip.Horizontally: return new Vector4(mOuterUV.xMax, mOuterUV.yMin, mOuterUV.xMin, mOuterUV.yMax);
				case Flip.Vertically: return new Vector4(mOuterUV.xMin, mOuterUV.yMax, mOuterUV.xMax, mOuterUV.yMin);
				case Flip.Both: return new Vector4(mOuterUV.xMax, mOuterUV.yMax, mOuterUV.xMin, mOuterUV.yMin);
				default: return new Vector4(mOuterUV.xMin, mOuterUV.yMin, mOuterUV.xMax, mOuterUV.yMax);
			}
		}
	}

	/// <summary>
	/// Final widget's color passed to the draw buffer.
	/// </summary>

	protected Color drawingColor
	{
		get
		{
			Color colF = color;
			colF.a = finalAlpha;
			if (premultipliedAlpha) colF = NGUITools.ApplyPMA(colF);
			return colF;
		}
	}

	/// <summary>
	/// Fill the draw buffers.
	/// </summary>

	protected void Fill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols, Rect outer, Rect inner)
	{
		switch (mFlip)
		{
			case Flip.Horizontally:
				mOuterUV = new Rect(outer.xMax, outer.yMin, -outer.width, outer.height);
				mInnerUV = new Rect(inner.xMax, inner.yMin, -inner.width, inner.height);
				break;
			case Flip.Vertically:
				mOuterUV = new Rect(outer.xMin, outer.yMax, outer.width, -outer.height);
				mInnerUV = new Rect(inner.xMin, inner.yMax, inner.width, -inner.height);
				break;
			case Flip.Both:
				mOuterUV = new Rect(outer.xMax, outer.yMax, -outer.width, -outer.height);
				mInnerUV = new Rect(inner.xMax, inner.yMax, -inner.width, -inner.height);
				break;
			default:
				mOuterUV = outer;
				mInnerUV = inner;
				break;
		}

		switch (type)
		{
			case Type.Simple:
			SimpleFill(verts, uvs, cols);
			break;

			case Type.Sliced:
			SlicedFill(verts, uvs, cols);
			break;

			case Type.Filled:
			FilledFill(verts, uvs, cols);
			break;

			case Type.Tiled:
			TiledFill(verts, uvs, cols);
			break;

			case Type.Advanced:
			AdvancedFill(verts, uvs, cols);
			break;
		}
	}

	/// <summary>
	/// Regular sprite fill function is quite simple.
	/// </summary>

	void SimpleFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		Vector4 v = drawingDimensions;
		Color gc = drawingColor;

		int partsX = 0, partsY = 0;
		if (centerType != AdvancedType.Invisible)
		{
			if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
			{
				partsX = 2;
				mTempPos[1].x = (v.x + v.z) * 0.5F;
				mTempUVs[1].x = mInnerUV.xMax;

				if (mFillBorder || !hasBorder)
				{
					mTempPos[0].x = v.x;
					mTempPos[2].x = v.z;
					mTempUVs[0].x = mOuterUV.xMin;
					mTempUVs[2].x = mOuterUV.xMin;
				}
				else
				{
					float borderWidth = (mTempPos[1].x - v.x) * (mInnerUV.xMin - mOuterUV.xMin) / (mInnerUV.xMax - mOuterUV.xMin);
					mTempPos[0].x = v.x + borderWidth;
					mTempPos[2].x = v.z - borderWidth;
					mTempUVs[0].x = mInnerUV.xMin;
					mTempUVs[2].x = mInnerUV.xMin;
				}
			}
			else
			{
				partsX = 1;

				if (mFillBorder || !hasBorder)
				{
					mTempPos[0].x = v.x;
					mTempPos[1].x = v.z;
					mTempUVs[0].x = mOuterUV.xMin;
					mTempUVs[1].x = mOuterUV.xMax;
				}
				else
				{
					float factor = (v.z - v.x) / (mOuterUV.xMax - mOuterUV.xMin);
					mTempPos[0].x = v.x + (mInnerUV.xMin - mOuterUV.xMin) * factor;
					mTempPos[1].x = v.z - (mOuterUV.xMax - mInnerUV.xMax) * factor;
					mTempUVs[0].x = mInnerUV.xMin;
					mTempUVs[1].x = mInnerUV.xMax;
				}
			}
			if (mMirror == Flip.Vertically || mMirror == Flip.Both)
			{
				partsY = 2;
				mTempPos[1].y = (v.y + v.w) * 0.5F;
				mTempUVs[1].y = mInnerUV.yMax;

				if (mFillBorder || !hasBorder)
				{
					mTempPos[0].y = v.y;
					mTempPos[2].y = v.w;
					mTempUVs[0].y = mOuterUV.yMin;
					mTempUVs[2].y = mOuterUV.yMin;
				}
				else
				{
					float borderHeight = (mTempPos[1].y - v.y) * (mInnerUV.yMin - mOuterUV.yMin) / (mInnerUV.yMax - mOuterUV.yMin);
					mTempPos[0].y = v.y + borderHeight;
					mTempPos[2].y = v.w - borderHeight;
					mTempUVs[0].y = mInnerUV.yMin;
					mTempUVs[2].y = mInnerUV.yMin;
				}
			}
			else
			{
				partsY = 1;

				if (mFillBorder || !hasBorder)
				{
					mTempPos[0].y = v.y;
					mTempPos[1].y = v.w;
					mTempUVs[0].y = mOuterUV.yMin;
					mTempUVs[1].y = mOuterUV.yMax;
				}
				else
				{
					float factor = (v.w - v.y) / (mOuterUV.yMax - mOuterUV.yMin);
					mTempPos[0].y = v.y + (mInnerUV.yMin - mOuterUV.yMin) * factor;
					mTempPos[1].y = v.w - (mOuterUV.yMax - mInnerUV.yMax) * factor;
					mTempUVs[0].y = mInnerUV.yMin;
					mTempUVs[1].y = mInnerUV.yMax;
				}
			}
		}
		else
		{
			if (!mFillBorder || !hasBorder)
				return;

			if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
			{
				partsX = 4;

				float borderWidth = (mTempPos[1].x - v.x) * (mInnerUV.xMin - mOuterUV.xMin) / (mInnerUV.xMax - mOuterUV.xMin);
				mTempPos[0].x = v.x;
				mTempPos[1].x = v.x + borderWidth;
				mTempPos[2].x = (v.x + v.z) * 0.5F;
				mTempPos[3].x = v.z - borderWidth;
				mTempPos[4].x = v.z;

				mTempUVs[0].x = mOuterUV.xMin;
				mTempUVs[1].x = mInnerUV.xMin;
				mTempUVs[2].x = mInnerUV.xMax;
				mTempUVs[3].x = mInnerUV.xMin;
				mTempUVs[4].x = mOuterUV.xMin;
			}
			else
			{
				partsX = 3;

				float factor = (v.z - v.x) / (mOuterUV.xMax - mOuterUV.xMin);
				mTempPos[0].x = v.x;
				mTempPos[1].x = v.x + (mInnerUV.xMin - mOuterUV.xMin) * factor;
				mTempPos[2].x = v.z - (mOuterUV.xMax - mInnerUV.xMax) * factor;
				mTempPos[3].x = v.z;

				mTempUVs[0].x = mOuterUV.xMin;
				mTempUVs[1].x = mInnerUV.xMin;
				mTempUVs[2].x = mInnerUV.xMax;
				mTempUVs[3].x = mOuterUV.xMax;
			}
			if (mMirror == Flip.Vertically || mMirror == Flip.Both)
			{
				partsY = 4;

				float borderHeight = (mTempPos[1].y - v.y) * (mInnerUV.yMin - mOuterUV.yMin) / (mInnerUV.yMax - mOuterUV.yMin);
				mTempPos[0].y = v.y;
				mTempPos[1].y = v.y + borderHeight;
				mTempPos[2].y = (v.y + v.w) * 0.5F;
				mTempPos[3].y = v.w - borderHeight;
				mTempPos[4].y = v.w;

				mTempUVs[0].y = mOuterUV.yMin;
				mTempUVs[1].y = mInnerUV.yMin;
				mTempUVs[2].y = mInnerUV.yMax;
				mTempUVs[3].y = mInnerUV.yMin;
				mTempUVs[4].y = mOuterUV.yMin;
			}
			else
			{
				partsY = 3;

				float factor = (v.w - v.y) / (mOuterUV.yMax - mOuterUV.yMin);
				mTempPos[0].y = v.y;
				mTempPos[1].y = v.y + (mInnerUV.yMin - mOuterUV.yMin) * factor;
				mTempPos[2].y = v.w - (mOuterUV.yMax - mInnerUV.yMax) * factor;
				mTempPos[3].y = v.w;

				mTempUVs[0].y = mOuterUV.yMin;
				mTempUVs[1].y = mInnerUV.yMin;
				mTempUVs[2].y = mInnerUV.yMax;
				mTempUVs[3].y = mOuterUV.yMax;
			}
		}

		if (mFillDirection == FillDirection.Nothing)
		{
			for (int x1 = 0, x2 = x1 + 1; x1 < partsX; x2 = ++x1 + 1)
			{
				for (int y1 = 0, y2 = y1 + 1; y1 < partsY; y2 = ++y1 + 1)
				{
					if (centerType == AdvancedType.Invisible && x1 > 0 && x2 < partsX && y1 > 0 && y2 < partsY)
						continue;

					if (mTempPos[x1].x != mTempPos[x2].x && mTempPos[y1].y != mTempPos[y2].y)
					{
						verts.Add(new Vector3(mTempPos[x1].x, mTempPos[y1].y));
						verts.Add(new Vector3(mTempPos[x1].x, mTempPos[y2].y));
						verts.Add(new Vector3(mTempPos[x2].x, mTempPos[y2].y));
						verts.Add(new Vector3(mTempPos[x2].x, mTempPos[y1].y));

						uvs.Add(new Vector2(mTempUVs[x1].x, mTempUVs[y1].y));
						uvs.Add(new Vector2(mTempUVs[x1].x, mTempUVs[y2].y));
						uvs.Add(new Vector2(mTempUVs[x2].x, mTempUVs[y2].y));
						uvs.Add(new Vector2(mTempUVs[x2].x, mTempUVs[y1].y));

						if (!mApplyGradient)
						{
							cols.Add(gc);
							cols.Add(gc);
							cols.Add(gc);
							cols.Add(gc);
						}
						else
						{
							AddVertexColours(cols, gc, y1, partsY);
							AddVertexColours(cols, gc, y2, partsY);
							AddVertexColours(cols, gc, y2, partsY);
							AddVertexColours(cols, gc, y1, partsY);
						}
					}
				}
			}
			return;
		}

		//#region fill

		Vector2[] fillBounds = new Vector2[2];

		if (mFillDirection == FillDirection.Horizontal)
		{
			if (mInvert)
			{
				fillBounds[0].x = Mathf.Lerp(mTempPos[partsX].x, mTempPos[0].x, mFillAmount);
				fillBounds[1].x = mTempPos[partsX].x;
			}
			else
			{
				fillBounds[0].x = mTempPos[0].x;
				fillBounds[1].x = Mathf.Lerp(mTempPos[0].x, mTempPos[partsX].x, mFillAmount);
			}
			fillBounds[0].y = mTempPos[0].y;
			fillBounds[1].y = mTempPos[partsY].y;
		}
		else if (mFillDirection == FillDirection.Vertical)
		{
			if (mInvert)
			{
				fillBounds[0].y = Mathf.Lerp(mTempPos[partsY].y, mTempPos[0].y, mFillAmount);
				fillBounds[1].y = mTempPos[partsY].y;
			}
			else
			{
				fillBounds[0].y = mTempPos[0].y;
				fillBounds[1].y = Mathf.Lerp(mTempPos[0].y, mTempPos[partsY].y, mFillAmount);
			}
			fillBounds[0].x = mTempPos[0].x;
			fillBounds[1].x = mTempPos[partsX].x;
		}
		else
		{
			return;
		}

		for (int x1 = 0, x2 = x1 + 1; x1 < partsX; x2 = ++x1 + 1)
		{
			for (int y1 = 0, y2 = y1 + 1; y1 < partsY; y2 = ++y1 + 1)
			{
				if (centerType == AdvancedType.Invisible && x1 > 0 && x2 < partsX && y1 > 0 && y2 < partsY)
					continue;

				if (mTempPos[x1].x != mTempPos[x2].x && mTempPos[y1].y != mTempPos[y2].y
					&& mTempPos[x1].x < fillBounds[1].x && mTempPos[x2].x > fillBounds[0].x
					&& mTempPos[y1].y < fillBounds[1].y && mTempPos[y2].y > fillBounds[0].y)
				{
					Vector2[] tempPos = new Vector2[2];
					Vector2[] tempUVs = new Vector2[2];

					if (mTempPos[x1].x < fillBounds[0].x)
					{
						tempPos[0].x = fillBounds[0].x;
						float ratio = (fillBounds[0].x - mTempPos[x1].x) / (mTempPos[x2].x - mTempPos[x1].x);
						tempUVs[0].x = Mathf.Lerp(mTempUVs[x1].x, mTempUVs[x2].x, ratio);
					}
					else
					{
						tempPos[0].x = mTempPos[x1].x;
						tempUVs[0].x = mTempUVs[x1].x;
					}
					if (mTempPos[x2].x > fillBounds[1].x)
					{
						tempPos[1].x = fillBounds[1].x;
						float ratio = (fillBounds[1].x - mTempPos[x2].x) / (mTempPos[x1].x - mTempPos[x2].x);
						tempUVs[1].x = Mathf.Lerp(mTempUVs[x2].x, mTempUVs[x1].x, ratio);
					}
					else
					{
						tempPos[1].x = mTempPos[x2].x;
						tempUVs[1].x = mTempUVs[x2].x;
					}
					if (mTempPos[y1].y < fillBounds[0].y)
					{
						tempPos[0].y = fillBounds[0].y;
						float ratio = (fillBounds[0].y - mTempPos[y1].y) / (mTempPos[y2].y - mTempPos[y1].y);
						tempUVs[0].y = Mathf.Lerp(mTempUVs[y1].y, mTempUVs[y2].y, ratio);
					}
					else
					{
						tempPos[0].y = mTempPos[y1].y;
						tempUVs[0].y = mTempUVs[y1].y;
					}
					if (mTempPos[y2].y > fillBounds[1].y)
					{
						tempPos[1].y = fillBounds[1].y;
						float ratio = (fillBounds[1].y - mTempPos[y2].y) / (mTempPos[y1].y - mTempPos[y2].y);
						tempUVs[1].y = Mathf.Lerp(mTempUVs[y2].y, mTempUVs[y1].y, ratio);
					}
					else
					{
						tempPos[1].y = mTempPos[y2].y;
						tempUVs[1].y = mTempUVs[y2].y;
					}

					verts.Add(new Vector3(tempPos[0].x, tempPos[0].y));
					verts.Add(new Vector3(tempPos[0].x, tempPos[1].y));
					verts.Add(new Vector3(tempPos[1].x, tempPos[1].y));
					verts.Add(new Vector3(tempPos[1].x, tempPos[0].y));

					uvs.Add(new Vector2(tempUVs[0].x, tempUVs[0].y));
					uvs.Add(new Vector2(tempUVs[0].x, tempUVs[1].y));
					uvs.Add(new Vector2(tempUVs[1].x, tempUVs[1].y));
					uvs.Add(new Vector2(tempUVs[1].x, tempUVs[0].y));

					if (!mApplyGradient)
					{
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
					}
					else
					{
						AddVertexColours(cols, gc, y1, partsY);
						AddVertexColours(cols, gc, y2, partsY);
						AddVertexColours(cols, gc, y2, partsY);
						AddVertexColours(cols, gc, y1, partsY);
					}
				}
			}
		}

		//#endregion
	}

	/// <summary>
	/// Sliced sprite fill function is more complicated as it generates 9 quads instead of 1.
	/// </summary>

	void SlicedFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		Vector4 br = flippedBorder * pixelSize;

		if (br.x == 0f && br.y == 0f && br.z == 0f && br.w == 0f)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}

		Color gc = drawingColor;
		Vector4 v = drawingDimensions;

		int partsX, partsY;

		if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
		{
			partsX = 4;

			mTempPos[0].x = v.x;
			mTempPos[1].x = v.x + br.x;
			mTempPos[2].x = (v.x + v.z) * 0.5F;
			mTempPos[3].x = v.z - br.x;
			mTempPos[4].x = v.z;

			mTempUVs[0].x = mOuterUV.xMin;
			mTempUVs[1].x = mInnerUV.xMin;
			mTempUVs[2].x = mInnerUV.xMax;
			mTempUVs[3].x = mInnerUV.xMin;
			mTempUVs[4].x = mOuterUV.xMin;
		}
		else
		{
			partsX = 3;

			mTempPos[0].x = v.x;
			mTempPos[1].x = v.x + br.x;
			mTempPos[2].x = v.z - br.z;
			mTempPos[3].x = v.z;

			mTempUVs[0].x = mOuterUV.xMin;
			mTempUVs[1].x = mInnerUV.xMin;
			mTempUVs[2].x = mInnerUV.xMax;
			mTempUVs[3].x = mOuterUV.xMax;
		}
		if (mMirror == Flip.Vertically || mMirror == Flip.Both)
		{
			partsY = 4;

			mTempPos[0].y = v.y;
			mTempPos[1].y = v.y + br.y;
			mTempPos[2].y = (v.y + v.w) * 0.5F;
			mTempPos[3].y = v.w - br.y;
			mTempPos[4].y = v.w;

			mTempUVs[0].y = mOuterUV.yMin;
			mTempUVs[1].y = mInnerUV.yMin;
			mTempUVs[2].y = mInnerUV.yMax;
			mTempUVs[3].y = mInnerUV.yMin;
			mTempUVs[4].y = mOuterUV.yMin;
		}
		else
		{
			partsY = 3;

			mTempPos[0].y = v.y;
			mTempPos[1].y = v.y + br.y;
			mTempPos[2].y = v.w - br.w;
			mTempPos[3].y = v.w;

			mTempUVs[0].y = mOuterUV.yMin;
			mTempUVs[1].y = mInnerUV.yMin;
			mTempUVs[2].y = mInnerUV.yMax;
			mTempUVs[3].y = mOuterUV.yMax;
		}

		#region fill

		Vector2[] fillBounds = new Vector2[2];
		if (mFillDirection == FillDirection.Horizontal)
		{
			if (mInvert)
			{
				fillBounds[0].x = Mathf.Lerp(mTempPos[partsX].x, mTempPos[0].x, mFillAmount);
				fillBounds[1].x = mTempPos[partsX].x;
			}
			else
			{
				fillBounds[0].x = mTempPos[0].x;
				fillBounds[1].x = Mathf.Lerp(mTempPos[0].x, mTempPos[partsX].x, mFillAmount);
			}
			fillBounds[0].y = mTempPos[0].y;
			fillBounds[1].y = mTempPos[partsY].y;
		}
		else if (mFillDirection == FillDirection.Vertical)
		{
			if (mInvert)
			{
				fillBounds[0].y = Mathf.Lerp(mTempPos[partsY].y, mTempPos[0].y, mFillAmount);
				fillBounds[1].y = mTempPos[partsY].y;
			}
			else
			{
				fillBounds[0].y = mTempPos[0].y;
				fillBounds[1].y = Mathf.Lerp(mTempPos[0].y, mTempPos[partsY].y, mFillAmount);
			}
			fillBounds[0].x = mTempPos[0].x;
			fillBounds[1].x = mTempPos[partsX].x;
		}
		else
		{
			for (int x1 = 0, x2 = x1 + 1; x1 < partsX; x2 = ++x1 + 1)
			{
				for (int y1 = 0, y2 = y1 + 1; y1 < partsY; y2 = ++y1 + 1)
				{
					if (centerType == AdvancedType.Invisible && x1 > 0 && x2 < partsX && y1 > 0 && y2 < partsY)
						continue;

					if (!mFillBorder && !(x1 > 0 && x2 < partsX && y1 > 0 && y2 < partsY))
						continue;

					if (mTempPos[x1].x != mTempPos[x2].x && mTempPos[y1].y != mTempPos[y2].y)
					{
						verts.Add(new Vector3(mTempPos[x1].x, mTempPos[y1].y));
						verts.Add(new Vector3(mTempPos[x1].x, mTempPos[y2].y));
						verts.Add(new Vector3(mTempPos[x2].x, mTempPos[y2].y));
						verts.Add(new Vector3(mTempPos[x2].x, mTempPos[y1].y));

						uvs.Add(new Vector2(mTempUVs[x1].x, mTempUVs[y1].y));
						uvs.Add(new Vector2(mTempUVs[x1].x, mTempUVs[y2].y));
						uvs.Add(new Vector2(mTempUVs[x2].x, mTempUVs[y2].y));
						uvs.Add(new Vector2(mTempUVs[x2].x, mTempUVs[y1].y));

						if (!mApplyGradient)
						{
							cols.Add(gc);
							cols.Add(gc);
							cols.Add(gc);
							cols.Add(gc);
						}
						else
						{
							AddVertexColours(cols, gc, y1, partsY);
							AddVertexColours(cols, gc, y2, partsY);
							AddVertexColours(cols, gc, y2, partsY);
							AddVertexColours(cols, gc, y1, partsY);
						}
					}
				}
			}
			return;
		}

		for (int x1 = 0, x2 = x1 + 1; x1 < partsX; x2 = ++x1 + 1)
		{
			for (int y1 = 0, y2 = y1 + 1; y1 < partsY; y2 = ++y1 + 1)
			{
				if (centerType == AdvancedType.Invisible && x1 > 0 && x2 < partsX && y1 > 0 && y2 < partsY)
					continue;

				if (!mFillBorder && !(x1 > 0 && x1 < partsX - 1 && y1 > 0 && y1 < partsY - 1))
					continue;

				if (mTempPos[x1].x != mTempPos[x2].x && mTempPos[y1].y != mTempPos[y2].y
					&& mTempPos[x1].x < fillBounds[1].x && mTempPos[x2].x > fillBounds[0].x
					&& mTempPos[y1].y < fillBounds[1].y && mTempPos[y2].y > fillBounds[0].y)
				{

					Vector2[] tempPos = new Vector2[2];
					Vector2[] tempUVs = new Vector2[2];

					if (mTempPos[x1].x < fillBounds[0].x)
					{
						tempPos[0].x = fillBounds[0].x;
						float ratio = (fillBounds[0].x - mTempPos[x1].x) / (mTempPos[x2].x - mTempPos[x1].x);
						tempUVs[0].x = Mathf.Lerp(mTempUVs[x1].x, mTempUVs[x2].x, ratio);
					}
					else
					{
						tempPos[0].x = mTempPos[x1].x;
						tempUVs[0].x = mTempUVs[x1].x;
					}
					if (mTempPos[x2].x > fillBounds[1].x)
					{
						tempPos[1].x = fillBounds[1].x;
						float ratio = (fillBounds[1].x - mTempPos[x2].x) / (mTempPos[x1].x - mTempPos[x2].x);
						tempUVs[1].x = Mathf.Lerp(mTempUVs[x2].x, mTempUVs[x1].x, ratio);
					}
					else
					{
						tempPos[1].x = mTempPos[x2].x;
						tempUVs[1].x = mTempUVs[x2].x;
					}
					if (mTempPos[y1].y < fillBounds[0].y)
					{
						tempPos[0].y = fillBounds[0].y;
						float ratio = (fillBounds[0].y - mTempPos[y1].y) / (mTempPos[y2].y - mTempPos[y1].y);
						tempUVs[0].y = Mathf.Lerp(mTempUVs[y1].y, mTempUVs[y2].y, ratio);
					}
					else
					{
						tempPos[0].y = mTempPos[y1].y;
						tempUVs[0].y = mTempUVs[y1].y;
					}
					if (mTempPos[y2].y > fillBounds[1].y)
					{
						tempPos[1].y = fillBounds[1].y;
						float ratio = (fillBounds[1].y - mTempPos[y2].y) / (mTempPos[y1].y - mTempPos[y2].y);
						tempUVs[1].y = Mathf.Lerp(mTempUVs[y2].y, mTempUVs[y1].y, ratio);
					}
					else
					{
						tempPos[1].y = mTempPos[y2].y;
						tempUVs[1].y = mTempUVs[y2].y;
					}

					verts.Add(new Vector3(tempPos[0].x, tempPos[0].y));
					verts.Add(new Vector3(tempPos[0].x, tempPos[1].y));
					verts.Add(new Vector3(tempPos[1].x, tempPos[1].y));
					verts.Add(new Vector3(tempPos[1].x, tempPos[0].y));

					uvs.Add(new Vector2(tempUVs[0].x, tempUVs[0].y));
					uvs.Add(new Vector2(tempUVs[0].x, tempUVs[1].y));
					uvs.Add(new Vector2(tempUVs[1].x, tempUVs[1].y));
					uvs.Add(new Vector2(tempUVs[1].x, tempUVs[0].y));

					if (!mApplyGradient)
					{
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
					}
					else
					{
						AddVertexColours(cols, gc, y1, partsY);
						AddVertexColours(cols, gc, y2, partsY);
						AddVertexColours(cols, gc, y2, partsY);
						AddVertexColours(cols, gc, y1, partsY);
					}
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Adds a gradient-based vertex color to the sprite.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	void AddVertexColours(List<Color> cols, Color color, int y, int partsY)
	{
		if (y < partsY * 0.5F)
		{
			cols.Add(color * mGradientBottom);
		}
		else if (y > partsY * 0.5F)
		{
			cols.Add(color * mGradientTop);
		}
		else
		{
			cols.Add(color * (mGradientBottom + mGradientTop) * 0.5F);
		}
	}

	/// <summary>
	/// Tiled sprite fill function.
	/// </summary>

	void TiledFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (mainTexture == null) return;
		
		Vector2[] size = new Vector2[2];

		size[0] = new Vector2(Mathf.Abs(mInnerUV.width) * textureWidth, Mathf.Abs(mInnerUV.height) * textureHeight);
		size[0] *= pixelSize;

		if (!mTileBorder && (size[0].x < 2f || size[0].y < 2f)) return;

		Color gc = drawingColor;
		Vector4 d = drawingDimensions;
		Vector4 v = d;

		mTempUVs[0].x = mOuterUV.xMin;
		mTempUVs[1].x = mInnerUV.xMin;
		mTempUVs[2].x = mInnerUV.xMax;
		mTempUVs[0].y = mOuterUV.yMin;
		mTempUVs[1].y = mInnerUV.yMin;
		mTempUVs[2].y = mInnerUV.yMax;

		if (mTileBorder)
		{
			float uiWidth = Mathf.Abs(mTempUVs[2].x - mTempUVs[0].x);
			float uiHeight = Mathf.Abs(mTempUVs[2].y - mTempUVs[0].y);
			size[1] = new Vector2(uiWidth * textureWidth, uiHeight * textureHeight);
			size[1] *= pixelSize;

			mTempUVs[3] = mTempUVs[0];
			mTempUVs[4] = mTempUVs[2];
		}
		else
		{
			size[1] = size[0];
			mTempUVs[3] = mTempUVs[1];
			mTempUVs[4] = mTempUVs[2];
		}

		if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
			mTempPos[0].x = (v.x + v.z) * 0.5f;
		else
			mTempPos[0].x = v.z;
		if (mMirror == Flip.Vertically || mMirror == Flip.Both)
			mTempPos[0].y = (v.y + v.w) * 0.5f;
		else
			mTempPos[0].y = v.w;

		int xIndex, yIndex;
		float x0, y0;
		for (yIndex = 0, y0 = v.y; y0 < mTempPos[0].y; yIndex++)
		{
			float y1 = y0 + (yIndex == 0 ? size[1] : size[0]).y;
			float v0 = (yIndex == 0 ? mTempUVs[3] : mTempUVs[1]).y;
			float v1 = (yIndex == 0 ? mTempUVs[4] : mTempUVs[2]).y;
			if (y1 > mTempPos[0].y)
			{
				v1 = Mathf.Lerp(v0, v1, (mTempPos[0].y - y0) / (yIndex == 0 ? size[1] : size[0]).y);
				y1 = mTempPos[0].y;
			}

			for (xIndex = 0, x0 = v.x; x0 < mTempPos[0].x; xIndex++)
			{
				float x1 = x0 + (xIndex == 0 ? size[1] : size[0]).x;
				float u0 = (xIndex == 0 ? mTempUVs[3] : mTempUVs[1]).x;
				float u1 = (xIndex == 0 ? mTempUVs[4] : mTempUVs[2]).x;
				if (x1 > mTempPos[0].x)
				{
					u1 = Mathf.Lerp(u0, u1, (mTempPos[0].x - x0) / (xIndex == 0 ? size[1] : size[0]).x);
					x1 = mTempPos[0].x;
				}

				verts.Add(new Vector3(x0, y0));
				verts.Add(new Vector3(x0, y1));
				verts.Add(new Vector3(x1, y1));
				verts.Add(new Vector3(x1, y0));

				uvs.Add(new Vector2(u0, v0));
				uvs.Add(new Vector2(u0, v1));
				uvs.Add(new Vector2(u1, v1));
				uvs.Add(new Vector2(u1, v0));

				if (!mApplyGradient)
				{
					cols.Add(gc);
					cols.Add(gc);
					cols.Add(gc);
					cols.Add(gc);
				}
				else
				{
					AddVertexColours(cols, gc, y0, v);
					AddVertexColours(cols, gc, y1, v);
					AddVertexColours(cols, gc, y1, v);
					AddVertexColours(cols, gc, y0, v);
				}

				#region mirror fill

				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
				{
					verts.Add(new Vector3(mTempPos[0].x + mTempPos[0].x - x1, y0));
					verts.Add(new Vector3(mTempPos[0].x + mTempPos[0].x - x1, y1));
					verts.Add(new Vector3(mTempPos[0].x + mTempPos[0].x - x0, y1));
					verts.Add(new Vector3(mTempPos[0].x + mTempPos[0].x - x0, y0));

					uvs.Add(new Vector2(u1, v0));
					uvs.Add(new Vector2(u1, v1));
					uvs.Add(new Vector2(u0, v1));
					uvs.Add(new Vector2(u0, v0));

					if (!mApplyGradient)
					{
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
					}
					else
					{
						AddVertexColours(cols, gc, y0, v);
						AddVertexColours(cols, gc, y1, v);
						AddVertexColours(cols, gc, y1, v);
						AddVertexColours(cols, gc, y0, v);
					}
				}

				if (mMirror == Flip.Vertically || mMirror == Flip.Both)
				{
					verts.Add(new Vector3(x0, mTempPos[0].y + mTempPos[0].y - y1));
					verts.Add(new Vector3(x0, mTempPos[0].y + mTempPos[0].y - y0));
					verts.Add(new Vector3(x1, mTempPos[0].y + mTempPos[0].y - y0));
					verts.Add(new Vector3(x1, mTempPos[0].y + mTempPos[0].y - y1));

					uvs.Add(new Vector2(u0, v1));
					uvs.Add(new Vector2(u0, v0));
					uvs.Add(new Vector2(u1, v0));
					uvs.Add(new Vector2(u1, v1));

					if (!mApplyGradient)
					{
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
					}
					else
					{
						AddVertexColours(cols, gc, y1, v);
						AddVertexColours(cols, gc, y0, v);
						AddVertexColours(cols, gc, y0, v);
						AddVertexColours(cols, gc, y1, v);
					}
				}

				if (mMirror == Flip.Both)
				{
					verts.Add(new Vector3(mTempPos[0].x + mTempPos[0].x - x1, mTempPos[0].y + mTempPos[0].y - y1));
					verts.Add(new Vector3(mTempPos[0].x + mTempPos[0].x - x1, mTempPos[0].y + mTempPos[0].y - y0));
					verts.Add(new Vector3(mTempPos[0].x + mTempPos[0].x - x0, mTempPos[0].y + mTempPos[0].y - y0));
					verts.Add(new Vector3(mTempPos[0].x + mTempPos[0].x - x0, mTempPos[0].y + mTempPos[0].y - y1));

					uvs.Add(new Vector2(u1, v1));
					uvs.Add(new Vector2(u1, v0));
					uvs.Add(new Vector2(u0, v0));
					uvs.Add(new Vector2(u0, v1));

					if (!mApplyGradient)
					{
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
					}
					else
					{
						AddVertexColours(cols, gc, y1, d);
						AddVertexColours(cols, gc, y0, d);
						AddVertexColours(cols, gc, y0, d);
						AddVertexColours(cols, gc, y1, d);
					}
				}

				#endregion

				x0 = x1;
			}
			y0 = y1;
		}
	}

	/// <summary>
	/// Adds a gradient-based vertex color to the sprite.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	void AddVertexColours(List<Color> cols, Color color, float y, Vector4 drawingDimensions)
	{
		float topPercent = (y - drawingDimensions.y) / (drawingDimensions.w - drawingDimensions.y);
		float bottomPercent = 1 - topPercent;
		cols.Add(color * (mGradientTop * topPercent + mGradientBottom * bottomPercent));
	}

	/// <summary>
	/// Filled sprite fill function.
	/// </summary>

	void FilledFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (mFillDirection == FillDirection.Nothing)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}

		if (mFillAmount < 0.001f) return;

		Vector4 v = drawingDimensions;
		Vector4 u = drawingUVs;
		Color c = drawingColor;

		// Horizontal and vertical filled sprites are simple -- just end the sprite prematurely
		if (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical)
		{
			if (mFillDirection == FillDirection.Horizontal)
			{
				float fill = (u.z - u.x) * mFillAmount;

				if (mInvert)
				{
					v.x = v.z - (v.z - v.x) * mFillAmount;
					u.x = u.z - fill;
				}
				else
				{
					v.z = v.x + (v.z - v.x) * mFillAmount;
					u.z = u.x + fill;
				}
			}
			else if (mFillDirection == FillDirection.Vertical)
			{
				float fill = (u.w - u.y) * mFillAmount;

				if (mInvert)
				{
					v.y = v.w - (v.w - v.y) * mFillAmount;
					u.y = u.w - fill;
				}
				else
				{
					v.w = v.y + (v.w - v.y) * mFillAmount;
					u.w = u.y + fill;
				}
			}
		}

		mTempPos[0] = new Vector2(v.x, v.y);
		mTempPos[1] = new Vector2(v.x, v.w);
		mTempPos[2] = new Vector2(v.z, v.w);
		mTempPos[3] = new Vector2(v.z, v.y);

		mTempUVs[0] = new Vector2(u.x, u.y);
		mTempUVs[1] = new Vector2(u.x, u.w);
		mTempUVs[2] = new Vector2(u.z, u.w);
		mTempUVs[3] = new Vector2(u.z, u.y);

		if (mFillAmount < 1f)
		{
			if (mFillDirection == FillDirection.Radial90)
			{
				if (RadialCut(mTempPos, mTempUVs, mFillAmount, mInvert, 0))
				{
					for (int i = 0; i < 4; ++i)
					{
						verts.Add(mTempPos[i]);
						uvs.Add(mTempUVs[i]);
						cols.Add(c);
					}
				}
				return;
			}

			if (mFillDirection == FillDirection.Radial180)
			{
				for (int side = 0; side < 2; ++side)
				{
					float fx0, fx1, fy0, fy1;

					fy0 = 0f;
					fy1 = 1f;

					if (side == 0) { fx0 = 0f; fx1 = 0.5f; }
					else { fx0 = 0.5f; fx1 = 1f; }

					mTempPos[0].x = Mathf.Lerp(v.x, v.z, fx0);
					mTempPos[1].x = mTempPos[0].x;
					mTempPos[2].x = Mathf.Lerp(v.x, v.z, fx1);
					mTempPos[3].x = mTempPos[2].x;

					mTempPos[0].y = Mathf.Lerp(v.y, v.w, fy0);
					mTempPos[1].y = Mathf.Lerp(v.y, v.w, fy1);
					mTempPos[2].y = mTempPos[1].y;
					mTempPos[3].y = mTempPos[0].y;

					mTempUVs[0].x = Mathf.Lerp(u.x, u.z, fx0);
					mTempUVs[1].x = mTempUVs[0].x;
					mTempUVs[2].x = Mathf.Lerp(u.x, u.z, fx1);
					mTempUVs[3].x = mTempUVs[2].x;

					mTempUVs[0].y = Mathf.Lerp(u.y, u.w, fy0);
					mTempUVs[1].y = Mathf.Lerp(u.y, u.w, fy1);
					mTempUVs[2].y = mTempUVs[1].y;
					mTempUVs[3].y = mTempUVs[0].y;

					float val = !mInvert ? fillAmount * 2f - side : mFillAmount * 2f - (1 - side);

					if (RadialCut(mTempPos, mTempUVs, Mathf.Clamp01(val), !mInvert, NGUIMath.RepeatIndex(side + 3, 4)))
					{
						for (int i = 0; i < 4; ++i)
						{
							verts.Add(mTempPos[i]);
							uvs.Add(mTempUVs[i]);
							cols.Add(c);
						}
					}
				}
				return;
			}

			if (mFillDirection == FillDirection.Radial360)
			{
				for (int corner = 0; corner < 4; ++corner)
				{
					float fx0, fx1, fy0, fy1;

					if (corner < 2) { fx0 = 0f; fx1 = 0.5f; }
					else { fx0 = 0.5f; fx1 = 1f; }

					if (corner == 0 || corner == 3) { fy0 = 0f; fy1 = 0.5f; }
					else { fy0 = 0.5f; fy1 = 1f; }

					mTempPos[0].x = Mathf.Lerp(v.x, v.z, fx0);
					mTempPos[1].x = mTempPos[0].x;
					mTempPos[2].x = Mathf.Lerp(v.x, v.z, fx1);
					mTempPos[3].x = mTempPos[2].x;

					mTempPos[0].y = Mathf.Lerp(v.y, v.w, fy0);
					mTempPos[1].y = Mathf.Lerp(v.y, v.w, fy1);
					mTempPos[2].y = mTempPos[1].y;
					mTempPos[3].y = mTempPos[0].y;

					mTempUVs[0].x = Mathf.Lerp(u.x, u.z, fx0);
					mTempUVs[1].x = mTempUVs[0].x;
					mTempUVs[2].x = Mathf.Lerp(u.x, u.z, fx1);
					mTempUVs[3].x = mTempUVs[2].x;

					mTempUVs[0].y = Mathf.Lerp(u.y, u.w, fy0);
					mTempUVs[1].y = Mathf.Lerp(u.y, u.w, fy1);
					mTempUVs[2].y = mTempUVs[1].y;
					mTempUVs[3].y = mTempUVs[0].y;

					float val = mInvert ?
						mFillAmount * 4f - NGUIMath.RepeatIndex(corner + 2, 4) :
						mFillAmount * 4f - (3 - NGUIMath.RepeatIndex(corner + 2, 4));

					if (RadialCut(mTempPos, mTempUVs, Mathf.Clamp01(val), mInvert, NGUIMath.RepeatIndex(corner + 2, 4)))
					{
						for (int i = 0; i < 4; ++i)
						{
							verts.Add(mTempPos[i]);
							uvs.Add(mTempUVs[i]);
							cols.Add(c);
						}
					}
				}
				return;
			}
		}

		// Fill the buffer with the quad for the sprite
		for (int i = 0; i < 4; ++i)
		{
			verts.Add(mTempPos[i]);
			uvs.Add(mTempUVs[i]);
			cols.Add(c);
		}
	}

	/// <summary>
	/// Advanced sprite fill function. Contributed by Nicki Hansen.
	/// </summary>

	void AdvancedFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (mainTexture == null) return;

		Vector4 br = flippedBorder * pixelSize;

		if (br.x == 0f && br.y == 0f && br.z == 0f && br.w == 0f)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}

		Color c = drawingColor;
		Vector4 v = drawingDimensions;
		Vector2 tileSize = new Vector2(Mathf.Abs(mInnerUV.width) * textureWidth, Mathf.Abs(mInnerUV.height) * textureHeight);
		tileSize *= pixelSize;

		if (tileSize.x < 1f) tileSize.x = 1f;
		if (tileSize.y < 1f) tileSize.y = 1f;

		mTempPos[0].x = v.x;
		mTempPos[1].x = v.x + br.x;
		mTempPos[2].x = v.z - br.z;
		mTempPos[3].x = v.z;

		mTempUVs[0].x = mOuterUV.xMin;
		mTempUVs[1].x = mInnerUV.xMin;
		mTempUVs[2].x = mInnerUV.xMax;
		mTempUVs[3].x = mOuterUV.xMax;

		mTempPos[0].y = v.y;
		mTempPos[1].y = v.y + br.y;
		mTempPos[2].y = v.w - br.w;
		mTempPos[3].y = v.w;

		mTempUVs[0].y = mOuterUV.yMin;
		mTempUVs[1].y = mInnerUV.yMin;
		mTempUVs[2].y = mInnerUV.yMax;
		mTempUVs[3].y = mOuterUV.yMax;

		for (int x = 0; x < 3; ++x)
		{
			int x2 = x + 1;

			for (int y = 0; y < 3; ++y)
			{
				if (centerType == AdvancedType.Invisible && x == 1 && y == 1) continue;
				int y2 = y + 1;

				if (x == 1 && y == 1) // Center
				{
					if (centerType == AdvancedType.Tiled)
					{
						float startPositionX = mTempPos[x].x;
						float endPositionX = mTempPos[x2].x;
						float startPositionY = mTempPos[y].y;
						float endPositionY = mTempPos[y2].y;
						float textureStartX = mTempUVs[x].x;
						float textureStartY = mTempUVs[y].y;
						float tileStartY = startPositionY;

						while (tileStartY < endPositionY)
						{
							float tileStartX = startPositionX;
							float textureEndY = mTempUVs[y2].y;
							float tileEndY = tileStartY + tileSize.y;

							if (tileEndY > endPositionY)
							{
								textureEndY = Mathf.Lerp(textureStartY, textureEndY, (endPositionY - tileStartY) / tileSize.y);
								tileEndY = endPositionY;
							}

							while (tileStartX < endPositionX)
							{
								float tileEndX = tileStartX + tileSize.x;
								float textureEndX = mTempUVs[x2].x;

								if (tileEndX > endPositionX)
								{
									textureEndX = Mathf.Lerp(textureStartX, textureEndX, (endPositionX - tileStartX) / tileSize.x);
									tileEndX = endPositionX;
								}

								Fill(verts, uvs, cols,
									tileStartX, tileEndX,
									tileStartY, tileEndY,
									textureStartX, textureEndX,
									textureStartY, textureEndY, c);

								tileStartX += tileSize.x;
							}
							tileStartY += tileSize.y;
						}
					}
					else if (centerType == AdvancedType.Sliced)
					{
						Fill(verts, uvs, cols,
							mTempPos[x].x, mTempPos[x2].x,
							mTempPos[y].y, mTempPos[y2].y,
							mTempUVs[x].x, mTempUVs[x2].x,
							mTempUVs[y].y, mTempUVs[y2].y, c);
					}
				}
				else if (x == 1) // Top or bottom
				{
					if ((y == 0 && bottomType == AdvancedType.Tiled) || (y == 2 && topType == AdvancedType.Tiled))
					{
						float startPositionX = mTempPos[x].x;
						float endPositionX = mTempPos[x2].x;
						float startPositionY = mTempPos[y].y;
						float endPositionY = mTempPos[y2].y;
						float textureStartX = mTempUVs[x].x;
						float textureStartY = mTempUVs[y].y;
						float textureEndY = mTempUVs[y2].y;
						float tileStartX = startPositionX;

						while (tileStartX < endPositionX)
						{
							float tileEndX = tileStartX + tileSize.x;
							float textureEndX = mTempUVs[x2].x;

							if (tileEndX > endPositionX)
							{
								textureEndX = Mathf.Lerp(textureStartX, textureEndX, (endPositionX - tileStartX) / tileSize.x);
								tileEndX = endPositionX;
							}

							Fill(verts, uvs, cols,
								tileStartX, tileEndX,
								startPositionY, endPositionY,
								textureStartX, textureEndX,
								textureStartY, textureEndY, c);

							tileStartX += tileSize.x;
						}
					}
					else if ((y == 0 && bottomType != AdvancedType.Invisible) || (y == 2 && topType != AdvancedType.Invisible))
					{
						Fill(verts, uvs, cols,
							mTempPos[x].x, mTempPos[x2].x,
							mTempPos[y].y, mTempPos[y2].y,
							mTempUVs[x].x, mTempUVs[x2].x,
							mTempUVs[y].y, mTempUVs[y2].y, c);
					}
				}
				else if (y == 1) // Left or right
				{
					if ((x == 0 && leftType == AdvancedType.Tiled) || (x == 2 && rightType == AdvancedType.Tiled))
					{
						float startPositionX = mTempPos[x].x;
						float endPositionX = mTempPos[x2].x;
						float startPositionY = mTempPos[y].y;
						float endPositionY = mTempPos[y2].y;
						float textureStartX = mTempUVs[x].x;
						float textureEndX = mTempUVs[x2].x;
						float textureStartY = mTempUVs[y].y;
						float tileStartY = startPositionY;

						while (tileStartY < endPositionY)
						{
							float textureEndY = mTempUVs[y2].y;
							float tileEndY = tileStartY + tileSize.y;

							if (tileEndY > endPositionY)
							{
								textureEndY = Mathf.Lerp(textureStartY, textureEndY, (endPositionY - tileStartY) / tileSize.y);
								tileEndY = endPositionY;
							}

							Fill(verts, uvs, cols,
								startPositionX, endPositionX,
								tileStartY, tileEndY,
								textureStartX, textureEndX,
								textureStartY, textureEndY, c);

							tileStartY += tileSize.y;
						}
					}
					else if ((x == 0 && leftType != AdvancedType.Invisible) || (x == 2 && rightType != AdvancedType.Invisible))
					{
						Fill(verts, uvs, cols,
							mTempPos[x].x, mTempPos[x2].x,
							mTempPos[y].y, mTempPos[y2].y,
							mTempUVs[x].x, mTempUVs[x2].x,
							mTempUVs[y].y, mTempUVs[y2].y, c);
					}
				}
				else // Corner
				{
					if (y == 0 && bottomType == AdvancedType.Invisible) continue;
					if (y == 2 && topType == AdvancedType.Invisible) continue;
					if (x == 0 && leftType == AdvancedType.Invisible) continue;
					if (x == 2 && rightType == AdvancedType.Invisible) continue;

					Fill(verts, uvs, cols,
						mTempPos[x].x, mTempPos[x2].x,
						mTempPos[y].y, mTempPos[y2].y,
						mTempUVs[x].x, mTempUVs[x2].x,
						mTempUVs[y].y, mTempUVs[y2].y, c);
				}
			}
		}
	}

	/// <summary>
	/// Adjust the specified quad, making it be radially filled instead.
	/// </summary>

	static bool RadialCut (Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
	{
		// Nothing to fill
		if (fill < 0.001f) return false;

		// Even corners invert the fill direction
		if ((corner & 1) == 1) invert = !invert;

		// Nothing to adjust
		if (!invert && fill > 0.999f) return true;

		// Convert 0-1 value into 0 to 90 degrees angle in radians
		float angle = Mathf.Clamp01(fill);
		if (invert) angle = 1f - angle;
		angle *= 90f * Mathf.Deg2Rad;

		// Calculate the effective X and Y factors
		float cos = Mathf.Cos(angle);
		float sin = Mathf.Sin(angle);

		RadialCut(xy, cos, sin, invert, corner);
		RadialCut(uv, cos, sin, invert, corner);
		return true;
	}

	/// <summary>
	/// Adjust the specified quad, making it be radially filled instead.
	/// </summary>

	static void RadialCut (Vector2[] xy, float cos, float sin, bool invert, int corner)
	{
		int i0 = corner;
		int i1 = NGUIMath.RepeatIndex(corner + 1, 4);
		int i2 = NGUIMath.RepeatIndex(corner + 2, 4);
		int i3 = NGUIMath.RepeatIndex(corner + 3, 4);

		if ((corner & 1) == 1)
		{
			if (sin > cos)
			{
				cos /= sin;
				sin = 1f;

				if (invert)
				{
					xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
					xy[i2].x = xy[i1].x;
				}
			}
			else if (cos > sin)
			{
				sin /= cos;
				cos = 1f;

				if (!invert)
				{
					xy[i2].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
					xy[i3].y = xy[i2].y;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}

			if (!invert) xy[i3].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
			else xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
		}
		else
		{
			if (cos > sin)
			{
				sin /= cos;
				cos = 1f;

				if (!invert)
				{
					xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
					xy[i2].y = xy[i1].y;
				}
			}
			else if (sin > cos)
			{
				cos /= sin;
				sin = 1f;

				if (invert)
				{
					xy[i2].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
					xy[i3].x = xy[i2].x;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}

			if (invert) xy[i3].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
			else xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
		}
	}

	/// <summary>
	/// Helper function that adds the specified values to the buffers.
	/// </summary>

	static void Fill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols,
		float v0x, float v1x, float v0y, float v1y, float u0x, float u1x, float u0y, float u1y, Color col)
	{
		verts.Add(new Vector3(v0x, v0y));
		verts.Add(new Vector3(v0x, v1y));
		verts.Add(new Vector3(v1x, v1y));
		verts.Add(new Vector3(v1x, v0y));

		uvs.Add(new Vector2(u0x, u0y));
		uvs.Add(new Vector2(u0x, u1y));
		uvs.Add(new Vector2(u1x, u1y));
		uvs.Add(new Vector2(u1x, u0y));

		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
	}
#endregion // Fill functions
}
