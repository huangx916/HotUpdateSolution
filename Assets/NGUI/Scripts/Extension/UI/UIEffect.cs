//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Effect is a empty but makes a draw call element in the UI hierarchy.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Effect")]
public class UIEffect : UIBlank {
	
	[SerializeField] bool mMatClone = true;
	[SerializeField] Renderer[] mRenderers = new Renderer[0];

	[System.NonSerialized] List<Material> mMaterialList = new List<Material>();
	[System.NonSerialized] int mRenderQueue;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (mMaterialList.Count <= 0)
		{
			InitMaterials();
		}
	}

	/// <summary>
	/// Copy materials
	/// </summary>
	private void InitMaterials()
	{
		if (Application.isPlaying)
		{
			mMaterialList.Clear();
			foreach (Renderer rend in mRenderers)
			{
				if (rend)
				{
					Material mat = mMatClone ? rend.material : rend.sharedMaterial;
					if (mat)
					{
						mMaterialList.Add(mat);
					}
				}
			}
		}
		mRenderQueue = int.MaxValue;
	}

	public override UIDrawCall drawCall
	{
		get
		{
			return mDrawCall;
		}
		set
		{
			if (mDrawCall != null)
			{
				mDrawCall.onRenderQueueChanged -= OnRenderQueueChanged;
			}
			mDrawCall = value;
			if (mDrawCall != null)
			{
				mDrawCall.onRenderQueueChanged += OnRenderQueueChanged;
			}
		}
	}

	private void OnRenderQueueChanged(int renderQueue)
	{
		if (renderQueue != mRenderQueue)
		{
			mRenderQueue = renderQueue;
			foreach (Material mat in mMaterialList)
			{
				mat.renderQueue = mRenderQueue;
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

	[ContextMenu("Set From Children")]
	private void SetChildren()
	{
		mRenderers = GetComponentsInChildren<Renderer>();
		Apply();
	}

	[ContextMenu("Add From Children")]
	private void AddChildren()
	{
		List<Renderer> renderList = new List<Renderer>();
		foreach (Renderer renderer in mRenderers)
		{
			renderList.Add(renderer);
		}
		Renderer[] newRenderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in newRenderers)
		{
			renderList.Add(renderer);
		}
		mRenderers = renderList.ToArray();
		Apply();
	}

	[ContextMenu("Apply")]
	private void Apply()
	{
		InitMaterials();
		OnRenderQueueChanged(drawCall.renderQueue);
	}

	[ContextMenu("Log")]
	private void Log()
	{
		string logStr = "";
		foreach (Material mat in mMaterialList)
		{
			logStr += mat.name + ": " + mat.renderQueue + " ";
		}
		Debug.LogError(logStr);
	}
}
