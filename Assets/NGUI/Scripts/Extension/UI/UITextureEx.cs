using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Extension/NGUI Texture Ex")]
public class UITextureEx : UITexture
{
	// Replacement texture can be used to completely bypass this texture, pulling the data from another one instead.
	[HideInInspector][SerializeField] UITexture mReplacement;

	/// <summary>
	/// Setting a replacement texture value will cause everything using this texture to use the replacement texture instead.
	/// Suggested use: set up all your widgets to use a dummy texture that points to the real texture. Switching that texture to
	/// another one (for example an eastern language one) is then a simple matter of setting this field on your dummy texture.
	/// </summary>

	public UITexture replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UITexture rep = value;
			if (rep == this) rep = null;

			if (mReplacement != rep)
			{
				RemoveFromPanel();
				if (rep is UITextureEx)
				{
					UITextureEx repEx = rep as UITextureEx;
					if (repEx.replacement == this)
						repEx.replacement = null;
				}
				if (mReplacement != null) MarkAsChanged();
				mReplacement = rep;

				if (rep != null)
				{
					Clear();
				}
			}
		}
	}

	/// <summary>
	/// Texture used by the UITexture. You can set it directly, without the need to specify a material.
	/// </summary>

	public override Texture mainTexture
	{
		get
		{
			if (mReplacement != null)
				return mReplacement.mainTexture;
			return base.mainTexture;
		}
		set
		{
			if (mReplacement != null)
				mReplacement.mainTexture = value;
			else
				base.mainTexture = value;
		}
	}

	/// <summary>
	/// Alpha Texture used by the UITexture. You can set it directly, without the need to specify a material.
	/// </summary>

	public override Texture alphaTexture
	{
		get
		{
			if (mReplacement != null)
				return mReplacement.alphaTexture;
			return base.alphaTexture;
		}
		set
		{
			if (mReplacement != null)
				mReplacement.alphaTexture = value;
			else
				base.alphaTexture = value;
		}
	}

	/// <summary>
	/// Material used by the widget.
	/// </summary>

	public override Material material
	{
		get
		{
			if (mReplacement != null)
				return mReplacement.material;
			return base.material;
		}
		set
		{
			if (mReplacement != null)
				mReplacement.material = value;
			else
				base.material = value;
		}
	}

	/// <summary>
	/// Shader used by the texture when creating a dynamic material (when the texture was specified, but the material was not).
	/// </summary>

	public override Shader shader
	{
		get
		{
			if (mReplacement != null)
				return mReplacement.shader;
			return base.shader;
		}
		set
		{
			if (mReplacement != null)
				mReplacement.shader = value;
			else
				base.shader = value;
		}
	}

	/// <summary>
	/// Whether the texture is using a premultiplied alpha material.
	/// </summary>

	public override bool premultipliedAlpha
	{
		get
		{
			if (mReplacement != null)
				return mReplacement.premultipliedAlpha;
			return base.premultipliedAlpha;
		}
	}

	/// <summary>
	/// UV rectangle used by the texture.
	/// </summary>

	public override Rect uvRect
	{
		get
		{
			if (mReplacement != null)
				return mReplacement.uvRect;
			return base.uvRect;
		}
		set
		{
			if (mReplacement != null)
				mReplacement.uvRect = value;
			else
				base.uvRect = value;
		}
	}

	/// <summary>
	/// Adjust the size of the widget to make it pixel-perfect.
	/// </summary>

	public override void MakePixelPerfect()
	{
		Texture tex = mainTexture;
		if (tex == null) return;

		int texWidth, texHeight;
		if (mReplacement is UITextureEx)
		{
			texWidth = mReplacement.width;
			texHeight = mReplacement.height;
		}
		else
		{
			texWidth = tex.width;
			texHeight = tex.height;
		}

		if (mType == Type.Filled || mType == Type.Advanced)
		{
			int w = texWidth;
			int h = texHeight;

			if ((w & 1) == 1) ++w;
			if ((h & 1) == 1) ++h;

			width = w;
			height = h;
		}
		else if (hasBorder)
		{
			if (mType == Type.Tiled)
			{
				float wTemp = texWidth;

				if (!mTileBorder)
					wTemp -= border.x + border.z;
				else if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
					wTemp -= border.x;
				else
					wTemp -= border.z;

				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
					wTemp += wTemp;

				int w = Mathf.RoundToInt(wTemp);

				float hTemp = texHeight;
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
				int w = texWidth;
				if (mMirror == Flip.Horizontally || mMirror == Flip.Both)
				{
					if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
						w = Mathf.RoundToInt((w << 1) - border.x - border.x);
					else
						w = Mathf.RoundToInt((w << 1) - border.z - border.z);
				}
				int h = texHeight;
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
			int w = texWidth;
			if (mMirror == Flip.Horizontally || mMirror == Flip.Both) w <<= 1;
			else if ((w & 1) == 1) ++w;
			int h = texHeight;
			if (mMirror == Flip.Vertically || mMirror == Flip.Both) h <<= 1;
			else if ((h & 1) == 1) ++h;

			width = w;
			height = h;
		}
	}
}
