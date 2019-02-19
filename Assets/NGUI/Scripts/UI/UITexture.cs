//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// If you don't have or don't wish to create an atlas, you can simply use this script to draw a texture.
/// Keep in mind though that this will create an extra draw call with each UITexture present, so it's
/// best to use it only for backgrounds or temporary visible widgets.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Texture")]
public class UITexture : UIBasicSprite
{
	[HideInInspector][SerializeField] Rect mRect = new Rect(0f, 0f, 1f, 1f);
	[HideInInspector][SerializeField] Texture mTexture;
	[HideInInspector][SerializeField] Texture mAlphaTexture;
	[HideInInspector][SerializeField] Shader mShader;
	[HideInInspector][SerializeField] Vector4 mBorder = Vector4.zero;
	[HideInInspector][SerializeField] bool mFixedAspect = false;

	[System.NonSerialized] int mPMA = -1;

	/// <summary>
	/// Texture used by the UITexture. You can set it directly, without the need to specify a material.
	/// </summary>

	public override Texture mainTexture
	{
		get
		{
			if (mTexture != null) return mTexture;
			if (mMat != null) return mMat.mainTexture;
			return null;
		}
		set
		{
			if (mTexture as object != value as object)
			{
				if (value && drawCall != null && drawCall.widgetCount == 1 && mMat == null)
				{
					mTexture = value;
					drawCall.mainTexture = value;
				}
				else
				{
					RemoveFromPanel();
					mTexture = value;
					mPMA = -1;
					MarkAsChanged();
				}
			}
		}
	}

	/// <summary>
	/// Alpha Texture used by the UITexture. You can set it directly, without the need to specify a material.
	/// </summary>

	public override Texture alphaTexture
	{
		get
		{
			if (mAlphaTexture != null)
				return mAlphaTexture;
			if (mMat != null && mMat.HasProperty("_AlphaTex"))
				return mMat.GetTexture("_AlphaTex");
			return null;
		}
		set
		{
			if (mAlphaTexture as object != value as object)
			{
				if (drawCall != null && drawCall.widgetCount == 1 && mMat == null)
				{
					mAlphaTexture = value;
					drawCall.alphaTexture = value;
				}
				else
				{
					RemoveFromPanel();
					mAlphaTexture = value;
					mPMA = -1;
					MarkAsChanged();
				}
			}
		}
	}

	/// <summary>
	/// Material used by the widget.
	/// </summary>

	public override Material material
	{
		get
		{
			return mMat;
		}
		set
		{
			if (mMat != value)
			{
				RemoveFromPanel();
				mShader = null;
				mMat = value;
				mPMA = -1;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Shader used by the texture when creating a dynamic material (when the texture was specified, but the material was not).
	/// </summary>

	public override Shader shader
	{
		get
		{
			if (mMat != null) return mMat.shader;
			if (mShader == null) mShader = Shader.Find("Unlit/Transparent Colored");
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				if (drawCall != null && drawCall.widgetCount == 1 && mMat == null)
				{
					mShader = value;
					drawCall.shader = value;
				}
				else
				{
					RemoveFromPanel();
					mShader = value;
					mPMA = -1;
					mMat = null;
					MarkAsChanged();
				}
			}
		}
	}

	/// <summary>
	/// Whether the texture is using a premultiplied alpha material.
	/// </summary>

	public override bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Material mat = material;
				mPMA = (mat != null && mat.shader != null && mat.shader.name.Contains("Premultiplied")) ? 1 : 0;
			}
			return (mPMA == 1);
		}
	}


	/// <summary>
	/// Sprite's border. X = left, Y = bottom, Z = right, W = top.
	/// </summary>

	public override Vector4 border
	{
		get
		{
			return mBorder;
		}
		set
		{
			if (mBorder != value)
			{
				mBorder = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Sprite's flipped border. X = flipped left, Y = flipped bottom, Z = flipped right, W = flipped top.
	/// </summary>

	public override Vector4 flippedBorder
	{
		get
		{
			return FlipBorder(border);
		}
		set
		{
			border = FlipBorder(value);
		}
	}

	/// <summary>
	/// UV rectangle used by the texture.
	/// </summary>

	public virtual Rect uvRect
	{
		get
		{
			return mRect;
		}
		set
		{
			if (mRect != value)
			{
				mRect = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Widget's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
	/// This function automatically adds 1 pixel on the edge if the texture's dimensions are not even.
	/// It's used to achieve pixel-perfect sprites even when an odd dimension widget happens to be centered.
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

			Texture texture = mainTexture;
			if (texture != null && mType != UISprite.Type.Tiled)
			{
				int w = texture.width;
				int h = texture.height;
				int padLeft = 0;
				int padTop = 0;

				float px = 1f;
				float py = 1f;

				if (w > 0 && h > 0 && (mType == UISprite.Type.Simple || mType == UISprite.Type.Filled))
				{
					if ((w & 1) != 0) ++padLeft;
					if ((h & 1) != 0) ++padTop;

					px = (1f / w) * mWidth;
					py = (1f / h) * mHeight;
				}
				x0 += padLeft * px;
				y1 -= padTop * py;
			}

			float fw, fh;

			if (mFixedAspect)
			{
				fw = 0f;
				fh = 0f;
			}
			else
			{
				Vector4 br = border;
				fw = br.x + br.z;
				fh = br.y + br.w;
			}

			float vx = Mathf.Lerp(x0, x1 - fw, mDrawRegion.x);
			float vy = Mathf.Lerp(y0, y1 - fh, mDrawRegion.y);
			float vz = Mathf.Lerp(x0 + fw, x1, mDrawRegion.z);
			float vw = Mathf.Lerp(y0 + fh, y1, mDrawRegion.w);

			return new Vector4(vx, vy, vz, vw);
		}
	}

	/// <summary>
	/// Whether the drawn texture will always maintain a fixed aspect ratio.
	/// This setting is not compatible with drawRegion adjustments (sliders, progress bars, etc).
	/// </summary>

	public bool fixedAspect
	{
		get
		{
			return mFixedAspect;
		}
		set
		{
			if (mFixedAspect != value)
			{
				mFixedAspect = value;
				mDrawRegion = new Vector4(0f, 0f, 1f, 1f);
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Adjust the scale of the widget to make it pixel-perfect.
	/// </summary>

	public override void MakePixelPerfect ()
	{
		base.MakePixelPerfect();

		Texture tex = mainTexture;
		if (tex == null) return;

		if (mType == Type.Filled || mType == Type.Advanced)
		{
			int w = tex.width;
			int h = tex.height;

			if ((w & 1) == 1) ++w;
			if ((h & 1) == 1) ++h;

			width = w;
			height = h;
		}
		else if (hasBorder)
		{
			if (mType == Type.Tiled)
			{
				float wTemp = tex.width;

				if (!mTileBorder)
					wTemp -= border.x + border.z;
				else if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
					wTemp -= border.x;
				else
					wTemp -= border.z;

				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
					wTemp += wTemp;

				int w = Mathf.RoundToInt(wTemp);

				float hTemp = tex.height;
				if (!mTileBorder)
					hTemp -= border.y + border.w;
				else if (mFlip == Flip.Vertically || mFlip == Flip.Both)
					hTemp -= border.y;
				else
					hTemp -= border.w;

				if (mMirror == Flip.Vertically || mMirror == Flip.Both)
					hTemp += hTemp;

				int h = Mathf.RoundToInt(hTemp);

				if ((w & 1) == 1) ++w;
				if ((h & 1) == 1) ++h;

				width = w;
				height = h;
			}
			else
			{
				int w = tex.width;
				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
				{
					if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
						w = Mathf.RoundToInt((w << 1) - border.x - border.x);
					else
						w = Mathf.RoundToInt((w << 1) - border.z - border.z);
				}
				int h = tex.height;
				if (mMirror == Flip.Vertically || mMirror == Flip.Both)
				{
					if (mFlip == Flip.Vertically || mFlip == Flip.Both)
						h = Mathf.RoundToInt((h << 1) - border.y - border.y);
					else
						h = Mathf.RoundToInt((h << 1) - border.w - border.w);
				}

				if ((w & 1) == 1) ++w;
				if ((h & 1) == 1) ++h;

				width = w;
				height = h;
			}
		}
		else
		{
			int w = tex.width;
			if (mMirror == Flip.Horizontally || mMirror == Flip.Both) w <<= 1;
			else if ((w & 1) == 1) ++w;
			int h = tex.height;
			if (mMirror == Flip.Vertically || mMirror == Flip.Both) h <<= 1;
			else if ((h & 1) == 1) ++h;

			width = w;
			height = h;
		}
	}


	/// <summary>
	/// Returns whether this is support any texture without material.
	/// </summary>
	protected override bool isSupportOnlyTexture
	{
		get
		{
			return true;
		}
	}

	/// <summary>
	/// Adjust the draw region if the texture is using a fixed aspect ratio.
	/// </summary>

	protected override void OnUpdate ()
	{
		base.OnUpdate();
		
		if (mFixedAspect)
		{
			Texture tex = mainTexture;

			if (tex != null)
			{
				int w = tex.width;
				int h = tex.height;
				if ((w & 1) == 1) ++w;
				if ((h & 1) == 1) ++h;
				float widgetWidth = mWidth;
				float widgetHeight = mHeight;
				float widgetAspect = widgetWidth / widgetHeight;
				float textureAspect = (float)w / h;

				if (textureAspect < widgetAspect)
				{
					float x = (widgetWidth - widgetHeight * textureAspect) / widgetWidth * 0.5f;
					drawRegion = new Vector4(x, 0f, 1f - x, 1f);
				}
				else
				{
					float y = (widgetHeight - widgetWidth / textureAspect) / widgetHeight * 0.5f;
					drawRegion = new Vector4(0f, y, 1f, 1f - y);
				}
			}
		}
	}

	/// <summary>
	/// Virtual function called by the UIPanel that fills the buffers.
	/// </summary>

	public override void OnFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		Texture tex = mainTexture;
		if (tex == null) return;

		Rect outer = new Rect(uvRect.x * tex.width, uvRect.y * tex.height, tex.width * uvRect.width, tex.height * uvRect.height);
		Rect inner = outer;
		Vector4 br = border;
		inner.xMin += br.x;
		inner.yMin += br.y;
		inner.xMax -= br.z;
		inner.yMax -= br.w;

		float w = 1f / tex.width;
		float h = 1f / tex.height;

		outer.xMin *= w;
		outer.xMax *= w;
		outer.yMin *= h;
		outer.yMax *= h;

		inner.xMin *= w;
		inner.xMax *= w;
		inner.yMin *= h;
		inner.yMax *= h;

		int offset = verts.Count;
		Fill(verts, uvs, cols, outer, inner);

		if (onPostFill != null)
			onPostFill(this, offset, verts, uvs, cols);
	}

	protected void Clear()
	{
		mPMA = -1;
		mTexture = null;
		mAlphaTexture = null;
		mMat = null;
		mShader = null;
		Vector4 mBorder = Vector4.zero;
	}

	/// <summary>
	/// It is used for animation
	/// </summary>

	public void SetDepth(int newDepth)
	{
		depth = newDepth;
	}
}
