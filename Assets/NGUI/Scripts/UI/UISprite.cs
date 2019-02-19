//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sprite is a textured element in the UI hierarchy.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Sprite")]
public class UISprite : UIBasicSprite
{
	// Cached and saved values
	[HideInInspector][SerializeField] UIAtlas mAtlas;
	[HideInInspector][SerializeField] string mSpriteName;

	// Deprecated, no longer used
	[HideInInspector][SerializeField] bool mFillCenter = true;

	[System.NonSerialized] protected UISpriteData mSprite;
	[System.NonSerialized] bool mSpriteSet = false;

	/// <summary>
	/// Main texture is assigned on the atlas.
	/// </summary>

	public override Texture mainTexture
	{
		get
		{
			return mAtlas ? mAtlas.texture : null;
		}
		set
		{
			base.mainTexture = value;
		}
	}

	/// <summary>
	/// Main texture is assigned on the atlas.
	/// </summary>

	public override Texture alphaTexture
	{
		get
		{
			return mAtlas ? mAtlas.alphaTexture : null;
		}
		set
		{
			base.mainTexture = value;
		}
	}

	/// <summary>
	/// Material comes from the base class first, and sprite atlas last.
	/// </summary>

	public override Material material
	{
		get
		{
			var mat = base.material;
			if (mat != null) return mat;
			return (mAtlas != null ? mAtlas.spriteMaterial : null);
		}
		set
		{
			base.material = value;
		}
	}

	/// <summary>
	/// Atlas used by this widget.
	/// </summary>
 
	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if (mAtlas != value)
			{
				RemoveFromPanel();

				mAtlas = value;
				mSpriteSet = false;
				mSprite = null;

				// Automatically choose the first sprite
				if (string.IsNullOrEmpty(mSpriteName))
				{
					if (mAtlas != null && mAtlas.spriteList.Count > 0)
					{
						SetAtlasSprite(mAtlas.spriteList[0]);
						mSpriteName = mSprite.name;
					}
				}

				// Re-link the sprite
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					string sprite = mSpriteName;
					mSpriteName = "";
					spriteName = sprite;
					MarkAsChanged();
				}
			}
		}
	}

	/// <summary>
	/// Sprite within the atlas used to draw this widget.
	/// </summary>
 
	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				// If the sprite name hasn't been set yet, no need to do anything
				if (string.IsNullOrEmpty(mSpriteName)) return;

				// Clear the sprite name and the sprite reference
				mSpriteName = "";
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
			}
			else if (mSpriteName != value)
			{
				// If the sprite name changes, the sprite reference should also be updated
				mSpriteName = value;
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
			}
		}
	}

	/// <summary>
	/// It is used for animation
	/// </summary>

	public void SetDepth(int newDepth)
	{
		depth = newDepth;
	}

	/// <summary>
	/// Width of texture.
	/// </summary>

	public override int textureWidth
	{
		get
		{
			return (atlas != null) ? atlas.width : base.textureWidth;
		}
	}

	/// <summary>
	/// Height of texture.
	/// </summary>

	public override int textureHeight
	{
		get
		{
			return (atlas != null) ? atlas.height : base.textureHeight;
		}
	}

	/// <summary>
	/// Is there a valid sprite to work with?
	/// </summary>

	public bool isValid { get { return GetAtlasSprite() != null; } }

	/// <summary>
	/// Whether the center part of the sprite will be filled or not. Turn it off if you want only to borders to show up.
	/// </summary>

	[System.Obsolete("Use 'centerType' instead")]
	public bool fillCenter
	{
		get
		{
			return centerType != AdvancedType.Invisible;
		}
		set
		{
			if (value != (centerType != AdvancedType.Invisible))
			{
				centerType = value ? AdvancedType.Sliced : AdvancedType.Invisible;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether a gradient will be applied.
	/// </summary>

	public bool applyGradient
	{
		get
		{
			return mApplyGradient;
		}
		set
		{
			if (mApplyGradient != value)
			{
				mApplyGradient = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Top gradient color.
	/// </summary>

	public Color gradientTop
	{
		get
		{
			return mGradientTop;
		}
		set
		{
			if (mGradientTop != value)
			{
				mGradientTop = value;
				if (mApplyGradient) MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Bottom gradient color.
	/// </summary>

	public Color gradientBottom
	{
		get
		{
			return mGradientBottom;
		}
		set
		{
			if (mGradientBottom != value)
			{
				mGradientBottom = value;
				if (mApplyGradient) MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Sliced sprites generally have a border. X = left, Y = bottom, Z = right, W = top.
	/// </summary>

	public override Vector4 border
	{
		get
		{
			UISpriteData sp = GetAtlasSprite();
			if (sp == null) return base.border;
			return new Vector4(sp.borderLeft, sp.borderBottom, sp.borderRight, sp.borderTop);
		}
	}

	/// <summary>
	/// The flipped border. X = flipped left, Y = flipped bottom, Z = flipped right, W = flipped top.
	/// </summary>

	public override Vector4 flippedBorder
	{
		get
		{
			return FlipBorder(border);
		}
	}

	/// <summary>
	/// Size of the pixel -- used for drawing.
	/// </summary>

	override public float pixelSize { get { return mAtlas != null ? mAtlas.pixelSize : 1f; } }

	/// <summary>
	/// Minimum allowed width for this widget.
	/// </summary>

	override public int minWidth
	{
		get
		{
			if (type == Type.Sliced)
			{
				UISpriteData sp = GetAtlasSprite();
				float paddingLeft = sp == null ? 0 : sp.paddingLeft;
				float paddingRight = sp == null ? 0 : sp.paddingRight;

				float minFloat;
				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
				{
					if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
						minFloat = border.z + border.z + paddingRight + paddingRight;
					else
						minFloat = border.x + border.x + paddingLeft + paddingLeft;
				}
				else
				{
					minFloat = border.x + border.z + paddingLeft + paddingRight;
				}
				int min = Mathf.RoundToInt(pixelSize * minFloat);
				return Mathf.Max(base.minWidth, ((min & 1) == 1) ? min + 1 : min);
			}
			else if (type == Type.Advanced)
			{
				float ps = pixelSize;
				Vector4 b = border * pixelSize;
				int min = Mathf.RoundToInt(b.x + b.z);

				UISpriteData sp = GetAtlasSprite();
				if (sp != null) min += Mathf.RoundToInt(ps * (sp.paddingLeft + sp.paddingRight));

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
				UISpriteData sp = GetAtlasSprite();
				float paddingTop = sp == null ? 0 : sp.paddingTop;
				float paddingBottom = sp == null ? 0 : sp.paddingBottom;

				float minFloat;
				if (mMirror == Flip.Vertically || mMirror == Flip.Both)
				{
					if (mFlip == Flip.Vertically || mFlip == Flip.Both)
						minFloat = border.w + border.w + paddingTop + paddingTop;
					else
						minFloat = border.y + border.y + paddingBottom + paddingBottom;
				}
				else
				{
					minFloat = border.y + border.w + paddingBottom + paddingTop;
				}
				int min = Mathf.RoundToInt(pixelSize * minFloat);
				return Mathf.Max(base.minHeight, ((min & 1) == 1) ? min + 1 : min);
			}
			else if (type == Type.Advanced)
			{
				float ps = pixelSize;
				Vector4 b = border * pixelSize;
				int min = Mathf.RoundToInt(b.y + b.w);

				UISpriteData sp = GetAtlasSprite();
				if (sp != null) min += Mathf.RoundToInt(ps * (sp.paddingTop + sp.paddingBottom));

				return Mathf.Max(base.minHeight, ((min & 1) == 1) ? min + 1 : min);
			}
			return base.minHeight;
		}
	}

	/// <summary>
	/// Sprite's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
	/// This function automatically adds 1 pixel on the edge if the sprite's dimensions are not even.
	/// It's used to achieve pixel-perfect sprites even when an odd dimension sprite happens to be centered.
	/// </summary>

	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 offset = pivotOffset;

			float x0 = -offset.x * mWidth;
			float y0 = -offset.y * mHeight;
			float x1 = x0 + mWidth;
			float y1 = y0 + mHeight;

			if (GetAtlasSprite() != null && mType != Type.Tiled)
			{
				int padLeft = mSprite.paddingLeft;
				int padBottom = mSprite.paddingBottom;
				int padRight = mSprite.paddingRight;
				int padTop = mSprite.paddingTop;

				if (mType != Type.Simple)
				{
					float ps = pixelSize;

					if (ps != 1f)
					{
						padLeft = Mathf.RoundToInt(ps * padLeft);
						padBottom = Mathf.RoundToInt(ps * padBottom);
						padRight = Mathf.RoundToInt(ps * padRight);
						padTop = Mathf.RoundToInt(ps * padTop);
					}
				}

				int w = mSprite.width + padLeft + padRight;
				int h = mSprite.height + padBottom + padTop;
				float px = 1f;
				float py = 1f;

				if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
				{
					padRight ^= padLeft;
					padLeft ^= padRight;
					padRight ^= padLeft;
				}
				if (mFlip == Flip.Vertically || mFlip == Flip.Both)
				{
					padBottom ^= padTop;
					padTop ^= padBottom;
					padBottom ^= padTop;
				}

				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
				{
					padRight = padLeft;
				}
				if (mMirror == Flip.Vertically || mMirror == Flip.Both)
				{
					padTop = padBottom;
				}

				if (w > 0 && h > 0 && (mType == Type.Simple || mType == Type.Filled))
				{
					if ((w & 1) != 0) ++padLeft;
					if ((h & 1) != 0) ++padTop;

					px = (1f / w) * mWidth;
					py = (1f / h) * mHeight;
				}

				x0 += padLeft * px;
				x1 -= padRight * px;
				y0 += padBottom * py;
				y1 -= padTop * py;
			}

			Vector4 br = (mAtlas != null) ? border * pixelSize : Vector4.zero;

			float fw = br.x + br.z;
			float fh = br.y + br.w;

			float vx = Mathf.Lerp(x0, x1 - fw, mDrawRegion.x);
			float vy = Mathf.Lerp(y0, y1 - fh, mDrawRegion.y);
			float vz = Mathf.Lerp(x0 + fw, x1, mDrawRegion.z);
			float vw = Mathf.Lerp(y0 + fh, y1, mDrawRegion.w);

			return new Vector4(vx, vy, vz, vw);
		}
	}

	/// <summary>
	/// Whether the texture is using a premultiplied alpha material.
	/// </summary>

	public override bool premultipliedAlpha { get { return (mAtlas != null) && mAtlas.premultipliedAlpha; } }

	/// <summary>
	/// Retrieve the atlas sprite referenced by the spriteName field.
	/// </summary>

	public UISpriteData GetAtlasSprite ()
	{
		if (!mSpriteSet) mSprite = null;

		if (mSprite == null && mAtlas != null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UISpriteData sp = mAtlas.GetSprite(mSpriteName);
				if (sp == null) return null;
				SetAtlasSprite(sp);
			}

			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UISpriteData sp = mAtlas.spriteList[0];
				if (sp == null) return null;
				SetAtlasSprite(sp);

				if (mSprite == null)
				{
					Debug.LogError(mAtlas.name + " seems to have a null sprite!");
					return null;
				}
				mSpriteName = mSprite.name;
			}
		}
		return mSprite;
	}

	/// <summary>
	/// Set the atlas sprite directly.
	/// </summary>

	protected void SetAtlasSprite (UISpriteData sp)
	{
		mChanged = true;
		mSpriteSet = true;

		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = (mSprite != null) ? mSprite.name : "";
			mSprite = sp;
		}
	}

	/// <summary>
	/// Adjust the scale of the widget to make it pixel-perfect.
	/// </summary>

	public override void MakePixelPerfect ()
	{
		if (!isValid) return;
		base.MakePixelPerfect();

		UISpriteData sp = GetAtlasSprite();
		if (sp == null) return;

		Texture tex = mainTexture;
		if (tex == null) return;

		int w, h;
		if (mType == Type.Filled || mType == Type.Advanced)
		{
			w = Mathf.RoundToInt(pixelSize * (sp.width + sp.paddingLeft + sp.paddingRight));
			h = Mathf.RoundToInt(pixelSize * (sp.height + sp.paddingBottom + sp.paddingTop));
		}
		else if (hasBorder)
		{
			if (mType == Type.Tiled)
			{
				float wTemp;

				if (!mTileBorder)
					wTemp = pixelSize * (sp.width - sp.borderLeft - sp.borderRight);
				else if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
					wTemp = pixelSize * (sp.width - sp.borderLeft);
				else
					wTemp = pixelSize * (sp.width - sp.borderRight);

				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
					wTemp += wTemp;

				w = Mathf.RoundToInt(wTemp);

				float hTemp;
				if (!mTileBorder)
					hTemp = pixelSize * (sp.height - sp.borderBottom - sp.borderTop);
				else if (mFlip == Flip.Vertically || mFlip == Flip.Both)
					hTemp = pixelSize * (sp.height - sp.borderBottom);
				else
					hTemp = pixelSize * (sp.height - sp.borderTop);

				if (mMirror == Flip.Vertically || mMirror == Flip.Both)
					hTemp += hTemp;

				h = Mathf.RoundToInt(hTemp);
			}
			else
			{
				if (!(mMirror == Flip.Horizontally || mMirror == Flip.Both))
					w = Mathf.RoundToInt(pixelSize * (sp.width + sp.paddingLeft + sp.paddingRight));
				else if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
					w = Mathf.RoundToInt(pixelSize * (sp.width - sp.borderLeft + sp.paddingRight) * 2);
				else
					w = Mathf.RoundToInt(pixelSize * (sp.width - sp.borderRight + sp.paddingLeft) * 2);

				if (!(mMirror == Flip.Vertically || mMirror == Flip.Both))
					h = Mathf.RoundToInt(pixelSize * (sp.height + sp.paddingBottom + sp.paddingTop));
				else if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
					h = Mathf.RoundToInt(pixelSize * (sp.height - sp.borderBottom + sp.paddingTop) * 2);
				else
					h = Mathf.RoundToInt(pixelSize * (sp.height - sp.borderTop + sp.paddingBottom) * 2);
			}
		}
		else
		{
			if (mType == Type.Tiled)
			{
				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
					w = Mathf.RoundToInt(pixelSize * sp.width * 2);
				else
					w = Mathf.RoundToInt(pixelSize * sp.width);

				if (mMirror == Flip.Vertically || mMirror == Flip.Both)
					h = Mathf.RoundToInt(pixelSize * sp.height * 2);
				else
					h = Mathf.RoundToInt(pixelSize * sp.height);
			}
			else
			{
				if (!(mMirror == Flip.Horizontally || mMirror == Flip.Both))
					w = Mathf.RoundToInt(pixelSize * (sp.width + sp.paddingLeft + sp.paddingRight));
				else if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
					w = Mathf.RoundToInt(pixelSize * (sp.width + sp.paddingRight) * 2);
				else
					w = Mathf.RoundToInt(pixelSize * (sp.width + sp.paddingLeft) * 2);

				if (!(mMirror == Flip.Vertically || mMirror == Flip.Both))
					h = Mathf.RoundToInt(pixelSize * (sp.height + sp.paddingBottom + sp.paddingTop));
				else if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
					h = Mathf.RoundToInt(pixelSize * (sp.height + sp.paddingTop) * 2);
				else
					h = Mathf.RoundToInt(pixelSize * (sp.height + sp.paddingBottom) * 2);
			}
		}

		if ((w & 1) == 1) ++w;
		if ((h & 1) == 1) ++h;

		width = w;
		height = h;
	}

	/// <summary>
	/// Auto-upgrade.
	/// </summary>

	protected override void OnInit ()
	{
		if (!mFillCenter)
		{
			mFillCenter = true;
			centerType = AdvancedType.Invisible;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
		base.OnInit();
	}

	/// <summary>
	/// Update the UV coordinates.
	/// </summary>

	protected override void OnUpdate ()
	{
		base.OnUpdate();

		if (mChanged || !mSpriteSet)
		{
			mSpriteSet = true;
			mSprite = null;
			mChanged = true;
		}
	}


	/// <summary>
	/// Returns whether this is support any texture without material.
	/// </summary>
	protected override bool isSupportOnlyTexture
	{
		get
		{
			return false;
		}
	}

	/// <summary>
	/// Virtual function called by the UIPanel that fills the buffers.
	/// </summary>

	public override void OnFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		Texture tex = mainTexture;
		if (tex == null) return;

		if (mSprite == null) mSprite = atlas.GetSprite(spriteName);
		if (mSprite == null) return;

		Rect outer = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
		Rect inner = new Rect(mSprite.x + mSprite.borderLeft, mSprite.y + mSprite.borderTop,
			mSprite.width - mSprite.borderLeft - mSprite.borderRight,
			mSprite.height - mSprite.borderBottom - mSprite.borderTop);

		outer = NGUIMath.ConvertToTexCoords(outer, textureWidth, textureHeight);
		inner = NGUIMath.ConvertToTexCoords(inner, textureWidth, textureHeight);

		int offset = verts.Count;
		Fill(verts, uvs, cols, outer, inner);

		if (onPostFill != null)
			onPostFill(this, offset, verts, uvs, cols);
	}
}
