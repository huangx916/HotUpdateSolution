using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public class TgaManager {

	private static Regex mTgaRegex = new Regex("\\.tga$", RegexOptions.IgnoreCase);

	public static bool IsMatchTga(string path)
	{
		return mTgaRegex.IsMatch(path);
	}

	public static string GetAlphaPath(string path)
	{
		return mTgaRegex.Replace(path, "`alpha.tga");
	}

	public static void AlphaSplit(Texture2D tex)
	{
		int width = tex.width;
		int height = tex.height;
		Color32[] pixels = tex.GetPixels32();

		Bitmap bitmapRgb = new Bitmap(width, height, PixelFormat.Format24bppRgb);
		Bitmap bitmapAlpha = new Bitmap(width, height, PixelFormat.Format24bppRgb);
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				Color32 pixel = pixels[y * width + x];
				bitmapRgb.SetPixel(x, height - 1 - y, System.Drawing.Color.FromArgb(pixel.r, pixel.g, pixel.b));
				bitmapAlpha.SetPixel(x, height - 1 - y, System.Drawing.Color.FromArgb(pixel.a, pixel.a, pixel.a));
			}
		}

		ImageTGA imageTga = new ImageTGA();

		imageTga.Image = bitmapRgb;
		string rgbPath = Application.dataPath + AssetDatabase.GetAssetPath(tex.GetInstanceID()).Substring("Assets".Length);
		imageTga.SaveImage(rgbPath);

		imageTga.Image = bitmapAlpha;
		string alphaPath = GetAlphaPath(rgbPath);
		imageTga.SaveImage(alphaPath);
	}

	public static void AlphaMerge(Texture2D rgbTex, Texture2D alphaTex)
	{
		int width = rgbTex.width;
		int height = rgbTex.height;

		if (width > 0 && height > 0 && alphaTex.width == width && alphaTex.height == height)
		{
			Color32[] rgbPixels = rgbTex.GetPixels32();
			Color32[] alphaPixels = alphaTex.GetPixels32();

			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
			bitmap.MakeTransparent();
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					Color32 rgbPixel = rgbPixels[i * width + j];
					Color32 alphaPixel = alphaPixels[i * width + j];
					bitmap.SetPixel(j, height - 1 - i, System.Drawing.Color.FromArgb(alphaPixel.r, rgbPixel.r, rgbPixel.g, rgbPixel.b));
				}
			}

			ImageTGA imageTga = new ImageTGA();

			imageTga.Image = bitmap;
			string path = Application.dataPath + AssetDatabase.GetAssetPath(rgbTex.GetInstanceID()).Substring("Assets".Length);
			imageTga.SaveImage(path);

			string alphaPath = Application.dataPath + AssetDatabase.GetAssetPath(alphaTex.GetInstanceID()).Substring("Assets".Length);
			File.Delete(alphaPath);
		}
	}
}

public class ImageTGA {

	private byte mIdSize = 0;
	private byte mColorTableType = 0;
	private byte mImageType = 2;
	private ushort mColorTableIndex = 0;
	private ushort mColorTableCount = 0;
	private byte mColorTableSize = 24;
	private ushort mImageX = 0;
	private ushort mImageY = 0;
	private ushort mImageWidth = 0;
	private ushort mImageHeight = 0;
	private byte mPixSize = 0;
	private byte mRemark = 0;
	private Bitmap mImage;

	public Bitmap Image
	{
		get
		{
			return mImage;
		}
		set
		{
			mImage = value;
			if (value != null)
			{
				switch (value.PixelFormat)
				{
					case PixelFormat.Format8bppIndexed:
						mColorTableType = 1;
						mImageType = 1;
						mColorTableCount = 256;
						mPixSize = 8;
						mRemark = 32;
						break;
					case PixelFormat.Format32bppArgb:
						mColorTableType = 0;
						mImageType = 2;
						mColorTableCount = 0;
						mPixSize = 32;
						mRemark = 32;
						break;
					default:
						mColorTableType = 0;
						mImageType = 2;
						mColorTableCount = 0;
						mPixSize = 24;
						mRemark = 32;
						break;
				}
				mImageWidth = (ushort) value.Width;
				mImageHeight = (ushort) value.Height;
			}
		}
	}

	public void SaveImage(string fileFullName)
	{
		byte[] bytes = SaveImageToTGA();
		if (bytes != null)
			File.WriteAllBytes(fileFullName, bytes);
	}

	private byte[] SaveImageToTGA()
	{
		if (mImage != null)
		{
			MemoryStream imageMemory = new MemoryStream();
			imageMemory.WriteByte(mIdSize);
			imageMemory.WriteByte(mColorTableType);
			imageMemory.WriteByte(mImageType);
			imageMemory.Write(BitConverter.GetBytes(mColorTableIndex), 0, 2);
			imageMemory.Write(BitConverter.GetBytes(mColorTableCount), 0, 2);
			imageMemory.WriteByte(mColorTableSize);
			imageMemory.Write(BitConverter.GetBytes(mImageX), 0, 2);
			imageMemory.Write(BitConverter.GetBytes(mImageY), 0, 2);
			imageMemory.Write(BitConverter.GetBytes(mImageWidth), 0, 2);
			imageMemory.Write(BitConverter.GetBytes(mImageHeight), 0, 2);
			imageMemory.WriteByte(mPixSize);
			imageMemory.WriteByte(mRemark);

			int colorSize = 0;
			Bitmap saveBitmap = mImage;
			switch (saveBitmap.PixelFormat)
			{
				case PixelFormat.Format24bppRgb:
					colorSize = 3;
					break;
				case PixelFormat.Format8bppIndexed:
					colorSize = 1;
					for (int i = 0; i != mColorTableCount; i++)
					{
						imageMemory.WriteByte(mImage.Palette.Entries[i].B);
						imageMemory.WriteByte(mImage.Palette.Entries[i].G);
						imageMemory.WriteByte(mImage.Palette.Entries[i].R);
					}
					break;
				case PixelFormat.Format32bppArgb:
					colorSize = 4;
					break;
				default:
					saveBitmap = new Bitmap(mImage.Width, mImage.Height, PixelFormat.Format24bppRgb);
					System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(saveBitmap);
					graphics.DrawImage(mImage, new Rectangle(0, 0, saveBitmap.Width, saveBitmap.Height));
					graphics.Dispose();
					colorSize = 3;
					break;
			}
			BitmapData imageData = saveBitmap.LockBits(new Rectangle(0, 0, saveBitmap.Width, saveBitmap.Height), ImageLockMode.ReadWrite, saveBitmap.PixelFormat);
			byte[] bytes = new byte[imageData.Stride * imageData.Height];
			Marshal.Copy(imageData.Scan0, bytes, 0, bytes.Length);
			saveBitmap.UnlockBits(imageData);
			int index = 0;
			for (int i = 0; i != imageData.Height; i++)
			{
				index = imageData.Stride * i;
				imageMemory.Write(bytes, index, colorSize * imageData.Width);
			}
			return imageMemory.ToArray();
		}
		return null;
	}
}